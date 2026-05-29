using System.Text.Json;

namespace APITask.Models.Errors;

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty; // Identifica a requisição onde o erro ocorreu para rastrear nos logs
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
