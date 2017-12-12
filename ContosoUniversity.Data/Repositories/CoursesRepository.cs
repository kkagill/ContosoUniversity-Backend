using ContosoUniversity.Data;
using ContosoUniversity.Data.Abstract;
using ContosoUniversity.Data.Repositories;
using ContosoUniversity.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Scheduler.Data.Repositories
{
    public class CoursesRepository : EntityBaseRepository<Course>, ICoursesRepository
    {
        private ContosoContext _context;

        public CoursesRepository(ContosoContext context)
            : base(context)
        {
            _context = context;
        }      

        public IEnumerable<Department> PopulateDepartmentDropdownList()
        {          
            return _context.Departments.OrderBy(d => d.Name);
        }
    }
}