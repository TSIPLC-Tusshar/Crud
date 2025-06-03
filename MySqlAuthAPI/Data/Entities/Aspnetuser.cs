using Microsoft.AspNetCore.Identity;

namespace MySqlAuthAPI.Data.Entities;

public partial class Aspnetuser : IdentityUser
{

    public virtual LoginSession LoginSession { get; set; }

    public virtual UserMaster UserMaster { get; set; }
}
