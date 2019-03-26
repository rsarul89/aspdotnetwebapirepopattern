using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    [Table("Standards", Schema = "WebApi")]
    public class Standard : BaseEntity
    {
        public Guid StandardId { get; set; }
        public string StandardName { get; set; }
        public string Description { get; set; }

        public ICollection<Student> Students { get; set; }
    }
}
