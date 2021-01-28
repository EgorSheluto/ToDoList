using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ToDoList.API.Code
{
    public static class JsonResultDefault
    {
        public static JsonResult Default(object obj = null, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var jsonResult = new JsonResult(obj)
            {
                ContentType = "application/json",
                StatusCode = (int)statusCode
            };

            return jsonResult;
        }
    }
}
