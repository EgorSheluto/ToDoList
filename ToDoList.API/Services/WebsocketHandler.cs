using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ToDoList.API.Code;
using ToDoList.API.Services.Interfaces;
using ToDoList.DAL.Models;
using ToDoList.DAL.Repository.Interfaces;
using ToDoList.DTO.ToDoItem;

namespace ToDoList.API.Services
{
    public class WebsocketHandler : IWebsocketHandler
    {
        public List<SocketConnection> websocketConnections = new List<SocketConnection>();

        private readonly IMapper _mapper;

        public WebsocketHandler(IMapper mapper)
        {
            _mapper = mapper;
            SetupCleanUpTask();
        }

        public async Task Handle(Guid id, WebSocket webSocket,/*, Func<IQueryable<ToDoItemModel>, Task> func*/IRepository<ToDoItemModel> rep)
        {
            SocketConnection socketConnection;

            lock (websocketConnections)
            {
                socketConnection = new SocketConnection
                {
                    Id = id,
                    Websocket = webSocket
                };
                websocketConnections.Add(socketConnection);
            }

            await SendMessageToSocket(id, rep);

            while (webSocket.State == WebSocketState.Open)
            {
                var message = await ReceiveMessage(id, webSocket);
                await SendMessageToSockets(rep);
            }

            websocketConnections.Remove(socketConnection);
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed.", CancellationToken.None);
        }

        private async Task<string> ReceiveMessage(Guid id, WebSocket webSocket)
        {
            var arraySegment = new ArraySegment<byte>(new byte[4096]);
            var receivedMessage = await webSocket.ReceiveAsync(arraySegment, CancellationToken.None);
            return null;
        }

        private async Task SendMessageToSockets(IRepository<ToDoItemModel> rep)
        {
            IEnumerable<SocketConnection> toSentTo;

            lock (websocketConnections)
            {
                toSentTo = websocketConnections.ToList();
            }

            var dto = await GetItems(rep);

            var tasks = toSentTo.Select(async websocketConnection =>
            {
                var jsonString = JsonSerializer.Serialize(dto);
                var bytes = Encoding.ASCII.GetBytes(jsonString);
                var arraySegment = new ArraySegment<byte>(bytes);
                await websocketConnection.Websocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
            });
            await Task.WhenAll(tasks);
        }

        private async Task SendMessageToSocket(Guid id, IRepository<ToDoItemModel> rep)
        {
            SocketConnection toSentTo;

            lock (websocketConnections)
            {
                toSentTo = websocketConnections.Where(s => s.Id == id)
                                               .SingleOrDefault();
            }

            var dto = await GetItems(rep);

            var jsonString = JsonSerializer.Serialize(dto);
            var bytes = Encoding.ASCII.GetBytes(jsonString);
            var arraySegment = new ArraySegment<byte>(bytes);
            await toSentTo.Websocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task<List<ToDoItemDto>> GetItems(IRepository<ToDoItemModel> rep)
        {
            var items = await rep.QueryNoTracking().ToListAsync();

            return _mapper.Map<List<ToDoItemDto>>(items); /*await _toDoItemRepository.QueryNoTracking().ToListAsync();*/
        }

        private void SetupCleanUpTask()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    IEnumerable<SocketConnection> openSockets;
                    IEnumerable<SocketConnection> closedSockets;

                    lock (websocketConnections)
                    {
                        openSockets = websocketConnections.Where(s => s.Websocket.State == WebSocketState.Open || s.Websocket.State == WebSocketState.Connecting);
                        closedSockets = websocketConnections.Where(s => s.Websocket.State != WebSocketState.Open && s.Websocket.State != WebSocketState.Connecting);

                        websocketConnections = openSockets.ToList();
                    }

                    await Task.Delay(5000);
                }
            });
        }
    }
}
