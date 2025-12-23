using ArtNet.Packets;
using Unity_DMX.Core;

namespace Unity_DMX
{
    public static class BufferUtility
    {
        /// <summary>
        /// This will send a whole universe to a target from global buffer.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="universe"></param>
        /// <param name="buffer"></param>
        public static void SendUniverseFromGlobalBuffer(DmxController target, short universe, DmxData buffer)
        {
            var dmxData = TakeUniverseFromGlobalBuffer(universe, buffer);
            
            target.Send(universe, dmxData);
        }

        /// <summary>
        /// This will write a ArtNetDmxPacket into global buffer and then invokes a callback with changed universe.
        /// </summary>
        /// <param name="globalDmxBuffer"></param>
        /// <param name="packet"></param>
        /// <param name="callback"></param>
        public static void WriteDmxToGlobalBuffer(ref DmxData globalDmxBuffer, ref ArtNetDmxPacket packet, Action<short, DmxData> callback)
        {
            int offset = packet.Universe * 512;

            DmxData dmxData = new DmxData(packet.DmxData);
            globalDmxBuffer.SetRange(offset, dmxData);
            
            callback?.Invoke(packet.Universe, TakeUniverseFromGlobalBuffer(packet.Universe, globalDmxBuffer));
        }

        /// <summary>
        /// This will take a whole universe from global buffer.
        /// </summary>
        /// <param name="universe"></param>
        /// <param name="globalDmxBuffer"></param>
        /// <returns></returns>
        public static DmxData TakeUniverseFromGlobalBuffer(short universe, DmxData globalDmxBuffer)
        {
            int srcOffset = universe * 512;
            DmxData buffer = new DmxData(512);

            buffer.SetRange(0, globalDmxBuffer, srcOffset, 512);
            
            return buffer;
        }
    }
}
