using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Voxo.GoIpSmsServer
{
    public struct UdpState
    {
        public UdpClient u;
        public IPEndPoint e;
    }

    public enum ServerStatus : byte { Stopped, Started, Stopping, Starting }

    public class GoIPSmsServer
    {
        private readonly object balanceLock = new object();
        private bool messageReceived = false;
        private GoIPSmsServerOptions _options;
        private CancellationTokenSource cancelationTokenSource;

        // registration event
        public delegate void RegistrationHandler(object server, GoIPRegisterEventArgs registerData);
        public event RegistrationHandler OnRegistration;

        // message event
        public delegate void MessageHandler(object server, GoIPMessageEventArgs messageData);
        public event MessageHandler OnMessage;

        public bool ServerStarted { get { return Status == ServerStatus.Started; } }
        public ServerStatus Status { get; internal set; } = ServerStatus.Stopped;


        public GoIPSmsServer(GoIPSmsServerOptions options)
        {
            _options = options;
        }

        public void StartServer()
        {
            if (Status == ServerStatus.Stopped)
            {
                Status = ServerStatus.Starting;
                cancelationTokenSource = new CancellationTokenSource();
                GoIPSmsServerListener(_options.Port, cancelationTokenSource.Token);
            } // else log?
        }

        public void StopServer()
        {
            if (Status == ServerStatus.Started)
            {
                Status = ServerStatus.Stopping;
                cancelationTokenSource.Cancel();
            }
        }

        private Task GoIPSmsServerListener(int port, CancellationToken cancellationToken)
        {
            Task task = null;

            // Start a task and return it
            task = Task.Run(() =>
            {
                ReceiveMessages(cancellationToken);

                Status = ServerStatus.Stopped;               
            });

            Status = ServerStatus.Started;

            return task;
        }

        public void ReceiveMessages(CancellationToken cancellationToken)
        {
            // Receive a message
            IPEndPoint e = new IPEndPoint(IPAddress.Any, _options.Port);
            UdpClient u = new UdpClient(e);

            UdpState s = new UdpState();
            s.e = e;
            s.u = u;

            while (true)
            {
                u.BeginReceive(new AsyncCallback(ReceiveCallback), s);

                // Do some work while we wait for a message. For this example, we'll just sleep
                while (!messageReceived && !cancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(100);
                }

                messageReceived = false;

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }

            Status = ServerStatus.Stopped;
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            UdpState us = (UdpState)(ar.AsyncState);
            
            byte[] receiveBytes = us.u.EndReceive(ar, ref us.e);
            string receiveString = Encoding.ASCII.GetString(receiveBytes);

            ExtractData(receiveString, us.e.Address.ToString(), us.e.Port);
            
            messageReceived = true;
        }

        private void Send(string data, string host, int port)
        {
            UdpClient udpClient = new UdpClient(host, port);
            Byte[] sendBytes = Encoding.UTF8.GetBytes(data);
            try
            {
                udpClient.Send(sendBytes, sendBytes.Length);
            }
            catch (Exception e)
            {
                // TODO : log?
            }
        }

        private void SendThread(string data, string host, int port)
        {
            Task.Run(() =>
            {
                Send(data, host, port);
            });
        }     

        /// <summary>
        /// A beérkező adatokat értelmezi
        /// </summary>
        /// <param name="Data">UDP adat</param>
        private void ExtractData(string Data, string host, int port)
        {
            if (Data.StartsWith("req:"))
            {
                Registration(Data, host, port);
            }

            if (Data.StartsWith("RECEIVE:"))
            {
                ReceiveSms(Data, host, port);
            }
        }

        private void ReceiveSms(string data, string host, int port)
        {
            GoIPMessagePacket packet = new GoIPMessagePacket(data);
            // if auth error  
            if (packet.Id != _options.ServerId || packet.Password != _options.AuthPassword)
            {
                // TODO: log?
                Send(ACKPacketFactory.RECEIVE_SMS_ACK(packet.ReceiveId, "Authentication error!"), host, port);
                OnMessage(this, new GoIPMessageEventArgs("Authentication error!", packet, host, port));
                return;
            }

            packet.Password = "";  // Delete password for security reasons

            Send(ACKPacketFactory.RECEIVE_SMS_ACK(packet.ReceiveId, ""), host, port);
            OnMessage(this, new GoIPMessageEventArgs("OK", packet, host, port));
        }

        /// <summary>
        /// Extract registration packet
        /// </summary>
        /// <param name="data"></param>
        private void Registration(string data, string host, int port)
        {
            GoIPRegistrationPacket packet = new GoIPRegistrationPacket(data);

            // if auth error  
            if (packet.id != _options.ServerId || packet.password != _options.AuthPassword)
            {
                // TODO: log?
                Send(ACKPacketFactory.ACK_MESSAGE(packet.req, 400), host, port);
                OnRegistration(this, new GoIPRegisterEventArgs("Authentication error!", packet, host, port, 400));
                return;
            }

            packet.password = "";  // Delete password for security reasons

            if (string.IsNullOrEmpty(packet.imei))
            {
                // TODO: log?
                Send(ACKPacketFactory.ACK_MESSAGE(packet.req, 400), host, port);
                OnRegistration(this, new GoIPRegisterEventArgs("No IMEI! (No SIM?)", packet, host, port, 400));
                return;
            }

            Send(ACKPacketFactory.ACK_MESSAGE(packet.req, 200), host, port);
            OnRegistration(this, new GoIPRegisterEventArgs("OK", packet, host, port, 400));
        }
    }
}
