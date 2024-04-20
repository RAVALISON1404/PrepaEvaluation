using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrepaEvaluation.Models.View;

[Table("sceance_lib")]
public class SceanceLib
{
    [Key]
    [Column("id")]
    [Required]
    public int Id { get; set; }
    [Column("film")]
    [Required]
    public string Film { get; set; } = null!;
    [Column("categorie")]
    [Required]
    public string Categorie { get; set; } = null!;
    [Required]
    [Column("salle")]
    public string Salle { get; set; } = null!;
    [Column("date")]
    [Required]
    public DateOnly Date { get; set; }
    [Column("heure")]
    [Required]
    public TimeOnly Heure { get; set; }
}