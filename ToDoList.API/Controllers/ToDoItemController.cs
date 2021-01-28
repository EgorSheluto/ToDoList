using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ToDoList.API.Controllers.Base;
using ToDoList.API.Helpers.Interfaces;
using ToDoList.API.Services.Interfaces;
using ToDoList.Common.Enums;
using ToDoList.DAL.Models;
using ToDoList.DAL.Repository.Interfaces;
using ToDoList.DTO.ToDoItem;

namespace ToDoList.API.Controllers
{
    public class ToDoItemController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IRepository<UserModel> _repositoryUser;
        private readonly IRepository<ToDoItemModel> _repositoryToDoItem;
        private readonly IAccountHelper _accountHelper;
        private readonly IWebsocketHandler _websocketHandler;

        public ToDoItemController(IMapper mapper,
                                  IRepository<UserModel> repositoryUser,
                                  IRepository<ToDoItemModel> repositoryToDoItem,
                                  IAccountHelper accountHelper,
                                  IWebsocketHandler websocketHandler)
        {
            _mapper = mapper;
            _repositoryUser = repositoryUser;
            _repositoryToDoItem = repositoryToDoItem;
            _accountHelper = accountHelper;
            _websocketHandler = websocketHandler;
        }

        /*[HttpGet]
        [ProducesResponseType(typeof(List<ToDoItemDto>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [Route("get")]
        public async Task<IActionResult> Get()
        {
            var toDoItems = await _repositoryToDoItem.QueryNoTracking()
                                                     .OrderByDescending(s => s.CreateDateUTC)
                                                         .ThenByDescending(s => s.UpdateDateUTC)
                                                     .ToListAsync();

            return Json(_mapper, toDoItems, typeof(List<ToDoItemDto>));
        }*/

        [HttpGet]
        [ProducesResponseType(typeof(List<ToDoItemDto>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [Route("get")]
        [AllowAnonymous]
        public async Task Get()
        {
            var context = ControllerContext.HttpContext;
            var isSocketRequest = context.WebSockets.IsWebSocketRequest;

            if (isSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

                /*// Data fetching from DB
                var toDoItems = await _repositoryToDoItem.QueryNoTracking()
                                                         .OrderByDescending(s => s.CreateDateUTC)
                                                             .ThenByDescending(s => s.UpdateDateUTC)
                                                         .ToListAsync();

                // Websocket sending
                var jsonString = JsonSerializer.Serialize(toDoItems);
                var bytes = Encoding.ASCII.GetBytes(jsonString);
                var arraySegment = new ArraySegment<byte>(bytes);
                await webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);*/
                await _websocketHandler.Handle(Guid.NewGuid(), webSocket, _repositoryToDoItem);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ToDoItemDto), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [Route("post")]
        public async Task<IActionResult> Post([FromBody] ToDoItemNewDto dto, [FromHeader(Name = "Authorization")] string token)
        {
            if (dto is null)
            {
                return Json("To-do item is empty");
            }

            // It's better to do validation separately
            if (string.IsNullOrEmpty(dto.Title) || string.IsNullOrWhiteSpace(dto.Title))
            {
                return Json("Title is empty");
            }

            // Get user login
            var principal = _accountHelper.GetPrincipal(token, false);
            if (principal is null)
                return Json("Incorrect token");
            var login = principal?.Identity?.Name;

            var existUserId = await _repositoryUser.QueryNoTracking()
                                                   .Where(s => s.Login.Equals(login))
                                                   .Select(s => s.Id)
                                                   .SingleOrDefaultAsync();

            if (existUserId <= 0)
            {
                return Json("Incorrect user");
            }

            // It's better to use custom Equals with validation on: '', "" and another symbols.
            var existingToDoItem = await _repositoryToDoItem.QueryNoTracking()
                                                            .Where(s => s.Title.Equals(dto.Title))
                                                            .SingleOrDefaultAsync();

            if (!(existingToDoItem is null))
            {
                return Json("There is another duplicate of the to-do item");
            }

            var toDoItem = _mapper.Map<ToDoItemModel>(dto);
            toDoItem.UserId = existUserId;
            await _repositoryToDoItem.SaveAsync(toDoItem);

            return Json(_mapper, toDoItem, typeof(ToDoItemDto));
        }

        [HttpPut]
        [ProducesResponseType(typeof(ToDoItemDto), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [Route("put")]
        public async Task<IActionResult> Put([FromBody] ToDoItemUpdateDto dto)
        {
            if (dto is null)
            {
                return Json("To-do item is empty");
            }

            // It's better to do validation separately
            if (dto.Id <= 0)
            {
                return Json("There isn't to-do item id");
            }

            if (dto.StatusId <= 0)
            {
                return Json("There isn't to-do status");
            }

            var existingToDoItem = await _repositoryToDoItem.Query(dto.Id)
                                                            .Where(s => (int)s.Status != dto.StatusId)
                                                            .SingleOrDefaultAsync();

            if (existingToDoItem is null)
            {
                return Json("There is no the to-do item");
            }

            //var toDoItem = _mapper.Map<ToDoItemModel>(dto);
            existingToDoItem.Status = (ToDoItemStatus)dto.StatusId;
            existingToDoItem.UpdateDateUTC = DateTime.UtcNow;
            await _repositoryToDoItem.UpdateAsync(existingToDoItem);

            return Json(_mapper, existingToDoItem, typeof(ToDoItemDto));
        }
    }
}
