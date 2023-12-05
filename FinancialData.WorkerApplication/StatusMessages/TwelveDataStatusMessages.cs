namespace FinancialData.WorkerApplication.StatusMessages;

public static class TwelveDataStatusMessages
{
    public const string TooManyRequestsMessage = "The Twelve Data API has a rate limit of 8 requests per minute." +
                " Not enough time is allowed to complete all of the requests." +
                " Please configure the Http Client Timeout to a higher value.";

    public const string UnauthorisedMessage = "Invalid API key.";
    public const string NotFoundMessage = "Symbol not found.";
    public const string BadRequestMessage = "Invalid output size. Output size must be between 1 - 5000";
}
