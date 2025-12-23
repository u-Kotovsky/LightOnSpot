namespace Unity_DMX.Core
{
    public static class Extensions
    {
        /// <summary>
        /// This will ensure buffer has same or more capacity.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="capacity"></param>
        public static byte[] EnsureCapacity(this byte[] buffer, int capacity)
        {
            if (buffer.Length < capacity)
            {
                var temp = new byte[capacity];
                Buffer.BlockCopy(buffer, 0, temp, 0, buffer.Length);
                buffer = temp;
            }
            
            return buffer;
        }


        public static byte[] BlockCopy(this byte[] dstBuffer, byte[] srcBuffer, int srcOffset, int dstOffset, int count)
        {
            Buffer.BlockCopy(srcBuffer, srcOffset, dstBuffer, dstOffset, count);
            return dstBuffer;
        }

        /// <summary>
        /// This will ensure buffer has same or more capacity.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="capacity"></param>
        /// <returns></returns>
        public static List<byte> EnsureCapacity(this List<byte> buffer, int capacity)
        {
            if (buffer.Count >= capacity) return buffer;

            int range = capacity - buffer.Count;
            buffer.AddRange(new byte[range]);

            return buffer;
        }

        public static List<byte> BlockCopy(this List<byte> dstBuffer, int dstOffset, List<byte> srcBuffer, int srcOffset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                dstBuffer[i + dstOffset] = srcBuffer[i + srcOffset];
            }

            return dstBuffer;
        }

        public static List<byte> BlockCopy(this List<byte> dstBuffer, int dstOffset, byte[] srcBuffer, int srcOffset, int length)
        {
            for (int i = srcOffset; i < length; i++)
            {
                dstBuffer[i + dstOffset] = srcBuffer[i + srcOffset];
            }

            return dstBuffer;
        }
    }
}
