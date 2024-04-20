using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrepaEvaluation.Models;

public class Sceance
{
    [Key]
    [Column("id")]
    [Required]
    public int Id { get; set; }
    [Required]
    [Column("idfilm")]
    public int IdFilm { get; set; }
    public Film Film { get; set; } = null!;
    [Required]
    [Column("idsalle")]
    public int IdSalle { get; set; }
    public Salle Salle { get; set; } = null!;
    [Column("date")]
    [Required]
    public DateOnly Date { get; set; }
    [Column("heure")]
    [Required]
    public TimeOnly Heure { get; set; }
}
