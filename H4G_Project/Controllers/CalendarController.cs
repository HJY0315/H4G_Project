using Microsoft.AspNetCore.Mvc;
using H4G_Project.DAL;
using System.Linq;
using System.Threading.Tasks;

public class CalendarController : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetEvents()
    {
        EventsDAL dal = new EventsDAL();
        var events = await dal.GetAllEvents();

        // FullCalendar requires: id, title, start, end
        return Json(events.Select(e => new
        {
            id = e.Id,
            title = e.Name,   // 🔁 map Firestore "name" → FullCalendar "title"
            start = e.Start,
            end = e.End
        }));
    }
}
