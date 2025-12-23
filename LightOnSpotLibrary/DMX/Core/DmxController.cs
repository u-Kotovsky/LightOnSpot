using System.Net;
using ArtNet.Packets;
using ArtNet.Sockets;
using ArtNet.Enums;
using LightOnSpot.Core.unity.dmx.Core;

namespace Unity_DMX.Core
{
    public class DmxController
    {
        internal const string Prefix = "DmxControlller";
        internal DmxBuffer? dmxBuffer;

        #region Properties
        internal bool isBroadcast;
        public bool IsBroadcast
        {
            get { return isBroadcast; }
            set { isBroadcast = value; }
        }

        internal bool isServer;
        public bool IsServer
        {
            get { return isServer; }
            set { isServer = value; }
        }
        public string IsServerOrClient => isServer ? "server" : "client";

        internal SimpleSocketAddress remoteAddress;
        public SimpleSocketAddress RemoteAddress
        {
            get { return remoteAddress; }
            set { remoteAddress = value; }
        }


        internal bool redirectPackets;
        public bool RedirectPackets
        {
            get { return redirectPackets; }
            set { redirectPackets = value; }
        }

        internal DmxController? dmxOutput;
        public DmxController? DmxOutput
        {
            get { return dmxOutput; }
            set { dmxOutput = value; }
        }
        #endregion

        internal ArtNetSocket? socket;
        internal ArtNetDmxPacket? dmxToSend;
        
        public DmxController(DmxBuffer? dmxBuffer = null)
        {
            dmxToSend ??= new ArtNetDmxPacket();
            dmxToSend.DmxData ??= new byte[512];
            dmxDataMap = [];

            if (dmxBuffer != null)
            {
                this.dmxBuffer = dmxBuffer;
            }
            else
            {
                dmxBuffer = new DmxBuffer();
            }
            dmxBuffer.OnBufferUpdate += OnBufferUpdate;
        }
        
        ~DmxController()
        {
            if (socket == null)
            {
                return;
            }
            socket.Close();
        }

        public void InitializeSocket()
        {
            Console.WriteLine($"'{Prefix}' '{remoteAddress.Port}' is {IsServerOrClient}");
            socket = new ArtNetSocket(remoteAddress.Port);
            socket.NewPacket += OnNewPacketReceived;

            IPAddress address = remoteAddress.Host;
            if (isServer) // !useBroadcast || !isServer
            {
                // When you set the subnet mask, it will set the address you do not send to yourself (Convenient!）
                // However, debugging becomes troublesome
                socket.Open(address, remoteAddress.Port, address);
            }

            if (dmxBuffer == null)
            {
                return;
            }
            dmxBuffer.OnBufferUpdate += OnBufferUpdate;
        }

        #region ArtNet Toggle
        public bool IsArtNetOn { get; private set; }

        /// <summary>
        /// Starts ArtNet
        /// </summary>
        public void StartArtNet()
        {
            Console.WriteLine($"'{Prefix}' '{remoteAddress.Port}' ({IsServerOrClient}) was requested to be started.");
            if (IsArtNetOn)
            {
                return;
            }
            InitializeSocket();
            IsArtNetOn = true;
        }

        /// <summary>
        /// Stops ArtNet
        /// </summary>
        public void StopArtNet()
        {
            Console.WriteLine($"'{Prefix}' '{remoteAddress.Port}' ({IsServerOrClient}) was requested to be stopped.");

            if (!IsArtNetOn || socket == null || dmxBuffer == null)
            {
                return;
            }

            socket.Close();
            socket.NewPacket -= OnNewPacketReceived;
            socket = null;
            IsArtNetOn = false;
            dmxBuffer.OnBufferUpdate -= OnBufferUpdate;
        }
        #endregion

        #region Buffer or DMX512 data
        private Dictionary<int, byte[]> dmxDataMap; // TODO: I know this thing is weird, I will look into new solution later..
        public event Action<short, DmxData, DmxData> OnDmxDataChanged = delegate { };

        public void ForceBufferUpdate()
        {
            if (dmxBuffer == null)
            {
                throw new Exception("buffer is null");
            }

            OnBufferUpdate(dmxBuffer.Buffer);
        }

