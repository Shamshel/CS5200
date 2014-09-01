using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace SimpleUDPSocket
{
    public class SimpleReceiver
    {
        private UdpClient myUdpClient;

        public SimpleReceiver()
        {
            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, 0);
            myUdpClient = new UdpClient(localEP);
        }

        public IPEndPoint LocalEP
        {
            get
            {
                IPEndPoint result = null;
                if (myUdpClient != null)
                    result = myUdpClient.Client.LocalEndPoint as IPEndPoint;
                return result;
            }
        }

        public void Receive()
        {
            DisplayEndPoints();

            Console.WriteLine();
            Console.WriteLine("Receiving...");
            string message = string.Empty;
            while (message.Trim().ToUpper() != "EXIT")
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] receiveBuffer = myUdpClient.Receive(ref remoteEP);
                message = Encoding.Unicode.GetString(receiveBuffer);
                Console.WriteLine("Message from " + remoteEP.ToString() + " --> " + message);
            }
        }

        private void DisplayEndPoints()
        {
            Console.WriteLine();
            IPEndPoint localEP = myUdpClient.Client.LocalEndPoint as IPEndPoint;
            if (localEP.Address.Equals(IPAddress.Any))
            {
                Console.WriteLine("Receiver accepting messages at the following end points");
                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in adapters)
                {
                    if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                        DisplayAddressesForAdapter(adapter, localEP.Port);
                }
            }
            else
            {
                Console.WriteLine("Receiver accepting messages at the following end point");
                Console.WriteLine("\t{0}", localEP);
            }
        }

        private void DisplayAddressesForAdapter(NetworkInterface adapter, int port)
        {
            IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
            UnicastIPAddressInformationCollection uniCast = adapterProperties.UnicastAddresses;
            if (uniCast != null)
            {
                foreach (UnicastIPAddressInformation uni in uniCast)
                    if (uni.Address.AddressFamily == AddressFamily.InterNetwork)
                        Console.WriteLine("\t{0}:{1}", uni.Address, port);
            }
        }
    }
}
