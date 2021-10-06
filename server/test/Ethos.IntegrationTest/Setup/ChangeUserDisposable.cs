using System;
using Ethos.Application.Identity;
using Ethos.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Ethos.IntegrationTest.Setup
{
    internal class ChangeUserDisposable : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ApplicationUser _user;
        private readonly ICurrentUser _currentUser;

        public ApplicationUser User => _user;

        public ChangeUserDisposable(IServiceProvider serviceProvider, ApplicationUser user)
        {
            _serviceProvider = serviceProvider;
            _user = user;

            _currentUser = _serviceProvider.GetRequiredService<ICurrentUser>();
            _currentUser.GetCurrentUser().Returns(_user);
        }

        public void Dispose()
        {
            _currentUser.GetCurrentUser().Returns((ApplicationUser)null);
        }
    }
}
