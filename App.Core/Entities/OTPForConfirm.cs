using App.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace App.Core.Entities
{
    public class OTPForConfirm : IBaseEntity
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        public Account Account { get; set; }
        [DataType(DataType.Time)]
        public TimeSpan LifeTime { get; set; }
        public string OTPHashed { get; set; }
    }
}
