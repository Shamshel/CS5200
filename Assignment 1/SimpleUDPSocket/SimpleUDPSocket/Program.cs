using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace SimpleUDPSocket
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Do you want to send or receive message? (enter S or R): ");
            string choice = System.Console.ReadLine();
            if (!string.IsNullOrEmpty(choice) && choice.Length>0)
            {
                switch (choice.Trim().ToUpper().Substring(0,1))
                {
                    case "S":
                        SimpleSender sender = new SimpleSender();
                        sender.DoStuff();
                        break;
                    case "R":
                        SimpleReceiver receiver = new SimpleReceiver();
                        receiver.Receive();
                        break;                  
                }
            }
        }

    }
}
