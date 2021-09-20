// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="GSD Logic">
//   Copyright © 2020 GSD Logic. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MCBE
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 19132));

            IPEndPoint xbox = null;
            IPEndPoint host = new IPEndPoint(IPAddress.Parse("192.168.1.211"), 19132);
            var active = false;

            while (true)
            {
                var remoteEndPoint = socket.LocalEndPoint;
                var bytes = new byte[2048];
                var count = socket.ReceiveFrom(bytes, ref remoteEndPoint);

                //Console.WriteLine("Received {0} bytes from {1}.", count, remoteEndPoint);

                if (!(remoteEndPoint is IPEndPoint ep))
                {
                    continue;
                }

                if (!ep.Address.Equals(host.Address))
                {
                    if (xbox == null)
                    {
                        Console.WriteLine("XBox: {0}", ep);
                        xbox = ep;
                    }

                    socket.SendTo(bytes, count, SocketFlags.None, host);
                }
                else if (ep.Address.Equals(host.Address) && (xbox != null))
                {
                    if (!active)
                    {
                        Console.WriteLine("Active");
                        active = true;
                    }

                    socket.SendTo(bytes, count, SocketFlags.None, xbox);
                }
            }
        }
    }
}