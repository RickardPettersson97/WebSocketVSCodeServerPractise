using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Newtonsoft.Json;




namespace WebSocketServer.middleware
{

    public class WebSocketServerMiddleware 
    {

        private readonly RequestDelegate _next;

       private readonly WebSocketServerConnectionManager _manager;




        public WebSocketServerMiddleware(RequestDelegate next, WebSocketServerConnectionManager manager) 
        {
           _next = next; 
           _manager = manager;

        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

                Console.WriteLine("WebSocket Connected");

                string ConnID = _manager.AddSocket(webSocket);

                await SendConnIDAsync(webSocket, ConnID);

                await Receive(webSocket, async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        Console.WriteLine($"Receive->Text");
                        Console.WriteLine($"Message: {Encoding.UTF8.GetString(buffer, 0, result.Count)}");
                        await RouteJSONMessageAsync(Encoding.UTF8.GetString(buffer, 0, result.Count));
                        return;
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {

                        string id = _manager.GetAllSockets().FirstOrDefault(s => s.Value == webSocket).Key;
                        Console.WriteLine($"Receive->Close");

                        _manager.GetAllSockets().TryRemove(id, out WebSocket sock);
                        Console.WriteLine("Managed Connections: " + _manager.GetAllSockets().Count.ToString());
                        await sock.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

                        return;
                    }
                });
            }
            else
            {
                Console.WriteLine("Hello from 2nd Request Delegate - No WebSocket");
                await _next(context);
            }
        }

        public static async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer), cancellationToken: CancellationToken.None);

                handleMessage(result, buffer);

            }

        }



        private async Task SendConnIDAsync(WebSocket socket, string connID)
        {
         var buffer = Encoding.UTF8.GetBytes("ConnID: " + connID);
         await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

        }





private async Task RouteJSONMessageAsync(string message)
{

 var routeOb = JsonConvert.DeserializeObject<dynamic>(message);

 if (Guid.TryParse(routeOb.To.ToString(), out Guid guidOutput))         //is guid present? 
 {
    Console.WriteLine("Targeted");
    var sock = _manager.GetAllSockets().FirstOrDefault(s => s.Key == routeOb.To.ToString());          //is server managing this guid? (checking in getallsockets in manager)
    if (sock.Value != null) 
    {
        if (sock.Value.State == WebSocketState.Open)                  //calling sendAsync method, to send content.
        await sock.Value.SendAsync(Encoding.UTF8.GetBytes(routeOb.Message.ToString()), WebSocketMessageType.Text, true, CancellationToken.None);
    }
    else 
    {
        Console.WriteLine("Invalid Recipient");                 //else attached to above if (sock.value != null), otherwise its invalid, error message
    }
 }
 else 
 {
    Console.WriteLine("Broadcast");                   //  else its not to a specific one, then its broadcast 
    foreach (var sock in _manager.GetAllSockets())
    {
        if (sock.Value.State == WebSocketState.Open)
        await sock.Value.SendAsync(Encoding.UTF8.GetBytes(routeOb.Message.ToString()), WebSocketMessageType.Text, true, CancellationToken.None);
    }
 }

 }

    }
}

                





            





            



