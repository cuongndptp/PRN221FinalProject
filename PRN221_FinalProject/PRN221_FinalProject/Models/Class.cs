using System;
using System.Collections.Generic;

namespace PRN221_FinalProject.Models
{
    public partial class Class
    {
        public Class()
        {
            Slots = new HashSet<Slot>();
        }

        public int ClassId { get; set; }
        public string? ClassName { get; set; }

        public virtual ICollection<Slot> Slots { get; set; }
    }
}
