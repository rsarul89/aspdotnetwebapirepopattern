using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    [Table("Students", Schema = "WebApi")]
    public class Student : BaseEntity
    {
        public Guid StudentID { get; set; }
        public string StudentName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Guid StandardRefId { get; set; }
        [ForeignKey("StandardRefId")]
        public Standard Standard { get; set; }
    }
}
