using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FastFoodAPI.Entities;

public class ApplicationUser : IdentityUser
{
    // Navigation
    public ICollection<RefreshToken> RefreshTokens { get; set; }
        = new List<RefreshToken>();

    // Thông tin thêm
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}