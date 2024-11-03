using DockerBackup.App.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DockerBackup.Business.Interfaces.Services;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace DockerBackup.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IContainersService _containersService;

        public HomeController(ILogger<HomeController> logger, IContainersService containersService)
        {
            _logger = logger;
            _containersService = containersService;
        }

        public async Task<IActionResult> Index()
        {
            var allContainers = await _containersService.GetAllAsync();


            return View(allContainers);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
