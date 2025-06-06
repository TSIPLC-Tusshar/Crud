namespace Authentication.Models.DTOs
{
    public class LoginDto
    {
        public string Token { get; set; }

        public DateTime ExpiredOn { get; set; }

        public string SessionId { get; set; }
    }
}
