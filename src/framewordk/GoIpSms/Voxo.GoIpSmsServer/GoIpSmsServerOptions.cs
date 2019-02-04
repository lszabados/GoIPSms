namespace Voxo.GoIpSms
{
    /// <summary>
    /// GoIpSmsServer initialization options
    /// </summary>
    public class GoIpSmsServerOptions
    {
        public int Port { get; set; } = 44444;
        public string AuthId { get; set; }
        public string AuthPassword { get; set; }
    }
}