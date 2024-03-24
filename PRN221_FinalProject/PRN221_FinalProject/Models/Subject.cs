using System;
using System.Collections.Generic;

namespace PRN221_FinalProject.Models
{
    public partial class Subject
    {
        public Subject()
        {
            Slots = new HashSet<Slot>();
        }

        public int SubjectId { get; set; }
        public string? SubjectName { get; set; }

        public virtual ICollection<Slot> Slots { get; set; }
    }
}
