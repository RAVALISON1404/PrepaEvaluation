using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrepaEvaluation.Models;

public class Film
{
    [Key]
    [Column("id")]
    [Required]
    public int Id { get; set; }
    [Required]
    [Column("titre")]
    public string Title { get; set; } = null!;
    public Categorie Categorie { get; set; } = null!;
}
