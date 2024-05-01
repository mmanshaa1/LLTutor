using App.Core.Interfaces;

namespace App.Core.Entities
{
    public class Course : IBaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ModelId { get; set; }
    }
}
