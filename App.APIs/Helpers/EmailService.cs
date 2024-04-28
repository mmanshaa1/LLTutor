using System.Net.Mail;

namespace App.APIs.Helpers
{
    public static class EmailService
    {
        // inject Global Variables 
        private static EmailCredentials _EmailCredentials;
        public static void Initialize(IConfiguration configuration)
            => _EmailCredentials = configuration.GetSection("EmailCredentials").Get<EmailCredentials>();

        public static async Task SendEmailHtmlOTP(EmailDTOForOTP email)
        {
            string filePath, subject;
            if (email.Type == EmailOTPType.Verification)
            {
                subject = "Activate Your Account";
            }
            else
            {
                subject = "Reset Your Password";
            }

            using MailMessage m = new();
            m.From = new MailAddress($"LLTutor <{_EmailCredentials.Email}>");
            m.To.Add(email.ToEmail);
            m.Body = $"{email.OTP}";
            m.Subject = subject;
            m.IsBodyHtml = false;

            using SmtpClient sc = new();
            sc.Host = _EmailCredentials.Host;
            sc.Port = _EmailCredentials.Port;
            sc.DeliveryMethod = SmtpDeliveryMethod.Network;
            sc.UseDefaultCredentials = false;
            sc.Credentials = new NetworkCredential(_EmailCredentials.Email, _EmailCredentials.Password);
            sc.EnableSsl = true;

            try
            {
                await sc.SendMailAsync(m);
            }
            catch (Exception)
            {

            }

            sc.Dispose();
        }
    }

    public class EmailCredentials
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public int POP3Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
