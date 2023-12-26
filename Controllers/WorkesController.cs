using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Munkanaplo2.Data;
using Munkanaplo2.Models;
using Microsoft.AspNetCore.Identity;
using Munkanaplo2.Global;
using dotenv.net;

namespace Munkanaplo2.Controllers
{
    public class WorkersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (!IsConfigCorrect()) return View("ConfigError");

            if (!ManagerHelper.IsManager(User) && DotEnv.Read()["USE_MANAGERS"].ToLower() == "true") return View("AccesDenied");
            if (DotEnv.Read()["USE_MANAGERS"].ToLower() == "false" && User.Identity.Name.ToLower() != DotEnv.Read()["ADMIN_USERNAME"].ToLower()) return View("AccesDenied");

            if (ManagerHelper.IsManager(User) && DotEnv.Read()["USE_MANAGERS"].ToLower() == "true")
            {
                List<ProjectMembership> myProjectMemberships = await _context.ProjectMemberships.Where(pm => pm.Member == User.Identity.Name.ToString()).ToListAsync();
                List<ProjectModel> myProjects = new List<ProjectModel>();

                foreach (ProjectMembership membership in myProjectMemberships)
                {
                    if (!myProjects.Contains(_context.ProjectModel.Find(membership.ProjectId))) myProjects.Add(_context.ProjectModel.Find(membership.ProjectId));
                }

                List<string> othersInSameProject = new List<string>();
                foreach (ProjectModel project in myProjects)
                {
                    foreach (ProjectMembership membership in await _context.ProjectMemberships.Where(pm => pm.ProjectId == project.Id).ToListAsync())
                    {
                        if (!ManagerHelper.IsManager(membership.Member) && !othersInSameProject.Contains(membership.Member)) othersInSameProject.Add(membership.Member);
                    }
                }
                List<WorkerModel> workers = new List<WorkerModel>();
                workers.Add(_context.Workers.Where(w => w.User == User.Identity.Name).ToList()[0]);
                foreach (string worker in othersInSameProject)
                {
                    workers.Add(_context.Workers.Where(wm => wm.User == worker).ToList()[0]);
                }
                return View(workers);
            }
            else if (DotEnv.Read()["USE_MANAGERS"].ToLower() == "false" && DotEnv.Read()["ADMIN_USERNAME"] == User.Identity.Name)
            {
                List<WorkerModel> workers = _context.Workers.ToList();
                return View("Index", workers);
            }
            return View("AccesDenied");
        }

        [Authorize]
        [HttpPost()]
        public async Task<IActionResult> EditMoneyPerHour([Bind("WorkerId")] int WorkerId, [Bind("MoneyPerHour")] int MoneyPerHour)
        {
            if (!IsConfigCorrect()) return View("ConfigError");

            if (!ManagerHelper.IsManager(User) && DotEnv.Read()["USE_MANAGERS"].ToLower() == "true") return View("AccesDenied");
            if (DotEnv.Read()["USE_MANAGERS"].ToLower() == "false" && User.Identity.Name.ToLower() != DotEnv.Read()["ADMIN_USERNAME"].ToLower()) return View("AccesDenied");

            if (ManagerHelper.IsManager(User) && DotEnv.Read()["USE_MANAGERS"].ToLower() == "true")
            {
                List<ProjectMembership> myProjectMemberships = await _context.ProjectMemberships.Where(pm => pm.Member == User.Identity.Name.ToString()).ToListAsync();
                List<ProjectModel> myProjects = new List<ProjectModel>();

                foreach (ProjectMembership membership in myProjectMemberships)
                {
                    if (!myProjects.Contains(_context.ProjectModel.Find(membership.ProjectId))) myProjects.Add(_context.ProjectModel.Find(membership.ProjectId));
                }

                List<string> othersInSameProjects = new List<string>();
                foreach (ProjectModel project in myProjects)
                {
                    foreach (ProjectMembership membership in await _context.ProjectMemberships.Where(pm => pm.ProjectId == project.Id).ToListAsync())
                    {
                        if (!ManagerHelper.IsManager(membership.Member) && !othersInSameProjects.Contains(membership.Member)) othersInSameProjects.Add(membership.Member);
                    }
                }
                List<WorkerModel> workers = new List<WorkerModel>();
                workers.Add(_context.Workers.Where(w => w.User == User.Identity.Name).ToList()[0]);
                foreach (string worker in othersInSameProjects)
                {
                    workers.Add(_context.Workers.Where(wm => wm.User == worker).ToList()[0]);
                }


                if (_context.Workers.Find(WorkerId) == null) return NotFound();
                if (!workers.Contains(_context.Workers.Find(WorkerId))) return View("AccesDenied");

            }

            WorkerModel Worker = _context.Workers.Find(WorkerId);
            WorkerModel workerToUpdate = new WorkerModel
            {
                Id = WorkerId,
                User = Worker.User,
                MoneyPerHour = MoneyPerHour
            };

            _context.Remove(Worker);
            _context.Add(workerToUpdate);
            await _context.SaveChangesAsync();

            return LocalRedirect("/beosztottak");
        }

        bool IsConfigCorrect()
        {
            if (DotEnv.Read()["ADMIN_USERNAME"].ToLower() == null) return false;
            if (DotEnv.Read()["USE_MANAGERS"].ToLower() == null) return false;
            if (DotEnv.Read()["USE_MANAGERS"].ToLower().Trim() == "") return false;

            if (DotEnv.Read()["USE_MANAGERS"].ToLower() == "false" && DotEnv.Read()["ADMIN_USERNAME"].ToLower().Trim() == "") return false;
            if (DotEnv.Read()["USE_MANAGERS"].ToLower() != "true" && DotEnv.Read()["USE_MANAGERS"].ToLower() != "false") return false;

            return true;
        }
    }
}