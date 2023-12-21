// https://art-net.org.uk/how-it-works/streaming-packets/artdmx-packet-definition/

using System;
using HRYooba.ArtNet.Core;

namespace HRYooba.ArtNet.Packet
{
    /// <summary>
    /// ArtDmxPacket
    /// </summary>
    public class ArtDmxPacket
    {
        /// <summary>
        /// ArtDmxPacket. Length must be 18 to 530 bytes. 主に受信用(UdpReceiveResult.Bufferをそのまま渡す)
        /// </summary>
        /// <param name="data">all bytes</param>
        public ArtDmxPacket(byte[] data)
        {
            if (data.Length < 18 || data.Length > 530)
            {
                throw new ArgumentOutOfRangeException(nameof(data), data.Length, "[ArtDmxPacket] data must be 18 to 530 bytes.");
            }

            ID = new byte[8];
            for (var i = 0; i < 8; i++)
            {
                ID[i] = data[i];
            }
            OpCode = (OpCodeType)(data[8] | data[9] << 8);
            ProtVer = (ushort)(data[10] << 8 | data[11]);
            Sequence = data[12];
            Physical = data[13];
            Universe = (ushort)(data[14] | data[15] << 8);
            Length = (ushort)(data[16] << 8 | data[17]);
            Data = new byte[Length];
            for (var i = 0; i < Length; i++)
            {
                Data[i] = data[18 + i];
            }
        }

        /// <summary>
        /// ArtDmxPacket. Length must be 1 to 512 bytes. 主に送信用
        /// </summary>
        /// <param name="protVer"></param>
        /// <param name="sequence"></param>
        /// <param name="physical"></param>
        /// <param name="universe"></param>
        /// <param name="data"></param>
        public ArtDmxPacket(ushort protVer, byte sequence, byte physical, ushort universe, byte[] data)
        {
            if (data.Length < 1 || data.Length > 512)
            {
                throw new ArgumentOutOfRangeException(nameof(data), data.Length, "[ArtDmxPacket] data must be 1 to 512 bytes.");
            }

            ID = ArtNetDefine.ID;
            OpCode = OpCodeType.OpDmx;
            ProtVer = protVer;
            Sequence = sequence;
            Physical = physical;
            Universe = universe;
            Length = (ushort)data.Length;
            Data = data;
        }

        /// <summary>
        /// ArtDmxPacket
        /// </summary>
        /// <param name="universe"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ArtDmxPacket(ushort universe, byte[] data) : this(14, 0, 0, universe, data) { }

        public byte[] ID { get; private set; }
        public OpCodeType OpCode { get; private set; }
        public ushort ProtVer { get; private set; } // ProtVerHi and ProtVerLo (default 14)
        public byte Sequence { get; private set; }
        public byte Physical { get; private set; }
        public ushort Universe { get; private set; } // SubUni and Net
        public ushort Length { get; private set; } // LengthHi and LengthLo
        public byte[] Data { get; private set; }

        /// <summary>
        /// ToBytes
        /// [0] ID[] 8bytes
        /// [8] OpCodeLo 1byte
        /// [9] OpCodeHi 1byte
        /// [10] ProtVerHi 1byte
        /// [11] ProtVerLo 1byte
        /// [12] Sequence 1byte
        /// [13] Physical 1byte
        /// [14] SubUni 1byte (Low 8bit of Port-Address)
        /// [15] Net 1byte (High 8bit of Port-Address)
        /// [16] LengthHi 1byte
        /// [17] LengthLo 1byte
        /// [18~] Data[] Length bytes
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            var data = new byte[18 + Length];
            for (var i = 0; i < 8; i++)
            {
                data[i] = ID[i];
            }
            data[8] = (byte)((ushort)OpCode & 0xFF); // OpCode & 0000000011111111 で下位8bitを取得
            data[9] = (byte)((ushort)OpCode >> 8); // OpCode 右に8bitシフトして上位8bitを取得
            data[10] = (byte)(ProtVer >> 8);
            data[11] = (byte)(ProtVer & 0xFF);
            data[12] = Sequence;
            data[13] = Physical;
            data[14] = (byte)(Universe & 0xFF);
            data[15] = (byte)(Universe >> 8);
            data[16] = (byte)(Length >> 8);
            data[17] = (byte)(Length & 0xFF);
            for (var i = 0; i < Length; i++)
            {
                data[18 + i] = Data[i];
            }
            return data;
        }
    }
}