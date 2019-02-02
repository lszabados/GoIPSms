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

                while (true)
                {
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Q)
                    {
                        break;
                    }

                    if (key.Key == ConsoleKey.B)
                    {
                        GoIpSmsClient client = new GoIpSmsClient("192.168.14.252", "malacka");
                        client.OnSmsSendEnd += Client_OnSmsSendEnd;
                        client.OnSmsSendError += Client_OnSmsSendError;
                        client.OnSmsSendMessage += Client_OnSmsSendMessage;
                        client.SendBulkSMS("Helló", new string[] { "06209472212", "06209472212" });
                    }
                }

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

        private static void Client_OnSmsSendMessage(object server, GoIPSendMessageEventArgs eventArgs)
        {
            Console.WriteLine("Bulk SMS Info: "+eventArgs.PhoneNumber+", "+eventArgs.SendId+", "+eventArgs.Status);
        }

        private static void Client_OnSmsSendError(object server, GoIPSmsSendErrorEventArgs eventArgs)
        {
            Console.WriteLine("Bulk SMS ERROR: " + eventArgs.SendId + ", " + eventArgs.Message);
        }

        private static void Client_OnSmsSendEnd(object server, GoIPSmsSendEndEventArgs eventArgs)
        {
            Console.WriteLine("Bulk SMS sending ended.");
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
