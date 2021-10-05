namespace Ethos.Application.Contracts.Identity
{
    public partial class LoginResponseDto
    {
        public class UserDto
        {
            public string Email { get; set; }

            public string Id { get; set; }

            public string UserName { get; set; }

            public string FullName { get; set; }
        }
    }
}