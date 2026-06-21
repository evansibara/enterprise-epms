namespace EPMS.Domain.Exceptions;

/// <summary>Dilempar ketika ada aturan bisnis domain yang dilanggar.</summary>
public class DomainValidationException : Exception
{
    public DomainValidationException(string message) : base(message)
    {
    }
}
