namespace FinancialData.WorkerApplication.Abstractions;

public class ClientResult<T> where T : class
{
    public bool IsError { get; set; }
    public string? ErrorMessage { get; set; }
    public T? Result { get; set; }
}
