using ContosoUniversity.Data;
using ContosoUniversity.Data.Abstract;
using ContosoUniversity.Data.Repositories;
using ContosoUniversity.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Data.Repositories
{
    public class DepartmentsRepository : EntityBaseRepository<Department>, IDepartmentsRepository
    {
        private ContosoContext _context;

        public DepartmentsRepository(ContosoContext context)
            : base(context)
        {
            _context = context;
        }  
    }
}