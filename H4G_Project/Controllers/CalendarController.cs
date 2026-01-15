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

        return Json(events.Select(e => new
        {
            id = e.Id,
            title = e.Name,
            start = e.Start.ToDateTime().ToString("yyyy-MM-ddTHH:mm:ss"),
            end = e.End.HasValue ? e.End.Value.ToDateTime().ToString("yyyy-MM-ddTHH:mm:ss") : null
        }));
    }

}