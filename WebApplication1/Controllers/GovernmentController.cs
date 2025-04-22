using IRepositoryService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using WebApplication1.Dtos.Governments;
using WebApplication1.Middlewares.ExceptionFeatures;
using static System.Net.Mime.MediaTypeNames;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GovernmentController : ControllerBase
    {
        private readonly IUnitOfWork unitOFWork;

        public GovernmentController(IUnitOfWork unitOFWork)
        {
            this.unitOFWork = unitOFWork;
        }

        [HttpGet]
        public IActionResult GetGovernmet()
        {
            List<GovernmentGetDto> governments = new List<GovernmentGetDto>();
            var dbgovernments = unitOFWork.Governments.GetAllWith();
            foreach(var gov in dbgovernments)
            {
                governments.Add(new GovernmentGetDto()
                {
                    Id = gov.GovernmentId,
                    Name = gov.Name.ToLower(),
                    ImageUrl = $"{Request.Scheme}://{Request.Host}/images/{gov.Image}",
                });
            }
            
            return Ok(governments);
        }

        [HttpPost]
        public IActionResult AddGovernmet([FromForm] GovernmentAddDto government)
        {
            if (government.image == null || government.image.Length<=0)
            {
                return BadRequest(new { message = "government image is required." });
            }
            // check government existings
         var existgovernment=unitOFWork.Governments
                .FindAll(e => e.Name.ToLower().Contains(government.Name.ToLower()));

            if(existgovernment.Count() != 0) throw new CustomExecption("government already exist");

            string imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(imagesDirectory))
            {
                Directory.CreateDirectory(imagesDirectory);
            }

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(government.image.FileName)}";
            string filePath = Path.Combine(imagesDirectory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                government.image.CopyTo(stream);
            }

                 // Save the file path in the database
            var Gover = new Government
            {
                Name = government.Name.ToLower(),
                Image = fileName
            };
            unitOFWork.Governments.Addone(Gover);

            unitOFWork.Compelet();
            return Ok(new {message="government added successfully"});
        }


        [HttpPut]
        public IActionResult UpdateGovernmet([FromForm] GovernmentupdateDto government)
        {
            var dbgovernment = unitOFWork.Governments.Findme(e => e.GovernmentId == government.Id);

            if(dbgovernment == null)
            {
                return BadRequest(new { message = "government not found" });
            }
            if(government.ImageUrl == null && (government.image == null || government.image.Length<=0))
            {
                return BadRequest(new { message = "images is requried" });
            }

            // check change name of government is existings
            var existgovernment = unitOFWork.Governments
                   .FindAll(e => e.Name.ToLower().Contains(government.Name.ToLower()) &&
                   e.GovernmentId !=government.Id);

            if (existgovernment.Any()) throw new CustomExecption("government already exist");


            if (government.image != null)
            {
                string imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(imagesDirectory))
                {
                    Directory.CreateDirectory(imagesDirectory);
                }

                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(government.image.FileName)}";
                string filePath = Path.Combine(imagesDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    government.image.CopyTo(stream);
                }
                string fullPath = Path.Combine(imagesDirectory, dbgovernment.Image);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                dbgovernment.Image = fileName;
            }

            dbgovernment.Name = government.Name;
            unitOFWork.Compelet();
            return Ok(new { message = "government updated successfully" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteGovernment([FromRoute] int id)
        {
            var gov = unitOFWork.Governments.Findme(e => e.GovernmentId == id);
            if (gov == null) return BadRequest(new { message = "not found" });

            string imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            string fullPath = Path.Combine(imagesDirectory, gov.Image);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
            unitOFWork.Governments.Delete(gov);
            unitOFWork.Compelet();
            return Ok(new { message = "government deleted successfully" });
        }
    }
}
