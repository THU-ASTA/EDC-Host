using System;

namespace EdcHost;

/// <summary>
/// A packet for communication.
/// </summary>
public abstract class Packet
{
    #region Public static methods.

    /// <summary>
    /// Calculate the checksum of a raw byte array.
    /// </summary>
    /// <param name="bytes">The raw byte array.</param>
    /// <returns>The checksum.</returns>
    public static byte CalculateChecksum(byte[] bytes)
    {
        byte checksum = 0x00;
        foreach (var byte_item in bytes)
        {
            checksum ^= byte_item;
        }
        return checksum;
    }

    /// <summary>
    /// Extract the data from a packet in raw byte array form.
    /// </summary>
    /// <param name="bytes">
    /// The packet in raw byte array form.
    /// </param>
    /// <returns>The data.</returns>
    public static byte[] ExtractPacketData(byte[] bytes)
    {
        // Validate the byte array
        if (bytes.Length < 6)
        {
            throw new Exception("The header of the packet is broken.");
        }

        byte packetId = bytes[0];
        uint dataLength = BitConverter.ToUInt32(bytes, 1);
        byte checksum = bytes[5];

        if (bytes.Length < dataLength + 6)
        {
            throw new Exception("The data length of the packet is incorrect.");
        }

        var data = new byte[dataLength];
        Array.Copy(bytes, 6, data, 0, dataLength);

        if (checksum != Packet.CalculateChecksum(data))
        {
            throw new Exception("The data of the packet is broken.");
        }

        return data;
    }

    /// <summary>
    /// Generate the header of some data.
    /// </summary>
    /// <param name="packetId">The packet ID.</param>
    /// <param name="data">The data.</param>
    /// <returns></returns>
    public static byte[] GeneratePacketHeader(byte packetId, byte[] data)
    {
        uint dataLength = (uint)data.Length;
        byte checksum = Packet.CalculateChecksum(data);

        var header = new byte[6];
        header[0] = packetId;
        Array.Copy(BitConverter.GetBytes(dataLength), 0, header, 1, 4);
        header[5] = checksum;

        return header;
    }

    /// <summary>
    /// Make a packet from a raw byte array.
    /// </summary>
    /// <param name="bytes">The raw byte array.</param>
    /// <returns>The packet.</returns>
    public static Packet Make(byte[] bytes)
    {
        if (bytes.Length < 6)
        {
            throw new Exception("The packet is broken.");
        }

        byte packetId = bytes[0];

        switch (packetId)
        {
            case PacketGetGameInformationSlave.PacketId:
                return new PacketGetGameInformationSlave(bytes);

            case PacketGetGameInformationHost.PacketId:
                return new PacketGetGameInformationHost(bytes);

            case PacketSetChargingPileSlave.PacketId:
                return new PacketSetChargingPileSlave(bytes);

            case PacketGetStatusHost.PacketId:
                return new PacketGetStatusHost(bytes);

            default:
                throw new Exception("The packet ID is invalid.");
        }
    }

    #endregion

    #region Public member methods.

    /// <summary>
    /// Get the raw byte array of the packet.
    /// </summary>
    /// <returns>The raw byte array.</returns>
    public abstract byte[] GetBytes();

    #endregion
}