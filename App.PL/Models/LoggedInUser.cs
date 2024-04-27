using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace App.PL.Models
{
    public class LoggedInUser
    {
        public string? Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("email")]
        [EmailAddress]
        public string Email { get; set; }
        [JsonProperty("token")]
        public string? Token { get; set; }
        [JsonProperty("userName")]
        public string UserName { get; set; }
        [JsonProperty("emailConfirmed")]
        public bool EmailConfirmed { get; set; }
        [JsonProperty("Phone")]
        [Phone]
        public string Phone { get; set; }
    }
}
