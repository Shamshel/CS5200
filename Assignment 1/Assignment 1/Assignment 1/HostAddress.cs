using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Assignment_1
{
    class HostAddress
    {
        //storage for human-readable io
        private string hostname;
        private int port;

        //built-in .NET wrappers
        private IPHostEntry hostInfo;
        private IPAddress hostIPAddress;
        private IPAddress[] hostIPAddresses;
        private IPEndPoint hostEndPoint;

        //utiliy vars
        int ipAddressIndex;

        public HostAddress()
        {
            hostname = "";
            port = -1;
            ipAddressIndex = 0;
            hostInfo = null;
            hostIPAddress = null;
            hostEndPoint = null;

        }

        public HostAddress(string hostname, string port)
        {
            SetHostInfo(hostname, port);

        }

        //functions to resolve wrapper vars upon update of human-readable strings
        public void SetHostInfo(string hostname, string port)
        {
            ipAddressIndex = 0;

            this.hostname = hostname;

            try
            {
                this.port = Convert.ToInt32(port);

            }

            catch(Exception e)
            {
                Console.Write("Failed to convert port number: {0}\n\r", e.Message);
                return;

            }


            //port conversion succeeded, update wrapper vars
            try
            {
                hostInfo = Dns.GetHostEntry(hostname);

                //shamelessly copied from SimpleUDPSocket EndPointParser.cs
                hostIPAddresses = Dns.GetHostAddresses(hostname);

                for (int i = 0; i < hostIPAddresses.Length && hostIPAddress == null; i++)
                {
                    if(hostIPAddresses[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        hostIPAddress = hostIPAddresses[i];

                        ipAddressIndex = i;

                    }

                }

                hostEndPoint = new IPEndPoint(hostIPAddress, this.port);

            }

            catch(Exception e)
            {
                Console.Write("Failed to look up host info: {0}\n\r", e.Message);

            }
            
        }

        public void SetHostInfo(IPAddress hostIP, string port)
        {
            ipAddressIndex = 0;

            try
            {
                this.port = Convert.ToInt32(port);

            }

            catch (Exception e)
            {
                Console.Write("Failed to convert port number: {0}\n\r", e.Message);
                return;

            }


            //port conversion succeeded, update wrapper vars
            try
            {
                hostIPAddress = hostIP;

                hostEndPoint = new IPEndPoint(hostIPAddress, this.port);

            }

            catch (Exception e)
            {
                Console.Write("Failed to look up host info: {0}\n\r", e.Message);

            }

        }

        public bool SetNextHostIPAddress()
        {
            //try next ip address in the list returned from DNS lookup
            for (int i = ipAddressIndex; i < hostIPAddresses.Length; i++)
            {
                if(hostIPAddresses[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    hostIPAddress = hostIPAddresses[i];
                    ipAddressIndex = i;
                    return true;

                }

            }

            return false;

        }

        public string GetHostName()
        {
            try
            {
                return hostInfo.HostName;

            }

            catch(Exception e)
            {
                return null;

            }

        }

        public int GetHostPort()
        {
            return hostEndPoint.Port;

        }

        public IPHostEntry GetHostEntry()
        {
            return hostInfo;

        }

        public IPAddress GetHostIPAddress()
        {
            return hostIPAddress;

        }

        public IPEndPoint GetHostEndPoint()
        {
            return hostEndPoint;

        }

    }
}
