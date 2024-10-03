using Data;
using Domain;

namespace Services;

public class RefreshTokenService(AppDbContext context)
{
    public void SaveRefreshToken(RefreshToken refreshToken)
    {
        context.RefreshTokens.Update(refreshToken);
        context.SaveChanges();
    }

    public RefreshToken? GetStoredRefreshToken(string token)
    {
        return context.RefreshTokens.SingleOrDefault(rt => rt.Token == token);
    }

    public void RemoveRefreshToken(RefreshToken refreshToken)
    {
        context.RefreshTokens.Remove(refreshToken);
        context.SaveChanges();
    }
}