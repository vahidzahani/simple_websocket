
using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;



namespace ConsoleApp1
{
    internal class Program
    {
        static void Main()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://127.0.0.1:8080/");
            listener.Start();
            Console.WriteLine("Listening connections [8080] ...");

            ThreadPool.QueueUserWorkItem(ProcessWebSocketRequests, listener);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            listener.Stop();
        }

        static void ProcessWebSocketRequests(object state)
        {
            HttpListener listener = (HttpListener)state;
            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                if (context.Request.IsWebSocketRequest)
                {
                    ProcessWebSocketRequest(context);
                }
            }
        }

        static async void ProcessWebSocketRequest(HttpListenerContext context)
        {
            HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
            WebSocket webSocket = webSocketContext.WebSocket;

            try
            {
                byte[] buffer = new byte[1024];
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                while (!result.CloseStatus.HasValue)
                {
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine("Message: " + receivedMessage);

                    buffer = new byte[1024];
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }

                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine("WebSocket exception: " + ex.Message);
            }
            finally
            {
                webSocket.Dispose();
            }
        }



    }
}
