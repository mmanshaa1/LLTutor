using System.ComponentModel.DataAnnotations;

namespace App.APIs.DTOs
{
    #region Login DTO
    public class LoginDto
    {
        [Required]
        public string EmailOrPhone { get; set; }
        [Required]
        public string Password { get; set; }
        public int RememberMe { get; set; } = 0;
    }
    #endregion

    #region Account
    public class EmailOrPhoneOrUserNameDTO
    {
        public required string EmailOrPhoneOrUserName { get; set; }
    }

    public class AccountDTOwithToken
    {
        public string Name { get; set; }
        public string EmailOrPhone { get; set; }
        public string Token { get; set; }
        [JsonIgnore]
        public string RefreshToken { get; set; }
    }

    public class AccountDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class ActiveAccountDto
    {
        public string Id { get; set; }
        public string Token { get; set; }
    }

    #endregion

    #region Password DTO
    public class ChangePasswordDTO
    {
        public string current_password { get; set; }
        public string new_password { get; set; }
    }

    public class ResetPasswordDTO
    {
        public string Email { get; set; }
        public string token { get; set; }
        public string new_password { get; set; }
    }
    #endregion

    #region Register DTOs

    public class RegisterAccountDTO
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Nationality { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string? Gender { get; set; } = "M";

    }
    #endregion

    #region Update DTOs

    public class UpdateAccountDTO
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Nationality { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PhoneNumber2 { get; set; }
        public DateTime? dateOfBirth { get; set; }
        public string? Location { get; set; }
        public string? Gender { get; set; }
    }
    #endregion
}
