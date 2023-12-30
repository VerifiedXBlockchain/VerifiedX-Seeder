using System.ComponentModel.DataAnnotations;

namespace RBXOSeed.Models
{
    public class AuditFields
    {
        protected AuditFields()
        {
            Active = true;
            CreateDateTimeUtc = DateTime.UtcNow;
            ModifiedDateTimeUtc = DateTime.UtcNow;
        }
        public int Id { get; set; }

        [Required]
        public DateTime CreateDateTimeUtc { get; set; }

        [Required]
        public DateTime ModifiedDateTimeUtc { get; set; }

        public bool Active { get; set; }
    }
}
