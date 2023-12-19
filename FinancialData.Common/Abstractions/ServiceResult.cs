namespace FinancialData.Common.Abstractions;

public class ServiceResult<T> where T : class
{
    public bool IsError { get; set; }
    public string? ErrorMessage { get; set; }
    public T? Payload { get; set; }
}
