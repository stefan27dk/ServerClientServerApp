using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ServerClientServerApp
{
    public  class Program
    {
        // Client Handlers <--List-->
        public static List<Object> clientHandlersList = new List<Object>();


        // Main
        public  static void Main(string[] args)
        {

           
            try
            {
                IPAddress ipAd = IPAddress.Parse("127.0.0.1"); // Server Adress  
                TcpListener listener = new TcpListener(ipAd, 8001); // Server port is 8001  
                listener.Start();     
                Console.WriteLine("The local End point is  :" + listener.LocalEndpoint); // The Server Adress

                // Start Reader - Reads from all users
                //System.Threading.Tasks.Task.Factory.StartNew(MessagesReaderAllCH);

                // Loop - Listen For Users to Connect
                while (true)
                {
                    Console.WriteLine("Waiting for Connection..........");
                    Socket socket = listener.AcceptSocket();
                    ClientHandler ch = new ClientHandler(socket);

                    clientHandlersList.Add(ch);

                    Thread t = new Thread(delegate () {
                        ch.HandleClient();
                    });
                    t.Start();
                
                    Console.WriteLine("Connected");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
                Environment.Exit(0);
            }


        }



        //public static void MessagesReaderAllCH()
        //{
        //    while(true)
        //    {
        //        lock(clientHandlersList)
        //        {   
        //          foreach (ClientHandler ch in clientHandlersList)
        //          {
        //              Console.WriteLine(ch.reader.ReadString());   
        //          }
        //        }
                 
        //    }
        //}


        // Client Handler: || CLASS ||
        public class ClientHandler
        {

            private Socket _socket;
            private NetworkStream socketStream;
            public BinaryWriter writer;
            public BinaryReader reader;
                 


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
                string txt;
                while (true)
                {
                         txt = this.reader.ReadString();
                         Console.WriteLine(txt);
                         this.writer.Write($"{txt} + BACK");
                             
                }
            }

        }
    }
}
