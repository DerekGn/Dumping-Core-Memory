using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using DumpingCoreMemory.Saml;

namespace DumpingCoreMemory.SamlTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SamlTokenProcessor processor = new SamlTokenProcessor();

                X509Certificate2 encryptingCertificate = CertificateResolver.ResolveCertificate(StoreLocation.CurrentUser, StoreName.My, "CN=RP");
                X509Certificate2 signingCertificate = CertificateResolver.ResolveCertificate(StoreLocation.CurrentUser, StoreName.My, "CN=STS");

                List<Claim> claims = new List<Claim>() { new Claim("Claim", "Value") };

                SecurityToken token = processor.CreateSecurityToken(new Uri("http://rp.com"), "sts", new TimeSpan(0, 0, 10), claims, 
                    encryptingCertificate, signingCertificate);

                using(Stream stream = new FileStream("token.xml", FileMode.OpenOrCreate))
                {
                    processor.SerializeSecurityToken(token, stream);
                }

                Saml2SecurityToken samlToken;

                using (Stream stream = new FileStream("token.xml", FileMode.Open))
                {
                    samlToken = (Saml2SecurityToken)processor.ReadSecurityToken(stream);
                }

                Console.WriteLine("Issuer: {0}", samlToken.Assertion.Issuer.Value);
                Console.WriteLine("Audience: {0}", samlToken.Assertion.Conditions.AudienceRestrictions[0].Audiences[0].Host);

                var attributeStatement = (Saml2AttributeStatement)samlToken.Assertion.Statements[0];

                Console.WriteLine("Statement: [{0} : {1}]", attributeStatement.Attributes[0].Name, attributeStatement.Attributes[0].Values[0]);

                IReadOnlyCollection<ClaimsIdentity> extractedClaims = processor.VerifySecurityToken(samlToken);

                foreach (var claimIdentity in extractedClaims)
                {
                    foreach (var claim in claimIdentity.Claims)
                    {
                        Console.WriteLine(claim);
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.ToString());
            }
        }
    }
}
