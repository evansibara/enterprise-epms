using EPMS.Application.Interfaces.Services;

namespace EPMS.Infrastructure.Auth;

public class BCryptPasswordHasher : IPasswordHasher
{
    // Section 3.4 mensyaratkan work factor minimum 10; dipakai 12 sebagai
    // margin keamanan tambahan tanpa beban komputasi yang berlebihan.
    private const int WorkFactor = 12;

    public string Hash(string plainPassword) =>
        BCrypt.Net.BCrypt.HashPassword(plainPassword, workFactor: WorkFactor);

    public bool Verify(string plainPassword, string hash) =>
        BCrypt.Net.BCrypt.Verify(plainPassword, hash);
}
