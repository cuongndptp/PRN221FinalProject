using System;
using System.Collections.Generic;

namespace PRN221_FinalProject.Models
{
    public partial class Teacher
    {
        public Teacher()
        {
            Slots = new HashSet<Slot>();
        }

        public int TeacherId { get; set; }
        public string? TeacherName { get; set; }

        public virtual ICollection<Slot> Slots { get; set; }
    }
}
