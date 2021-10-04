namespace Ethos.Shared
{
    public class JwtConfig
    {
        public const string Key = nameof(JwtConfig);

        public string Secret { get; set; }
        public string TokenIssuer { get; set; }
        public string ValidAudience { get; set; }
    }
}