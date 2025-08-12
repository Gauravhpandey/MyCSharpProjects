using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

//server
class TcpServer
{
    static Dictionary<string, Dictionary<string, int>> data = new()
    {
        { "SetA", new(){{"One", 1}, {"Two", 2}} },
        { "SetB", new(){{"Three", 3}, {"Four", 4}} },
        { "SetC", new(){{"Five", 5}, {"Six", 6}} },
        { "SetD", new(){{"Seven", 7}, {"Eight", 8}} },
        { "SetE", new(){{"Nine", 9}, {"Ten", 10}} }
    };

    static void Main()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();
        Console.WriteLine("Server started on port 5000...");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Task.Run(() => HandleClient(client));
        }
    }

    static void HandleClient(TcpClient client)
    {
        try
        {
            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new(stream, Encoding.UTF8);
            using StreamWriter writer = new(stream, Encoding.UTF8) { AutoFlush = true };

            string encryptedRequest = reader.ReadLine();
            string request = AesEncryption.Decrypt(encryptedRequest);
            Console.WriteLine($"Received: {request}");

            string[] parts = request.Split('-');
            string setName = parts.Length > 0 ? parts[0] : "";
            string keyName = parts.Length > 1 ? parts[1] : "";

            if (data.ContainsKey(setName) && data[setName].ContainsKey(keyName))
            {
                int count = data[setName][keyName];
                for (int i = 0; i < count; i++)
                {
                    string message = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                    writer.WriteLine(AesEncryption.Encrypt(message));
                    Task.Delay(1000).Wait();
                }
            }
            else
            {
                writer.WriteLine(AesEncryption.Encrypt("EMPTY"));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            client.Close();
        }
    }
}
