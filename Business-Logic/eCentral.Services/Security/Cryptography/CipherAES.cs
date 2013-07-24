using eCentral.Core.Domain.Security;

namespace eCentral.Services.Security.Cryptography
{
    internal sealed class CipherAES : IBlockCipherAES
    {
        private CipherAESMode cipherMode = CipherAESMode.Both;

        public CipherAES()
            : this(CipherAESMode.Both)
        {
        }

        public CipherAES(CipherAESMode mode)
        {
            this.cipherMode = mode;
        }

        #region IBlockCipherAES Members

        public int[] KeySizesInBytes()
        {
            return new[] { 16, 24, 32 };
        }

        public int BlockSizeInBytes()
        {
            return 16;
        }

        #endregion
    }
}
