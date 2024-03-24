using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PRN221_FinalProject.Logics;
using PRN221_FinalProject.Models;
using System.Formats.Asn1;
using System.Globalization;

namespace PRN221_FinalProject.Pages.Schedules
{
    public class ImportModel : PageModel
    {
        [BindProperty]
        public List<string> Errors { get; set; } = new List<string>();

        [BindProperty]
        public string ErrorMessage { get; set; }
        string Error;
        private PRN221FinalProjectContext _context;
        private ScheduleServices _scheduleServices;
        public ImportModel(PRN221FinalProjectContext context, ScheduleServices scheduleServices)
        {
            _context = context;
            _scheduleServices = scheduleServices;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost(IFormFile file, int weekNumber)
        {
            if (file != null && file.Length > 0)
            {
                //Check if there is any old schedule and remove them if there is
                List<Slot> slots = _scheduleServices.GetSlots(weekNumber);
                List<Slot> tempslots = slots;
                if (slots.Count > 0) { _context.Slots.RemoveRange(slots);
                    _context.SaveChanges();
                }
                // Read and process the CSV file
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" }))
                {
                    var records = csv.GetRecords<ImportRecord>(); // Define ImportRecord class to map CSV columns
                    foreach (var record in records)
                    {
                        try
                        {
                            string recordString = " Slot " + record.TimeSlot + " - " + record.Subject + " - " + record.Teacher + " - " + record.Class + " can not be added!";
                            // Check if the room exists
                            Room room = _context.Rooms.FirstOrDefault(r => r.RoomName == record.Room);
                            if (room == null)
                            {
                                Errors.Add("Room " + record.Room + " not found! " + recordString);
                                continue;
                            }
                            // Check if the teacher exists
                            Teacher teacher = _context.Teachers.FirstOrDefault(t => t.TeacherName == record.Teacher);
                            if (teacher == null)
                            {
                                Errors.Add("Teacher " + record.Teacher + " not found! " + recordString);
                                continue;
                            }
                            // Check if the class exists
                            Class classRecord = _context.Classes.FirstOrDefault(c => c.ClassName == record.Class);
                            if (classRecord == null)
                            {
                                Errors.Add("Class " + record.Class + " not found! " + recordString);
                                continue;
                            }

                            // Check if the subject exists
                            Subject subject = _context.Subjects.FirstOrDefault(s => s.SubjectName == record.Subject);
                            if (subject == null)
                            {
                                Errors.Add("Subject " + record.Subject + " not found! " + recordString);
                                continue;
                            }

                            // Check if the time slot exists

                            TimeSlot timeSlot = _context.TimeSlots.FirstOrDefault(ts =>
                             ts.TimeOfDay == record.TimeSlot[0].ToString()
                            && ts.FirstDay == int.Parse(record.TimeSlot[1].ToString())
                            && ts.SecondDay == int.Parse(record.TimeSlot[2].ToString())
                            );
                            if (timeSlot == null)
                            {
                                Errors.Add("Time Slot " + record.TimeSlot + "Is Not Valid! " + recordString);
                                continue;
                            }


                            // Create a new slot
                            var slot = new Slot
                            {
                                RoomId = room.RoomId,
                                TimeSlotId = timeSlot.TimeSlotId,
                                TeacherId = teacher.TeacherId,
                                ClassId = classRecord.ClassId,
                                SubjectId = subject.SubjectId,
                                WeekNumber = weekNumber
                            };
                            //Validation
                            Error = _scheduleServices.SlotValidation(slot);
                            if (!Error.Equals("ok"))
                            {
                                //Show the error to the user
                                Errors.Add(Error);
                                continue;
                            }
                            else
                            {
                                //Add the slot to the database
                                _context.Slots.Add(slot);
                                _context.SaveChanges();
                                // Redirect to the page corresponding to the imported week
                            }
                        }
                        catch (Exception ex)
                        {
                            Errors.Add($"Error: {ex.Message}");
                            continue;
                        }

                    }
                   
                    
                }
            }
            return RedirectToPage("/Schedules/Index", new { weekNumber, Errors });
        }

        

        private string DBErrorTemplate(Slot slot)
        {
            string error = " Slot " + slot.TimeSlot.Code + " - " + slot.Subject.SubjectName + " - " + slot.Teacher.TeacherName + " - " + slot.Class.ClassName + " can not be added!";
            return error;
        }

        

    }




}
