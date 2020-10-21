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
        public static List<ClientHandler> clientHandlersList = new List<ClientHandler>();


        // Main                     
        public  static void Main(string[] args)
        {       
            try
            {
                //------Settings-----------------------------------------------------------------
                IPAddress ipAd = IPAddress.Parse("127.0.0.1"); // Server Adress  
                TcpListener listener = new TcpListener(ipAd, 8001); // Server port is 8001  
                listener.Start();     
                Console.WriteLine("The local End point is  :" + listener.LocalEndpoint); // The Server Adress
                    
                  
                // Listen-------------- For Users to Connect-------------------------------------
                while (true)
                {
                    Console.WriteLine("Waiting for Connection..........");
                    Socket socket = listener.AcceptSocket(); // Listening - internal loop in Listener - Break loop if uer is connected
                    ClientHandler ch = new ClientHandler(socket); // New Client handler
                     
                    lock(clientHandlersList) { clientHandlersList.Add(ch);} // Add User to List
                    new Thread(()=>{ch.HandleClient();}).Start(); // User Thread for each user      
                    Console.WriteLine("User - Connected");
                }    
            }     
            // Catch
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
                Environment.Exit(0);
            }  
        }


        


        


         // Bid Class
        public static class BidData
        {
            public static int Bid { get; private set; }

            private static object lockObj = new object();  // Lock Obj



            // Update Bid ||Method||
            public static void UpdateBid(int UserBid)
            {
                      lock(lockObj)
                      {   
                              if (UserBid > Bid)
                              {    
                                  Bid = UserBid;
                                  Console.WriteLine("Current BID is: " + BidData.Bid);
                                  NotifyAllUsers();    
                              }
                              
                      }  
            }

            // Notify Clients Method
            public static void NotifyAllUsers()
            {
                for (int i = 0; i < clientHandlersList.Count; i++)     
                {
                     
                     try
                     {
                        clientHandlersList[i].writer.Write("Current BID is: " + BidData.Bid); 
                     }
                     catch(Exception)
                     {   
                       clientHandlersList.Remove(clientHandlersList[i]);
                        i--;
                     }
                       
                }
            }

        }


                 








        // Client Handler: || CLASS || -----------------------------------------------------------
        public class ClientHandler 
        {
            // Resources--------------------------------------------------------------------------
            public Socket _socket;
            public NetworkStream socketStream;
            public BinaryWriter writer;
            public BinaryReader reader;
            
         

            // Constructor------------------------------------------------------------------------
            public ClientHandler(Socket socket)
            {
                _socket = socket; // Got Socket
                socketStream = new NetworkStream(_socket); // Network Stream from Socket
                writer = new BinaryWriter(socketStream);
                reader = new BinaryReader(socketStream);
            }
 
              

            // Handle Client  || Method ||--------------------------------------------------------
            public void HandleClient()
            {
                Console.WriteLine("Connection Accepted from " + _socket.RemoteEndPoint);
                string txt;
                writer.Write("Current BID is: " + BidData.Bid);
                while (true)
                {
                    if(socketStream.DataAvailable == true) // Prevents Crashing on User Disconnect
                    {    
                      txt = reader.ReadString(); // Get Bid from this Client
                      Task.Run(()=> { BidData.UpdateBid(Int32.Parse(txt)); }); // Try to Update and return True or false
                         
                      //Console.WriteLine("Now Bid is: " + txt);  // Write on Server
                    }
                    
                }
            }    
        }
    

    }
}
