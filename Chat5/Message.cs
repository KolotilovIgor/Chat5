using System;
using System.Text.Json;

namespace Chat5
{
    public class Message
    {
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public string NicknameFrom { get; set; }
        public string NicknameTo { get; set; }
        public bool IsRead { get; set; }

        public string SerializeMessageToJson() => JsonSerializer.Serialize(this);
        public static Message DeserializeFromJson(string message) => JsonSerializer.Deserialize<Message>(message);

        public Message Clone()
        {
            return new Message { Text = this.Text, DateTime = this.DateTime, NicknameFrom = this.NicknameFrom, NicknameTo = this.NicknameTo };
        }

        public void Print()
        {
            Console.WriteLine(ToString());
        }

        public override string ToString()
        {
            return $"{DateTime} получено сообщение '{Text}' от {NicknameFrom}";
        }
    }
}
