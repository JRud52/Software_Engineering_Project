using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Software_Engineering_Project.Controllers
{
    public class CalendarController : Controller
    {
        public PartialViewResult UpdateCalendar(System.DateTime currentCalendarDate, bool next)
        {

            if (next)
            {
                currentCalendarDate = currentCalendarDate.AddMonths(1);
            }
            else
            {
                if (currentCalendarDate.Month == 1)
                {
                    currentCalendarDate = currentCalendarDate.AddYears(-1);
                    currentCalendarDate = currentCalendarDate.AddMonths(11);                    
                }
                else
                {
                    currentCalendarDate = currentCalendarDate.AddMonths(-1);
                }
            }
            return PartialView("Calendar", new Models.Calendar() { date = currentCalendarDate });
        }


        public PartialViewResult GetCalendar(System.DateTime currentCalendarDate)
        {                            
            return PartialView("Calendar", new Models.Calendar() { date = currentCalendarDate });
        }

        public PartialViewResult GetDaySchedule(System.DateTime date)
        {

            return PartialView("DaySchedule", new Models.Calendar() { date = date });
        }

    }
}