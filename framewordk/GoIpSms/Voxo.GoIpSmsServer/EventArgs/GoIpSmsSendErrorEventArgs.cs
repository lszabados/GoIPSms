using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSms
{
    public class GoIpSmsSendErrorEventArgs : EventArgs
    {
        public GoIpSmsSendErrorEventArgs(string message, string sendId)
        {
            Message = message;
            SendId = sendId;
        }

        public string Message { get; }
        public string SendId { get; }
    }
}
