using System.Net.Sockets;
using CoreOSC;
using CoreOSC.IO;

namespace LightOnSpotCore.OSC
{
    public class OscSender
    {
        private UdpClient client;

        public OscSender(string address = "127.0.0.1", int port = 7000)
        {
            client = new UdpClient(address, port);
        }

        public void Send(string address, string value)
        {
            //string address = "/composition/layers/1/clips/11/video/source/textgenerator/text/params/lines";
            var message = new OscMessage(new Address(address), [..value]);
            Task.Run(async () =>
            {
                await client.SendMessageAsync(message);
            });
        }
    }
}
