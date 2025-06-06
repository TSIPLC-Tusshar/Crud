using System.Net;

namespace Authentication.Models.DTOs
{
    public class ResponseDto
    {
        public bool Success { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string Message { get; set; }
    }

    public class ResponseDto<T> where T : class
    {
        public bool Success { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }
    }
}
