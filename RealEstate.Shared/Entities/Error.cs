namespace RealEstate.Shared.Entities;

public record Error
{
    public string Message { get; }
    public string? StackTrace { get; set; }
    public string? ErrorCode { get; }
    
    public Error(string message, string? errorCode)
    {
        Message = message;
        ErrorCode = errorCode;
    }

    private Error(string message, string? errorCode, string? stackTrace)
    {
        Message = message;
        ErrorCode = errorCode;
        StackTrace = stackTrace;
    }

    public static Error Create(string message, string? errorCode = null)
    {
        return new Error(message, errorCode);
    }

    public static Error Create(string message, string? code, string? stackTrace)
    {
        return new Error(message, code, stackTrace);
    }
}