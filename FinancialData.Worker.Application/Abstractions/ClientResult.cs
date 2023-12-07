namespace FinancialData.Worker.Application.Abstractions;

public class ClientResult<T> where T : class
{
    public bool IsError { get; set; }
    public string? ErrorMessage { get; set; }
    public T? Payload { get; set; }
}