        /// <summary>
        /// When buffer updates it does post-processing
        /// </summary>
        /// <param name="buffer"></param>
        /// <exception cref="Exception"></exception>
        private void OnBufferUpdate(DmxData buffer)
        {
            if (dmxBuffer == null)
            {
                throw new Exception("buffer is null");
            }

            // Ensure we have at least one universe
            dmxBuffer.Buffer.EnsureCapacity(512);
            
            var universeCount = (short)(dmxBuffer.Buffer.Count / 512);
            for (short i = 0; i < universeCount; i++)
            {
                var universeBuffer = BufferUtility.TakeUniverseFromGlobalBuffer(i, dmxBuffer.Buffer);
                OnDmxDataChanged?.Invoke(i, universeBuffer, dmxBuffer.Buffer);
                SendDmxData(i);
            }
        }
        #endregion
        
        /// <summary>
        /// Once we receive a packet, we put it in our data pool.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NullReferenceException"></exception>
        private void OnNewPacketReceived(object sender, NewPacketEventArgs<ArtNetPacket> e)
        {
            if (e.Packet.OpCode != ArtNetOpCodes.Dmx) return; // We look only for DMX
            
            var packet = e.Packet as ArtNetDmxPacket ?? throw new NullReferenceException();
            if (packet.DmxData == null)
            {
                return;
            }
            if (dmxDataMap.ContainsKey(packet.Universe) && dmxDataMap[packet.Universe] == packet.DmxData)
            {
                return;
            }

            if (dmxDataMap.Count < packet.Universe)
            {
                for (int i = dmxDataMap.Count; i <= packet.Universe; i++)
                {
                    if (i == packet.Universe)
                    {
                        dmxDataMap.Add(i, packet.DmxData);
                    }
                    else
                    {
                        dmxDataMap.Add(i, new byte[512]);
                    }
                }
            }
            else
            {
                dmxDataMap[packet.Universe] = packet.DmxData;
            }

            dmxDataMap[packet.Universe] = packet.DmxData;

            if (dmxBuffer == null)
            {
                return;
            }
            
            // Server only?
            BufferUtility.WriteDmxToGlobalBuffer(ref dmxBuffer.Buffer, ref packet, (universe, data) =>
            {
                OnDmxDataChanged?.Invoke(universe, data, dmxBuffer.Buffer);
                SendDmxData(universe);
            });
        }

        /// <summary>
        /// Send universe buffer if we are redirecting packets.
        /// </summary>
        /// <param name="universe"></param>
        public void SendDmxData(short universe)
        {
            if (!redirectPackets || dmxOutput == null || dmxBuffer == null)
            {
                return;
            }

            BufferUtility.SendUniverseFromGlobalBuffer(dmxOutput, universe, dmxBuffer.Buffer);
        }
        
        /// <summary>
        /// Send DMX512 with specified universe
        /// </summary>
        /// <param name="universe"></param>
        /// <param name="dmxData"></param>
        /// <exception cref="Exception"></exception>
        public void Send(short universe, DmxData dmxData)
        {
            if (dmxData == null)
            {
                throw new Exception("Dmx data is null");
            }
            if (dmxData.Count != 512)
            {
                throw new Exception($"Dmx data length is outside of bounds {dmxData.Count}/512");
            }
            if (dmxToSend == null)
            {
                throw new Exception("Dmx to send is null");
            }
            if (dmxToSend.DmxData == null)
            {
                throw new Exception("Dmx data to send is null");
            }
            if (socket == null)
            {
                return; // Ignore that
            }
            
            dmxToSend.Universe = universe;
            dmxToSend.DmxData.BlockCopy(dmxData.GetBufferArray(), 0, 0, 512);

            if (isBroadcast && isServer)
            {
                socket.Send(dmxToSend);
                return;
            }
            else if (remoteAddress.EndPoint != null)
            {
                socket.Send(dmxToSend, remoteAddress.EndPoint);
                return;
            }

            throw new Exception("Something weird happened here!");
        } 
    }
}
