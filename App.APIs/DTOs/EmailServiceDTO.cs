namespace App.APIs.DTOs
{
    public class EmailDTOForOTP
    {
        public EmailOTPType Type { get; set; }
        public string OTP { get; set; }
        public string ToEmail { get; set; }
    }

    public enum EmailOTPType
    {
        Verification,
        ResetPassword
    }
}
