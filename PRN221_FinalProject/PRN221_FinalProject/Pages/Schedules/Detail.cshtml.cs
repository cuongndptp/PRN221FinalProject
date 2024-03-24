using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN221_FinalProject.Logics;
using PRN221_FinalProject.Models;

namespace PRN221_FinalProject.Pages.Schedules
{
    public class DetailModel : PageModel
    {
        private PRN221FinalProjectContext _context;
        private ScheduleServices _scheduleServices;
        public List<Class> classes;
        public List<Subject> subjects;
        public List<Teacher> teachers;
        public List<Room> rooms;

        public List<string> Errors = new List<string>();
        public Slot slot { get; set; }
        public DetailModel(PRN221FinalProjectContext context, ScheduleServices scheduleServices)
        {
            _context = context;
            _scheduleServices = scheduleServices;

            classes = _context.Classes.ToList();
            subjects = _context.Subjects.ToList();
            teachers = _context.Teachers.ToList();
            rooms = _context.Rooms.ToList();
        }
        public void OnGet(int SlotId, List<string> errors)
        {
            slot = _context.Slots.FirstOrDefault(s => s.SlotId == SlotId);
            slot = _scheduleServices.LoadFull(slot);
            Errors = errors;
        }

        public IActionResult OnPostUpdate(Slot updatingSlot, string timeSlot)
        {
           
            // Check timeSlot input
            TimeSlot timeSlotTemp = _context.TimeSlots.FirstOrDefault(ts =>
                 ts.TimeOfDay == timeSlot[0].ToString()
                && ts.FirstDay == int.Parse(timeSlot[1].ToString())
                && ts.SecondDay == int.Parse(timeSlot[2].ToString())
            );

            if (timeSlotTemp == null)
            {
                Errors.Add("Time Slot " + timeSlot + " is not valid! ");
                return RedirectToPage("/Schedules/Detail/" + updatingSlot.SlotId);
            }
            // Assign timeslot if valid
            updatingSlot.TimeSlot = timeSlotTemp;
            updatingSlot = _scheduleServices.LoadFull(updatingSlot);
            // Logic validation
            //string error = _scheduleServices.SlotValidation(updatingSlot);
            string error = UpdateValidation(updatingSlot);
            if (!error.Equals("ok"))
            {
                Errors.Add(error);
            }
            else
            {
                // If validation passes, save changes to database
                _context.Slots.Update(updatingSlot); 
                _context.SaveChanges();

                int weekNumber = updatingSlot.WeekNumber.Value;
                return RedirectToPage("/Schedules/Index", new { weekNumber, Errors });
            }
            return RedirectToPage("/Schedules/Detail", new { updatingSlot.SlotId, Errors });
        }

        private string UpdateValidation(Slot updatingSlot)
        {
            string slotString = " Slot " + updatingSlot.TimeSlot.Code + " - " + updatingSlot.Subject.SubjectName + " - " + updatingSlot.Teacher.TeacherName + " - " + updatingSlot.Class.ClassName + " cannot be added!";

            // Check if the room is already occupied by another class
            bool isRoomOccupied = _context.Slots.Any(s => s.SlotId != updatingSlot.SlotId && // Exclude the current slot being updated
                                                        s.WeekNumber == updatingSlot.WeekNumber &&
                                                        s.TimeSlotId == updatingSlot.TimeSlotId &&
                                                        s.RoomId == updatingSlot.RoomId);
            if (isRoomOccupied)
            {
                return "Room " + updatingSlot.Room.RoomName + " is already occupied." + slotString;
            }

            // Check if the teacher is already teaching another class
            bool isTeacherTeaching = _context.Slots.Any(s => s.SlotId != updatingSlot.SlotId && // Exclude the current slot being updated
                                                        s.WeekNumber == updatingSlot.WeekNumber &&
                                                        s.TeacherId == updatingSlot.TeacherId &&
                                                        s.TimeSlotId == updatingSlot.TimeSlotId);
            if (isTeacherTeaching)
            {
                return "Teacher " + updatingSlot.Teacher.TeacherName + " is already teaching." + slotString;
            }

            // Check if the class is already attending another slot
            bool isClassAttending = _context.Slots.Any(s => s.SlotId != updatingSlot.SlotId && // Exclude the current slot being updated
                                                        s.WeekNumber == updatingSlot.WeekNumber &&
                                                        s.ClassId == updatingSlot.ClassId &&
                                                        s.TimeSlotId == updatingSlot.TimeSlotId);
            if (isClassAttending)
            {
                return "Class " + updatingSlot.Class.ClassName + " is already attending." + slotString;
            }

            // No validation errors, return empty string
            return "ok";
        }

        public IActionResult OnPostDelete(int slotId)
        {
            var slotToDelete = _context.Slots.FirstOrDefault(s => s.SlotId == slotId);
            if (slotToDelete == null)
            {
                return NotFound();
            }

            _context.Slots.Remove(slotToDelete);
            _context.SaveChanges();

            int weekNumber = slotToDelete.WeekNumber.Value;
            return RedirectToPage("/Schedules/Index", new { weekNumber });
        }
    }
}
