using System;
using System.ServiceModel;

namespace FTN.ServiceContracts
{
    public class ProxyConnector
    {
        private static readonly Lazy<ProxyConnector> instance =
            new Lazy<ProxyConnector>(() => new ProxyConnector());

        private readonly ChannelFactory<INetworkModelGDAContract> channelFactory;

        public static ProxyConnector Instance => instance.Value;

        private ProxyConnector()
        {
            try
            {
                // Initialize the ChannelFactory with appropriate binding and endpoint address
                var binding = new NetTcpBinding
                {
                    Security = { Mode = SecurityMode.None }
                };
                var endpointAddress = new EndpointAddress("net.tcp://localhost:10000/NetworkModelService/GDA/");

                channelFactory = new ChannelFactory<INetworkModelGDAContract>(binding, endpointAddress);
            }
            catch { }
        }

        public INetworkModelGDAContract GetProxy()
        {
            if (channelFactory == null)
                return new ChannelFactory<INetworkModelGDAContract>().CreateChannel();
            else
                return channelFactory.CreateChannel();
        }

        // Dispose method
        public void Dispose()
        {
            if (channelFactory != null)
            {
                channelFactory.Close();
            }
        }
    }
}
