namespace NoteBoardServer.services;

public static class GenerateUniqueToken
{
    // to generate unique token
    public static string GenerateToken()
    {
        return DateTime.Now.ToString().GetHashCode().ToString("x"); 
    }
}