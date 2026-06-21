namespace EPMS.Domain.Exceptions;

/// <summary>Dilempar ketika entity yang dicari tidak ditemukan.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"Entity \"{entityName}\" dengan key ({key}) tidak ditemukan.")
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }
}
