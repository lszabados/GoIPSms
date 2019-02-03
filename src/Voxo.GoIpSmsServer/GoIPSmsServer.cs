using Microsoft.Extensions.Logging;
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

    /// <summary>
    /// GoIP Sms Server implementation
    /// </summary>
    public class GoIPSmsServer
    {
        private readonly object balanceLock = new object();
        private bool messageReceived = false;
        private GoIPSmsServerOptions _options;
        private readonly ILogger<GoIPSmsServer> _logger;
        private CancellationTokenSource cancelationTokenSource;

        // registration event
        public delegate void RegistrationHandler(object server, GoIPRegisterEventArgs registerData);
        public event RegistrationHandler OnRegistration;

        // message event
        public delegate void MessageHandler(object server, GoIPMessageEventArgs messageData);
        public event MessageHandler OnMessage;

        // delivery report event
        public delegate void DeliveryReportHandler(object server, GoIPDeliveryReportEventArgs messageData);
        public event DeliveryReportHandler OnDeliveryReport;

        // STATE event
        public delegate void StateHandler(object server, GoIPStateEventArgs messageData);
        public event StateHandler OnStateChange;

        // RECORD event
        public delegate void RecordHandler(object server, GoIPRecordEventArgs messageData);
        public event RecordHandler OnRecord;

        // REMAIN event
        public delegate void RemainHandler(object server, GoIPRemainEventArgs messageData);
        public event RemainHandler OnRemain;

        // Cell list event
        public delegate void CellListHandler(object server, GoIPCellListEvenetArgs messageData);
        public event CellListHandler OnCellListChanged;

        public bool ServerStarted { get { return Status == ServerStatus.Started; } }
        public ServerStatus Status { get; internal set; } = ServerStatus.Stopped;

        /// <summary>
        /// GoIPSmsServer constructor
        /// </summary>
        /// <param name="options">Initialization options</param>
        /// <param name="logger">logging interface</param>
        public GoIPSmsServer(GoIPSmsServerOptions options, ILogger<GoIPSmsServer> logger)
        {
            _options = options;
            _logger = logger;
            _logger.LogDebug("Create GoIPSmsServer. ServerId: {0} Listening port: {1}", _options.ServerId, _options.Port);
        }

        /// <summary>
        /// Start server instance
        /// </summary>
        public void StartServer()
        {
            if (Status == ServerStatus.Stopped)
            {
                _logger.LogInformation("Server start request. ServerId: {0} Listening port: {1}", _options.ServerId, _options.Port);
                Status = ServerStatus.Starting;
                cancelationTokenSource = new CancellationTokenSource();
                GoIPSmsServerListener(_options.Port, cancelationTokenSource.Token);
            } // else log?
        }

        /// <summary>
        /// Stop server instance
        /// </summary>
        public void StopServer()
        {
            if (Status == ServerStatus.Started)
            {
                _logger.LogInformation("Server stop request. ServerId: {0} Listening port: {1}", _options.ServerId, _options.Port);
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
                try
                {
                    ReceiveMessages(cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogWarning("Server stopped error. ServerId: {0} Listening port: {1} Error message: {2}", _options.ServerId, _options.Port, e.Message);
                }

                Status = ServerStatus.Stopped;
                _logger.LogInformation("Server stopped successfully. ServerId: {0} Listening port: {1}", _options.ServerId, _options.Port);
            });

            Status = ServerStatus.Started;
            
            _logger.LogInformation("Server started successfully. ServerId: {0} Listening port: {1}", _options.ServerId, _options.Port);

            return task;
        }

        private void ReceiveMessages(CancellationToken cancellationToken)
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
                    _logger.LogDebug("Server cancellation request. ServerId: {0} Listening port: {1}", _options.ServerId, _options.Port);
                    break;
                }
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            UdpState us = (UdpState)(ar.AsyncState);

            int port = us.e.Port;
            string host = us.e.Address.ToString();

            _logger.LogDebug("Start receive data. ServerId: {0} Server port {1} Sender host: {2} Sender port: {3}", 
                _options.ServerId, _options.Port, host, port);

            string receiveString;

            try
            {
                byte[] receiveBytes = us.u.EndReceive(ar, ref us.e);
                receiveString = Encoding.ASCII.GetString(receiveBytes);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Data receive error. ServerId: {0} Server port {1} Sender host: {2} Sender port: {3} Error message: {4}",
                _options.ServerId, _options.Port, host, port, e.Message);
                messageReceived = true;
                return;
            } 

            _logger.LogDebug("End receive data. ServerId: {0} Server port {1} Sender host: {2} Sender port: {3} Data: {4}",
                _options.ServerId, _options.Port, host, port, receiveString);

            ExtractData(receiveString, host, port);
            
            messageReceived = true;
        }



        private void Send(string data, string host, int port)
        {
            _logger.LogDebug("Start send data. LocalId: {0} Local port: {1} Destination host: {2} Destination port: {3} Data: {4}",
                _options.ServerId, _options.Port, host, port, data);

            UdpClient udpClient = new UdpClient(host, port);
            Byte[] sendBytes = Encoding.UTF8.GetBytes(data);
            try
            {
                udpClient.Send(sendBytes, sendBytes.Length);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Data sending error. LocalId: {0} Local port: {1} Destination host: {2} Destination port: {3}",
                _options.ServerId, _options.Port, host, port);
            }

            _logger.LogDebug("Send data end. LocalId: {0} Local port: {1} Destination host: {2} Destination port: {3}",
                _options.ServerId, _options.Port, host, port);
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
            _logger.LogDebug("Start GoIP Cell list event");

            GoIPCellListPacket packet = new GoIPCellListPacket(data);

            // if auth error  
            if (packet.authid != _options.ServerId || packet.password != _options.AuthPassword)
            {
                // TODO: log?
                _logger.LogInformation("GoIP Cell list event authentication error. Data: {0}", data);
                Send(ACKPacketFactory.ACK("CELLS", packet.receiveid.ToString(), "Cell list event authentication error!"), host, port);
                OnCellListChanged(this, new GoIPCellListEvenetArgs("GoIP Cell list event authentication error!", packet.celllist, host, port));
                return;
            }

            _logger.LogInformation("Received GoIP Cell list event. ReceiveId: {0} Cell list: {1}", packet.receiveid, packet.celllist);

            Send(ACKPacketFactory.ACK("CELLS", packet.receiveid.ToString(), ""), host, port);
            OnCellListChanged(this, new GoIPCellListEvenetArgs("OK", packet.celllist, host, port));
        }

        private void RemainData(string data, string host, int port)
        {
            _logger.LogDebug("Start GoIP Remain event");

            GoIPRemainPacket packet = new GoIPRemainPacket(data);

            // if auth error  
            if (packet.authid != _options.ServerId || packet.password != _options.AuthPassword)
            {
                // TODO: log?
                _logger.LogInformation("GoIP remain event authentication error. Data: {0}", data);
                Send(ACKPacketFactory.ACK("REMAIN", packet.receiveid.ToString(), "Remain event authentication error!"), host, port);
                OnRemain(this, new GoIPRemainEventArgs("GoIP remain event authentication error!", packet, host, port));
                return;
            }

            packet.password = "";  // Delete password for security reasons

            _logger.LogInformation("Received GoIP record event. ReceiveId: {0} Remain time: {1}", packet.receiveid, packet.gsm_remain_time);

            Send(ACKPacketFactory.ACK("REMAIN", packet.receiveid.ToString(), ""), host, port);
            OnRemain(this, new GoIPRemainEventArgs("OK", packet, host, port));
        }

        private void RecordData(string data, string host, int port)
        {
            _logger.LogDebug("Start GoIP Record event");

            GoIPRecordPacket packet = new GoIPRecordPacket(data);

            // if auth error  
            if (packet.authid != _options.ServerId || packet.password != _options.AuthPassword)
            {
                // TODO: log?
                _logger.LogInformation("GoIP record event authentication error. Data: {0}", data);
                Send(ACKPacketFactory.ACK("RECORD", packet.receiveid.ToString(), "Record event authentication error!"), host, port);
                OnRecord(this, new GoIPRecordEventArgs("GoIP record event authentication error!", packet, host, port));
                return;
            }

            packet.password = "";  // Delete password for security reasons

            _logger.LogInformation("Received GoIP record event. ReceiveId: {0} Send num: {1} Direction: {2}", packet.receiveid, packet.send_num, packet.direction);

            Send(ACKPacketFactory.ACK("RECORD", packet.receiveid.ToString(), ""), host, port);
            OnRecord(this, new GoIPRecordEventArgs("OK", packet, host, port));
        }

        private void State(string data, string host, int port)
        {
            _logger.LogDebug("Start GoIP State");

            GoIPStatePacket packet = new GoIPStatePacket(data);

            // if auth error  
            if (packet.authid != _options.ServerId || packet.password != _options.AuthPassword)
            {
                // TODO: log?
                _logger.LogInformation("GoIP state report authentication error. Data: {0}", data);
                Send(ACKPacketFactory.ACK("STATE", packet.receiveid.ToString(), "State authentication error!"), host, port);
                OnStateChange(this, new GoIPStateEventArgs("GoIP state authentication error!", packet, host, port));
                return;
            }

            packet.password = "";  // Delete password for security reasons

            _logger.LogInformation("Received GoIP state. ReceiveId: {0} : Gsm state: {1}", packet.receiveid, packet.gsm_remain_state );

            Send(ACKPacketFactory.ACK("STATE", packet.receiveid.ToString(), ""), host, port);
            OnStateChange(this, new GoIPStateEventArgs("OK", packet, host, port));
        }

        private void DeliverReport(string data, string host, int port)
        {
            _logger.LogDebug("Start SMS delivery report");

            GoIPSmsDeliveryReportPacket packet = new GoIPSmsDeliveryReportPacket(data);

            // if auth error  
            if (packet.authid != _options.ServerId || packet.password != _options.AuthPassword)
            {
                // TODO: log?
                _logger.LogInformation("Received SMS delivery report authentication error. Data: {0}", data);
                Send(ACKPacketFactory.ACK("DELIVER", packet.receiveid.ToString(), "Authentication error!"), host, port);
                OnDeliveryReport(this, new GoIPDeliveryReportEventArgs("Delivery report authentication error!", packet, host, port));
                return;
            }

            packet.password = "";  // Delete password for security reasons

            _logger.LogInformation("Received SMS delivery report OK. ReceiveId: {3} Send number: {0} SMS no: {1}", packet.send_num, packet.sms_no, packet.receiveid);

            Send(ACKPacketFactory.ACK("DELIVER", packet.receiveid.ToString(), ""), host, port);
            OnDeliveryReport(this, new GoIPDeliveryReportEventArgs("OK", packet, host, port));
        }

        private void ReceiveSms(string data, string host, int port)
        {
            _logger.LogDebug("Start SMS processing");

            GoIPMessagePacket packet = new GoIPMessagePacket(data);
            
            // if auth error  
            if (packet.AuthId != _options.ServerId || packet.Password != _options.AuthPassword)
            {
                // TODO: log?
                _logger.LogInformation("Received SMS data authentication error. Data: {0}", data);
                Send(ACKPacketFactory.ACK("RECEIVE", packet.ReceiveId, "Authentication error!"), host, port);
                OnMessage(this, new GoIPMessageEventArgs("Receive SMS authentication error!", packet, host, port));
                return;
            }

            packet.Password = "";  // Delete password for security reasons

            _logger.LogInformation("Received SMS OK. ReceiveId: {3} Mobile: {0} Message: {1}", packet.Srcnum, packet.Message, packet.ReceiveId);

            Send(ACKPacketFactory.ACK("RECEIVE", packet.ReceiveId, ""), host, port);
            OnMessage(this, new GoIPMessageEventArgs("OK", packet, host, port));
        }

        /// <summary>
        /// Extract registration packet
        /// </summary>
        /// <param name="data"></param>
        private void Registration(string data, string host, int port)
        {
            _logger.LogDebug("Start Registration processing");

            GoIPRegistrationPacket packet = new GoIPRegistrationPacket(data);

            // if auth error  
            if (packet.authid != _options.ServerId || packet.password != _options.AuthPassword)
            {
                // TODO: log?
                _logger.LogInformation("Received registration data authentication error. Data: {0}", data);
                Send(ACKPacketFactory.ACK_MESSAGE(packet.req, 400), host, port);
                OnRegistration(this, new GoIPRegisterEventArgs("Authentication error!", packet, host, port, 400));
                return;
            }

            packet.password = "";  // Delete password for security reasons

            if (string.IsNullOrEmpty(packet.imei))
            {
                _logger.LogInformation("Received SMS data without IMEI. packet id: {0}", packet.authid);
                Send(ACKPacketFactory.ACK_MESSAGE(packet.req, 400), host, port);
                OnRegistration(this, new GoIPRegisterEventArgs("No IMEI! (No SIM?)", packet, host, port, 400));
                return;
            }

            _logger.LogInformation("Received registration OK. Packet id: {0} IMEI: {1}", packet.authid, packet.imei);
            Send(ACKPacketFactory.ACK_MESSAGE(packet.req, 200), host, port);
            OnRegistration(this, new GoIPRegisterEventArgs("OK", packet, host, port, 400));
        }
    }
}
