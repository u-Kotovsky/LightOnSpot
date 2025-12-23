using System;
using System.Collections.Generic;

namespace Unity_DMX.Core
{
    public class DmxBuffer
    {
        public DmxData Buffer;
        
        public event Action<DmxData> OnBufferUpdate = delegate { };

        public DmxBuffer(int size = 0)
        {
            Buffer = new DmxData(size);
        }

        public void Update()
        {
            OnBufferUpdate?.Invoke(Buffer);
        }
    }
}
