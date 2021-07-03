using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        public EmployeeService(ShopifyContext db , UserManager<ApplicationUser> userManager)
        {
            _db = db;
            userManager = _userManager;
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
           Employee employee = _db.Employees.Include(i=>i.ApplicationUser).FirstOrDefault(e => e.EmployeeId == id && e.Isdeleted == false);
        
           if(employee!=null)
            {
                employee.Isdeleted = true;
                _db.SaveChanges();
                return true;
            }
            return false;
        }


        public async Task<bool> EditEmployeeAsync(EditEmployeeModelView model , IIdentity user)
        {
            var employeeId = HelperMethods.GetAuthnticatedUserId(user);
            Employee employee = _db.Employees.Include(i=>i.ApplicationUser).FirstOrDefault(e => e.EmployeeId == employeeId && e.ApplicationUser.AdminLocked == false);
            if(employee != null)
             {
                employee.ApplicationUser.Fname = model.Fname;
                employee.ApplicationUser.Lname = model.Lname;
                employee.ApplicationUser.Address = model.Address;

                string token = await _userManager.GenerateChangeEmailTokenAsync(employee.ApplicationUser, model.Email);
                await _userManager.ChangeEmailAsync(employee.ApplicationUser, model.Email, token);

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
