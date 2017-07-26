using System;

namespace HR.Security.Core.Results
{
    public enum SignInStatus
    {
        Failure,
        Succeeded,
        LocketOut,
        WrongPassword
    }
}