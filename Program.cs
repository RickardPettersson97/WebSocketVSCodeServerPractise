using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.WebEncoders.Testing;
using System.Threading;
using System.Net.WebSockets;
using myNameSpace;
using WebSocketServer.middleware;
using WebSocketServer.Middleware;     //add in Middleware, so the app knows about it to go through http request when app.UseWebSocketServer

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddControllersWithViews(); 
builder.Services.AddWebSocketServerConnectionManager();    //this works? 

var app = builder.Build();

// Add WebSocket support


app.UseWebSockets();
app.UseWebSocketServer();


app.Use(async (context, next) =>
{


     var env = app.Environment;

 if (context.Request.Path == "/favicon.ico")         //this is it??? 
    {
        // Ignore favicon.ico request to avoid logging it
        return;
    }

    

    if (context.WebSockets.IsWebSocketRequest)
    {


        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
        Console.WriteLine("WebSocket Connected");






    await WebSocketServer.middleware.WebSocketServerMiddleware.Receive(webSocket, async (result, buffer) =>
     {
    if (result.MessageType == WebSocketMessageType.Text)
    {
      Console.WriteLine($"Receive->Text");

      return;
    }
    else if (result.MessageType == WebSocketMessageType.Close)
    {
      Console.WriteLine($"Receive->Close");

      return;
    }
  });
}

    else
    {

        Console.WriteLine("Hello from 2nd request delegate, no websocket yet");
        await next();
    }
});




app.Run(async context =>
{
    Console.WriteLine("Hello from 3rd (terminal) Request Delegate");
    await context.Response.WriteAsync("Hello from 3rd (terminal) Request Delegate");


});


// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();




/*
if (env.IsDevelopment())
    {
        // Log request details
        Console.WriteLine("Request Method: " + context.Request.Method);
        Console.WriteLine("Request Protocol: " + context.Request.Protocol);

        if (context.Request.Headers != null)
        {
            Console.WriteLine("Request Headers: ");
            foreach (var h in context.Request.Headers)
            {
                Console.WriteLine("--> " + h.Key + ": " + h.Value);
            }
        }
    }
    */