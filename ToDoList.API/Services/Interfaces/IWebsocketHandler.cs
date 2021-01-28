using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using ToDoList.DAL.Models;
using ToDoList.DAL.Repository.Interfaces;

namespace ToDoList.API.Services.Interfaces
{
    public interface IWebsocketHandler
    {
        Task Handle(Guid id, WebSocket webSocket, IRepository<ToDoItemModel> rep);
    }
}
