using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopify.Models;
using Shopify.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    [Route("api/[controller]")]
    [ApiController]
    public class GovernorateController : ControllerBase
    {
        GovernorateService _governorateRebo;

        public GovernorateController(GovernorateService governorateRebo)
        {
            _governorateRebo = governorateRebo;
        }
        //get all Governorate
        [AllowAnonymous]
        [HttpGet]
        public ActionResult<List<Governorate>> getall()
        {
            return _governorateRebo.GetAllGovernorate();
        }

        // get Governorate by id
        [AllowAnonymous]
        [HttpGet("{id}")]
        public ActionResult<Governorate> GetStatus(int id)
        {
            var result = _governorateRebo.GetGovernorate(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        // add Governorate
        
        [HttpPost]
        public ActionResult<Governorate> AddGovernorate([FromBody] Governorate governorate)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            else
            {
                Governorate result = _governorateRebo.AddGovernorate(governorate);

                return Ok(result);
            }

        }
        //edit Governorate
       
        [HttpPut("{id}")]
        public ActionResult EditGovernorate(int id, [FromBody] Governorate governorate)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            else
            {
                governorate.GovernorateId = id;
                var result = _governorateRebo.EditStatus(governorate);
                if (result != null)
                    return NoContent();
                return NotFound();
            }

        }




        // delete Governorate
        [HttpDelete("{id}")]
        public ActionResult deleteGovernorate(int id)
        {
            var result = _governorateRebo.DeleteGovernorate(id);
            if (result.Status == "Success")
                return NoContent();
            else if (result.Status == "Error")
                return new StatusCodeResult(StatusCodes.Status405MethodNotAllowed);
            else
                return NotFound();
        }






    }
}
