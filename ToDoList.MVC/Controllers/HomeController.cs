using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Threading.Tasks;
using ToDoList.Common.Constants;
using ToDoList.DTO.ToDoItem;

namespace ToDoList.MVC.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return await Task.Run(() => View("Home"));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromHeader(Name = "Authorization")] string authorization,
                                              string title)
        {
            if (ModelState.IsValid)
            {
                var client = new RestClient($"{ApiUrlConstants.HttpUrl}/ToDoItem/Post");
                var request = new RestRequest(Method.POST);
                request.AddJsonBody(new ToDoItemNewDto { Title = title });
                request.AddHeader("Authorization", authorization);

                var response = await client.ExecuteAsync(request);
                var data = JsonConvert.DeserializeObject<object>(response.Content);

                if (response.IsSuccessful)
                {
                    return Json(data);
                }
            }

            return await Task.Run(() => View("Home"));
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromHeader(Name = "Authorization")] string authorization,
                                              int id, int statusId)
        {
            if (ModelState.IsValid)
            {
                var client = new RestClient($"{ApiUrlConstants.HttpUrl}/ToDoItem/Put");
                var request = new RestRequest(Method.PUT);
                request.AddJsonBody(new ToDoItemUpdateDto { Id = id, StatusId = statusId });
                request.AddHeader("Authorization", authorization);

                var response = await client.ExecuteAsync(request);
                var data = JsonConvert.DeserializeObject<object>(response.Content);

                if (response.IsSuccessful)
                {
                    return Json(data);
                }
            }

            return await Task.Run(() => View("Home"));
        }
    }
}
