using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Employee_Mg_Core.Models
{
    public  partial class EmployeeMgContext: DbContext
    {
        public EmployeeMgContext(DbContextOptions<EmployeeMgContext> options)
    : base(options)
        { }

        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Data Source = (localdb)\mssqllocaldb; Initial Catalog = EmployeeManagement; Integrated Security = True");//(@"Server =(localdb)\mssqllocaldb;Database=EmployeeManagement;Trusted_Connection=True;;MultipleActiveResultSets=true");
            }
        }

        public virtual DbSet<DepartmentMaster> DepartmentMaster { get; set; }
        public virtual DbSet<EmployeeMaster> EmployeeMaster { get; set; }
        public virtual DbSet<Registration> Registration { get; set; }

    }
}
