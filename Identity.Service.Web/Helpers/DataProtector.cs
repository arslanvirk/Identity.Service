using Microsoft.AspNetCore.DataProtection;

namespace Identity.Service.Web.Helpers
{
    public class DataProtector
    {
        private readonly IDataProtector protector;
        public DataProtector(IDataProtectionProvider provider)
        {
            ArgumentNullException.ThrowIfNull(provider);

            var purpose = GetType().FullName ?? throw new InvalidOperationException("Type name cannot be null.");
            this.protector = provider.CreateProtector(purpose);
        }
        public string Encrypt(string plaintext)
        {
            return protector.Protect(plaintext);
        }
        public string Decrypt(string plaintext)
        {
            return protector.Unprotect(plaintext);
        }
    }
}
