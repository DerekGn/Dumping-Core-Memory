using System;

namespace DumpingCoreMemory.Saml
{
    [Serializable]
    public class X509CertificateNotFoundException : Exception
    {
        public X509CertificateNotFoundException() { }
        public X509CertificateNotFoundException(string message) : base(message) { }
        public X509CertificateNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected X509CertificateNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
