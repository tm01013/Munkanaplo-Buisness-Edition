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
using Munkanaplo2.Global;
using dotenv.net;
using Munkanaplo2.Services;

namespace Munkanaplo2.Controllers
{
	public class JobsController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IWorkService workService;

		public JobsController(ApplicationDbContext context, IWorkService workService)
		{
			_context = context;
			this.workService = workService;
		}

		#region JobStuff
		// GET: Jobs
		[Authorize]
		public async Task<IActionResult> Index([Bind("Id")] int Id)
		{
			if (!IsConfigCorrect()) return View("ConfigError");

			CleanupWork();
			//if (ManagerHelper.IsManager(User)) return RedirectToAction("TeacherView");
			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == Id)
									.ToList();

			if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
			{
				return View("AccesDenied");
			}

			ViewBag.ProjectId = Id;
			ViewBag.UnFinishedJobs = await _context.JobModel.Where(jm => jm.ProjectId == Id && jm.JobStatus == "folyamatban").ToListAsync();
			ViewBag.FinishedJobs = await _context.JobModel.Where(jm => jm.ProjectId == Id && jm.JobStatus != "folyamatban").ToListAsync();
			return View("Index");
		}

		// GET: Jobs/Details/5
		[Authorize]
		public async Task<IActionResult> Details(int id)
		{
			if (!IsConfigCorrect()) return View("ConfigError");

			if (id == null || _context.JobModel == null)
			{
				return NotFound();
			}

			var jobModel = await _context.JobModel
				.FirstOrDefaultAsync(m => m.Id == id);
			var subTasks = await _context.SubTaskModel
				.Where(m => m.JobId == id).ToListAsync();

			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == jobModel.ProjectId)
									.ToList();

			if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
			{
				return View("AccesDenied");
			}

			int totalMinutesWorked = 0;
			foreach (WorkModel work in _context.WorkModel.Where(w => w.JobId == id && w.isFinished == true))
			{
				DateTime endTime = DateTime.Parse(work.EndTiem);
				DateTime startTime = DateTime.Parse(work.StartTime);
				totalMinutesWorked += (int)endTime.Subtract(startTime).TotalMinutes;
			}

			ViewBag.ProjectId = jobModel.ProjectId;
			ViewBag.SubTasks = subTasks;
			ViewBag.TotalMinutesWorkedOn = totalMinutesWorked;

			if (jobModel == null)
			{
				return NotFound();
			}

			return View(jobModel);
		}


		// GET: Jobs/Create
		[Authorize]
		public async Task<IActionResult> Create([Bind("Id")] int Id)
		{
			if (!IsConfigCorrect()) return View("ConfigError");

			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == Id)
									.ToList();

			if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
			{
				return View("AccesDenied");
			}

			List<string> users = new List<string>();
			var usersInProject = await _context.ProjectMemberships.Where(pm => pm.ProjectId == Id).ToListAsync();
			foreach (ProjectMembership pm in usersInProject)
			{
				if (!ManagerHelper.IsManager(pm.Member)) users.Add(pm.Member);
			}
			ViewBag.Users = new SelectList(users);
			ViewBag.ProjectId = Id;
			return View();
		}

		// POST: Jobs/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost()]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateConfirmed([Bind("Id,JobTitle,JobDescription,JobOwner,JobCreator,CreationDate,JobStatus,FinishDate,ProjectId")] JobModel jobModel)
		{
			if (!IsConfigCorrect()) return View("ConfigError");

			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == jobModel.ProjectId)
									.ToList();

			if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
			{
				return View("AccesDenied");
			}

			if (jobModel.JobDescription == string.Empty) jobModel.JobDescription = "";
			if (jobModel.JobTitle != string.Empty && jobModel.JobOwner != string.Empty && jobModel.JobCreator != string.Empty && jobModel.CreationDate != string.Empty && jobModel.FinishDate != string.Empty && jobModel.JobStatus != string.Empty && jobModel.ProjectId != null)
			{
				_context.Add(jobModel);
				await _context.SaveChangesAsync();

				ViewBag.ProjectId = jobModel.ProjectId;
				return LocalRedirect("/" + jobModel.ProjectId + "/feladatok");
			}
			return View("Create");
		}

		[Authorize]
		// GET: Jobs/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (!IsConfigCorrect()) return View("ConfigError");

			if (id == null || _context.JobModel == null)
			{
				return NotFound();
			}

			var jobModel = await _context.JobModel.FindAsync(id);
			List<SubTaskModel> subTasks = await _context.SubTaskModel
					.Where(m => m.JobId == jobModel.Id).ToListAsync();

			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == jobModel.ProjectId)
									.ToList();
			if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
			{
				return View("AccesDenied");
			}

			if (jobModel == null)
			{
				return NotFound();
			}

			List<string> users = new List<string>();
			var usersInProject = await _context.ProjectMemberships.Where(pm => pm.ProjectId == jobModel.ProjectId).ToListAsync();
			foreach (ProjectMembership pm in usersInProject)
			{
				users.Add(pm.Member);
			}
			ViewBag.Users = new SelectList(users);

			ViewBag.ProjectId = jobModel.ProjectId;
			ViewBag.SubTasks = subTasks;
			return View(jobModel);
		}

		// POST: Jobs/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditConfirmed(int id, [Bind("Id,JobTitle,JobDescription,JobOwner,JobCreator,CreationDate,JobStatus,FinishDate,ProjectId")] JobModel jobModel)
		{
			if (!IsConfigCorrect()) return View("ConfigError");

			if (id != jobModel.Id)
			{
				return NotFound();
			}

			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == jobModel.ProjectId)
									.ToList();

			if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
			{
				return View("AccesDenied");
			}

			if (jobModel.JobDescription == string.Empty) jobModel.JobDescription = "";
			if (jobModel.Id != null && jobModel.JobTitle != string.Empty && jobModel.JobOwner != string.Empty && jobModel.JobCreator != string.Empty && jobModel.CreationDate != string.Empty && jobModel.FinishDate != string.Empty && jobModel.JobStatus != string.Empty && jobModel.ProjectId != null)
			{
				try
				{
					_context.Update(jobModel);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!JobModelExists(jobModel.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return LocalRedirect("/" + jobModel.ProjectId + "/feladatok");
			}
			return View("Hiba");
		}

		[Authorize]
		// GET: Jobs/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (!IsConfigCorrect()) return View("ConfigError");

			if (id == null || _context.JobModel == null)
			{
				return NotFound();
			}

			var jobModel = await _context.JobModel
				.FirstOrDefaultAsync(m => m.Id == id);
			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == jobModel.ProjectId)
									.ToList();

			if ((jobModel.JobOwner != User.Identity.Name.ToString()) && (jobModel.JobCreator != User.Identity.Name.ToString()))
			{
				return View("AccesDenied");
			}


			if (jobModel == null)
			{
				return NotFound();
			}

			return View(jobModel);
		}

		// POST: Jobs/Delete/5
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed([Bind("Id")] int id)
		{
			if (!IsConfigCorrect()) return View("ConfigError");

			if (_context.JobModel == null)
			{
				return Problem("Entity set 'ApplicationDbContext.JobModel'  is null.");
			}
			List<SubTaskModel> subTasks = await _context.SubTaskModel.Where(stm => stm.JobId == id).ToListAsync();
			var jobModel = await _context.JobModel.FindAsync(id);

			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == jobModel.ProjectId)
									.ToList();

			if ((jobModel.JobOwner != User.Identity.Name.ToString()) && (jobModel.JobCreator != User.Identity.Name.ToString()))
			{
				return View("AccesDenied");
			}

			foreach (SubTaskModel subTask in subTasks)
			{
				_context.SubTaskModel.Remove(subTask);
			}

			if (jobModel != null)
			{
				_context.JobModel.Remove(jobModel);
			}

			await _context.SaveChangesAsync();

			return LocalRedirect("/" + jobModel.ProjectId + "/feladatok");
		}

		#endregion

		#region SubTasks

		[HttpPost()]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateSubTask([Bind("Id,TaskTitle,TaskDetails,JobId,TaskCreator,TaskCreationDate")] SubTaskModel subTaskModel)
		{
			if (!IsConfigCorrect()) return View("ConfigError");

			var jobModel = await _context.JobModel.FindAsync(subTaskModel.JobId);
			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == jobModel.ProjectId)
									.ToList();

			if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
			{
				return View("AccesDenied");
			}

			if (ModelState.IsValid)
			{
				_context.Add(subTaskModel);
				await _context.SaveChangesAsync();

				return LocalRedirect("/feladat/" + subTaskModel.JobId);
			}
			return View("Hiba");
		}

		[HttpPost()]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteSubTask([Bind("Id")] int id)
		{
			if (!IsConfigCorrect()) return View("ConfigError");

			var subTask = await _context.SubTaskModel.FindAsync(id);
			if (subTask == null) return NotFound();

			var jobModel = await _context.JobModel.FindAsync(subTask.JobId);
			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == jobModel.ProjectId)
									.ToList();

			if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
			{
				return View("AccesDenied");
			}

			_context.Remove(subTask);
			await _context.SaveChangesAsync();


			return LocalRedirect("/feladat/" + subTask.JobId);
		}

		#endregion

		#region WorkStuff

		public async void CleanupWork()
		{
			List<WorkModel> oldWorks = _context.WorkModel.AsEnumerable().Where(wm => DateTime.Now.Month == int.Parse(wm.StartTime.Split('/', ' ', ':')[0])).ToList();
			foreach (WorkModel work in oldWorks)
			{
				if (int.Parse(work.StartTime.Split('/', ' ', ':')[2]) < DateTime.Now.Year)
				{
					_context.Remove(work);
				}

			}
			await _context.SaveChangesAsync();
		}

		[HttpPost()]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> StartWork([Bind("JobId")] int JobId)
		{
			if (!IsConfigCorrect()) return View("ConfigError");

			if (_context.JobModel.Find(JobId) == null) return NotFound();
			int projectId = _context.JobModel.Find(JobId).ProjectId;
			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == projectId)
									.ToList();

			if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
			{
				return View("AccesDenied");
			}

			if (_context.WorkModel.Where(wm => wm.JobId == JobId && wm.User == User.Identity.Name.ToString() && wm.isFinished == false).ToList().Any())
			{
				ViewBag.JobTitle = _context.JobModel.Find(_context.WorkModel.Where(wm => wm.JobId == JobId && wm.User == User.Identity.Name.ToString() && wm.isFinished == false).ToList()[0].JobId).JobTitle;
				return View("Work", _context.WorkModel.Where(wm => wm.JobId == JobId && wm.User == User.Identity.Name.ToString() && wm.isFinished == false).ToList()[0]);
			}
			else
			{
				WorkModel work = new WorkModel
				{
					JobId = JobId,
					User = User.Identity.Name.ToString(),
					StartTime = DateTime.Now.ToString(),
					isFinished = false,
					EndTiem = "0"
				};

				ViewBag.JobTitle = _context.JobModel.Find(work.JobId).JobTitle;

				_context.WorkModel.Add(work);
				await _context.SaveChangesAsync();

				return View("Work", work);
			}
		}

		[Authorize]
		public async Task<IActionResult> OpenWork(int id)
		{
			if (!IsConfigCorrect()) return View("ConfigError");

			WorkModel work = _context.WorkModel.Find(id);

			if (work == null) return NotFound();
			if (work.User != User.Identity.Name.ToString()) return View("AccesDenied");

			ViewBag.JobTitle = _context.JobModel.Find(work.JobId).JobTitle;

			return View("Work", work);

		}

		[HttpPost()]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EndWork([Bind("WorkId")] int WorkId)
		{
			if (!IsConfigCorrect()) return View("ConfigError");

			WorkModel work = await _context.WorkModel.FindAsync(WorkId);
			if (work == null) return NotFound();

			if (_context.JobModel.Find(work.JobId) == null) return NotFound();

			int projectId = _context.JobModel.Find(_context.WorkModel.Find(WorkId).JobId).ProjectId;

			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == projectId)
									.ToList();
			if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
			{
				return View("AccesDenied");
			}


			work.isFinished = true;
			work.EndTiem = DateTime.Now.ToString();

			_context.WorkModel.Update(work);
			await _context.SaveChangesAsync();

			return LocalRedirect("/feladat/" + work.JobId);
		}

		[Authorize]
		public async Task<IActionResult> OpenWorkTable(string UserName)
		{
			if (!IsConfigCorrect()) return View("ConfigError");

			if (DotEnv.Read()["USE_MANAGERS"].ToLower() == "true")
			{
				List<WorkModel> works = await _context.WorkModel.Where(wm => wm.User == UserName && wm.isFinished == true).ToListAsync();

				if (!ManagerHelper.IsManager(User) && User.Identity.Name.ToString() != UserName) return View("AccesDenied");

				//Check if manager have right to view user work table----------------
				if (ManagerHelper.IsManager(User) && User.Identity.Name != UserName)
				{
					List<ProjectMembership> projectMemberships = await _context.ProjectMemberships.Where(pm => pm.Member == User.Identity.Name.ToString()).ToListAsync();
					List<ProjectModel> Projects = new List<ProjectModel>();

					foreach (ProjectMembership membership in projectMemberships)
					{
						if (!Projects.Contains(_context.ProjectModel.Find(membership.ProjectId))) Projects.Add(_context.ProjectModel.Find(membership.ProjectId));
					}

					List<string> others = new List<string>();
					foreach (ProjectModel project in Projects)
					{
						foreach (ProjectMembership membership in await _context.ProjectMemberships.Where(pm => pm.ProjectId == project.Id).ToListAsync())
						{
							if (!ManagerHelper.IsManager(membership.Member) && !others.Contains(membership.Member)) others.Add(membership.Member);
						}
					}
					List<WorkerModel> workers = new List<WorkerModel>();
					foreach (string worker in others)
					{
						workers.Add(_context.Workers.Where(wm => wm.User == worker).ToList()[0]);
					}
					if (!workers.Where(w => w.User == UserName).Any()) return View("AccesDenied");
				}
				//-------------------------------------------------------------------

				//Remove works that arent in same projects that th manager----------
				if (ManagerHelper.IsManager(User) && User.Identity.Name != UserName)
				{
					List<JobModel> jobs = new List<JobModel>();
					foreach (WorkModel work in works)
					{
						if (!jobs.Contains(_context.JobModel.Find(work.JobId))) jobs.Add(_context.JobModel.Find(work.JobId));
					}
					foreach (JobModel job in jobs)
					{
						var projectMemberships = _context.ProjectMemberships
							.Where(pm => pm.ProjectId == job.ProjectId)
							.ToList();
						if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name.ToString()).Any())
						{
							foreach (JobModel job1 in jobs.Where(jm => jm.ProjectId == job.ProjectId).ToList())
							{
								jobs.Remove(job1);
							}
						}
					}
					foreach (WorkModel work in works)
					{
						if (!jobs.Where(j => j.Id == work.JobId).Any())
						{
							works.Remove(work);
						}
					}
				}
				//---------------------------------------------------------------------

				List<string> firstJobTitles = new List<string>();
				List<string> secondJobTitles = new List<string>();
				foreach (WorkModel work in works)
				{
					if (!firstJobTitles.Contains(workService.GetJobTitleByWorkId(work.Id))) firstJobTitles.Add(workService.GetJobTitleByWorkId(work.Id));
				}

				//Get jobs/-------------------------------------------------------------
				var ProjectMemberships = _context.ProjectMemberships
							.Where(pm => pm.Member == UserName)
							.ToList();
				var projects = new List<ProjectModel>();
				foreach (ProjectMembership membership in ProjectMemberships)
				{
					if (!projects.Contains(_context.ProjectModel.Find(membership.ProjectId))) projects.Add(_context.ProjectModel.Find(membership.ProjectId));
				}
				List<JobModel> Jobs = new List<JobModel>();
				foreach (ProjectModel project in projects)
				{
					foreach (JobModel job in _context.JobModel.Where(jm => jm.ProjectId == project.Id)) Jobs.Add(job);
				}
				//------------------------------------------------------------------------

				foreach (JobModel job in Jobs)
				{
					if (!secondJobTitles.Contains(job.JobTitle) && !firstJobTitles.Contains(job.JobTitle))
					{
						secondJobTitles.Add(job.JobTitle);
					}
				}
				ViewBag.MoneyPerHour = _context.Workers.Where(w => w.User == UserName).ToList()[0].MoneyPerHour;
				ViewBag.FirstJobTitles = firstJobTitles;
				ViewBag.SecondJobTitles = secondJobTitles;

				return View("WorkTable", works);
			}
			//----------------------------------------------------------------------------------------------
			//----------------------------------------------------------------------------------------------
			else
			{
				List<WorkModel> works = await _context.WorkModel.Where(wm => wm.User == UserName && wm.isFinished == true).ToListAsync();

				if (DotEnv.Read()["ADMIN_USERNAME"].ToLower() != User.Identity.Name.ToLower() && User.Identity.Name.ToString() != UserName) return View("AccesDenied");

				List<string> firstJobTitles = new List<string>();
				List<string> secondJobTitles = new List<string>();
				foreach (WorkModel work in works)
				{
					if (!firstJobTitles.Contains(workService.GetJobTitleByWorkId(work.Id))) firstJobTitles.Add(workService.GetJobTitleByWorkId(work.Id));
				}

				//Get jobs => secondJobTitles------------------------------------------------
				var ProjectMembershipsOfUser = _context.ProjectMemberships
							.Where(pm => pm.Member == UserName)
							.ToList();
				var projectsOfTheUser = new List<ProjectModel>();
				foreach (ProjectMembership membership in ProjectMembershipsOfUser)
				{
					if (!projectsOfTheUser.Contains(_context.ProjectModel.Find(membership.ProjectId))) projectsOfTheUser.Add(_context.ProjectModel.Find(membership.ProjectId));
				}
				List<JobModel> Jobs = new List<JobModel>();
				foreach (ProjectModel project in projectsOfTheUser)
				{
					foreach (JobModel job in _context.JobModel.Where(jm => jm.ProjectId == project.Id)) Jobs.Add(job);
				}
				//------------------------------------------------------------------------

				foreach (JobModel job in Jobs)
				{
					if (!secondJobTitles.Contains(job.JobTitle) && !firstJobTitles.Contains(job.JobTitle))
					{
						secondJobTitles.Add(job.JobTitle);
					}
				}
				ViewBag.MoneyPerHour = _context.Workers.Where(w => w.User == UserName).ToList()[0].MoneyPerHour;
				ViewBag.FirstJobTitles = firstJobTitles;
				ViewBag.SecondJobTitles = secondJobTitles;

				return View("WorkTable", works);
			}
		}

		#endregion
		private bool JobModelExists(int id)
		{
			return (_context.JobModel?.Any(e => e.Id == id)).GetValueOrDefault();
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
