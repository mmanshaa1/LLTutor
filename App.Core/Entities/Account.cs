using App.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace App.Core.Entities
{
    public class Account : IdentityUser, IBaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Country { get; set; }
        public RefreshToken? RefreshToken { get; set; }
    }
}
