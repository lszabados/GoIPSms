using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Voxo.GoIpSmsServer
{
    public class GoIpSmsClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host">Host ip address</param>
        /// <param name="port">port</param>
        /// <param name="line">GSM line</param>
        public GoIpSmsClient(ILogger<GoIpSmsClient> logger, string host, string password, int port =10991 ,  int line = 0)
        {
            Host = host;
            Port = port;
            Line = line;
            Password = password;
            _logger = logger;
            _logger.LogDebug("Create SMS Client. Host: {0} Port: {1} Line: {2}", Host, Port, Line);
        }

        private readonly ILogger<GoIpSmsClient> _logger;

        public string Host { get; }
        public int Port { get; }
        public int Line { get; }
        public string Password { get; }

        private string SendId { get; set; } = "";

        public string ErrorMessage { get; private set; } = "";

        // event handlers
        public delegate void SmsSendError(object server, GoIPSmsSendErrorEventArgs eventArgs);

        /// <summary>
        /// Raise for all SMS sending errors
        /// </summary>
        public event SmsSendError OnSmsSendError;

        public delegate void SmsSendMessageStatus(object server, GoIPSendMessageEventArgs eventArgs);

        /// <summary>
        /// Raise for every SMS sending event
        /// </summary>
        public event SmsSendMessageStatus OnSmsSendMessage;

        public delegate void SmsSendEnd(object server, GoIPSmsSendEndEventArgs eventArgs);

        /// <summary>
        /// Raise for nd of SMS sending process
        /// </summary>
        public event SmsSendEnd OnSmsSendEnd;



        /// <summary>
        /// Generate random sendid
        /// </summary>
        public string GenerateSendId()
        {
            Random rnd = new Random();
            SendId = rnd.Next(1000, 9999).ToString();
            _logger.LogInformation("Create random SendId. Value: {0}", SendId);
            return SendId;
        }

        /// <summary>
        /// SMS sending function
        /// </summary>
        /// <param name="message">Message to be sent</param>
        /// <param name="Numbers">phone numbers</param>
        /// <param name="sendId">send identifier</param>
        public void SendBulkSMS(string message, string[] Numbers, string sendId = "")
        {
            _logger.LogInformation("Bulk SMS sending start. SendId: {0} Message: {1}", SendId, message);
            if (!string.IsNullOrEmpty(sendId)) SendId = sendId;    

            if (!BulkSmsRequest(message))
            {
                _logger.LogInformation("Bulk SMS sending proceure ended. SendId: {0}", SendId);
                OnSmsSendEnd(this, new GoIPSmsSendEndEventArgs(SendId));
                return;
            }

            if (!AuthenticationRequest())
            {
                _logger.LogInformation("Bulk SMS sending proceure ended. SendId: {0}", SendId);
                OnSmsSendEnd(this, new GoIPSmsSendEndEventArgs(SendId));
                return;
            }

            int telid = 0;

            foreach (string number in Numbers)
            {
                telid++;
             
                try
                {
                    SubmitNumberRequest(number, telid);
                }
                catch (Exception e)
                {
                    // if unknown error
                    _logger.LogWarning("Bulk SMS sending error. TelId: {1} SendId: {2} Message: {0}", e.Message, telid, SendId);
                    OnSmsSendError(this, new GoIPSmsSendErrorEventArgs(e.Message, SendId));
                } 
            }

            DoneRequest();

            _logger.LogInformation("Bulk SMS sending proceure ended. SendId: {0}", SendId);
            OnSmsSendEnd(this, new GoIPSmsSendEndEventArgs(SendId));
        }

        /// <summary>
        /// Get GSM number
        /// </summary>
        /// <returns></returns>
        public string GetGsmNumber()
        {
            return "";
        }

        /// <summary>
        /// Set GSM number
        /// </summary>
        /// <returns></returns>
        public bool SetGsmNumber()
        {
            return false;
        }

        /// <summary>
        /// Get expiry time of out call of a channel 
        /// </summary>
        /// <returns></returns>
        public string GetExpirityTime()
        {
            return "";
        }

        /// <summary>
        /// Set expiry time of out call of a channel
        /// </summary>
        /// <returns></returns>
        public bool SetExpirityTime()
        {
            return false;
        }

        /// <summary>
        /// Get Remain time of out call
        /// </summary>
        /// <returns></returns>
        public string GetRemainTime()
        {
            return "";
        }

        /// <summary>
        /// Reset remain time of out call to expiry time
        /// </summary>
        public bool ResetRemainTime()
        {
            return true;
        }

        /// <summary>
        /// Get status of channel
        /// </summary>
        /// <returns></returns>
        public string GetChanelStatus()
        {
            return "";
        }

        /// <summary>
        /// Drop call
        /// </summary>
        /// <returns></returns>
        public bool DropCall()
        {
            return false;
        }

        /// <summary>
        /// Reboot channel
        /// </summary>
        /// <returns></returns>
        public bool RebootChannel()
        {
            return false;
        }

        /// <summary>
        /// Reboot GoIP
        /// </summary>
        /// <returns></returns>
        public bool RebootGoIP()
        {
            return false;
        }

        /// <summary>
        /// Set GSM call forward
        /// </summary>
        /// <returns></returns>
        public bool SetGsmCallForward()
        {
            return false;
        }

        /// <summary>
        /// Send USSD 
        /// </summary>
        /// <returns></returns>
        public string SendUSSD()
        {
            return "";
        }

        /// <summary>
        /// Get IMEI
        /// </summary>
        /// <returns></returns>
        public string GetIMEI()
        {
            return "";
        }

        /// <summary>
        /// Set IMEI
        /// </summary>
        /// <returns></returns>
        public bool SetIMEI()
        {
            return false;
        }

        /// <summary>
        /// Get out call interval
        /// </summary>
        /// <returns></returns>
        public string GetOutCallIntervall()
        {
            return "";
        }

        /// <summary>
        /// Set out call interval
        /// </summary>
        /// <returns></returns>
        public bool SetOutCallIntervall()
        {
            return false;
        }

        /// <summary>
        /// enable/disable this module
        /// </summary>
        /// <returns></returns>
        public bool EnableDisableThisModule()
        {
            return false;
        }

        /// <summary>
        /// enable/disable all modules
        /// </summary>
        /// <returns></returns>
        public bool EnableDisableAllModule()
        {
            return false;
        }

        /// <summary>
        /// Set cell
        /// </summary>
        /// <returns></returns>
        public string SetCell()
        {
            return "";
        }

        /// <summary>
        /// Get cells list
        /// </summary>
        /// <returns></returns>
        public string GetCellList()
        {
            return "";
        }

        /// <summary>
        /// Get current cell 
        /// </summary>
        /// <returns></returns>
        public string GetCurrentCell()
        {
            return "";
        }


        private void DoneRequest()
        {
            _logger.LogDebug("SMS sending END_REQUEST start. SendId: {0}", SendId);
            int localPort = Send(ACKPacketFactory.END_REQUEST(SendId));

            // If localport == 0 then sending error
            if (localPort == 0) return;

            // Get requvest
            string ret = Get(localPort);

            if (!ret.StartsWith(string.Format("DONE {0}", SendId)))
            {
                ErrorMessage = "Unknow DONE error!";
                _logger.LogWarning("Bulk SMS done error. SendId: {0} Return value: {1}", SendId, ret);
                OnSmsSendError(this, new GoIPSmsSendErrorEventArgs(ErrorMessage, SendId));
                return;
            }
            _logger.LogDebug("SMS sending END_REQUEST successfully. SendId: {0}", SendId);
            
        }

        private void SubmitNumberRequest(string number, int telid)
        {
            _logger.LogDebug("SMS sending SUBMIT_NUMBER_REQUEST start. SendId: {0} TeliId: {1} Phone number: {2}",
                        SendId, telid, number);
            while (true)
            {
                int localPort = Send(ACKPacketFactory.SUBMIT_NUMBER_REQUEST(SendId, telid, number));
                
                // If localport == 0 then sending error
                if (localPort == 0) return;

                // Get requvest
                string ret = Get(localPort);

                if (ret.StartsWith(string.Format("OK {0} {1}", SendId, telid)))
                {
                    _logger.LogInformation("SMS sending SUBMIT_NUMBER_REQUEST successfully. SendId: {0} TeliId: {1} Phone number: {2}", SendId, telid, number);
                    OnSmsSendMessage(this, new GoIPSendMessageEventArgs(number, SendId, "OK"));
                    break;
                }

                if (ret.StartsWith(string.Format("ERROR {0} {1}", SendId, telid)))
                {
                    _logger.LogWarning("SMS sending SUBMIT_NUMBER_REQUEST error. SendId: {0} TeliId: {1} Phone number: {2}", SendId, telid, number);
                    OnSmsSendMessage(this, new GoIPSendMessageEventArgs(number, SendId, "ERROR"));
                    break;
                }

                if (ret.StartsWith(string.Format("WAIT {0} {1}", SendId, telid)))
                {
                    _logger.LogInformation("SMS sending SUBMIT_NUMBER_REQUEST WAIT. SendId: {0} TeliId: {1} Phone number: {2}", SendId, telid, number);
                    OnSmsSendMessage(this, new GoIPSendMessageEventArgs(number, SendId, "WAIT"));
                }
                else
                {
                    _logger.LogDebug("SMS sending SUBMIT_NUMBER_REQUEST illegal operation. SendId: {0} TeliId: {1} Phone number: {2} Return value: {3}", 
                         SendId, telid, number, ret);
                }

                // Wait 3 second
                System.Threading.Thread.Sleep(3000);
            }

            _logger.LogDebug("SMS sending SUBMIT_NUMBER_REQUEST successfully.  SendId: {0} TeliId: {1} Phone number: {2}", SendId, telid, number);
            return;
        }

        private bool AuthenticationRequest()
        {
            _logger.LogDebug("SMS sending AUTHENTICATION_REQUEST start. SendId: {0}", SendId);

            int localPort = Send(ACKPacketFactory.AUTHENTICATION_REQUEST(SendId, Password));

            // If localport == 0 then sending error
            if (localPort == 0) return false;

            // Get requvest
            string ret = Get(localPort);

            // if request error message
            if (ret.StartsWith(string.Format("ERROR {0}", SendId)))
            {
                ErrorMessage = "Authentication error!";
                _logger.LogWarning("SMS sending AUTHENTICATION_REQUEST error. SendId: {0} Return value: {1}", SendId, ret);
                return false;
            }

            // if not request SEND message
            if (!ret.StartsWith(string.Format("SEND {0}", SendId)))
            {
                ErrorMessage = "Unknow Authentication error!";
                _logger.LogWarning("SMS sending AUTHENTICATION_REQUEST error. SendId: {0} Return value: {1}", SendId, ret);
                return false;
            }

            _logger.LogDebug("SMS sending AUTHENTICATION_REQUEST sucessfully. SendId: {0}", SendId);

            return true;
        }

        private bool BulkSmsRequest(string message)
        {
            _logger.LogDebug("SMS sending BULK_SMS_REQUEST start. SendId: {0}", SendId);
            // generate new random send Id
            if (string.IsNullOrEmpty(SendId)) GenerateSendId();
                
                
            int localPort = Send(ACKPacketFactory.BULK_SMS_REQUEST(SendId, message));

            // If localport == 0 then sending error
            if (localPort == 0) return false;

            // get request
            string ret = Get(localPort);

            if (!ret.StartsWith(string.Format("PASSWORD {0}", SendId)))
            {
                ErrorMessage = "BULK_SMS_REQUEST error!";
                _logger.LogWarning("Bulk SMS BULK_SMS_REQUEST error. SendID: {0} Return value: {1}", SendId, ret);
                OnSmsSendError(this, new GoIPSmsSendErrorEventArgs(ErrorMessage, SendId));
                return false;
            }

            _logger.LogDebug("SMS sending BULK_SMS_REQUEST successfully. SendId: {0}", SendId);
            return true;

        }

        private int Send(string data)
        {
            _logger.LogDebug("SMS sending Data send starting. SendId: {0}, Host: {1} Remote port {2} Data: {3}", 
                SendId, Host, Port+Line, data);
            UdpClient udpClient = new UdpClient(Host, Port+Line);
            Byte[] sendBytes = Encoding.UTF8.GetBytes(data);
            int port = 0;
            try
            {
                udpClient.Send(sendBytes, sendBytes.Length);
                port = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
            }
            catch (Exception e)
            {
                _logger.LogWarning("SMS sending Data send error. Exception: {0}", e.Message);
            }
            finally
            {
                udpClient.Close();
            }

            if (port == 0)
            {
                ErrorMessage = "Data sending error!";
                _logger.LogWarning("Bulk SMS data send error. SendId: {0}", SendId);
                OnSmsSendError(this, new GoIPSmsSendErrorEventArgs(ErrorMessage, SendId));
            }


            _logger.LogDebug("SMS sending Data send Ended. Local port: {0}", port);
            return port;
        }

        private string Get(int localPort, int max = 8)
        {
            _logger.LogDebug("SMS sending Data receive starting. Local port: {0} Remote Port {1}", localPort.ToString(), Port.ToString());
            UdpClient listener = new UdpClient(localPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, Port);
            string ret = "";

            try
            {
                byte[] bytes = listener.Receive(ref groupEP);
                ret = Encoding.ASCII.GetString(bytes, 0, bytes.Length);   
            }
            catch (SocketException e)
            {
                _logger.LogWarning("SMS sending Data send error. Exception: {0}", e.Message);
            }
            finally
            {
                listener.Close();
            }
            _logger.LogDebug("SMS sending Data receive ended. Data: {0}", ret);
            return ret;
        }
    }
}
