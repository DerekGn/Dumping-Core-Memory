using System;
using System.IO;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace DumpingCoreMemory.Saml
{
    /// <summary>
    /// Definition of a token processor
    /// </summary>
    public interface ISecurityTokenProcessor
    {
        /// <summary>
        /// Creates an instance of an <see="SecurityToken"/>
        /// </summary>
        /// <param name="appliesToAddress">The applies to address of the relying party</param>
        /// <param name="issuer">The issuer of the token</param>
        /// <param name="validityPeriod">The lenght of time a <see cref="SecurityToken"/> instance is valid for</param>
        /// <param name="claims">The list of <see cref="Claim"/> that this token will contain</param>
        /// <param name="encryptingCertificate">The <see cref="X509Certificate2"/> encrypting certificate. This corresponds to the </param>
        /// <param name="signingCertificate">The <see cref="X509Certificate2"/> signing certificate. This is normally the identity of the STS</param>
        /// <returns>A <see="SecurityToken"> instance</returns>
        SecurityToken CreateSecurityToken(Uri appliesToAddress, String issuer, TimeSpan validityPeriod, IEnumerable<Claim> claims,
            X509Certificate2 encryptingCertificate, X509Certificate2 signingCertificate);
        
        /// <summary>
        /// Serializes a <see cref="SecurityToken"/> to a stream
        /// </summary>
        /// <param name="securityToken">The <see cref="SecurityToken"/> to serialize</param>
        /// <param name="stream">The <see cref="Stream"/> to serialize the token</param>
        void SerializeSecurityToken(SecurityToken securityToken, Stream stream);

        /// <summary>
        /// Read a security token from a <see cref="Stream"/>
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> the token was serialised</param>
        /// <returns>A <see="SecurityToken"> read from the <see cref="Stream"/></returns>
        SecurityToken ReadSecurityToken(Stream stream);

        /// <summary>
        /// Verify a <see="SecurityToken"> against the configuration
        /// </summary>
        /// <param name="securityToken">A <see="SecurityToken"> to verify</param>
        /// <returns>The collection of the identities contained in the token</returns>
        IReadOnlyCollection<ClaimsIdentity> VerifySecurityToken(SecurityToken securityToken);
    }
}
