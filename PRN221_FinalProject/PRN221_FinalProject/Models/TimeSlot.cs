using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN221_FinalProject.Models
{
    public partial class TimeSlot
    {
        public TimeSlot()
        {
            Slots = new HashSet<Slot>();
            this.Code = "" + TimeOfDay + FirstDay + SecondDay;
        }

        public int TimeSlotId { get; set; }
        public string? TimeOfDay { get; set; }
        public int? FirstDay { get; set; }
        public int? SecondDay { get; set; }
        [NotMapped]
        public string Code { get; set; }

        public virtual ICollection<Slot> Slots { get; set; }

        
    }
}
