using Microsoft.AspNetCore.Identity;

namespace HelpDesk.Models;

public class User : IdentityUser
{
    public List<Complaint>? Complaints { get; set; }
}