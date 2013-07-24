namespace eCentral.Services.Security.Cryptography
{
    /// <summary>
    /// Block cipher wrapper must implment this interface
    /// </summary>
    public interface IBlockCipherAES
    {
        int[] KeySizesInBytes();

        // iv length will/must be the same as BlockSizeInBytes
        int BlockSizeInBytes();
    }
}