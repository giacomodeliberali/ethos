using System.Threading.Tasks;
using Ethos.Application.Contracts.Identity;

namespace Application.Identity
{
    public interface IIdentityService
    {
        Task CreateUserAsync(RegisterRequestDto input , string roleName);
        Task<string> GetTokenAsync(LoginRequestDto input);
    }
}
