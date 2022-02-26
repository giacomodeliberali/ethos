namespace Ethos.Common
{
    /// <summary>
    /// The JWT configuration needed to sign and verify the Bearer token.
    /// </summary>
    public class JwtConfig
    {
        /// <summary>
        /// The JWT secret used to sign the token.
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// The URI of the token issuer.
        /// </summary>
        public string TokenIssuer { get; set; }

        /// <summary>
        /// The URI of the consumers of the token (client spa).
        /// </summary>
        public string ValidAudience { get; set; }
    }
}
