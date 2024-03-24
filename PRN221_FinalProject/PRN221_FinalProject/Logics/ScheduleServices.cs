using Microsoft.EntityFrameworkCore;
using PRN221_FinalProject.Models;
using static System.Reflection.Metadata.BlobBuilder;

namespace PRN221_FinalProject.Logics
{
    public class ScheduleServices
    {

        private PRN221FinalProjectContext _context;
        public ScheduleServices(PRN221FinalProjectContext context)
        {
            _context = context;
        }
        public List<Slot> GetSlots(int WeekNumber)
        {
            List<Slot> slots = _context.Slots
                    .Include(x => x.TimeSlot)
                    .Include(x => x.Room)
                    .Include(x => x.Teacher)
                    .Include(x => x.Subject)
                    .Include(x => x.Class)
                    .OrderBy(x => x.TimeSlot.TimeOfDay)
                    .Where(x => x.WeekNumber == WeekNumber)
                    .ToList();
            foreach (var slot in slots)
            {
                slot.TimeSlot.Code = GenerateCode(slot.TimeSlot);
            }
            
            return slots;
        }

        public string GenerateCode(TimeSlot timeSlot)
        {
            // Check if TimeOfDay, FirstDay, and SecondDay have values
            if (!string.IsNullOrEmpty(timeSlot.TimeOfDay) && timeSlot.FirstDay.HasValue && timeSlot.SecondDay.HasValue)
            {
                return "" + timeSlot.TimeOfDay + timeSlot.FirstDay + timeSlot.SecondDay;
            }
            else
            {
                return "";
            }
        }

        public Slot LoadFull(Slot slot)
        {
            _context.Attach(slot);

            // Load related entities
            _context.Entry(slot)
                    .Reference(s => s.Room)
                    .Load();

            _context.Entry(slot)
                    .Reference(s => s.Teacher)
                    .Load();

            _context.Entry(slot)
                    .Reference(s => s.Class)
                    .Load();
            _context.Entry(slot)
                    .Reference(s => s.Subject)
                    .Load();
            _context.Entry(slot)
                    .Reference(s => s.TimeSlot)
                    .Load();

            slot.TimeSlot.Code = GenerateCode(slot.TimeSlot);
            return slot;
        }

        public string SlotValidation(Slot slot)
        {
            slot = LoadFull(slot);
            string slotString = " Slot " + slot.TimeSlot.Code + " - " + slot.Subject.SubjectName + " - " + slot.Teacher.TeacherName + " - " + slot.Class.ClassName + " can not be added!";
            // Check if the room is already occupied

            bool isRoomOccupied = _context.Slots.Any(s => s.WeekNumber == slot.WeekNumber &&
                                                     s.TimeSlotId == slot.TimeSlotId &&
                                                     s.RoomId == slot.RoomId);
            if (isRoomOccupied)
            {
                return "Room " + slot.Room.RoomName + " is already occupied."
                + slotString;
            }

            // Check if the teacher is already teaching
            bool isTeacherTeaching = _context.Slots.Any(s => s.WeekNumber == slot.WeekNumber &&
                                                             s.TeacherId == slot.TeacherId &&
                                                             s.TimeSlotId == slot.TimeSlotId);
            if (isTeacherTeaching)
            {
                return "Teacher " + slot.Teacher.TeacherName + " is already teaching."
                    + slotString;
            }

            // Check if the class is already attending
            bool isClassAttending = _context.Slots.Any(s => s.WeekNumber == slot.WeekNumber &&
                                                           s.ClassId == slot.ClassId &&
                                                           s.TimeSlotId == slot.TimeSlotId);
            if (isClassAttending)
            {
                return "Class " + slot.Class.ClassName + " is already attending."
                    + slotString;

            }

            // Check for overlapping time slots
            //bool isOverlappingTimeSlot = _context.Slots.Any(s => s.WeekNumber == slot.WeekNumber &&
            //                                                   s.RoomId == slot.RoomId &&
            //                                                   s.TimeSlot.FirstDay == slot.TimeSlot.FirstDay &&
            //                                                   s.TimeSlot.SecondDay == slot.TimeSlot.SecondDay);
            //if (isOverlappingTimeSlot)
            //{
            //    return "Overlapping time slots.";
            //}

            // No validation errors, return empty string
            return "ok";
        }
    }
}
