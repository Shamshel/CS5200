using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Assignment_1
{
    class ConnectionSocket
    {
        public static int buffSize = 1024;

        //tcp connection stuff
        private Socket localSocket;
        private HostAddress remoteHost;
        private HostAddress localHost;
        private SocketType socketType;
        private ProtocolType protocolType;

        //udp connection stuff
        private UdpClient udpSocket;
        private IPEndPoint incomingEndpoint;

        //constructors
        //client sockets
        public ConnectionSocket(HostAddress remoteHost, string localPort, SocketType socketType, ProtocolType protocolType)
        {
            this.remoteHost = remoteHost;
            this.socketType = socketType;
            this.protocolType = protocolType;

            localHost = new HostAddress();
            localHost.SetHostInfo(IPAddress.Any, localPort);

            //protect against accidental null assignment
            if (remoteHost != null)
            {
                if (protocolType != ProtocolType.Udp)
                {
                    localSocket = CreateSocket(socketType, protocolType);

                }

                else
                {
                    udpSocket = new UdpClient(localHost.GetHostEndPoint());

                }

            }

            else
            {
                localSocket = null;
                udpSocket = null;

            }

        }

        public ConnectionSocket(string hostname, string remotePort, string localPort, SocketType socketType, ProtocolType protocolType)
        {
            //resolve input parameters to create local socket
            remoteHost = new HostAddress();
            remoteHost.SetHostInfo(hostname, remotePort);
            this.socketType = socketType;
            this.protocolType = protocolType;

            localHost = new HostAddress();
            localHost.SetHostInfo(IPAddress.Any, localPort);

            //protect against lookup failure
            if (remoteHost != null)
            {
                if (protocolType != ProtocolType.Udp)
                {
                    localSocket = CreateSocket(socketType, protocolType);

                }

                else
                {
                    udpSocket = new UdpClient(localHost.GetHostEndPoint());

                }


            }

            else
            {
                localSocket = null;
                udpSocket = null;

            }

        }
        
        //(implicit) server sockets
        public ConnectionSocket(string remotePort, string localPort, SocketType socketType, ProtocolType protocolType)
        {
            remoteHost = new HostAddress();
            remoteHost.SetHostInfo(IPAddress.Any, remotePort);

            localHost = new HostAddress();
            localHost.SetHostInfo(IPAddress.Any, localPort);

            this.socketType = socketType;
            this.protocolType = protocolType;

            //protect against lookup failure (who knows, we are using .NET after all...)
            if (remoteHost != null)
            {
                if (protocolType != ProtocolType.Udp)
                {
                    localSocket = CreateSocket(socketType, protocolType);

                } 

                else
                {
                    udpSocket = new UdpClient(localHost.GetHostEndPoint());

                }

            }

            else
            {
                localSocket = null;
                udpSocket = null;

            }

        }

        //member functions
        //starts up a constructed socket
        public void Start()
        {
            Start(10);

        }

        public void Start(int backlog)
        {
            if(localSocket == null && udpSocket == null)
            {
                Console.Write("Cannot start, socket creation failed.\n\r");

            }

            //connect server socket
            else if(remoteHost.GetHostIPAddress() == IPAddress.Any)
            {
                try
                {
                    if(protocolType != ProtocolType.Udp)
                    {
                        localSocket.Bind(localHost.GetHostEndPoint());
                        localSocket.Listen(backlog);

                    }

                    //nothing needs to happen for UDP server
                    else
                    {


                    }

                }

                catch(Exception e)
                {
                    Console.Write("Server bind error: {0}\n\r", e.Message);

                }

            }

            //connect client socket
            else if (remoteHost.GetHostIPAddress() != IPAddress.Any)
            {
                try
                {
                    if (protocolType != ProtocolType.Udp)
                    {
                        localSocket.Connect(remoteHost.GetHostEndPoint());

                    }

                    //run as connectionless udp socket
                    else
                    {
                        //udpSocket.Connect(remoteHost.GetHostEndPoint());

                    }

                }

                //failed to connect with current ipaddress, (probably wrong address family) try other addresses
                catch(SocketException e)
                {
                    while(true)
                    {
                        if(remoteHost.SetNextHostIPAddress())
                        {
                            localSocket = CreateSocket(socketType, protocolType);

                            try
                            {
                                localSocket.Connect(remoteHost.GetHostEndPoint());
                                break;

                            }

                            //still no dice, try again
                            catch(SocketException f)
                            {
                            }

                            //something else went wrong, exit
                            catch (Exception f)
                            {
                                Console.Write("Local socket connect failed: {0}\n\r", f.Message);

                                return;

                            }

                        }

                        //none of the ip addresses returned from DNS lookup worked, fail
                        else
                        {
                            Console.Write("Unable to find suitable IP address: {0}\n\r", e.Message);

                            return;

                        }

                    }

                }

                catch(Exception e)
                {
                    Console.Write("Local socket connect failed: {0}\n\r", e.Message);

                    return;

                }

            }

            //bind server socket
            else
            {
                try
                {
                    localSocket.Bind(remoteHost.GetHostEndPoint());
                    localSocket.Listen(backlog);

                }

                //failed to bind with current ipaddress, (probably wrong address family) try other addresses
                catch (SocketException e)
                {
                    while (true)
                    {
                        if (remoteHost.SetNextHostIPAddress())
                        {
                            localSocket = CreateSocket(socketType, protocolType);

                            try
                            {
                                localSocket.Bind((EndPoint)remoteHost.GetHostEndPoint());
                                localSocket.Listen(backlog);

                                break;

                            }

                            //still no dice, try again
                            catch (SocketException f)
                            {
                            }

                            //something else went wrong, exit
                            catch (Exception f)
                            {
                                Console.Write("Local socket bind failed: {0}\n\r", f.Message);

                                return;

                            }

                        }

                        //none of the ip addresses returned from DNS lookup worked, fail
                        else
                        {
                            Console.Write("Unable to find suitable IP address: {0}\n\r", e.Message);

                            return;

                        }

                    }

                }

                catch(Exception e)
                {
                    Console.Write("Local socket bind failed: {0}\n\r", e.Message);

                    return;

                }

            }

        }

        //closes all connections on socket
        public void Stop()
        {
            try
            {
                if (protocolType == ProtocolType.Udp)
                {
                    udpSocket.Close();

                }

                else
                {
                    localSocket.Shutdown(SocketShutdown.Both);
                    localSocket.Close();

                }

            }

            catch(Exception e)
            {
                Console.Write("Failed to stop socket: {0}\n\r", e.Message);

            }

        }

        //socket re-initialization functions
        public void Restart()
        {
            if(localSocket == null)
            {
                Console.Write("Failed to restart local socket: socket construction failed.\n\r");

                return;

            }

            else if(protocolType == ProtocolType.Unknown || socketType == SocketType.Unknown)
            {
                Console.Write("Failed to restart local socket: invalid socket connection parameters.\n\r");

            }

            Stop();

            Start();

        }

        public void Restart(HostAddress hostAddress)
        {
            if(hostAddress == null)
            {
                Console.Write("Failed to restart local socket: provided host address invalid.\n\r");

                return;

            }

            remoteHost = hostAddress;

            Restart();

        }

        public void Restart(string hostname, string port)
        {
            remoteHost.SetHostInfo(hostname, port);

            if (remoteHost == null)
            {
                Console.Write("Failed to restart local socket: host lookup failed.\n\r");

            }

            Restart();

        }

        public void Restart(HostAddress hostAddress, SocketType socketType, ProtocolType protocolType)
        {
            this.socketType = socketType;
            this.protocolType = protocolType;

            if (hostAddress == null)
            {
                Console.Write("Failed to restart local socket: provided host address invalid.\n\r");

                return;

            }

            remoteHost = hostAddress;

            Restart();

        }

        public void Restart(string hostname, string port, SocketType socketType, ProtocolType protocolType)
        {
            this.socketType = socketType;
            this.protocolType = protocolType;

            remoteHost.SetHostInfo(hostname, port);

            if (remoteHost == null)
            {
                Console.Write("Failed to restart local socket: host lookup failed.\n\r");

            }

            Restart();

        }

        public void Send(string msg)
        {
            Send(Encoding.Unicode.GetBytes(msg));

        }

        public int Send(byte[] msg)
        {
            try
            {
                if (protocolType == ProtocolType.Udp)
                {
                    if (incomingEndpoint == null)
                    {
                        return udpSocket.Send(msg, msg.Length, remoteHost.GetHostEndPoint());

                    }

                    else
                    {
                        return udpSocket.Send(msg, msg.Length, incomingEndpoint);

                    }

                }

                else
                {
                    return localSocket.Send(msg);

                }

            }

            catch(Exception e)
            {
                Console.Write("Failed to send through socket: {0}\n\r", e.Message);

                return -1;

            }
            
        }

        public string Receive()
        {
            byte[] msgBuf = new Byte[buffSize];
            string result = null;
            int bytesRec = 0;

            //shamelessly copied from an MSDN tutorial on server/clients:
            //http://msdn.microsoft.com/en-us/library/6y0e13d3%28v=vs.110%29.aspx
            while(true)
            {
                //clear message buffer
                msgBuf = new Byte[buffSize];

                //receive message portion
                bytesRec = Receive(ref msgBuf);

                //append converted data to result
                result += Encoding.Unicode.GetString(msgBuf, 0, bytesRec);

                //check if there's any more data
                if(protocolType == ProtocolType.Udp)
                {
                    if(udpSocket.Available == 0)
                    {
                        break;

                    }

                }

                else
                {
                    if(localSocket.Available == 0)
                    {
                        break;

                    }

                }

            }

            return result;

        }

        public int Receive(ref byte[] msg)
        {
            try
            {
                if (protocolType == ProtocolType.Udp)
                {
                    msg = udpSocket.Receive(ref incomingEndpoint);

                    remoteHost.SetHostInfo(incomingEndpoint.Address.ToString(), incomingEndpoint.Port.ToString());

                    return msg.Length;

                }

                else
                {
                    return localSocket.Receive(msg);

                }

            }

            catch(Exception e)
            {
                Console.Write("Failed to receive through socket: {0}\n\r", e.Message);

                return -1;

            }

        }

        public int Available()
        {
            if(localSocket != null)
            {
                return localSocket.Available;

            }

            else if(udpSocket != null)
            {
                return udpSocket.Available;

            }

            else
            {
                return 0;

            }

        }

        //spawn new connection sockets for incoming connections
        //this function blocks until a new connection is receieved
        public ConnectionSocket AcceptIncoming()
        {
            //peeps can't connect to a client socket, this might throw an exception if misused.
            if (protocolType == ProtocolType.Tcp)
            {
                return new ConnectionSocket(localSocket.Accept());

            }

            else
            {
                return this;

            }

        }

        public IPEndPoint GetHostInfo()
        {
            return (IPEndPoint)localSocket.RemoteEndPoint;

        }

        public HostAddress GetRemoteHostAddress()
        {
            return remoteHost;

        }

        public HostAddress GetLocalHostAddress()
        {
            return localHost;

        }

        //private methods
        //constructor for handling incoming connections
        private ConnectionSocket(Socket incoming)
        {
            socketType = SocketType.Unknown;
            protocolType = ProtocolType.Unknown;
            remoteHost = null;
            if (incoming != null)
            {
                localSocket = incoming;

            }

            else
            {
                Console.Write("Failed to handle incoming socket.\n\r");
                localSocket = null;

            }

        }

        private Socket CreateSocket(SocketType socketType, ProtocolType protocolType)
        {
            Socket clientSocket;

            try
            {
                clientSocket = new Socket(remoteHost.GetHostIPAddress().AddressFamily, socketType, protocolType);

                return clientSocket;

            }

            catch (Exception e)
            {
                Console.Write("Could not create socket: {0}\n\r", e.Message);

                return null;

            }

        }

    }
}
