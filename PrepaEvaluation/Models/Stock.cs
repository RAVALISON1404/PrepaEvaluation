using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrepaEvaluation.Models;

[Table("stock")]
public class Stock
{
    [Column ("id")]
    public int Id { get; set; }
    [Required]
    [Column ("designation")]
    public string Designation { get; set; } = null!;
    [Required]
    [Column ("unity")]
    public string Unity { get; set; } = null!;
    [Required]
    [Column ("quantity")]
    public double Quantity { get; set; }
    [Column ("identification")]
    public string? Identification { get; set; }
}