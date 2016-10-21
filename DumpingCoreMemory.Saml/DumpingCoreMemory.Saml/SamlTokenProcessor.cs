using System;
using System.IO;
using System.Xml;
using System.Security.Claims;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IdentityModel.Configuration;
using System.IdentityModel.Protocols.WSTrust;
using System.Security.Cryptography.X509Certificates;

namespace DumpingCoreMemory.Saml
{
    public class SamlTokenProcessor : ISecurityTokenProcessor
    {
        private SecurityTokenHandlerCollection _securityTokenHandlerCollection;
        private SecurityTokenHandler _saml2SecurityTokenHandler;
        
        public SamlTokenProcessor()
        {
            _securityTokenHandlerCollection = new SecurityTokenHandlerCollection();
            _securityTokenHandlerCollection.Add(new Saml2SecurityTokenHandler());
            _securityTokenHandlerCollection.Add(new EncryptedSecurityTokenHandler());

            IdentityConfiguration configuration = new IdentityConfiguration();

            _saml2SecurityTokenHandler = configuration.SecurityTokenHandlers[typeof(Saml2SecurityToken)];

            _saml2SecurityTokenHandler.Configuration.ServiceTokenResolver = 
                new X509CertificateStoreTokenResolver(StoreName.My, StoreLocation.CurrentUser);
        }

        /// <summary>
        /// Creates an instance of an <see="Saml2SecurityToken"/>
        /// </summary>
        /// <param name="appliesToAddress">The applies to address of the relying party</param>
        /// <param name="issuer">The issuer of the token</param>
        /// <param name="validityPeriod">The lenght of time a <see cref="SecurityToken"/> instance is valid for</param>
        /// <param name="claims">The list of <see cref="Claim"/> that this token will contain</param>
        /// <param name="encryptingCertificate">The <see cref="X509Certificate2"/> encrypting certificate. This corresponds to the </param>
        /// <param name="signingCertificate">The <see cref="X509Certificate2"/> signing certificate. This is normally the identity of the STS</param>
        /// <returns>A <see="Saml2SecurityToken"> instance</returns>
        public SecurityToken CreateSecurityToken(Uri appliesToAddress, String issuer, TimeSpan validityPeriod, IEnumerable<Claim> claims, 
            X509Certificate2 encryptingCertificate, X509Certificate2 signingCertificate)
        {
            SecurityTokenDescriptor securityTokenDescriptor = BuildBaseSecurityTokenDescriptor(appliesToAddress, issuer, validityPeriod, 
                claims, encryptingCertificate, signingCertificate);

            return _securityTokenHandlerCollection.CreateToken(securityTokenDescriptor);
        }

        /// <summary>
        /// Serializes a <see cref="SecurityToken"/> to a stream
        /// </summary>
        /// <param name="securityToken">The <see cref="SecurityToken"/> to serialize</param>
        /// <param name="stream">The <see cref="Stream"/> to serialize the token</param>
        public void SerializeSecurityToken(SecurityToken securityToken, Stream stream)
        {
            XmlWriter tokenWriter = null;
            
            try
            {
                tokenWriter = XmlWriter.Create(stream);

                _securityTokenHandlerCollection.WriteToken(tokenWriter, securityToken);

                tokenWriter.Flush();
                tokenWriter.Close();
            }
            finally
            {
                if(tokenWriter != null)
                {
                    tokenWriter.Dispose();
                }
            }
        }

        public SecurityToken ReadSecurityToken(Stream stream)
        {
            SecurityToken securityToken;

            using(XmlReader rdr = new XmlTextReader(stream))
            {
                securityToken = _saml2SecurityTokenHandler.ReadToken(rdr);
            }

            return securityToken;
        }

        public IReadOnlyCollection<ClaimsIdentity> VerifySecurityToken(SecurityToken securityToken)
        {
            return _saml2SecurityTokenHandler.ValidateToken(securityToken);
        }

        private SecurityTokenDescriptor BuildBaseSecurityTokenDescriptor(Uri appliesToAddress, String issuer, TimeSpan validityPeriod, 
            IEnumerable<Claim> claims, X509Certificate2 encryptingCertificate, X509Certificate2 signingCertificate)
        {
            DateTime utcNow = DateTime.UtcNow;
            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor();
            securityTokenDescriptor.AppliesToAddress = appliesToAddress.ToString();
            securityTokenDescriptor.TokenIssuerName = issuer;
            securityTokenDescriptor.Lifetime = new Lifetime(utcNow, utcNow.Add(validityPeriod));
            securityTokenDescriptor.SigningCredentials = new X509SigningCredentials(signingCertificate);
            securityTokenDescriptor.Subject = new ClaimsIdentity(claims);
            securityTokenDescriptor.TokenType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0";
            securityTokenDescriptor.EncryptingCredentials = new EncryptedKeyEncryptingCredentials(encryptingCertificate);

            return securityTokenDescriptor;
        }
    }
}
