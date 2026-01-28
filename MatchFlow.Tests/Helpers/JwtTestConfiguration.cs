using Microsoft.Extensions.Configuration;

public static class JwtTestConfiguration
{
    public static IConfiguration Create()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Jwt:Key"] = "THIS_IS_A_256_BITS_TEST_KEY_01234567890",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience"
            })
            .Build();
    }
}