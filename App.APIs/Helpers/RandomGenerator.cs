public static class RandomGenerator
{
    private static readonly Random random = new Random();

    private static readonly List<string> usedOTPs = new List<string>();

    public static string GenerateNumericOTP(int length)
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
            if (!usedOTPs.Contains(otpString))
            {
                usedOTPs.Add(otpString);
                return otpString;
            }
        }
    }
}
