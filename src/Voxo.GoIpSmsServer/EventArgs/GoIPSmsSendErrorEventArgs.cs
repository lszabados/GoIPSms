using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    public class GoIPSmsSendErrorEventArgs : EventArgs
    {
        public GoIPSmsSendErrorEventArgs(string message, string sendId)
        {
            Message = message;
            SendId = sendId;
        }

        public string Message { get; }
        public string SendId { get; }
    }
}
