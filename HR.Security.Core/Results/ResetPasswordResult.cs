using System;

namespace HR.Security.Core.Results
{
    public enum ResetPasswordResult
    {
        WrongPassword,
        Succeeded,
        Failure,
        NotFound
    }
}
