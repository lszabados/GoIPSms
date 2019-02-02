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
        public GoIpSmsClient(string host, string password, int port =10991 , int line = 0)
        {
            Host = host;
            Port = port;
            Line = line;
            Password = password;
        }

        public delegate void SmsSendError(object server, GoIPSmsSendErrorEventArgs eventArgs);
        public event SmsSendError OnSmsSendError;

        public delegate void SmsSendMessageStatus(object server, GoIPSendMessageEventArgs eventArgs);
        public event SmsSendMessageStatus OnSmsSendMessage;

        public delegate void SmsSendEnd(object server, GoIPSmsSendEndEventArgs eventArgs);
        public event SmsSendEnd OnSmsSendEnd;

        public string Host { get; }
        public int Port { get; }
        public int Line { get; }
        public string Password { get; }
        private string SendId { get; set; } = "";

        public string ErrorMessage { get; set; } = "";

        /// <summary>
        /// Generate random sendid
        /// </summary>
        public string GenerateSendId()
        {
            Random rnd = new Random();
            SendId = rnd.Next(1000, 9999).ToString();
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
            if (!string.IsNullOrEmpty(sendId)) SendId = sendId;    

            if (!BulkSmsRequest(message))
            {
                OnSmsSendError(this, new GoIPSmsSendErrorEventArgs(ErrorMessage, SendId));
                OnSmsSendEnd(this, new GoIPSmsSendEndEventArgs(SendId));
                return;
            }


            if (!AuthenticationRequest())
            {
                OnSmsSendError(this, new GoIPSmsSendErrorEventArgs(ErrorMessage, SendId));
                OnSmsSendEnd(this, new GoIPSmsSendEndEventArgs(SendId));
                return;
            }

            int telid = 0;

            foreach (string number in Numbers)
            {
                telid++;

                // if error, then exit
                if (!SubmitNumberRequest(number, telid))
                {
                    OnSmsSendError(this, new GoIPSmsSendErrorEventArgs(ErrorMessage, SendId));
                    break;
                }
            }

            if (!DoneRequest())
            {
                OnSmsSendError(this, new GoIPSmsSendErrorEventArgs(ErrorMessage, SendId));
            }

            OnSmsSendEnd(this, new GoIPSmsSendEndEventArgs(SendId));
        }

        private bool DoneRequest()
        {
            int localPort = Send(ACKPacketFactory.END_REQUEST(SendId));
            string ret = Get(localPort);

            if (!ret.StartsWith(string.Format("DONE {0}", SendId)))
            {
                ErrorMessage = "Unknow DONE error!";
                return false;
            }

            return true;
        }

        private bool SubmitNumberRequest(string number, int telid)
        {
            
            while (true)
            {
                int localPort = Send(ACKPacketFactory.SUBMIT_NUMBER_REQUEST(SendId, telid, number));

                string ret = Get(localPort);

                if (ret.StartsWith(string.Format("OK {0} {1}", SendId, telid)))
                {
                    OnSmsSendMessage(this, new GoIPSendMessageEventArgs(number, SendId, "OK"));
                    break;
                }

                if (ret.StartsWith(string.Format("ERROR {0} {1}", SendId, telid)))
                {
                    OnSmsSendMessage(this, new GoIPSendMessageEventArgs(number, SendId, "ERROR"));
                    break;
                }

                if (ret.StartsWith(string.Format("WAIT {0} {1}", SendId, telid)))
                {
                    OnSmsSendMessage(this, new GoIPSendMessageEventArgs(number, SendId, "WAIT"));
                }

                // Wait 3 second
                System.Threading.Thread.Sleep(3000);
            }

            return true;
        }

        private bool AuthenticationRequest()
        {
            int localPort = Send(ACKPacketFactory.AUTHENTICATION_REQUEST(SendId, Password));
            string ret = Get(localPort);

            // if request error message
            if (ret.StartsWith(string.Format("ERROR {0}", SendId)))
            {
                ErrorMessage = "Authentication error!";
                return false;
            }

            // if not request SEND message
            if (!ret.StartsWith(string.Format("SEND {0}", SendId)))
            {
                ErrorMessage = "Unknow Authentication error!";
                return false;
            }

            return true;
        }

        private bool BulkSmsRequest(string message)
        {
            // generate new random send Id
            if (string.IsNullOrEmpty(SendId)) GenerateSendId();
                
                
            int localPort = Send(ACKPacketFactory.BULK_SMS_REQUEST(SendId, message));
            string ret = Get(localPort);

            if (!ret.StartsWith(string.Format("PASSWORD {0}", SendId)))
            {
                // ERROR LOG!
                ErrorMessage = "Request error!";
                return false;
            }

            return true;

        }

        private int Send(string data)
        {
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
                // TODO : log?
                //return 0;
            }
            finally
            {
                udpClient.Close();
            }

            return port;
        }

        private string Get(int localPort, int max = 8)
        {
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
                // TODO: log?
            }
            finally
            {
                listener.Close();
            }

            return ret;
        }
    }
}
