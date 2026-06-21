namespace EPMS.Domain.Exceptions;

/// <summary>Dilempar ketika user tidak memiliki izin untuk melakukan suatu aksi (akan dimapping ke HTTP 403).</summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "Anda tidak memiliki izin untuk melakukan aksi ini.")
        : base(message)
    {
    }
}

/// <summary>Dilempar ketika kredensial tidak valid (akan dimapping ke HTTP 401).</summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "Email atau password salah.")
        : base(message)
    {
    }
}

/// <summary>Dilempar ketika ada konflik data, misal email sudah terdaftar (akan dimapping ke HTTP 409).</summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
