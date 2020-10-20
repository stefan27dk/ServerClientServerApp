using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerClientServerApp
{
    public  class Program
    {
        // Client Handlers <--List-->
        public static ArrayList clientHandlersList = new ArrayList();


        // Main
        public  static void Main(string[] args)
        {

            // Server Action ::START::
            try
            {
                IPAddress ipAd = IPAddress.Parse("127.0.0.1"); // Server Adress is LocalHost IP-Adress
                TcpListener listener = new TcpListener(ipAd, 8001); // Server port is 8001 -  Listen for connection at this port "The server Locallhost Port" The Clients will connect on "localhost:8001"
                listener.Start();   

                Console.WriteLine("The local End point is  :" + listener.LocalEndpoint); // The Server Adress

                // Loop - Listen For Users to Connect
                while (true)
                {
                    Console.WriteLine("Waiting for Connection..........");
                    Socket socket = listener.AcceptSocket();
                    ClientHandler ch = new ClientHandler(socket);
                    lock (clientHandlersList)
                    {
                        clientHandlersList.Add(ch);
                    }
                    Thread t = new Thread(new ThreadStart(ch.HandleClient));
                    t.Start();
                    Console.WriteLine("Connected");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }


        }





        // Client Handler: || CLASS ||
        public class ClientHandler
        {

            private Socket _socket;
            private NetworkStream socketStream;
            public BinaryWriter writer;
            private BinaryReader reader;
                 


            // Constructor
            public ClientHandler(Socket socket)
            {
                _socket = socket;
                socketStream = new NetworkStream(_socket);
                writer = new BinaryWriter(socketStream);
                reader = new BinaryReader(socketStream);
            }



            // Handle Client  || Method ||
            public void HandleClient()
            {
                Console.WriteLine("Connection Accepted from " + _socket.RemoteEndPoint);
                string txt = "retur ";
                while (true)
                {
                    txt = reader.ReadString();

                    lock (Program.clientHandlersList)
                    {
                        foreach (ClientHandler ch in Program.clientHandlersList)
                        {
                            (ch.writer).Write(txt + "BACK");
                            Console.WriteLine(ch.reader.ReadString());
                        }
                    }
                }
            }

        }
    }
}
