using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gerenciamento.Shared.Models;

public class EmailSchedule
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [EmailAddress]
    public required string To { get; set; }

    [Required]
    public required string Subject { get; set; }

    public string? Body { get; set; }

    [Required]
    public required DateTime SendTime { get; set; }

    public bool IsSent { get; set; } = false;
}