using App.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Entities
{
    public class Course : IBaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Chat> Chats { get; set; }
    }
}
