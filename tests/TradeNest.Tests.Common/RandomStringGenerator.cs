using System.Text;

namespace TradeNest.Tests.Common;

public static class RandomStringGenerator
{
    private const string CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
    
    public static string RandomString(int minLength, int maxLength)
    {
        int length = Random.Shared.Next(minLength, maxLength);
        return RandomStringGenerator.RandomString(length);
    }

    public static string RandomString(int length)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            sb.Append(CHARS[Random.Shared.Next(CHARS.Length)]);
        }

        return sb.ToString();
    }
}