using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Threading.Tasks;
using ToDoList.Common.Constants;

namespace ToDoList.MVC.Controllers
{
    public class LoginController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return await Task.Run(() => View("Login"));
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromHeader(Name = "User-Agent")] string userAgent,
                                               [FromHeader(Name = "Authorization")] string authorization)
        {
            if (ModelState.IsValid)
            {
                var client = new RestClient($"{ApiUrlConstants.HttpUrl}/Account/login");
                var request = new RestRequest(Method.POST);

                request.AddHeader("User-Agent", userAgent);
                request.AddHeader("Authorization", authorization);

                var response = await client.ExecuteAsync(request);
                var data = JsonConvert.DeserializeObject<object>(response.Content);

                if (response.IsSuccessful)
                {
                    return /*await Task.Run(() => RedirectToAction("Index", "Home"))*/Json(data);
                }
            }

            return await Task.Run(() => View("Login"));
        }

        [HttpPost]
        public async Task<IActionResult> Refresh([FromHeader(Name = "User-Agent")] string userAgent,
                                                 [FromHeader(Name = "Authorization")] string authorization,
                                                 [FromHeader(Name = "Authorization-Refresh")] string refreshToken)
        {
            if (ModelState.IsValid)
            {
                var client = new RestClient($"{ApiUrlConstants.HttpUrl}/Account/login");
                var request = new RestRequest(Method.POST);

                request.AddHeader("User-Agent", userAgent);
                request.AddHeader("Authorization", authorization);
                request.AddHeader("Authorization-Refresh", refreshToken);

                var response = await client.ExecuteAsync(request);
                var data = JsonConvert.DeserializeObject<object>(response.Content);

                if (response.IsSuccessful)
                {
                    return /*await Task.Run(() => RedirectToAction("Index", "Home"))*/Json(data);
                }
            }

            return await Task.Run(() => View("Login"));
        }
    }
}
