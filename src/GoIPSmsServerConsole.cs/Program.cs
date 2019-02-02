using System;
using System.Threading.Tasks;
using Voxo.GoIpSmsServer;

namespace GoIPSmsServerConsole.cs
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("GoIP Sms Server");
            Console.Write("Starting....");
            GoIPSmsServer server = new GoIPSmsServer(new GoIPSmsServerOptions() {
                AuthPassword = "malacka",
                Port = 44444,
                ServerId = "lacika"
            });

            server.OnRegistration += Server_OnRegistration;
            server.OnMessage += Server_OnMessage;

            server.StartServer();
            while (server.Status == ServerStatus.Starting)
            {
                Console.Write(".");
                System.Threading.Thread.Sleep(1000);
            }

            if (server.Status == ServerStatus.Stopping)
            {
                Console.WriteLine("Starting error!");
                return;
            }

            if (server.Status == ServerStatus.Started)
            {
                Console.WriteLine("Ok");
                Console.WriteLine("Press any key to exit....");
                Console.ReadKey();

                //while (true)
                //{
                //    System.Threading.Thread.Sleep(100);
                //}

            }

            Console.Write("Stopping....");
            server.StopServer();
            
            while (server.Status != ServerStatus.Stopped)
            {
                Console.Write(".");
                System.Threading.Thread.Sleep(1000);
            }

            Console.WriteLine("Ok");
            Console.WriteLine("Good by");
        }

        private static void Server_OnMessage(object server, GoIPMessageEventArgs messageData)
        {
            if (messageData.Message == "OK")
            {
                Console.WriteLine(DateTime.Now.ToString() + "  Host: " + messageData.Host + ", Sms sender:" + messageData.Packet.Srcnum + ", Message: " + messageData.Packet.Message);
            }
            else
            {
                Console.WriteLine(DateTime.Now.ToString() + "  Host: " + messageData.Host + ", Sms receive failed:" + messageData.Message);
            }
            
        }

        private static void Server_OnRegistration(object server, GoIPRegisterEventArgs registerData)
        {
            Console.WriteLine(DateTime.Now.ToString()+"  Host: "+registerData.Host+", Req:"+registerData.Packet.req+", Message: "+registerData.Message);
        }
    }
}
