using App.Core.Interfaces;

namespace App.Core.Entities
{
    public class ChatHistory : IBaseEntity
    {
        public int Id { get; set; }
        public string? Subject { get; set; }
        public string? History { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public string AccountId { get; set; }
        public Account Account { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdateTime { get; set; } = DateTime.UtcNow;
    }
}