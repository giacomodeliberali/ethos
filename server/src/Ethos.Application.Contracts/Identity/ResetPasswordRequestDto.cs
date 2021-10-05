namespace Ethos.Application.Contracts.Identity
{
    public class ResetPasswordRequestDto
    {
        public string Email { get; set; }

        public string NewPassword { get; set; }

        public string ResetToken { get; set; }
    }
}
