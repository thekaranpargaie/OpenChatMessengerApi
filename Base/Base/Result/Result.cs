using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Base
{
    public class Result<T>  where T : class
    {
        public bool IsSuccess { get; set; }
        public int ResponseCode { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public Result(bool isSuccess, int responeCode, object data, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
            ResponseCode = responeCode;
            Data = (T?)data;
        }

        public Result(bool isSuccess, int responseCode, T? data, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
            ResponseCode = responseCode;
            Data = data;
        }

        public static implicit operator ActionResult(Result<T> result)
        {
            return new ObjectResult(result);
        }
    }

    public class Result
    {
        public bool IsSuccess { get; set; }
        public int ResponseCode { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
        public Result(bool isSuccess, int responseCode, string message)
        {
            IsSuccess = isSuccess;
            ResponseCode = responseCode;
            Message = message;
        }
        public Result(bool isSuccess, int responseCode, object? data, string message)
        {
            IsSuccess = isSuccess;
            ResponseCode = responseCode;
            Message = message;
            Data = data;
        }

        //Added default response code value to avoid extensive refactoring or existing api calls -- see specific response code method at bottom for future methods 
        public static async Task<Result> SuccessAsync()
        {
            return await Task.FromResult(new Result(isSuccess: true, responseCode: StatusCodes.Status200OK, message: string.Empty));
        }

        public static async Task<Result> SuccessAsync(string message)
        {
            return await Task.FromResult(new Result(isSuccess: true, responseCode:StatusCodes.Status200OK , message: message));
        }

        public static async Task<Result<T>> SuccessAsync<T>(string message) where T : class
        {
            return await Task.FromResult(new Result<T>(isSuccess: true, responseCode: StatusCodes.Status200OK, data: default, message: message));
        }
        public static async Task<Result<T>> SuccessAsync<T>() where T : class
        {
            return await Task.FromResult(new Result<T>(isSuccess: true, responseCode: StatusCodes.Status200OK ,  data: default, message: string.Empty));
        }

        public static async Task<Result<T>> SuccessAsync<T>(T data) where T : class
        {
            return await Task.FromResult(new Result<T>(isSuccess: true, responseCode:StatusCodes.Status200OK , data: data, message: string.Empty));
        }

        public static async Task<Result<T>> SuccessAsync<T>(T data, string message, int responseCode) where T : class
        {
            return await Task.FromResult(new Result<T>(isSuccess: true, responseCode, data: data, message: message));
        }
        public static async Task<Result> FailAsync()
        {
            return await Task.FromResult(new Result(isSuccess: false, responseCode:StatusCodes.Status400BadRequest, message: string.Empty));
        }
        public static async Task<Result<T>> FailAsync<T>() where T : class
        {
            return await Task.FromResult(new Result<T>(isSuccess: false, responseCode: StatusCodes.Status400BadRequest,  default, message: string.Empty));
        }

        public static async Task<Result<T>> FailAsync<T>(string message) where T : class
        {
            return await Task.FromResult(new Result<T>(isSuccess: false, responseCode:StatusCodes.Status400BadRequest, default, message: message));
        }

        public static async Task<Result> FailAsync(string message)
        {
            return await Task.FromResult(new Result(isSuccess: false, responseCode:StatusCodes.Status400BadRequest, data: null, message: message));
        }

        public static async Task<Result> FailAsyncWithException(string message)
        {
            return await Task.FromResult(new Result(isSuccess: false, responseCode: StatusCodes.Status500InternalServerError, data: null, message: message));
        }

        //support for more specific response codes like 204 NoContent 
        public static async Task<Result> SpecificResponseCode (int responseCode, string message)
        {
            return await Task.FromResult(new Result(isSuccess: false, responseCode: responseCode, data: null, message: message));
        }
        public Task ExecuteResultAsync(ActionContext context)
        {
            throw new NotImplementedException();
        }

        public static implicit operator ActionResult(Result result)
        {
            return new ObjectResult(result);
        }
    }
}
