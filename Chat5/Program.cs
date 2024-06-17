using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Chat5;

namespace Chat5
{
    public class Programm
    {
        public static event Action<string> MessageReceived;
        private static List<Message> messageList = new List<Message>(); 

        static void Main(string[] args)
        {
            MessageReceived += ConfirmMessageReceived;
            var cts = new CancellationTokenSource();
            var serverThread = new Thread(() => Server(cts.Token));
            serverThread.Start();

            Console.WriteLine("Нажмите любую клавишу для остановки сервера...");
            Console.ReadKey();
            cts.Cancel();
            serverThread.Join();
        }

        public static void ConfirmMessageReceived(string message)
        {
            Console.WriteLine($"Подтверждение: сообщение '{message}' получено.");
        }

        public static void Server(CancellationToken ct)
        {
            using (UdpClient udpClient = new UdpClient(12345))
            {
                IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);
                Console.WriteLine("Ожидание сообщений...");

                try
                {
                    while (true)
                    {
                        ct.ThrowIfCancellationRequested();

                        if (udpClient.Available > 0)
                        {
                            byte[] buffer = udpClient.Receive(ref iPEndPoint);
                            var messageText = Encoding.UTF8.GetString(buffer);
                            MessageReceived?.Invoke(messageText);
                            Message message = Message.DeserializeFromJson(messageText);

                            if (message.Text == "LIST")
                            {
                                SendUnreadMessages(iPEndPoint, udpClient);
                            }
                            else
                            {
                                messageList.Add(message); 
                                Message clonedMessage = message.Clone();
                                clonedMessage.Print();
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    udpClient.Close();
                    Console.WriteLine("Сервер остановлен по запросу.");
                }
            }
        }

        private static void SendUnreadMessages(IPEndPoint clientEndPoint, UdpClient udpClient)
        {
            foreach (var message in messageList)
            {
                string json = message.SerializeMessageToJson();
                byte[] data = Encoding.UTF8.GetBytes(json);
                udpClient.Send(data, data.Length, clientEndPoint);
            }
            messageList.Clear(); 
        }
    }
}