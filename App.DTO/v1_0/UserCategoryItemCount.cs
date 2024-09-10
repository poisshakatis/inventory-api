using App.Constants;

namespace App.DTO.v1_0;

public class UserCategoryItemCount
{
    public required string Email { get; set; }
    public Dictionary<Category, int> CategoryItemCount { get; set; } = new();
}