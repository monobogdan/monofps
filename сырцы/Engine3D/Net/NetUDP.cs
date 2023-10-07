using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Engine3D.Net
{
    public enum NetDataType
    {
        Unreliable,
        Reliable
    }

    public sealed class Client
    {
        
    }
    
    public sealed class Server
    {
        
        public struct Client
        {
            public IPEndPoint Address;
            public string Name;
        }

        public struct EnqueuedPacket
        {
            public NetDataType DataType;
            public int Message;
            public byte[] Data;
            public int Client;
        }

        public bool IsListening
        {
            get;
            set;
        }
        private List<Client> clients;
        private Queue<EnqueuedPacket> packets;
        private Thread listenThread;
        private Socket sock;

        public Server()
        {
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


            listenThread = new Thread(ListenThread);
            clients = new List<Client>();
        }

        private void ListenThread()
        {
            while(packets.Count > 0)
            {
                EnqueuedPacket packet = packets.Dequeue();
                MemoryStream strm = new MemoryStream(packet.Data.Length + 8);
                BinaryWriter writer = new BinaryWriter(strm);

                writer.Write(packet.Message);
                sock.SendTo(packet.Data, clients[packet.Client].Address);
            }


        }

        public void Start(int port)
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            
            sock.Bind(localEndPoint);
            sock.Listen(256);
            listenThread.Start();

            IsListening = true;
        }

        public void Broadcast(NetDataType dt, int msg, BinaryWriter data)
        {

        }

        public void Send(NetDataType dt, int clientNum, int msg, byte[] data)
        {
            lock(packets)
            {
                packets.Enqueue(new EnqueuedPacket()
                {
                    Data = data,
                    Message = msg,
                    DataType = dt,
                    Client = clientNum
                });
            }
        }
    }
}
