using System;
using System.Collections.Generic;

namespace PRN221_FinalProject.Models
{
    public partial class Room
    {
        public Room()
        {
            Slots = new HashSet<Slot>();
        }

        public int RoomId { get; set; }
        public string? RoomName { get; set; }

        public virtual ICollection<Slot> Slots { get; set; }
    }
}
