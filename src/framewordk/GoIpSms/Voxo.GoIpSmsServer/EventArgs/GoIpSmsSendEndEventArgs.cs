using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSms
{
    public class GoIpSmsSendEndEventArgs : EventArgs
    {
        public GoIpSmsSendEndEventArgs(string sendId)
        {
            SendId = sendId;
        }

        public string SendId { get; }
    }
}
