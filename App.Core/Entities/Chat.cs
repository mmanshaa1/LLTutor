using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Entities
{
    public class Chat
    {
        public int Id { get; set; }
        public string? Subject { get; set; }
        public string? ChatContent { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
