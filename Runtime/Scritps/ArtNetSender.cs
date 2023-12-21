using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using HRYooba.ArtNet.Packet;
using HRYooba.ArtNet.Core;

namespace HRYooba.ArtNet
{
    /// <summary>
    /// ArtNetSender
    /// </summary>
    public class ArtNetSender : IDisposable
    {
        private bool _disposed = false;

        private readonly UdpClient _udpClient = null;
        private readonly CancellationTokenSource _cancellationTokenSource = null;

        /// <summary>
        /// ArtNetSender
        /// </summary>
        public ArtNetSender(string hostname, int port = ArtNetDefine.Port)
        {
            _udpClient = new UdpClient(hostname, port);
            _cancellationTokenSource = new CancellationTokenSource();
        }

        ~ArtNetSender()
        {
            Dispose();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _udpClient.Dispose();

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        /// <summary>
        /// SendDmx
        /// </summary>
        /// <param name="artDmxData"></param>
        public void SendDmx(ArtDmxData artDmxData)
        {
            var artDmxPacket = new ArtDmxPacket(artDmxData.Universe, artDmxData.Data);
            var buffer = artDmxPacket.ToBytes();
            var _ = SendAsync(buffer, _cancellationTokenSource.Token);
        }

        /// <summary>
        /// SendAsync
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task SendAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            try
            {
                await _udpClient.SendAsync(buffer, buffer.Length);
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (ObjectDisposedException)
            {
            }
        }
    }
}