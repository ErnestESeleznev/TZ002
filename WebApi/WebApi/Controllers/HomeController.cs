using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    /// <summary>
    /// Тестовый контроллер
    /// </summary>
    public class HomeController : Controller
    {

        /// <summary>
        /// Тестовый метод
        /// </summary>
        [HttpGet("Index")]

        public IActionResult Index()
        {
            // http://localhost:5245/api/home/index

            // ViewData["Message"] = "Hello!";
            // return View("Index");

            return Ok($"Success Index");
        }
    }
}
