namespace App.DTO.v1_0.Identity;

public class TokenRefreshInfo
{
    public required string Jwt { get; set; }
    public required string RefreshToken { get; set; }
}