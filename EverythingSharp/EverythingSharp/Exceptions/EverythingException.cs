using EverythingSharp.Enums;

namespace EverythingSharp.Exceptions;

public class EverythingException : Exception
{
    public EverythingException(Error errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }

    public Error ErrorCode { get; }
}