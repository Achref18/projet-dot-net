using System.Text.Json.Serialization;

namespace HelpDesk.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Roles
{
    Technician,
    User
}