using System;

namespace WebApi.Repositories
{
    /// <summary>
    /// Example of an exception that represents an external REST request
    /// not being available
    /// </summary>
    public class RequestTimeoutException : Exception
    {
    }
}