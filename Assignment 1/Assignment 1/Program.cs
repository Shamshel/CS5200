using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Assignment_1
{
    class Program
    {
        static void Main(string[] args)
        {
            bool done = false;
            string hostname;
            string remotePort;
            string localPort;

            ConnectionSocket remoteSocket;

            HostAddress remoteAddress = new HostAddress();
            HostAddress localAddress = new HostAddress();

            Menu();

            while(!done)
            {
                string choice = System.Console.ReadLine();
                if(!string.IsNullOrEmpty(choice) && choice.Length>0)
                {
                    switch( choice.Trim().ToUpper().Substring(0,1) )
                    {
                        case "T":
                            Console.Write("enter host name: ");
                            hostname = System.Console.ReadLine();

                            Console.Write("enter remote port number: ");
                            remotePort = System.Console.ReadLine();

                            Console.Write("enter local port number: ");
                            localPort = System.Console.ReadLine();

                            remoteAddress.SetHostInfo(hostname, remotePort);
                            localAddress.SetHostInfo(IPAddress.Any, localPort);

                            Console.Write("remote host name: {0}\n\r", remoteAddress.GetHostName());
                            Console.Write("remote host IP address: {0}\n\r", remoteAddress.GetHostIPAddress());
                            Console.Write("remote host port: {0}\n\r", remoteAddress.GetHostPort());
                            Console.Write("local host port: {0}\n\r", localAddress.GetHostPort());

                            break;

                        case "S":
                            if (remoteAddress.GetHostEntry() == null)
                            {
                                Console.Write("Must set remote host info first!\n\r");
                                break;

                            }

                            remoteSocket = new ConnectionSocket(remoteAddress.GetHostPort().ToString(), localAddress.GetHostPort().ToString(), SocketType.Dgram, ProtocolType.Udp);

                            remoteSocket.Start();

                            while(done != true)
                            {
                                Console.Write("Waiting for incoming connection on address {0}:{1}...", remoteSocket.GetLocalHostAddress().GetHostIPAddress().ToString(), remoteSocket.GetLocalHostAddress().GetHostPort().ToString());
                                ConnectionSocket incoming = remoteSocket.AcceptIncoming();

                                string message;
                                message = incoming.Receieve();

                                Console.Write("connected from {0}\n\r", incoming.GetRemoteHostAddress().GetHostName());

                                Console.Write("Recieved message: {0}\n\r", message);

                                incoming.Send(message);

                                Console.Write("Continue? (y\\n): ");
                                choice = System.Console.ReadLine();
                                if (!string.IsNullOrEmpty(choice) && choice.Length > 0)
                                {
                                    switch (choice.Trim().ToUpper().Substring(0, 1))
                                    {
                                        case "Y":
                                            done = false;

                                            break;

                                        case "N":
                                            done = true;
                                            incoming.Stop();

                                            break;

                                        default:
                                            done = false;

                                            break;

                                    }

                                }

                            }

                            done = false;

                            break;

                        case "C":
                            if (remoteAddress.GetHostEntry() == null)
                            {
                                Console.Write("Must set remote host info first!\n\r");
                                break;

                            }

                            Console.Write("Starting client...");
                            remoteSocket = new ConnectionSocket(remoteAddress, localAddress.GetHostPort().ToString(), SocketType.Dgram, ProtocolType.Udp);
                            remoteSocket.Start();
                            Console.Write("done\n\r");

                            while (!done)
                            {
                                while (true)
                                {
                                    Console.Write("Enter message: \n\r");
                                    choice = System.Console.ReadLine();
                                    if (!string.IsNullOrEmpty(choice) && choice.Length > 0)
                                    {
                                        remoteSocket.Send(choice.Trim());
                                        break;

                                    }

                                }

                                Console.Write("Recieved response: {0}\n\r", remoteSocket.Receieve());

                                Console.Write("Continue? (y\\n): ");
                                choice = System.Console.ReadLine();
                                if (!string.IsNullOrEmpty(choice) && choice.Length > 0)
                                {
                                    switch (choice.Trim().ToUpper().Substring(0, 1))
                                    {
                                        case "Y":
                                            done = false;

                                            break;

                                        case "N":
                                            done = true;

                                            break;

                                        default:
                                            done = false;

                                            break;

                                    }

                                }

                            }

                            done = false;

                            break;

                        case "E":
                            done = true;

                            break;

                        default:
                            Menu();

                            break;

                    }

                }

            }

        }

        static public void Menu()
        {
            Console.Write("Assignment_1 menu:\n\r");
            Console.Write("\tse[T] host info\n\r");
            Console.Write("\tstart test [S]erver\n\r");
            Console.Write("\tstart test [C]lient\n\r");
            Console.Write("\t[E]xit\n\r");

        }

    }
}
