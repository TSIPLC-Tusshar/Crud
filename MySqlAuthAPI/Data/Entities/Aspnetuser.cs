using Microsoft.AspNetCore.Identity;

namespace MySqlAuthAPI.Data.Entities;

public partial class Aspnetuser : IdentityUser
{
    public virtual ICollection<Aspnetuserclaim> Aspnetuserclaims { get; set; } = new List<Aspnetuserclaim>();

    public virtual ICollection<Aspnetuserlogin> Aspnetuserlogins { get; set; } = new List<Aspnetuserlogin>();

    public virtual ICollection<Aspnetusertoken> Aspnetusertokens { get; set; } = new List<Aspnetusertoken>();

    public virtual ICollection<Aspnetrole> Roles { get; set; } = new List<Aspnetrole>();

    public virtual LoginSession LoginSession { get; set; }

    public virtual UserMaster UserMaster { get; set; }
}
