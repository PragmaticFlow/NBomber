namespace CSharpProd.HTTP.SimpleBookstore
{
    public static class PasswordGenerator
    {
        private const string Fingers = "1234567890";
        private const string BigLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string SmallLetters = "abcdefghijklmnopqrstuvwxyz";

        public static string GeneratePassword(int length)
        { 
            Random random = new Random();
            char[] password = new char[length];

            password[0] = Fingers[random.Next(Fingers.Length)];
            password[1] = BigLetters[random.Next(BigLetters.Length)];
            password[2] = SmallLetters[random.Next(SmallLetters.Length)];

            for (int i = 3; i < length; i++)
            {
                int category = random.Next(3);

                switch (category)
                {
                    case 0:
                        password[i] = Fingers[random.Next(Fingers.Length)];
                        break;
                    case 1:
                        password[i] = BigLetters[random.Next(BigLetters.Length)];
                        break;
                    case 2:
                        password[i] = SmallLetters[random.Next(SmallLetters.Length)];
                        break;
                }
            }

            for (int i = 0; i < length; i++)
            {
                int randomIndex = random.Next(length);
                char temp = password[i];
                password[i] = password[randomIndex];
                password[randomIndex] = temp;
            }

            return new string(password);
        }
    }
}

