using System.Net;
using ArtNet.Packets;
using ArtNet.Sockets;
using ArtNet.Enums;

namespace Unity_DMX.Core
{
    public class DmxController
    {
        private DmxBuffer? dmxBuffer;
        private bool useBroadcast;
        private string remoteIp = "127.0.0.1";
        private IPEndPoint? remote; // For sending packets?
        private int remotePort = 6454;
        private bool isServer;

        private bool redirectPackets;
        public bool RedirectPackets
        {
            get { return redirectPackets; }
            set { redirectPackets = value; }
        }
        private DmxController? redirectTo;

        private ArtNetSocket? socket;
        private ArtNetDmxPacket? dmxToSend;
        
        private const string Prefix = "DmxControlller";
        private string ServerOrClient => isServer ? "server" : "client";

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
            if (socket == null) return;
            socket.Close();
        }

        public void SetRedirect(DmxController controller)
        {
            redirectTo = controller;
        }

        public void SetRemote(string ip, int port)
        {
            remoteIp = ip;
            remotePort = port;
        }

        public void InitializeSocket()
        {
            Console.WriteLine($"'{Prefix}' '{remotePort}' is " + ServerOrClient);
            socket = new ArtNetSocket(remotePort);
            socket.NewPacket += OnNewPacketReceived;

            IPAddress address = NetworkUtility.FindFromHostName(remoteIp);


            if (isServer) // !useBroadcast || !isServer
            {
                // When you set the subnet mask, it will set the address you do not send to yourself (Convenient!）
                // However, debugging becomes troublesome
                socket.Open(address, remotePort, address);
            }
            else
            {
                remote = new IPEndPoint(address, remotePort);
            }

            if (dmxBuffer == null) return;
            dmxBuffer.OnBufferUpdate += OnBufferUpdate;
        }

        #region ArtNet Toggle
        public bool IsArtNetOn { get; private set; }
        public void StartArtNet()
        {
            Console.WriteLine($"'{Prefix}' '{remotePort}' ({ServerOrClient}) was requested to be started.");
            if (IsArtNetOn) return;
            InitializeSocket();
            IsArtNetOn = true;
        }

        public void StopArtNet()
        {
            Console.WriteLine($"'{Prefix}' '{remotePort}' ({ServerOrClient}) was requested to be stopped.");
            if (!IsArtNetOn || socket == null || dmxBuffer == null) return;
            socket.Close();
            socket.NewPacket -= OnNewPacketReceived;
            socket = null;
            remote = null;
            IsArtNetOn = false;
            dmxBuffer.OnBufferUpdate -= OnBufferUpdate;
        }
        #endregion

        #region Buffer or DMX512 data
        private Dictionary<int, byte[]> dmxDataMap;
        public event Action<short, DmxData, DmxData> OnDmxDataChanged = delegate { };

        public void ForceBufferUpdate()
        {
            if (dmxBuffer == null) return;
            OnBufferUpdate(dmxBuffer.Buffer);
        }

        private void OnBufferUpdate(DmxData buffer)
        {
            if (dmxBuffer == null) return;
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
        
        
        private void OnNewPacketReceived(object sender, NewPacketEventArgs<ArtNetPacket> e)
        {
            try
            {
                if (e.Packet.OpCode != ArtNetOpCodes.Dmx) return;
                
                var packet = e.Packet as ArtNetDmxPacket;
                if (packet == null) throw new NullReferenceException();
                
                if (dmxDataMap.ContainsKey(packet.Universe) && dmxDataMap[packet.Universe] == packet.DmxData) 
                    return;
                if (dmxDataMap.Count < packet.Universe)
                {
                    for (int i = dmxDataMap.Count; i <= packet.Universe; i++)
                        dmxDataMap.Add(i, i == packet.Universe ? packet.DmxData : new byte[512]);
                }
                else
                {
                    dmxDataMap[packet.Universe] = packet.DmxData;
                }

                dmxDataMap[packet.Universe] = packet.DmxData;

                if (dmxBuffer == null) return;
            
                // Server only?
                BufferUtility.WriteDmxToGlobalBuffer(ref dmxBuffer.Buffer, ref packet, (universe, data) =>
                {
                    OnDmxDataChanged?.Invoke(universe, data, dmxBuffer.Buffer);
                    SendDmxData(universe);
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Send universe buffer if we are redirecting packets.
        /// </summary>
        /// <param name="universe"></param>
        private void SendDmxData(short universe)
        {
            if (!redirectPackets) return;
            // TODO: look into comparing redirectTo component in a different way (optimization)
            if (redirectTo == null) return;
            if (dmxBuffer == null) return;
            BufferUtility.SendUniverseFromGlobalBuffer(redirectTo, universe, dmxBuffer.Buffer);
        }
        
        public void Send(short universe, DmxData dmxData)
        {
            if (dmxData == null) throw new Exception("Dmx data is null");
            if (dmxData.Count != 512) throw new Exception($"Dmx data length is outside of bounds {dmxData.Count}/512");
            if (dmxToSend == null) throw new Exception("Dmx to send is null");
            if (dmxToSend.DmxData == null) throw new Exception("Dmx data to send is null");
            if (socket == null) return; // ignore that
            
            dmxToSend.Universe = universe;
            dmxToSend.DmxData.BlockCopy(dmxData.GetBufferArray(), 0, 0, 512);

            if (useBroadcast && isServer)
            {
                socket.Send(dmxToSend);
            }
            else if (remote != null)
            {
                socket.Send(dmxToSend, remote);
            }
            else
            {
                // Fail?
            }
        } 
    }
}
