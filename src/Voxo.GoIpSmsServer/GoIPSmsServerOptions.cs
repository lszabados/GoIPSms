namespace Voxo.GoIpSmsServer
{
    public class GoIPSmsServerOptions
    {
        public int Port { get; set; } = 44444;
        public string ServerId { get; set; }
        public string AuthPassword { get; set; }
    }
}