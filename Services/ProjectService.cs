using Microsoft.EntityFrameworkCore;
using Munkanaplo2.Data;
using Munkanaplo2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Munkanaplo2.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ProjectModel> GetProjectsAsync()
        {
            return _context.ProjectModel.Include(pm => pm.ProjectMembers).ToList();
        }
    }
}
