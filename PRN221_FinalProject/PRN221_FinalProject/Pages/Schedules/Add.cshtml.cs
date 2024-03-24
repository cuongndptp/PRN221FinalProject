using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN221_FinalProject.Logics;
using PRN221_FinalProject.Models;

namespace PRN221_FinalProject.Pages.Schedules
{
    public class AddModel : PageModel
    {
        private PRN221FinalProjectContext _context;
        private ScheduleServices _scheduleServices;
        public List<Class> classes;
        public List<Subject> subjects;
        public List<Teacher> teachers;
        public List<Room> rooms;

        public List<string> Errors = new List<string>();

        public AddModel(PRN221FinalProjectContext context, ScheduleServices scheduleServices)
        {
            _context = context;
            _scheduleServices = scheduleServices;

            classes = _context.Classes.ToList();
            subjects = _context.Subjects.ToList();
            teachers = _context.Teachers.ToList();
            rooms = _context.Rooms.ToList();
        }

        public void OnGet(List<string> errors)
        {
            Errors = errors;
        }

        public IActionResult OnPostAdd(Slot addingSlot, string timeSlot)
        {
            //Check WeekNumber input
            if (addingSlot.WeekNumber == null)
            {
                Errors.Add("Week Number must not be empty!");
                return RedirectToPage("/Schedules/Add", new { Errors });
            }
            // Check timeSlot input
            if (string.IsNullOrEmpty(timeSlot)) {
                Errors.Add("Time Slot must not be empty!");
                return RedirectToPage("/Schedules/Add", new { Errors });
            }
            TimeSlot timeSlotTemp = _context.TimeSlots.FirstOrDefault(ts =>
                 ts.TimeOfDay == timeSlot[0].ToString()
                && ts.FirstDay == int.Parse(timeSlot[1].ToString())
                && ts.SecondDay == int.Parse(timeSlot[2].ToString())
            );

            if (timeSlotTemp == null)
            {
                Errors.Add("Time Slot " + timeSlot + " is not valid! ");
                return RedirectToPage("/Schedules/Add", new { Errors });
            }

            // Assign timeslot if valid
            addingSlot.TimeSlot = timeSlotTemp;
            _context.Slots.Add(addingSlot);
            addingSlot = LoadFull(addingSlot);

            // Logic validation
            string error = SlotValidation(addingSlot);
            if (!error.Equals("ok"))
            {
                Errors.Add(error);
            }
            else
            {

                _context.SaveChanges();
                int weekNumber = addingSlot.WeekNumber.Value;
                return RedirectToPage("/Schedules/Index", new { weekNumber, Errors });
            }
            return RedirectToPage("/Schedules/Add", new { Errors });

        }

        private Slot LoadFull(Slot slot)
        {
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

            slot.TimeSlot.Code = _scheduleServices.GenerateCode(slot.TimeSlot);
            return slot;
        }

        private string SlotValidation(Slot slot)
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
