using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrepaEvaluation.Models;

public class Categorie
{
    [Key]
    [Column("id")]
    [Required]
    public int Id { get; set; }
    [Column("nom")]
    [Required]
    public string Name { get; set; } = null!;
}
