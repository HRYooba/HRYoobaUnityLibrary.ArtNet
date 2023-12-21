namespace HRYooba.ArtNet.Core
{
    /// <summary>
    /// ArtNetDefine
    /// </summary>
    public static class ArtNetDefine
    {
        public const int Port = 6454;
        public static readonly byte[] ID = new byte[] { 0x41, 0x72, 0x74, 0x2d, 0x4e, 0x65, 0x74, 0x00 }; // Art-Net\0
    }
}