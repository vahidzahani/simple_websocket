using System.Net;

namespace ConsoleApp1
{
    internal class WebSocketServer
    {
        private IPAddress any;
        private int port;

        public WebSocketServer(IPAddress any, int port)
        {
            this.any = any;
            this.port = port;
        }
    }
}