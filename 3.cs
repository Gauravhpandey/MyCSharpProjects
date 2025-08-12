using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
//client
class TcpClientApp
{
    static void Main()
    {
        Console.Write("Enter server IP (default 127.0.0.1): ");
        string ip = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(ip)) ip = "127.0.0.1";

        using TcpClient client = new TcpClient(ip, 5000);
        using NetworkStream stream = client.GetStream();
        using StreamReader reader = new(stream, Encoding.UTF8);
        using StreamWriter writer = new(stream, Encoding.UTF8) { AutoFlush = true };

        Console.Write("Enter request (e.g., SetA-Two): ");
        string request = Console.ReadLine();
        writer.WriteLine(AesEncryption.Encrypt(request));

        Console.WriteLine("Response from server:");
        while (true)
        {
            string encryptedResponse = reader.ReadLine();
            if (string.IsNullOrEmpty(encryptedResponse)) break;

            string response = AesEncryption.Decrypt(encryptedResponse);
            Console.WriteLine(response);

            if (response == "EMPTY") break;
        }
    }
}
