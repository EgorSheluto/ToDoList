using Newtonsoft.Json;

namespace ToDoList.API.Code
{
    public class ErrorDetails
    {
        /// <summary>
        /// Response error status code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Response error message
        /// </summary>
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
