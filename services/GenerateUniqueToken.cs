namespace NoteBoardServer.services;

public static class GenerateUniqueToken
{
    // to generate unique token
    public static string GenerateToken()
    {
        return Guid.NewGuid().GetHashCode().ToString("x"); 
    }
}