using System.Text;
using System.Net;
using System.Net.Sockets;

namespace SocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] bytes = new byte[1024];
            bool isRorL = true;
            IPHostEntry ipHost = Dns.GetHostEntry("10.10.10.246");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new(ipAddr, 11000);

            Socket sListener = new(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            Console.WriteLine("Enter the number of clients: ");
            int countClients = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Let's go!!!");

            try
            {
                List<Socket> sockets = new();
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);

                while (true)
                {
                    Socket handl = sListener.Accept();
                    Console.Write("Another one with us ");

                    int bytesRec = handl.Receive(bytes);

                    Console.Write($"--> {Encoding.UTF8.GetString(bytes, 0, bytesRec)}\n");
                    sockets.Add(handl);

                    if (sockets.Count == countClients)
                    {
                        Console.ReadLine();
                        foreach (Socket handler in sockets)
                        {
                            handler.Send(Encoding.UTF8.GetBytes("Start"));

                        }
                        while (true)
                        {
                            foreach (Socket handler in sockets)
                            {
                                handler.Send(Encoding.UTF8.GetBytes(isRorL ? "R" : "L"));

                                handler.Receive(bytes);
                            }
                            sockets.Reverse();
                            isRorL = !isRorL;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}