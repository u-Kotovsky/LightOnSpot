using CoreOSC;
using CoreOSC.IO;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LightOnSpotCore.OSC
{
    public class OscResolume
    {
        public static void SendText(string text)
        {
            string address = "/composition/layers/1/clips/11/video/source/textgenerator/text/params/lines";
            var client = new UdpClient("127.0.0.1", 7000);
            var message = new OscMessage(new Address(address), new object[]{ text } );
            Task.Run(async () =>
            {
                await client.SendMessageAsync(message);
            });
        }
    }
}
