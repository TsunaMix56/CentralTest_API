using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyFavoritesApi.Models;

public class Favorite
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int PropertyId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [ForeignKey(nameof(PropertyId))]
    public Property Property { get; set; } = null!;
}
