using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Temp.Models
{
    public class AppDbContext :IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
         : base(options) {

            //options.
        }


        public DbSet<Employee> Employees { get; set; }
        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);
            // modelBuilder.Seed();

            foreach(var foreignKey in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys())) {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
