using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HelpDesk.Enums;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Status
{
    Open,
    Reviewing,
    Resolved,
    [Display(Name = "No Solution")]
    NoSolution,
    [Display(Name = "Require More Info")]
    RequireMoreInfo
}