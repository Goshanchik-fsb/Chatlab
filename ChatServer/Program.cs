using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("📡 Server v1.0 started");
        Console.WriteLine("🚀 Chat Server starting...");

        TcpListener server = new TcpListener(IPAddress.Any, 8888);
        server.Start();
        Console.WriteLine("✅ Server listening on port 8888");

        while (true)
        {
            var client = await server.AcceptTcpClientAsync();
            Console.WriteLine("👤 Client connected");
            _ = HandleClientAsync(client);
        }
    }

    static async Task HandleClientAsync(TcpClient client)
    {
        var stream = client.GetStream();
        var buffer = new byte[1024];

        try
        {
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string message = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"📨 Received: {message}");

                byte[] response = System.Text.Encoding.UTF8.GetBytes($"Server got: {message}");
                await stream.WriteAsync(response, 0, response.Length);
            }
        }
        catch { }
        finally { client.Close(); }
    }
}