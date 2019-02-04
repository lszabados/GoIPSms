using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Voxo.GoIpSms
{
    public struct UdpState
    {
        public UdpClient client;
        public IPEndPoint localEp;
        public IPEndPoint remoteEp;
    }

    public enum ServerStatus : byte { Stopped, Started, Stopping, Starting }

    /// <summary>
    /// GoIp Sms Server implementation
    /// </summary>
    public class GoIpSmsServer
    {
        private readonly object balanceLock = new object();
        private bool messageReceived = false;
        private GoIpSmsServerOptions _options;
        private readonly ILogger<GoIpSmsServer> _logger;
        private CancellationTokenSource cancelationTokenSource;

        // registration event
        public delegate void RegistrationHandler(object server, GoIpRegisterEventArgs registerData);
        public event RegistrationHandler OnRegistration;

        // message event
        public delegate void MessageHandler(object server, GoIpMessageEventArgs messageData);
        public event MessageHandler OnMessage;

        // delivery report event
        public delegate void DeliveryReportHandler(object server, GoIpDeliveryReportEventArgs messageData);
        public event DeliveryReportHandler OnDeliveryReport;

        // STATE event
        public delegate void StateHandler(object server, GoIpStateEventArgs messageData);
        public event StateHandler OnStateChange;

        // RECORD event
        public delegate void RecordHandler(object server, GoIpRecordEventArgs messageData);
        public event RecordHandler OnRecord;

        // REMAIN event
        public delegate void RemainHandler(object server, GoIpRemainEventArgs messageData);
        public event RemainHandler OnRemain;

        // Cell list event
        public delegate void CellListHandler(object server, GoIpCellListEvenetArgs messageData);
        public event CellListHandler OnCellListChanged;

        public bool ServerStarted { get { return Status == ServerStatus.Started; } }
        public ServerStatus Status { get; internal set; } = ServerStatus.Stopped;

        /// <summary>
        /// GoIpSmsServer constructor
        /// </summary>
        /// <param name="options">Initialization options</param>
        /// <param name="logger">logging interface</param>
        public GoIpSmsServer(IOptions<GoIpSmsServerOptions> options, ILogger<GoIpSmsServer> logger)
        {
            _options = options.Value;
            _logger = logger;
            _logger.LogDebug("Create GoIpSmsServer. AuthId: {0} Listening port: {1}", _options.AuthId, _options.Port);
        }

        public GoIpSmsServer(GoIpSmsServerOptions options, ILogger<GoIpSmsServer> logger)
        {
            _options = options;
            _logger = logger;
            _logger.LogDebug("Create GoIpSmsServer. AuthId: {0} Listening port: {1}", _options.AuthId, _options.Port);
        }

        /// <summary>
        /// Start server instance
        /// </summary>
        public void StartServer()
        {
            if (Status == ServerStatus.Stopped)
            {
                _logger.LogInformation("Server start request. AuthId: {0} Listening port: {1}", _options.AuthId, _options.Port);
                Status = ServerStatus.Starting;
                cancelationTokenSource = new CancellationTokenSource();
                GoIpSmsServerListener(_options.Port, cancelationTokenSource.Token);
            } // else log?
        }

        /// <summary>
        /// Stop server instance
        /// </summary>
        public void StopServer()
        {
            if (Status == ServerStatus.Started)
            {
                _logger.LogInformation("Server stop request. AuthId: {0} Listening port: {1}", _options.AuthId, _options.Port);
                Status = ServerStatus.Stopping;
                cancelationTokenSource.Cancel();
            }
        }

        private Task GoIpSmsServerListener(int port, CancellationToken cancellationToken)
        {
            Task task = null;

            // Start a task and return it
            task = Task.Run(() =>
            {
                try
                {
                    ReceiveMessages(cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogWarning("Server stopped error. AuthId: {0} Listening port: {1} Error message: {2}", _options.AuthId, _options.Port, e.Message);
                }

                Status = ServerStatus.Stopped;
                _logger.LogInformation("Server stopped successfully. AuthId: {0} Listening port: {1}", _options.AuthId, _options.Port);
            });

            Status = ServerStatus.Started;
            
            _logger.LogInformation("Server started successfully. AuthId: {0} Listening port: {1}", _options.AuthId, _options.Port);

            return task;
        }

        private void ReceiveMessages(CancellationToken cancellationToken)
        {
            // Receive a message
            IPEndPoint LocalEp = new IPEndPoint(IPAddress.Any, _options.Port);
            IPEndPoint RemoteEp = new IPEndPoint(IPAddress.Any, 0);
            UdpClient udpClient = new UdpClient(LocalEp);

            UdpState s = new UdpState
            {
                localEp = LocalEp,
                remoteEp = RemoteEp,
                client = udpClient
            };




            while (true)
            {
                s.client.BeginReceive(new AsyncCallback(ReceiveCallback), s);

                // Do some work while we wait for a message. For this example, we'll just sleep
                while (!messageReceived && !cancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(100);
                }

                messageReceived = false;

                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("Server cancellation request. AuthId: {0} Listening port: {1}", _options.AuthId, _options.Port);
                    break;
                }
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            UdpState us = (UdpState)(ar.AsyncState);

            _logger.LogDebug("Start receive data. AuthId: {0} Server port {1}", 
                _options.AuthId, _options.Port);

            string receiveString;

            try
            {
                byte[] receiveBytes = us.client.EndReceive(ar, ref us.remoteEp);

                _logger.LogDebug("Received data. AuthId: {0} Server port {1} Host ip: {2} Host port {3}",
                     _options.AuthId, _options.Port, us.remoteEp.Address.ToString(), us.remoteEp.Port);

                receiveString = Encoding.ASCII.GetString(receiveBytes);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Data receive error. AuthId: {0} Server port {1} Sender host: {2} Sender port: {3} Error message: {4}",
                _options.AuthId, _options.Port, us.remoteEp.Address.ToString(), us.remoteEp.Port, e.Message);
                messageReceived = true;
                return;
            } 

            _logger.LogDebug("End receive data. AuthId: {0} Server port {1} Sender host: {2} Sender port: {3} Data: {4}",
                _options.AuthId, _options.Port, us.remoteEp.Address.ToString(), us.remoteEp.Port, receiveString);

            ExtractData(receiveString, us.remoteEp.Address.ToString(), us.remoteEp.Port);
            
            messageReceived = true;
        }



        private void Send(string data, string host, int port)
        {
            _logger.LogDebug("Start send data. AuthId: {0} Local port: {1} Destination host: {2} Destination port: {3} Data: {4}",
                _options.AuthId, _options.Port, host, port, data);

            UdpClient udpClient = new UdpClient(host, port);
            Byte[] sendBytes = Encoding.UTF8.GetBytes(data);
            try
            {
                udpClient.Send(sendBytes, sendBytes.Length);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Data sending error. AuthId: {0} Local port: {1} Destination host: {2} Destination port: {3} Error message: {4}",
                _options.AuthId, _options.Port, host, port, e.Message);
            }

            _logger.LogDebug("Send data end. AuthId: {0} Local port: {1} Destination host: {2} Destination port: {3}",
                _options.AuthId, _options.Port, host, port);
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
            bool extracted = false;
            
            if (Data.StartsWith("req:"))
            {
                Registration(Data, host, port);
                extracted = true;
            }

            if (Data.StartsWith("RECEIVE:"))
            {
                ReceiveSms(Data, host, port);
                extracted = true;
            }

            if (Data.StartsWith("DELIVER:"))
            {
                DeliverReport(Data, host, port);
                extracted = true;
            }

            if (Data.StartsWith("STATE:"))
            {
                State(Data, host, port);
                extracted = true;
            }

            if (Data.StartsWith("RECORD:"))
            {
                RecordData(Data, host, port);
                extracted = true;
            }

            if (Data.StartsWith("REMAIN:"))
            {
                RemainData(Data, host, port);
                extracted = true;
            }

            if (Data.StartsWith("CELLS:"))
            {
                CellList(Data, host, port);
                extracted = true;
            }

            if (!extracted)
            {
                _logger.LogInformation("Unknown data, not extracted. Data {0}", Data);
            }
        }

        private void CellList(string data, string host, int port)
        {
            _logger.LogDebug("Start GoIp Cell list event");

            GoIpCellListPacket packet = new GoIpCellListPacket(data);

            // if auth error  
            if (packet.authid != _options.AuthId || packet.password != _options.AuthPassword)
            {
                // TODO: log?
                _logger.LogInformation("GoIp Cell list event authentication error. Data: {0}", data);
                Send(ACKPacketFactory.ACK("CELLS", packet.receiveid.ToString(), "Cell list event authentication error!"), host, port);
                OnCellListChanged?.Invoke(this, new GoIpCellListEvenetArgs("GoIp Cell list event authentication error!", packet.celllist, host, port));
                return;
            }

            _logger.LogInformation("Received GoIp Cell list event. ReceiveId: {0} Cell list: {1}", packet.receiveid, packet.celllist);

            Send(ACKPacketFactory.ACK("CELLS", packet.receiveid.ToString(), ""), host, port);
            OnCellListChanged?.Invoke(this, new GoIpCellListEvenetArgs("OK", packet.celllist, host, port));
        }

        private void RemainData(string data, string host, int port)
        {
            _logger.LogDebug("Start GoIp Remain event");

            GoIpRemainPacket packet = new GoIpRemainPacket(data);

            // if auth error  
            if (packet.authid != _options.AuthId || packet.password != _options.AuthPassword)
            {
                // TODO: log?
                _logger.LogInformation("GoIp remain event authentication error. Data: {0}", data);
                Send(ACKPacketFactory.ACK("REMAIN", packet.receiveid.ToString(), "Remain event authentication error!"), host, port);
                OnRemain?.Invoke(this, new GoIpRemainEventArgs("GoIp remain event authentication error!", packet, host, port));
                return;
            }

            packet.password = "";  // Delete password for security reasons

            _logger.LogInformation("Received GoIp record event. ReceiveId: {0} Remain time: {1}", packet.receiveid, packet.gsm_remain_time);

            Send(ACKPacketFactory.ACK("REMAIN", packet.receiveid.ToString(), ""), host, port);
            OnRemain?.Invoke(this, new GoIpRemainEventArgs("OK", packet, host, port));
        }

        private void RecordData(string data, string host, int port)
        {
            _logger.LogDebug("Start GoIp Record event");

            GoIpRecordPacket packet = new GoIpRecordPacket(data);

            // if auth error  
            if (packet.authid != _options.AuthId || packet.password != _options.AuthPassword)
            {
                // TODO: log?
                _logger.LogInformation("GoIp record event authentication error. Data: {0}", data);
                Send(ACKPacketFactory.ACK("RECORD", packet.receiveid.ToString(), "Record event authentication error!"), host, port);
                OnRecord?.Invoke(this, new GoIpRecordEventArgs("GoIp record event authentication error!", packet, host, port));
                return;
            }

            packet.password = "";  // Delete password for security reasons

            _logger.LogInformation("Received GoIp record event. ReceiveId: {0} Send num: {1} Direction: {2}", packet.receiveid, packet.send_num, packet.direction);

            Send(ACKPacketFactory.ACK("RECORD", packet.receiveid.ToString(), ""), host, port);
            OnRecord?.Invoke(this, new GoIpRecordEventArgs("OK", packet, host, port));
        }

        private void State(string data, string host, int port)
        {
            _logger.LogDebug("Start GoIp State");

            GoIpStatePacket packet = new GoIpStatePacket(data);

            // if auth error  
            if (packet.authid != _options.AuthId || packet.password != _options.AuthPassword)
            {
                // TODO: log?
                _logger.LogInformation("GoIp state report authentication error. Data: {0}", data);
                Send(ACKPacketFactory.ACK("STATE", packet.receiveid.ToString(), "State authentication error!"), host, port);
                OnStateChange?.Invoke(this, new GoIpStateEventArgs("GoIp state authentication error!", packet, host, port));
                return;
            }

            packet.password = "";  // Delete password for security reasons

            _logger.LogInformation("Received GoIp state. ReceiveId: {0} : Gsm state: {1}", packet.receiveid, packet.gsm_remain_state );

            Send(ACKPacketFactory.ACK("STATE", packet.receiveid.ToString(), ""), host, port);
            OnStateChange?.Invoke(this, new GoIpStateEventArgs("OK", packet, host, port));
        }

        private void DeliverReport(string data, string host, int port)
        {
            _logger.LogDebug("Start SMS delivery report");

            GoIpSmsDeliveryReportPacket packet = new GoIpSmsDeliveryReportPacket(data);

            // if auth error  
            if (packet.authid != _options.AuthId || packet.password != _options.AuthPassword)
            {
                // TODO: log?
                _logger.LogInformation("Received SMS delivery report authentication error. Data: {0}", data);
                Send(ACKPacketFactory.ACK("DELIVER", packet.receiveid.ToString(), "Authentication error!"), host, port);
                OnDeliveryReport?.Invoke(this, new GoIpDeliveryReportEventArgs("Delivery report authentication error!", packet, host, port));
                return;
            }

            packet.password = "";  // Delete password for security reasons

            _logger.LogInformation("Received SMS delivery report OK. ReceiveId: {0} Send number: {1} SMS no: {2}", packet.receiveid, packet.send_num, packet.sms_no);

            Send(ACKPacketFactory.ACK("DELIVER", packet.receiveid.ToString(), ""), host, port);
            OnDeliveryReport?.Invoke(this, new GoIpDeliveryReportEventArgs("OK", packet, host, port));
        }

        private void ReceiveSms(string data, string host, int port)
        {
            _logger.LogDebug("Start SMS processing");

            GoIpMessagePacket packet = new GoIpMessagePacket(data);
            
            // if auth error  
            if (packet.authid != _options.AuthId || packet.password != _options.AuthPassword)
            {
                // TODO: log?
                _logger.LogInformation("Received SMS data authentication error. Data: {0}", data);
                Send(ACKPacketFactory.ACK("RECEIVE", packet.receiveid.ToString(), "Authentication error!"), host, port);
                OnMessage?.Invoke(this, new GoIpMessageEventArgs("Receive SMS authentication error!", packet, host, port));
                return;
            }

            packet.password = "";  // Delete password for security reasons

            _logger.LogInformation("Received SMS OK. ReceiveId: {0} Mobile: {1} Message: {2}", packet.receiveid, packet.Srcnum, packet.Message);

            Send(ACKPacketFactory.ACK("RECEIVE", packet.receiveid.ToString(), ""), host, port);
            OnMessage?.Invoke(this, new GoIpMessageEventArgs("OK", packet, host, port));
        }

        /// <summary>
        /// Extract registration packet
        /// </summary>
        /// <param name="data"></param>
        private void Registration(string data, string host, int port)
        {
            _logger.LogDebug("Start Registration processing");

            GoIpRegistrationPacket packet = new GoIpRegistrationPacket(data);

            // if auth error  
            if (packet.authid != _options.AuthId || packet.password != _options.AuthPassword)
            {
                // TODO: log?
                _logger.LogInformation("Received registration data authentication error. Data: {0}", data);
                Send(ACKPacketFactory.ACK_MESSAGE(packet.req, 400), host, port);
                OnRegistration?.Invoke(this, new GoIpRegisterEventArgs("Authentication error!", packet, host, port, 400));
                return;
            }

            packet.password = "";  // Delete password for security reasons

            if (string.IsNullOrEmpty(packet.imei))
            {
                _logger.LogInformation("Received SMS data without IMEI. packet id: {0}", packet.authid);
                Send(ACKPacketFactory.ACK_MESSAGE(packet.req, 400), host, port);
                OnRegistration?.Invoke(this, new GoIpRegisterEventArgs("No IMEI! (No SIM?)", packet, host, port, 400));
                return;
            }

            _logger.LogInformation("Received registration OK. Packet id: {0} IMEI: {1}", packet.authid, packet.imei);
            Send(ACKPacketFactory.ACK_MESSAGE(packet.req, 200), host, port);
            OnRegistration?.Invoke(this, new GoIpRegisterEventArgs("OK", packet, host, port, 400));
        }
    }
}
