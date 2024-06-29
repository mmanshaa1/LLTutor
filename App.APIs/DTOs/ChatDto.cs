namespace App.APIs.DTOs
{
    public class OpenChatDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
    }

    public class ChatMessageDto
    {
        public int HistoryId { get; set; }
        public string message { get; set; }
    }
}
