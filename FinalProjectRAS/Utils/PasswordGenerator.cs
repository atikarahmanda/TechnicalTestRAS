namespace FinalProjectRAS.Utils
{
    using System;
    public class PasswordGenerator
    {
        private static Random _random = new Random();

        public static string GeneratePassword(int length = 10)
        {
            const string upperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*()_-+=<>?";

            // Gabungkan semua karakter yang bisa dipilih
            string allChars = upperCaseChars + lowerCaseChars + digits + specialChars;

            // Membuat password acak dengan panjang yang ditentukan
            char[] password = new char[length];

            for (int i = 0; i < length; i++)
            {
                password[i] = allChars[_random.Next(allChars.Length)];
            }

            return new string(password);
        }
    }
}
