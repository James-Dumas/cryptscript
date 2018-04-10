using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace scriptcoin
{
    public class Blockchain
    {
        /*
        public static void Connect(String message)
        {
            try
            {
                string remoteHost = String.Empty;
                string[] knownNodes = File.ReadAllLines(Environment.CurrentDirectory.ToString() + "\\documents\\knownNodes.txt");
                int length = knownNodes.Length;
                Console.WriteLine(length);
                
                IPHostEntry IPHost = Dns.GetHostEntry(Dns.GetHostName());
                string localHost = IPHost.AddressList[3].ToString();
                Console.WriteLine(localHost);
                Console.ReadKey();

                for (int i = 0; i < knownNodes.Length; i++)
                {
                    Ping x = new Ping();
                    PingReply reply = x.Send(IPAddress.Parse(knownNodes[i]));

                    if (reply.Status == IPStatus.Success)
                    {
                        remoteHost = knownNodes[i];
                        Console.WriteLine("Connected");
                        break;
                    }
                }

                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
                Int32 port = 8333;
                TcpClient client = new TcpClient(remoteHost, port);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", message);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }

        public static void Recieve()
        {

        }
        */
        public static void Difficulty()
        {
            string difficulty = "00";
            long prevBlock = 152164899737;
            long currentBlock = DateTimeOffset.Now.ToUnixTimeSeconds();
            long blockDelay = Math.Abs(currentBlock - prevBlock);
            prevBlock = currentBlock;

            if (blockDelay > 10)
            {
                difficulty = difficulty + "0";
            }
            
            if (blockDelay < 20)
            {
                if (difficulty.Length > 1)
                {
                    difficulty = difficulty.Substring(0, (difficulty.Length - 1));
                }
            }

            Miner.Difficulty = difficulty;
        }
        
        public static void Reward(int blockNumber)
        {
            double reward = 50;

            if (blockNumber % (423360) == 1)
            {
                if ((blockNumber <= 50))
                {
                    reward = 50;
                }
                else reward = reward * 0.75;
            }

            Miner.Reward =  reward.ToString() + "+" + blockNumber.ToString();
        }
    }
}

