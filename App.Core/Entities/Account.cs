using App.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace App.Core.Entities
{
    public class Account : IdentityUser, IBaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Nationality { get; set; }
        public string? PhoneNumber2 { get; set; }
        public RefreshToken? RefreshToken { get; set; }
        public DateTime? dateOfBirth { get; set; }
        public string? Location { get; set; }
        public bool? Gender { get; set; }
    }
}
