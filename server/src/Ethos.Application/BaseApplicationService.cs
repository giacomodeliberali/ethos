using Ethos.Application.Identity;

namespace Ethos.Application
{
    public class BaseApplicationService
    {
        protected ICurrentUser CurrentUser { get; }

        public BaseApplicationService(ICurrentUser currentUser)
        {
            CurrentUser = currentUser;
        }
    }
}
