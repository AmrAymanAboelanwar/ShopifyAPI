using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopify.Models;
using Shopify.Repository;
using Shopify.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        EmployeeService _employeeService;
        public EmployeeController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }



        // get all employee 
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult GetAllEmployee()
        {
            return Ok(_employeeService.GetAllEmployees());
        } 




        // get employee by id
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult GetEmployeeById(string id)
        {
           Employee employee =  _employeeService.GetEmployeeById(id);
            if (employee != null)
                return Ok(employee);
            return NotFound();
        }



        // delete employee 


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteEmployee(string id)
        {
            bool result = _employeeService.DeleteEmployee(id);
            if (result)
                return NoContent();
            return NotFound();
        }


        // edit employee
      
         [HttpPut("PersonalData")]
        [Authorize(Roles ="Employee")]
        public ActionResult EditEmployee([FromBody] EditEmployeeModelView model )
        {

            if (ModelState.IsValid)
            {
                bool result = _employeeService.EditEmployee(model , User.Identity);
                if (result)
                    return NoContent();
                return NotFound();
            }
            return BadRequest(ModelState);

        }




        // edit employee salary

        [HttpPut("EditSalary/{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult EditEmployeeSalary(string id ,[FromForm] float salary)
        {

            if (salary>0)
             {
                bool result = _employeeService.EditEmployeeSalary(id , salary);
                if (result)
                    return NoContent();
                return NotFound();
             }
            return BadRequest(ModelState);

        }






    }
}
