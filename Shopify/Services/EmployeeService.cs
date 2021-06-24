using Microsoft.EntityFrameworkCore;
using Shopify.Helper;
using Shopify.Models;
using Shopify.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Shopify.Repository
{
    public class EmployeeService
    {
        ShopifyContext _db;
        public EmployeeService(ShopifyContext db)
        {
            _db = db;
        }

        public void AddEmployeeId(string id , float salary , DateTime hireDate)
        {
            _db.Employees.Add(new Employee() { EmployeeId = id , Salary = salary , hireDate = hireDate});
            _db.SaveChanges();
        }

        public List<Employee> GetAllEmployees()
        {
          List<Employee> employees=   _db.Employees.Include(i => i.ApplicationUser).ToList();
            return employees;
        }

        public Employee GetEmployeeById(string id)
        {
           Employee employee = _db.Employees.Include(i=>i.ApplicationUser).FirstOrDefault(e => e.EmployeeId == id && e.ApplicationUser.AdminLocked == false);
            return employee;
        }

        public bool DeleteEmployee(string id)
        {
           Employee employee = _db.Employees.Include(i=>i.ApplicationUser).FirstOrDefault(e => e.EmployeeId == id && e.ApplicationUser.AdminLocked == false);
        
           if(employee!=null)
            {
                employee.ApplicationUser.AdminLocked = true;
                _db.SaveChanges();
                return true;
            }
            return false;
        }


        public bool EditEmployee(EditEmployeeModelView model , IIdentity user)
        {
            var employeeId = HelperMethods.GetAuthnticatedUserId(user);
            Employee employee = _db.Employees.Include(i=>i.ApplicationUser).FirstOrDefault(e => e.EmployeeId == employeeId && e.ApplicationUser.AdminLocked == false);
            if(employee != null)
             {
                employee.ApplicationUser.Fname = model.Fname;
                employee.ApplicationUser.Lname = model.Lname;
                employee.ApplicationUser.Email = model.Email;
                employee.ApplicationUser.Address = model.Address;
                _db.SaveChanges();
                return true;
             }
            return false;
        }




        public bool EditEmployeeSalary( string id , float salary)
        {
            Employee employee = _db.Employees.Include(i => i.ApplicationUser).FirstOrDefault(e => e.EmployeeId == id && e.ApplicationUser.AdminLocked == false);
            if (employee != null)
            {
                employee.Salary = salary;
                _db.SaveChanges();
                return true;
            }
            return false;
        }

    }
}
