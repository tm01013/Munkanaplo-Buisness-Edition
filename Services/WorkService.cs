using Munkanaplo2.Data;
using Munkanaplo2.Models;

namespace Munkanaplo2.Services
{
    public class WorkService : IWorkService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        public WorkService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public string GetJobTitleByWorkId(int id)
        {
            WorkModel work = _context.WorkModel.Find(id);
            if (work == null) return null;
            JobModel job = _context.JobModel.Find(work.JobId);
            if (job == null) return null;

            return job.JobTitle;
        }

        public WorkModel GetMyOngoingWork()
        {
            if (_httpContextAccessor.HttpContext.User.Identity.Name == null) return null;

            List<WorkModel> work = _context.WorkModel.Where(wm => wm.User == _httpContextAccessor.HttpContext.User.Identity.Name.ToString() && wm.isFinished == false).ToList();
            if (work.Any())
            {
                return work[0];
            }
            else
            {
                return null;
            }

        }
    }
}