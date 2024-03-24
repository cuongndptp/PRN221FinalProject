using System;
using System.Collections.Generic;

namespace PRN221_FinalProject.Models
{
    public partial class Slot
    {
        public int SlotId { get; set; }
        public int? ClassId { get; set; }
        public int? SubjectId { get; set; }
        public int? TeacherId { get; set; }
        public int? RoomId { get; set; }
        public int? WeekNumber { get; set; }
        public int? TimeSlotId { get; set; }

        public virtual Class? Class { get; set; }
        public virtual Room? Room { get; set; }
        public virtual Subject? Subject { get; set; }
        public virtual Teacher? Teacher { get; set; }
        public virtual TimeSlot? TimeSlot { get; set; }
    }
}
