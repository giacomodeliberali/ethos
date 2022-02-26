using System;
using System.Collections.Generic;
using Ethos.Application.Identity;
using Ethos.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NSubstitute.ExceptionExtensions;

namespace Ethos.IntegrationTest.Setup
{
    internal class ChangeUserDisposable : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ApplicationUser _user;
        private readonly IList<string> _roles;
        private readonly ICurrentUser _currentUser;

        public ApplicationUser User => _user;

        public ChangeUserDisposable(IServiceProvider serviceProvider, ApplicationUser user, IList<string> roles)
        {
            _serviceProvider = serviceProvider;
            _user = user;
            _roles = roles;

            _currentUser = _serviceProvider.GetRequiredService<ICurrentUser>();

            _currentUser.ClearSubstitute();
            _currentUser.GetCurrentUser().Returns(_user);
            _currentUser.UserId().Returns(_user.Id);
            _currentUser.IsInRole(RoleConstants.Admin).Returns(_roles.Contains(RoleConstants.Admin));
        }

        public void Dispose()
        {
            _currentUser.ClearSubstitute();
            _currentUser.GetCurrentUser().Throws(new BusinessException("User not logged in."));
            _currentUser.UserId().Throws(new BusinessException("User not logged in."));
            _currentUser.IsInRole(Arg.Any<string>()).Throws(new BusinessException("User not logged in."));
        }
    }
}
