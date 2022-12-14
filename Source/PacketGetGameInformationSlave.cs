using System;

namespace EdcHost;

/// <summary>
/// A packet for slaves to get site information.
/// </summary>
public class PacketGetGameInformationSlave : Packet
{
    #region Static, const and readonly fields.

    /// <summary>
    /// The packet ID.
    /// </summary>
    public const byte PacketId = 0x00;

    #endregion


    #region Constructors and finalizers.
    /// <summary>
    /// Construct a GetGameInformationSlave packet with fields.
    /// </summary>
    /// <remarks>
    /// There is no field in this type of packets.
    /// </remarks>
    public PacketGetGameInformationSlave()
    {
    }

    /// <summary>
    /// Construct a GetGameInformationSlave packet from a raw
    /// byte array.
    /// </summary>
    /// <param name="bytes">The raw byte array.</param>
    public PacketGetGameInformationSlave(byte[] bytes)
    {
        // Validate the packet and extract data.
        var data = Packet.ExtractPacketData(bytes);

        // Check the packet ID.
        byte packetId = bytes[2];
        if (packetId != PacketGetGameInformationSlave.PacketId)
        {
            throw new Exception("The packet ID is incorrect.");
        }
    }

    #endregion


    #region Methods.

    public override byte[] GetBytes()
    {
        var data = new byte[0];

        var header = Packet.GeneratePacketHeader(PacketGetGameInformationSlave.PacketId, data);

        var bytes = new byte[header.Length + data.Length];
        header.CopyTo(bytes, 0);
        data.CopyTo(bytes, header.Length);

        return bytes;
    }

    public override byte GetPacketId()
    {
        return PacketGetGameInformationSlave.PacketId;
    }

    #endregion
}