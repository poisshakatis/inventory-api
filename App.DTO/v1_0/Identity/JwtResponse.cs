namespace App.DTO.v1_0.Identity;

public class JwtResponse
{
    public required string Jwt { get; set; }
    public required string RefreshToken { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}