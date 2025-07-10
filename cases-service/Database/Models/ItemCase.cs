using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace CasesService.Database.Models;

/// <summary>Предмет кейса</summary>
[Table("item_case")]
[PrimaryKey(nameof(CaseId), nameof(ItemId))]
[Index(nameof(CaseId), nameof(ItemId), IsUnique = true)]
public class ItemCase
{
    [Column("case_id")]
    public string? CaseId { get; set; }

    [JsonIgnore]
    [Required]
    public Case? Case { get; set; }

    [Column("item_id")]
    public string? ItemId { get; set; }

    [JsonIgnore]
    [Required]
    public Item? Item { get; set; }
}