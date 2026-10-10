using Microsoft.AspNetCore.Mvc;

namespace Bidding_Common.Helpers
{
    
    public class ApiResponse<T>
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public T Data { get; set; }
    }
    public static class ApiResponseHelper
    {
        public static IActionResult CreateErrorResponse(string code, string description, int statusCode)
        {
            return new ObjectResult(new ApiResponse<object>
            {
                Code = code,
                Description = description,
                Data = null
            })
            {
                StatusCode = statusCode
            };
        }
    }
}