using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Chat5;

namespace Client
{
    public class Program
    {
        public static event Action<string> MessageSent;

        static void Main(string[] args)
        {
            MessageSent += ConfirmMessageSent;
            RequestUnreadMessages(args[0], args[1]); 
            SentMessage(args[0], args[1]);
        }

        public static void ConfirmMessageSent(string message)
        {
            Console.WriteLine($"Подтверждение: сообщение '{message}' отправлено.");
        }

        public static void RequestUnreadMessages(string From, string ip)
        {
            using (UdpClient udpClient = new UdpClient())
            {
                IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), 12345);
                Message message = new Message() { Text = "LIST", NicknameFrom = From, NicknameTo = "Server", DateTime = DateTime.Now };
                string json = message.SerializeMessageToJson();
                byte[] data = Encoding.UTF8.GetBytes(json);
                udpClient.Send(data, data.Length, iPEndPoint);
            }
        }
        public static void SentMessage(string From, string ip)
        {
            using (UdpClient udpClient = new UdpClient())
            {
                IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), 12345);
                string messageText = "привет";
                Message message = new Message() { Text = messageText, NicknameFrom = From, NicknameTo = "Server", DateTime = DateTime.Now, IsRead = false };
                string json = message.SerializeMessageToJson();
                byte[] data = Encoding.UTF8.GetBytes(json);
                udpClient.Send(data, data.Length, iPEndPoint);
                MessageSent?.Invoke(messageText);
            }
        }

        public static void RequestUnreadMessages(string ip)
        {
            using (UdpClient udpClient = new UdpClient())
            {
                IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), 12345);
                string request = "Запрос непрочитанных сообщений";
                byte[] data = Encoding.UTF8.GetBytes(request);
                udpClient.Send(data, data.Length, iPEndPoint);

                data = udpClient.Receive(ref iPEndPoint);
                var unreadMessages = JsonSerializer.Deserialize<List<Message>>(Encoding.UTF8.GetString(data));
                foreach (var message in unreadMessages)
                {
                    Console.WriteLine($"Непрочитанное сообщение: {message.Text}");
                }
            }
        }
    }
}
