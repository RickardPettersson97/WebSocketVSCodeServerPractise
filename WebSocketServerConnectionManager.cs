using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;


namespace WebSocketServer 
{

public class WebSocketServerConnectionManager
{
    private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();   //addar in vid _sockets.TryAdd(ConnID, socket), lägger då in en sträng som key, och value är vår socket key / value pair

    public string AddSocket(WebSocket socket)
    {
        string ConnID = Guid.NewGuid().ToString();
        _sockets.TryAdd(ConnID, socket);
        Console.WriteLine("WebSocketServerConnectionManager-> AddSocket: WebSocket added with ID: " + ConnID);
        return ConnID;
    }

    public ConcurrentDictionary<string, WebSocket> GetAllSockets()       //returnar alla våra sockets connection.
    {
        return _sockets;
    }

    }

}







