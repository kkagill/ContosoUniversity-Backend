using ContosoUniversity.Data;
using ContosoUniversity.Data.Abstract;
using ContosoUniversity.Data.Repositories;
using ContosoUniversity.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Scheduler.Data.Repositories
{
    public class EnrollmentRepository : EntityBaseRepository<Enrollment>, IEnrollmentRepository
    {
        private ContosoContext _context;

        public EnrollmentRepository(ContosoContext context)
            : base(context)
        {
            _context = context;
        }
    }
}