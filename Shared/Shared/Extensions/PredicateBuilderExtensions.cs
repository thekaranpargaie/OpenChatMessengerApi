
using Base.Domain;
using System.Linq.Expressions;
using System.Reflection;

namespace Shared.Extensions
{
    public static class PredicateBuilderExtensions
    {
        public static Expression<Func<T, bool>> True<T>() { return f => true; }
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
                                                            Expression<Func<T, bool>> expr2)
        {
            var finalExpression = Expression.Lambda<Func<T, bool>>(Expression.OrElse(
                       new SwapVisitor(expr1.Parameters[0], expr2.Parameters[0]).Visit(expr1.Body), expr2.Body), expr2.Parameters);

            return finalExpression;
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
                                                             Expression<Func<T, bool>> expr2)
        {
            var finalExpression = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(
            new SwapVisitor(expr1.Parameters[0], expr2.Parameters[0]).Visit(expr1.Body), expr2.Body), expr2.Parameters);
            return finalExpression;
        }
        public static Type GetTypeOfPropertyName<T>(string propertyName) where T : Entity
        {
            string[] bits = propertyName.Split('.');
            PropertyInfo propertyInfo = typeof(T).GetProperty(bits[0]);
            for (int i = 1; i < bits.Length; i++)
            {
                if (propertyInfo.PropertyType.GetGenericArguments().Count() > 0)
                {
                    propertyInfo = propertyInfo.PropertyType.GetGenericArguments()[0].GetProperty(bits[i]);
                }
                else
                {
                    propertyInfo = propertyInfo.PropertyType.GetProperty(bits[i]);
                }
            }
            return propertyInfo.PropertyType;
        }
        public static bool IsNullableType(Type type)
        {
            if (!type.IsGenericType)
            {
                return false;
            }
            if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Expression<Func<T, bool>> AddPred<T>(this Expression<Func<T, bool>> expr, object value, string propertyName) where T : Entity
        {
            Type typeOfValue = GetTypeOfPropertyName<T>(propertyName);

            if (HasToEvaluateValue(value, typeOfValue))
            {
                Expression<Func<T, bool>> predicate = BuildExpressionForProperty<T>(value, propertyName, typeOfValue);
                return expr.Or(predicate);
            }

            return expr;
        }

        public static Expression<Func<T, bool>> BuildExpressionForProperty<T>(object value, string propertyName, Type typeOfValue) where T : Entity
        {
            Expression<Func<T, bool>> predicate;
            ParameterExpression paramExp = Expression.Parameter(typeof(T), "e");
            var propExp = CreateExpression(paramExp, propertyName);
            MethodInfo method = typeOfValue.GetMethod(GetMethodTypeOfValue(typeOfValue), new[] { typeOfValue });
            Expression expression;
            if (IsNullableType(typeOfValue)) typeOfValue = typeof(object);

            var someValue = Expression.Constant(value, typeOfValue);

            expression = Expression.Call(propExp, method, someValue);



            predicate = Expression.Lambda<Func<T, bool>>(expression, paramExp);

            return predicate;

        }

        private static Expression CreateExpression(ParameterExpression paramExp, string propertyName)
        {
            Expression propExp = paramExp;
            foreach (var propName in propertyName.Split('.'))
            {
                propExp = Expression.Property(propExp, propName);
                MethodInfo miLower = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                propExp = Expression.Call(propExp, miLower);
            }
            return propExp;
        }

        private static bool HasToEvaluateValue(object value, Type valueType)
        {
            if (value == null) return false;

            if (valueType == typeof(string) && string.IsNullOrEmpty(value.ToString()))
            {
                return false;
            }
            if (valueType == typeof(Guid))
            {
                Guid valueConverted = Guid.Parse(value.ToString());
                return Guid.Empty != valueConverted;
            }

            return true;
        }

        private static string GetMethodTypeOfValue(Type type)
        {
            if (type == typeof(string)) return "Contains";

            return "Equals";
        }
    }

    class SwapVisitor : ExpressionVisitor
    {
        private readonly Expression from, to;
        public SwapVisitor(Expression from, Expression to)
        {
            this.from = from;
            this.to = to;
        }
        public override Expression Visit(Expression node)
        {
            return node == from ? to : base.Visit(node);
        }
    }
}
