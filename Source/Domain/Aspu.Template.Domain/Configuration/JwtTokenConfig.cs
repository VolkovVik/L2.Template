namespace Aspu.Template.Domain.Configuration;

public class JwtTokenConfig
{
    public string Audience { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public int ExpirationTime { get; set; } = 12 * 60;
}
