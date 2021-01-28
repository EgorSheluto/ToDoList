using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Mime;
using ToDoList.API.Code;

namespace ToDoList.API.Controllers.Base
{
    [ProducesResponseType(500)]
    [ProducesResponseType(401)]
    [ProducesResponseType(typeof(string), 400)]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiController]
    public class BaseController : Controller
    {
        /// <summary>
        /// For incorrect user request or message
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        protected JsonResult Json(string msg, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            // May will be a problems with a client in response
            var result = JsonResultDefault.Default(msg, statusCode);
            return result;
        }

        /// <summary>
        /// For successfull request
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        protected JsonResult Json(object obj = null, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var result = JsonResultDefault.Default(obj, statusCode);
            return result;
        }

        /// <summary>
        /// For successfull request
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="sourceObject"></param>
        /// <param name="dtoType"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        protected JsonResult Json(IMapper mapper,
                                  object sourceObject,
                                  Type dtoType,
                                  HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var result = JsonResultDefault.Default(null, statusCode);
            if (sourceObject is null || dtoType is null)
                return result;

            var data = sourceObject.GetType() == dtoType ? sourceObject : mapper?.Map(sourceObject, sourceObject.GetType(), dtoType);
            result = JsonResultDefault.Default(data, statusCode);
            return result;
        }
    }
}
