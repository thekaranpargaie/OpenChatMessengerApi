using System.ComponentModel;
using System.Reflection;

namespace Shared.Extensions
{
    public static class EnumHelper
    {
        public static string GetDescription<T>(this T enumValue)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                return null;

            var description = enumValue.ToString();
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    description = ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return description;
        }
        // Custom method to get description on the basis of id of the enum 
        public static string GetDescriptionFromId<TEnum>(int id) where TEnum : System.Enum
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("Type parameter must be an enum type.");

            foreach (TEnum enumValue in System.Enum.GetValues(typeof(TEnum)))
            {
                int enumId = Convert.ToInt32(enumValue);
                if (enumId == id)
                {
                    FieldInfo fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

                    if (fieldInfo != null)
                    {
                        DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (attributes.Length > 0)
                        {
                            return attributes[0].Description;
                        }
                    }

                    // If no DescriptionAttribute is found, return the default enum name
                    return enumValue.ToString();
                }
            }

            throw new ArgumentOutOfRangeException(nameof(id), "Enum value with the specified ID not found.");
        }
    }
}
