using Microsoft.Extensions.Logging;

namespace IDM.Exceptions;

public class ValidationException : Exception
{
    public List<string> ErrorList { get; set; }
}