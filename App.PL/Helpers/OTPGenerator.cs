namespace App.PL.Helpers
{
    public static class OTPGenerator
    {
        private static readonly Random random = new Random();

        private static readonly List<string> usedOtps = new List<string>();

        public static string GenerateNumericOtp(int length)
        {
            const string allowedChars = "0123456789";

            while (true)
            {
                var otp = new char[length];
                for (int i = 0; i < length; i++)
                {
                    otp[i] = allowedChars[random.Next(0, allowedChars.Length)];
                }

                var otpString = new string(otp);
                if (!usedOtps.Contains(otpString))
                {
                    usedOtps.Add(otpString);
                    return otpString;
                }
            }
        }
    }
}