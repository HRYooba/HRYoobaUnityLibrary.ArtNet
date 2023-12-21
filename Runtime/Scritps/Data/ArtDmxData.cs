using System;
using UnityEngine;

namespace HRYooba.ArtNet
{
    /// <summary>
    /// ArtDmxData
    /// </summary>
    [Serializable]
    public class ArtDmxData
    {
        [SerializeField] private PortAddressData _portAddress = default;
        [SerializeField, Range(0, 255)] private byte[] _data = null;

        /// <summary>
        /// ArtDmxData
        /// </summary>
        /// <param name="portAddress"></param>
        /// <param name="data"></param>
        public ArtDmxData(PortAddressData portAddress, byte[] data)
        {
            if (data.Length < 1 || data.Length > 512) throw new ArgumentException($"[ArtDmxData] Data length must be 1 to 512. {data.Length}");

            _portAddress = portAddress;
            _data = data;
        }

        /// <summary>
        /// ArtDmxData
        /// </summary>
        /// <param name="net"></param>
        /// <param name="subNet"></param>
        /// <param name="universe"></param>
        /// <param name="data"></param>
        public ArtDmxData(byte net, byte subNet, byte universe, byte[] data) : this(new PortAddressData(net, subNet, universe), data) { }

        /// <summary>
        /// ArtDmxData
        /// </summary>
        /// <param name="universe">DMX Universe</param>
        /// <param name="data"></param>
        public ArtDmxData(ushort universe, byte[] data) : this(new PortAddressData(universe), data) { }

        public byte[] Data => _data;
        public PortAddressData PortAddress => _portAddress;
        public ushort Universe => _portAddress.ToUniverse();

        public override string ToString()
        {
            var dataString = string.Empty;
            for (var i = 0; i < _data.Length; i++)
            {
                dataString += $"{_data[i]} ";
            }
            return $"PortAddress: {_portAddress}, Data: {dataString}";
        }

        /// <summary>
        /// PortAddressData
        /// </summary>
        [Serializable]
        public struct PortAddressData
        {
            [SerializeField, Range(0, 127)] private byte _net;
            [SerializeField, Range(0, 15)] private byte _subNet;
            [SerializeField, Range(0, 15)] private byte _universe;

            /// <summary>
            /// PortAddressData
            /// </summary>
            /// <param name="net"></param>
            /// <param name="subNet"></param>
            /// <param name="universe"></param>
            public PortAddressData(byte net, byte subNet, byte universe)
            {
                if (net > 127) throw new ArgumentException($"[PortAddressData] Net must be 0 to 127. {net}");
                if (subNet > 15) throw new ArgumentException($"[PortAddressData] Sub-Net must be 0 to 15. {subNet}");
                if (universe > 15) throw new ArgumentException($"[PortAddressData] Universe must be 0 to 15. {universe}");

                _net = net;
                _subNet = subNet;
                _universe = universe;
            }

            /// <summary>
            /// PortAddressData
            /// </summary>
            /// <param name="universe">DMX Universe</param>
            public PortAddressData(ushort universe)
                : this((byte)(universe >> 8), (byte)((universe & 0xF0) >> 4), (byte)(universe & 0x0F))
            {
                if (universe > 32767) throw new ArgumentException($"[PortAddressData] Universe must be 0 to 32767. {universe}");
            }

            public readonly byte Net => _net;
            public readonly byte SubNet => _subNet;
            public readonly byte Universe => _universe;

            public readonly ushort ToUniverse()
            {
                return (ushort)(_net << 8 | _subNet << 4 | _universe);
            }

            public override string ToString()
            {
                return $"Net: {_net}, SubNet: {_subNet}, Universe: {_universe}";
            }
        }
    }
}