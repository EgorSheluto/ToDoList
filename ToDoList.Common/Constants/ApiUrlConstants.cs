namespace ToDoList.Common.Constants
{
    public static class ApiUrlConstants
    {
        private static readonly string HttpProtocol = "https";
        private static readonly string WssProtocol = "wss";

        private static readonly string Domain = "localhost:5001";

        public static readonly string HttpUrl = $"{HttpProtocol}://{Domain}/api";
        public static readonly string WssUrl = $"{WssProtocol}://{Domain}/api";
    }
}
