using System;
using System.Security.Cryptography.X509Certificates;

namespace DumpingCoreMemory.Saml
{
    public class CertificateResolver
    {
        public static X509Certificate2 ResolveCertificate(StoreLocation storeLocation, StoreName storeName, String subjectName)
        {
            X509Certificate2Collection certificates = null;
            X509Certificate2 result = null;
            X509Store store = null;
            
            try
            {
                store = new X509Store(storeName, storeLocation);
                store.Open(OpenFlags.ReadOnly);
                
                certificates = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, subjectName, true);

                if(certificates.Count == 1)
                {
                    result = certificates[0];
                }
                else if(certificates.Count > 1)
                {
                    throw new X509CertificateNotFoundException(String.Format("More than one certificate was found for SubjectName: {0} StoreName: {1} Location: {2}", subjectName, storeName, storeLocation));
                }
                else if(certificates.Count == 0)
                {
                    throw new X509CertificateNotFoundException(String.Format("No certificate was found for SubjectName: {0} StoreName: {1} Location: {2}", subjectName, storeName, storeLocation));
                }
            }
            finally
            {
                if(store != null)
                {
                    store.Close();
                }
            }

            return result;
        }
    }
}
