using Microsoft.EntityFrameworkCore;
using MinimalAPIPostgreSQLEF.Models;

namespace MinimalAPIPostgreSQLEF.Data
{   
    // Representacion de la BD ("OfficeDb") para entityFramework
    public class OfficeDb : DbContext
    {   
        public OfficeDb(DbContextOptions<OfficeDb> options) : base(options)
        {

        }

        // Representacion de la tabla empleados usando un DbSet llamado Employess
        public DbSet<Employee> Employees => Set<Employee>();
    }
}