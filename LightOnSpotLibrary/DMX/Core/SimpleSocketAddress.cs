using System.Net;
using Unity_DMX;

namespace LightOnSpot.Core.unity.dmx.Core
{
    public struct SimpleSocketAddress
    {
        private string _address;
        public string Address { get { return _address; } }
        private short _port;
        public short Port { get { return _port; } }

        public SimpleSocketAddress(string address, short port)
        {
            _address = address;
            _port = port;
        }

        public void Set(string address, short port)
        {
            _address = address;
            _port = port;
        }

        private IPAddress host;
        public IPAddress Host
        {
            get
            {
                if (host == null) NetworkUtility.FindFromHostName(_address);
                if (host == null) host = IPAddress.Loopback;
                return host;
            }
        }

        private IPEndPoint? endPoint = null;
        public IPEndPoint EndPoint
        {
            get
            {
                if (endPoint == null) endPoint = new IPEndPoint(IPAddress.Parse(_address), _port);
                return endPoint;
            }
        }
    }
}
