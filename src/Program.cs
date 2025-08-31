using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace RTSPapplication
{
    class Program
    {
        static bool isRunning = true;
        private string _host;
        private int _port;
        private string _streamPath;
        private TcpClient _tcpClient;
        private NetworkStream _networkStream;
        private int _cSeq = 1;
        private string _sessionId;

        static void Main(string[] args)
        {

            Console.WriteLine("=======================================");
            Console.WriteLine("        RTSP Application v1.0.0        ");
            Console.WriteLine("=======================================");
            Console.WriteLine("Welcome to the RTSP Application!");
            Console.WriteLine();
            Console.WriteLine("This application connects to an RTSP stream and records the video feed.");
            Console.WriteLine("To use this application, please provide the RTSP URL as a command line argument.");
            Console.WriteLine("Example: RTSPapplication.exe rtsp://example.com/stream");
            Console.WriteLine();
            Console.WriteLine($"Command Console Started at {DateTime.Now}. Type 'exit' to quit.");

            while (isRunning)
            {
                Console.Write("> ");
                string input = Console.ReadLine().Trim().ToLower();

                switch (input)
                {
                    case "test":
                        Console.WriteLine("RTSP Client Connect() called.");
                        break;
                    case "time":
                        Console.WriteLine($"Current time: {DateTime.Now}");
                        break;
                    case "exit":
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Unknown command. Available commands: hello, time, exit");
                        break;
                }
            }
        }
     }
}