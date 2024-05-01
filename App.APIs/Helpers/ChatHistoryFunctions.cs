namespace App.APIs.Helpers
{
    public static class ChatHistoryHelpers
    {
        public static string GetLastMessage(string history)
        {
            List<(string, string)> messages = ConvertToTuple(history);

            if (messages.Count > 0)
                return messages[messages.Count - 1].Item2;
            else
                return null;
        }

        public static string AddMessage(string message, string sender, string history)
        {
            List<(string, string)> messages = [];
            if (!string.IsNullOrEmpty(history))
                messages = ConvertToTuple(history);

            messages.Add(($"{sender}::{DateTime.UtcNow:dd-MM-yy,HH:mm}", message));
            var response = ConvertToJson(messages);
            return response;
        }

        public static string SendMessage(string history)
        {
            // SENDING THE HISTORY TO THE MODEL AND GET THE NEW ONE

            var response = AddMessage("this is model response", "model", history);
            return response;
        }

        public static string ConvertToJson(List<(string, string)> messages)
        {
            string josn = "{";
            foreach (var message in messages)
            {
                josn += "\"" + message.Item1 + "\":\"" + message.Item2 + "\",";
            }
            josn = josn.Remove(josn.Length - 1);
            josn += "}";
            return josn;
        }

        public static List<(string, string)> ConvertToTuple(string json)
        {
            List<(string, string)> messages = new List<(string, string)>();
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            foreach (var item in dict)
            {
                messages.Add((item.Key, item.Value));
            }
            return messages;
        }
    }
}
