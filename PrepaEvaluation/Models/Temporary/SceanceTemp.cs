using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrepaEvaluation.Models.Temporary;

[Table("sceancetemp")]
public class SceanceTemp
{
    [Key]
    [Column ("id")]
    public int Id { get; set; }
    [Required]
    [Column ("film")]
    [Display(Name = "Film")]
    public string Film { get; set; } = null!;
    [Required]
    [Column ("categorie")]
    [Display(Name = "Categorie")]
    public string Categorie { get; set; } = null!;
    [Required]
    [Column ("salle")]
    [Display(Name = "Salle")]
    public string Salle { get; set; } = null!;
    [Required]
    [Column ("date")]
    [Display(Name = "Date")]
    public DateOnly Date { get; set; }
    [Required]
    [Column ("heure")]
    [Display(Name = "Heure")]
    public TimeOnly Heure { get; set; }
}