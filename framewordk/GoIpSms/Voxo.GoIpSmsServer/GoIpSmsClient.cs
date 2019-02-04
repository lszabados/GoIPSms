using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Voxo.GoIpSms
{
    public class GoIpSmsClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host">Host ip address</param>
        /// <param name="port">port</param>
        /// <param name="line">GSM line</param>
        public GoIpSmsClient(ILogger<GoIpSmsClient> logger)
        {
            Port = 10991;
            _logger = logger;
            GenerateSendId();
            _logger.LogDebug("Create SMS Client. ");
        }

        private readonly ILogger<GoIpSmsClient> _logger;

        public string Host { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }

        private string SendId { get; set; } = "";

        public string ErrorMessage { get; private set; } = "";

        // event handlers
        public delegate void SmsSendError(object server, GoIpSmsSendErrorEventArgs eventArgs);

        /// <summary>
        /// Raise for all SMS sending errors
        /// </summary>
        public event SmsSendError OnSmsSendError;

        public delegate void SmsSendMessageStatus(object server, GoIpSendMessageEventArgs eventArgs);

        /// <summary>
        /// Raise for every SMS sending event
        /// </summary>
        public event SmsSendMessageStatus OnSmsSendMessage;

        public delegate void SmsSendEnd(object server, GoIpSmsSendEndEventArgs eventArgs);

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
                OnSmsSendEnd(this, new GoIpSmsSendEndEventArgs(SendId));
                return;
            }

            if (!AuthenticationRequest())
            {
                _logger.LogInformation("Bulk SMS sending proceure ended. SendId: {0}", SendId);
                OnSmsSendEnd(this, new GoIpSmsSendEndEventArgs(SendId));
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
                    _logger.LogWarning("Bulk SMS sending error. SendId: {0} TelId: {1} Message: {2}", SendId, telid, e.Message);
                    OnSmsSendError(this, new GoIpSmsSendErrorEventArgs(e.Message, SendId));
                } 
            }

            DoneRequest();

            _logger.LogInformation("Bulk SMS sending proceure ended. SendId: {0}", SendId);
            OnSmsSendEnd(this, new GoIpSmsSendEndEventArgs(SendId));
        }

        private string GetCommand(string command, string logtext, bool needPassword = true)
        {
            // null Error message
            ErrorMessage = "";

            _logger.LogDebug("Start {0} func", logtext);
            int localPort = 0;

            if (needPassword)
            {
                localPort = Send(ACKPacketFactory.REQUEST(command, SendId, Password));
            }
            else
            {
                localPort = Send(ACKPacketFactory.SEND(command, SendId));
            }
            

            if (localPort == 0)
            {
                ErrorMessage = string.Format("{0} sending error!", logtext);
                return "";
            }

            // get request
            string ret = Get(localPort);

            if (ret.StartsWith(string.Format("ERROR {0}", SendId)))
            {
                ErrorMessage = ret.Substring(string.Format("ERROR {0} ", SendId).Length);
                _logger.LogWarning("{0} receive error. SendID: {1} Error message: {2}", logtext, SendId, ErrorMessage);
                OnSmsSendError(this, new GoIpSmsSendErrorEventArgs(ErrorMessage, SendId));
                return "";
            }

            if (!ret.StartsWith(string.Format("{0} {1}", command, SendId)))
            {
                ErrorMessage = string.Format("{0} sending error!", logtext);
                _logger.LogWarning("{0} receive error. SendID: {1} Return value: {2}", logtext, SendId, ret);
                OnSmsSendError(this, new GoIpSmsSendErrorEventArgs(ErrorMessage, SendId));
                return "";
            }


            _logger.LogDebug("End {0} func", logtext);
            return ret.Substring(string.Format("{0} {1} ", command, SendId).Length);
        }

        public string SetCommand(string command, string param, string logtext)
        {
            // null Error message
            ErrorMessage = "";

            _logger.LogDebug("Start {0} func", logtext);
            int localPort = Send(ACKPacketFactory.REQUEST(command, SendId, param));

            if (localPort == 0)
            {
                ErrorMessage = string.Format("{0} sending error!", logtext);
                return "";
            }

            // get request
            string ret = Get(localPort);

            if (ret.StartsWith(string.Format("ERROR {0}", SendId)))
            {
                ErrorMessage = ret.Substring(string.Format("ERROR {0} ", SendId).Length);
                _logger.LogWarning("{0} receive error. SendID: {1} Error message: {2}", logtext, SendId, ErrorMessage);
                OnSmsSendError(this, new GoIpSmsSendErrorEventArgs(ErrorMessage, SendId));
                return "";
            }

            if (!ret.StartsWith(string.Format("{0} {1}", command, SendId)))
            {
                ErrorMessage = string.Format("{0} sending error!", logtext);
                _logger.LogWarning("{0} receive error. SendID: {1} Return value: {2}", logtext, SendId, ret);
                OnSmsSendError(this, new GoIpSmsSendErrorEventArgs(ErrorMessage, SendId));
                return "";
            }

            _logger.LogDebug("End {0} func", logtext);
            return ret.Substring(string.Format("{0} {1} ", command, SendId).Length);
        }

        /// <summary>
        /// Get GSM number
        /// </summary>
        /// <returns></returns>
        public string GetGsmNumber()
        {
            return GetCommand("get_gsm_num", "Get GSM number");
        }

        /// <summary>
        /// Set GSM number
        /// </summary>
        /// <returns></returns>
        public string SetGsmNumber(string gsmnumber)
        {
            string s = SetCommand("set_gsm_num", string.Format("{0} {1}", gsmnumber, Password) , "Set GSM number");
            if (string.IsNullOrEmpty(s)) return s;

            return "ok";
        }

        /// <summary>
        /// Get expiry time of out call of a channel 
        /// </summary>
        /// <returns></returns>
        public string GetExpirityTime()
        {
            return GetCommand("get_exp_time", "GSM expiry time");
        }

        /// <summary>
        /// Set expiry time of out call of a channel
        /// </summary>
        /// <param name="expiritiTime">expiry time (minute) witch want to set</param>
        /// <returns>OK or "" if error. Error message</returns>
        public string SetExpirityTime(int expiritiTime)
        {
            string s = SetCommand("set_gsm_num", string.Format("{0} {1}", expiritiTime, Password), "Set GSM number");
            if (string.IsNullOrEmpty(s)) return s;

            return "ok";
         }

        /// <summary>
        /// Get Remain time of out call
        /// </summary>
        /// <returns></returns>
        public string GetRemainTime()
        {
            return GetCommand("get_remain_time", "GSM remain time");
        }

        /// <summary>
        /// Reset remain time of out call to expiry time
        /// </summary>
        public string ResetRemainTime()
        {
            string s = SetCommand("reset_remain_time", string.Format("{0}", Password), "Reset remain time");
            if (string.IsNullOrEmpty(s)) return s;

            return "ok";
        }

        /// <summary>
        /// Get status of channel
        /// </summary>
        /// <returns></returns>
        public string GetChanelStatus()
        {
            return GetCommand("get_gsm_state", "GSM status of channel");
        }

        /// <summary>
        /// Drop call
        /// </summary>
        /// <returns></returns>
        public string DropCall()
        {
            return GetCommand("svr_drop_call", "Drop call");
        }

        /// <summary>
        /// Reboot channel
        /// </summary>
        /// <returns></returns>
        public string RebootChannel()
        {
            return GetCommand("svr_reboot_module", "Reboot channel");
        }

        /// <summary>
        /// Reboot GoIp
        /// </summary>
        /// <returns></returns>
        public string RebootGoIp()
        {
            return GetCommand("svr_reboot_dev", "Reboot GoIp");
        }

        /// <summary>
        /// Set GSM call forward
        /// </summary>
        /// <param name="ftime">timeout (second) of noreply forward type. Other types set to 0.</param>
        /// <param name="mode">enable or disable forward。3:enable，4:disable</param>
        /// <param name="num">forward to this number</param>
        /// <param name="reason">type of call forward. 0: unconditional，1: busy，2: noreply，3: noreachable, 4: all，5:busy,noreply,noreachable</param>
        /// <returns></returns>
        public bool SetGsmCallForward(string reason, string mode, string num, int ftime)
        {
            string s = SetCommand("CF", string.Format("{0} {1} {2} {3} {4}", Password, reason, mode, num, ftime.ToString()), "Set GSM call forward");
            if (string.IsNullOrEmpty(s)) return false;

            // null Error message
            ErrorMessage = "";

            _logger.LogDebug("Start {0} func", "Set GSM call forward");

            int localPort = Send(ACKPacketFactory.REQUEST("CF", SendId, Password, reason, mode, num, ftime.ToString()));

            if (localPort == 0)
            {
                ErrorMessage = string.Format("{0} sending error!", "Set GSM call forward");
                return false;
            }

            // get request
            string ret = Get(localPort);

            if (ret.StartsWith(string.Format("CFERROR {0}", SendId)))
            {
                ErrorMessage = ret.Substring(string.Format("ERROR {0} ", SendId).Length);
                _logger.LogWarning("{0} receive error. SendID: {1} Error message: {2}", "Set GSM call forward", SendId, ErrorMessage);
                OnSmsSendError(this, new GoIpSmsSendErrorEventArgs(ErrorMessage, SendId));
                return false;
            }

            if (!ret.StartsWith(string.Format("{0} {1}", "CFOK", SendId)))
            {
                ErrorMessage = string.Format("{0} sending error!", "Set GSM call forward");
                _logger.LogWarning("{0} receive error. SendID: {1} Return value: {2}", "Set GSM call forward", SendId, ret);
                OnSmsSendError(this, new GoIpSmsSendErrorEventArgs(ErrorMessage, SendId));
                return false;
            }

            Send(ACKPacketFactory.SEND("DONE", SendId));

            _logger.LogDebug("End {0} func", "Set GSM call forward");
            return true;
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
            return GetCommand("get_imei", "Get IMEI");
        }

        /// <summary>
        /// Set IMEI
        /// </summary>
        /// <param name="imei">IMEI number，15 digits</param>
        /// <returns></returns>
        public string SetIMEI(string imei)
        {
            string s = SetCommand("set_imei", string.Format("{0} {1}", imei, Password), "Set IMEI");
            if (string.IsNullOrEmpty(s)) return s;

            return "ok";
        }

        /// <summary>
        /// Get out call interval
        /// </summary>
        /// <returns></returns>
        public string GetOutCallIntervall()
        {
            return GetCommand("get_out_call_interval", "Get out call interval");
        }

        /// <summary>
        /// Set out call interval
        /// </summary>
        /// <param name="interval">out call interval (second)</param>
        /// <returns></returns>
        public string SetOutCallIntervall(int interval)
        {
            string s = SetCommand("set_out_call_interval", string.Format("{0} {1}", interval.ToString(), Password), "Set out call interval");
            if (string.IsNullOrEmpty(s)) return s;

            return "ok";
        }

        /// <summary>
        /// enable/disable this module
        /// </summary>
        /// <param name="Enable">1 to enable, 2 to disable</param>
        /// <returns></returns>
        public string EnableDisableThisModule(int value)
        {
            string s = SetCommand("module_ctl_i", string.Format("{0} {1}", value.ToString(), Password), "enable/disable this module");
            if (string.IsNullOrEmpty(s)) return s;

            return "ok";
        }

        /// <summary>
        /// enable/disable all modules
        /// </summary>
        /// <param name="Enable">1 to enable, 0 to disable, 2 to not change, each digit for each channel. For example 10120121, means channel 1 enable, channel 2 disable, channel 3 enable, channel 4 not change, channel 5 disable, channel 6 enable, channel 7 not change and channel 8 enable</param>
        /// <returns></returns>
        public string EnableDisableAllModule(int value)
        {
            string s = SetCommand("module_ctl", string.Format("{0} {1}", value.ToString(), Password), "enable/disable this module");
            if (string.IsNullOrEmpty(s)) return s;

            return "ok";
        }

        /// <summary>
        /// Set cell
        /// </summary>
        /// <param name="cellid">cell id</param>
        /// <returns></returns>
        public string SetCell(string cellid)
        {
            string s = SetCommand("set_base_cell", string.Format("{0} {1}",cellid, Password), "Set cell");
            if (string.IsNullOrEmpty(s)) return s;

            return "ok";
        }

        /// <summary>
        /// Get cells list
        /// </summary>
        /// <returns></returns>
        public string GetCellList()
        {
            return GetCommand("get_cells_list", "Get cells list");
        }

        /// <summary>
        /// Get current cell 
        /// </summary>
        /// <returns></returns>
        public string GetCurrentCell()
        {
            return GetCommand("CURCELL", "Get current cell");
        }


        private void DoneRequest()
        {
            GetCommand("DONE", "DONE request", false);
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
                    OnSmsSendMessage(this, new GoIpSendMessageEventArgs(number, SendId, "OK"));
                    break;
                }

                if (ret.StartsWith(string.Format("ERROR {0} {1}", SendId, telid)))
                {
                    _logger.LogWarning("SMS sending SUBMIT_NUMBER_REQUEST error. SendId: {0} TeliId: {1} Phone number: {2}", SendId, telid, number);
                    OnSmsSendMessage(this, new GoIpSendMessageEventArgs(number, SendId, "ERROR"));
                    break;
                }

                if (ret.StartsWith(string.Format("WAIT {0} {1}", SendId, telid)))
                {
                    _logger.LogInformation("SMS sending SUBMIT_NUMBER_REQUEST WAIT. SendId: {0} TeliId: {1} Phone number: {2}", SendId, telid, number);
                    OnSmsSendMessage(this, new GoIpSendMessageEventArgs(number, SendId, "WAIT"));
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
                
            int localPort = Send(ACKPacketFactory.BULK_SMS_REQUEST(SendId, message));

            // If localport == 0 then sending error
            if (localPort == 0) return false;

            // get request
            string ret = Get(localPort);

            if (!ret.StartsWith(string.Format("PASSWORD {0}", SendId)))
            {
                ErrorMessage = "BULK_SMS_REQUEST error!";
                _logger.LogWarning("Bulk SMS BULK_SMS_REQUEST error. SendID: {0} Return value: {1}", SendId, ret);
                OnSmsSendError(this, new GoIpSmsSendErrorEventArgs(ErrorMessage, SendId));
                return false;
            }

            _logger.LogDebug("SMS sending BULK_SMS_REQUEST successfully. SendId: {0}", SendId);
            return true;

        }

        private int Send(string data)
        {
            _logger.LogDebug("SMS sending Data send starting. SendId: {0}, Host: {1} Remote port {2} Data: {3}", 
                SendId, Host, Port, data);
            UdpClient udpClient = new UdpClient(Host, Port);
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
                OnSmsSendError(this, new GoIpSmsSendErrorEventArgs(ErrorMessage, SendId));
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
