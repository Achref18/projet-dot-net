using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HelpDesk.Enums;

namespace HelpDesk.Models;

public class Complaint
{
    [Key] public Guid Id { get; set; } = new();
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(80,MinimumLength = 10,ErrorMessage = "Title characters are out of range. (10 - 80)")]
    public string Title { get; set; } = string.Empty;
    [Required(ErrorMessage = "Description is required.")]
    [MinLength(50, ErrorMessage = "Description is too short.")]
    public string Description { get; set; } = string.Empty;

    public Status Status { get; set; } = Status.Open;


    public string? Action { get; set; } 

    public User? User { get; set; }
    public bool IsClosed { get; set; } = false;
    [DisplayName("Created at")] public DateTime CreatedAt { get; set; } = DateTime.Now;
    [DisplayName("Updated at")] public DateTime? UpdatedAt { get; set; }
    [DisplayName("Closed at at")] public DateTime? ClosedAt { get; set; }
}