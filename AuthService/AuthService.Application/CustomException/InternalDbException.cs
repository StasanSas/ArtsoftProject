using System;

namespace AuthService.Application.CustomException;

public class InternalDbException : Exception
{
    public InternalDbException() { }

    public InternalDbException(string message) 
        : base(message) { }
}