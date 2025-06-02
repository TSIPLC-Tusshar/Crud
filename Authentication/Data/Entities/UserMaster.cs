namespace Authentication.Data.Entities;

public partial class UserMaster
{
    public int Id { get; set; }

    public string UserId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public virtual AspNetUser User { get; set; }
}
