using System;
using System.Net.WebSockets;

namespace ToDoList.API.Code
{
    public class SocketConnection
    {
        public Guid Id { get; set; }

        public WebSocket Websocket { get; set; }
    }
}
