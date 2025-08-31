using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace RtspClient
{
    public class RtspClient
    {
        private string _host;
        private int _port;
        private string _streamPath;
        private TcpClient _tcpClient;
        private NetworkStream _networkStream;
        private int _cSeq = 1;
        private string _sessionId;

        public RtspClient(string host, int port = 554, string streamPath = "")
        {
            _host = host;
            _port = port;
            _streamPath = streamPath;
        }

        public void Connect()
        {
            _tcpClient = new TcpClient();
            _tcpClient.Connect(_host, _port);
            _networkStream = _tcpClient.GetStream();
            Console.WriteLine($"Connected to {_host}:{_port}");
        }

        public void SendOptions()
        {
            string request = BuildRequest("OPTIONS", $"rtsp://{_host}/{_streamPath}");
            SendRequest(request);
        }

        public void SendDescribe()
        {
            string request = BuildRequest("DESCRIBE", $"rtsp://{_host}/{_streamPath}", new[] {
                "Accept: application/sdp"
            });
            SendRequest(request);
        }

        public void SendSetup(string trackUrl)
        {
            string request = BuildRequest("SETUP", trackUrl, new[] {
                "Transport: RTP/AVP;unicast;client_port=8000-8001"
            });
            SendRequest(request);
        }

        public void SendPlay(string streamUrl)
        {
            string request = BuildRequest("PLAY", streamUrl, new[] {
                $"Session: {_sessionId}"
            });
            SendRequest(request);
        }

        private string BuildRequest(string method, string url, string[] headers = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{method} {url} RTSP/1.0");
            sb.AppendLine($"CSeq: {_cSeq++}");
            if (headers != null)
            {
                foreach (var header in headers)
                    sb.AppendLine(header);
            }
            sb.AppendLine(); // End of headers
            return sb.ToString();
        }

        private void SendRequest(string request)
        {
            byte[] requestBytes = Encoding.ASCII.GetBytes(request);
            _networkStream.Write(requestBytes, 0, requestBytes.Length);
            Console.WriteLine("Sent:\n" + request);

            using var reader = new StreamReader(_networkStream, Encoding.ASCII, leaveOpen: true);
            string response = ReadResponse(reader);
            Console.WriteLine("Received:\n" + response);

            if (response.Contains("Session:"))
            {
                int start = response.IndexOf("Session:") + 8;
                int end = response.IndexOf("\r\n", start);
                _sessionId = response.Substring(start, end - start).Trim();
            }
        }

        private string ReadResponse(StreamReader reader)
        {
            var sb = new StringBuilder();
            string line;
            while (!string.IsNullOrEmpty(line = reader.ReadLine()))
            {
                sb.AppendLine(line);
            }
            return sb.ToString();
        }

        public void Disconnect()
        {
            _networkStream?.Close();
            _tcpClient?.Close();
        }
    }
}