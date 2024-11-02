using DockerBackup.App.Models;
using DockerBackup.Business.Interfaces.Services;
using DockerBackup.Model.Models;
using Microsoft.AspNetCore.Mvc;

namespace DockerBackup.App.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly ILogger<ScheduleController> _logger;
        private readonly IContainersService _containersService;
        private readonly ISchedulesService _schedulesService;

        public ScheduleController(ILogger<ScheduleController> logger, IContainersService containersService, ISchedulesService schedulesService)
        {
            _logger = logger;
            _containersService = containersService;
            _schedulesService = schedulesService;
        }

        [HttpGet]
        public async Task<IActionResult> Add(string containerName)
        {
            var volumes = await _containersService.GetAllVolumesAsync(containerName);

            return View(new SaveScheduleVM(){ ContainerVolumes = volumes });
        }

        [HttpPost]
        public async Task<IActionResult> Add(SaveScheduleVM vm)
        {
            if (!vm.SelectedVolumes.Any())
            {
                _logger.LogInformation("No volumes to backup were selected");

                return RedirectToAction("Index", "Home");
            }

            await _schedulesService.AddAsync(vm.ContainerName, vm.Frequency, vm.Hour, vm.Minute, vm.SelectedVolumes);

            return RedirectToAction("Index", "Home");
        }
    }
}
