using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using R3;
using HRYooba.ArtNet.Packet;
using HRYooba.ArtNet.Core;

namespace HRYooba.ArtNet
{
    /// <summary>
    /// ArtNetReceiver
    /// </summary>
    public class ArtNetReceiver : IDisposable
    {
        private bool _disposed = false;

        private readonly UdpClient _udpClient = null;
        private readonly CancellationTokenSource _cancellationTokenSource = null;

        private readonly Subject<ArtDmxData> _onDmxReceivedSubject = null;
        public Observable<ArtDmxData> OnDmxReceivedObservable => _onDmxReceivedSubject.ObserveOnMainThread();

        /// <summary>
        /// ArtNetReceiver
        /// </summary>
        public ArtNetReceiver(string hostname, int port = ArtNetDefine.Port)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _onDmxReceivedSubject = new Subject<ArtDmxData>();

            try
            {
                _udpClient = new UdpClient(hostname, port);
                var _ = ReceiveAsync(_cancellationTokenSource.Token);
            }
            catch
            {
                throw;
            }
        }

        ~ArtNetReceiver()
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

            _udpClient?.Dispose();

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            _onDmxReceivedSubject.Dispose();
        }

        /// <summary>
        /// ReceiveAsync
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task ReceiveAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = await _udpClient?.ReceiveAsync();
                    var buffer = result.Buffer;
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!IsArtNetID(buffer)) continue;
                    switch (GetOpCodeType(buffer))
                    {
                        case OpCodeType.OpDmx:
                            var artDmxPacket = new ArtDmxPacket(buffer);
                            var artDmxData = new ArtDmxData(artDmxPacket.Universe, artDmxPacket.Data);
                            _onDmxReceivedSubject.OnNext(artDmxData);
                            break;

                        // 以下に追加想定
                        // case OpCodeType.OpPoll:
                        //     break;

                        case OpCodeType.None:
                        default:
                            break;
                    }
                }
            }
            catch (ObjectDisposedException)
            {
            }
        }

        /// <summary>
        /// IsArtNetID
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool IsArtNetID(byte[] data)
        {
            if (data.Length < 8) return false;

            return
                data[0] == ArtNetDefine.ID[0] &&
                data[1] == ArtNetDefine.ID[1] &&
                data[2] == ArtNetDefine.ID[2] &&
                data[3] == ArtNetDefine.ID[3] &&
                data[4] == ArtNetDefine.ID[4] &&
                data[5] == ArtNetDefine.ID[5] &&
                data[6] == ArtNetDefine.ID[6] &&
                data[7] == ArtNetDefine.ID[7];
        }

        /// <summary>
        /// GetOpCodeType
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private OpCodeType GetOpCodeType(byte[] data)
        {
            if (data.Length < 10) return OpCodeType.None;

            var opCode = (ushort)(data[8] | data[9] << 8);
            return (OpCodeType)opCode;
        }
    }
}