namespace Domain;

public class RefreshToken
{
    public int Id { get; set; }
    public required string Token { get; set; }
    public required string Username { get; set; }
    public DateTime ExpirationDate { get; set; }
}