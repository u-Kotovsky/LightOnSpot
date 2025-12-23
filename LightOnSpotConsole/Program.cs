using LightOnSpotLibrary;

namespace LightOnSpotConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running ArtNet");

            //var manager = new ArtNetManager();

            //manager.Start();

            var unityDmx = new UnityDmxWrapper();

            unityDmx.Start();

            Console.WriteLine("After start");
            Console.ReadLine();
        }
    }
}
