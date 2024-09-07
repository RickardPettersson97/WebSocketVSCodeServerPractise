using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;




namespace myNameSpace 
{
    public static class WebSocketHelper 
    {

public static async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
{
  var buffer = new byte[1024 * 4];

  while (socket.State == WebSocketState.Open)
  {
    var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                                                       cancellationToken: CancellationToken.None);
    handleMessage(result, buffer);
  }
 }
}
}