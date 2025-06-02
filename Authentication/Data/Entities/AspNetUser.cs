using Microsoft.AspNetCore.Identity;

namespace Authentication.Data.Entities;

public partial class AspNetUser : IdentityUser
{
    public virtual LoginSession LoginSession { get; set; }

    public virtual UserMaster UserMaster { get; set; }
}
