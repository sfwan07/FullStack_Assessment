namespace Shared
{
    public class ConsulServiceSetting
    {
        public string Name { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Scheme { get; set; }
        public string DiscoveryAddress { get; set; }
        public string[] Tags { get; set; }
    }
}