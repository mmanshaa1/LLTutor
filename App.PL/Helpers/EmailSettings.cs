using System.Net;
using System.Net.Mail;

namespace App.PL.Helpers
{
    public static class EmailSettings
    {
        // inject Global Variables 
        private static EmailCredentials _EmailCredentials;
        public static void Initialize(IConfiguration configuration)
        {
            _EmailCredentials = configuration.GetSection("EmailCredentials").Get<EmailCredentials>();
        }

        public static void SendEmail(EmailDTO email)
        {
            MailMessage m = new();
            SmtpClient sc = new();
            m.From = new MailAddress($"Title <{_EmailCredentials.Email}>");
            m.To.Add(email.ToEmail);
            m.Subject = email.Subject;
            m.Body = email.Body;
            sc.Host = _EmailCredentials.Host;
            try
            {
                sc.Port = _EmailCredentials.Port;
                sc.Credentials = new NetworkCredential(_EmailCredentials.Email, _EmailCredentials.Password);
                sc.EnableSsl = true;
                sc.Send(m);
            }
            catch (Exception)
            {
                return;
            }
        }

        public static void SendEmailHtmlOTP(EmailDTOForOTP email)
        {
            MailMessage m = new();
            SmtpClient sc = new();
            m.From = new MailAddress($"Title <{_EmailCredentials.Email}>");
            m.To.Add(email.ToEmail);
            m.Subject = email.Type == EmailOTPType.Verification ? "تفعيل حسابك" : "إعادة تعيين كلمة المرور";
            m.Body = email.Type == EmailOTPType.Verification ?
                $""
                : $"";
            m.IsBodyHtml = true;
            sc.Host = _EmailCredentials.Host;
            try
            {
                sc.Port = _EmailCredentials.Port;
                sc.Credentials = new NetworkCredential(_EmailCredentials.Email, _EmailCredentials.Password);
                sc.EnableSsl = true;
                sc.Send(m);
            }
            catch (Exception)
            {
                return;
            }
        }

        public static void SendEmailHtmlToVerifyWithLink(EmailDTOForLinkVerification email)
        {
            MailMessage m = new();
            SmtpClient sc = new();
            m.From = new MailAddress($"Title <{_EmailCredentials.Email}>");
            m.To.Add(email.ToEmail);
            m.Subject = email.Subject;
            m.Body = $"";
            m.IsBodyHtml = true;
            sc.Host = _EmailCredentials.Host;
            try
            {
                sc.Port = _EmailCredentials.Port;
                sc.Credentials = new NetworkCredential(_EmailCredentials.Email, _EmailCredentials.Password);
                sc.EnableSsl = true;
                sc.Send(m);
            }
            catch (Exception)
            {
                return;
            }
        }



        // validate the connection to the email server
        public static async Task<bool> ValidateEmailCredentials()
        {
            try
            {
                using SmtpClient sc = new();
                sc.Host = _EmailCredentials.Host;
                sc.Port = _EmailCredentials.Port;
                sc.Credentials = new NetworkCredential(_EmailCredentials.Email, _EmailCredentials.Password);
                sc.EnableSsl = true;

                await sc.SendMailAsync(new MailMessage(_EmailCredentials.Email, _EmailCredentials.Email, "Testing Mail Server!", "Testing Mail Server!"));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    #region Email DTOs

    public class EmailDTO
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string ToEmail { get; set; }
    }

    public class EmailDTOForLinkVerification
    {
        public string Subject { get; set; } = "تفعيل حسابك";
        public string VerificationLink { get; set; }
        public string ToEmail { get; set; }
    }

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
    #endregion
}
