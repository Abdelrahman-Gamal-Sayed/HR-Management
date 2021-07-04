using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Temp.Models
{
    public class SQLEmployeeReprosotry:IEmployeeRepository
    {
        private readonly AppDbContext dbContext;
        public SQLEmployeeReprosotry(AppDbContext dbContext) => this.dbContext = dbContext;

        public Employee Add(Employee employee) {
            dbContext.Employees.Add(employee);
            dbContext.SaveChanges();
            return employee;
        }

        public Employee Delete(int Id) {
            Employee employee = dbContext.Employees.Find(Id);
            if(employee != null) {
                dbContext.Employees.Remove(employee);
                dbContext.SaveChanges();
            }
            return employee;
        }

        public IEnumerable<Employee> GetAllEmployee() {
            return dbContext.Employees;
        }

        public Employee GetEmployee(int Id) {
            return dbContext.Employees.Find(Id);
        }

        public Employee Update(Employee employeeChanges) {
            var employee = dbContext.Employees.Attach(employeeChanges);
            employee.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            dbContext.SaveChanges();
            return employeeChanges;
        }
    }
}
