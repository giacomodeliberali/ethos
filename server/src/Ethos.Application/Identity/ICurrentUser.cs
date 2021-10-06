using System;
using System.Threading.Tasks;
using Ethos.Domain.Entities;

namespace Ethos.Application.Identity
{
    public interface ICurrentUser
    {
        public Task<ApplicationUser> GetCurrentUser();
    }
}