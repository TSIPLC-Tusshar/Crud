namespace Authentication.Data.Entities;

public partial class LoginSession
{
    public int SessionId { get; set; }

    public string UserId { get; set; }

    public string SessionName { get; set; }

    public string LoginFrom { get; set; }

    public string Token { get; set; }

    public string Validity { get; set; }

    public DateTime ExpiredOn { get; set; }

    public bool IsSessionExpired { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual AspNetUser User { get; set; }
}
