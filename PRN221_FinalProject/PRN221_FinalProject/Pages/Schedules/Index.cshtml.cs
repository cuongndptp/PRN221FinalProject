using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN221_FinalProject.Logics;
using PRN221_FinalProject.Models;

namespace PRN221_FinalProject.Pages.Schedules
{
    public class IndexModel : PageModel
    {
        private PRN221FinalProjectContext _context;
        private ScheduleServices _scheduleServices;
        public List<Slot> Slots { get; set; }
        public List<Room> Rooms { get; set; }
        public List<int> Days { get; set; }
        [BindProperty]
        public List<string> Errors { get; set; }
        public int WeekNumber { get; set; }

        public IndexModel(PRN221FinalProjectContext context, ScheduleServices scheduleServices)
        {
            _context = context;
            _scheduleServices = scheduleServices;
        }
        public void OnGet(int? WeekNumber, List<string> errors)
        {
            this.WeekNumber = WeekNumber ?? 10;
            if (WeekNumber != null)
            {
                Slots = _scheduleServices.GetSlots(WeekNumber.Value);
            }
            else
            {
                Slots = _scheduleServices.GetSlots(10);
            }
            Errors = errors;
            Rooms = _context.Rooms.ToList();
            Days = new List<int> { 2, 3, 4, 5, 6 };
        }
    }
}
