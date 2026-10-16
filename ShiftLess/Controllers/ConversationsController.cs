using Microsoft.AspNetCore.Mvc;

namespace ShiftLessAPI.Controllers
{
    public class ConversationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
