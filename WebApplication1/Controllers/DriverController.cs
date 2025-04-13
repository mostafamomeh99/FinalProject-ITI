
using IRepositoryService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using WebApplication1.Const;
using WebApplication1.Dtos.Drivers;
using WebApplication1.Middlewares.ExceptionFeatures;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly IUnitOfWork unitOFWork;

        public DriverController(IUnitOfWork unitOFWork)
        {
            this.unitOFWork = unitOFWork;
        }

        [HttpGet]
        public IActionResult GetDrivers([FromQuery] int pagenumber)
        {
            if (pagenumber <= 0) { return BadRequest(new {message= "Invalid page number."}); }

            List<DriverGetDto> drivers = new List<DriverGetDto>();
            var dbDrivers = unitOFWork.Drivers.GetAllWith(
                new[] {"Transportation"} 
                ,skip: ((pagenumber - 1)* ConstantProject.NumberOfData),
                take: ConstantProject.NumberOfData); 

            foreach (var driver in dbDrivers)
            {
                drivers.Add(new DriverGetDto()
                {
                    Id = driver.Id,
                    Name = driver.Name,
                    TransportationId = driver.TransportationId,
                    TransportationName = driver.Transportation.Name 
                });
            }

            return Ok(drivers);
        }

        [HttpPost]
        public IActionResult AddDriver([FromBody] DriverAddDto driverDto)
        {
            // check transportaion is exist
            var transport = unitOFWork.Transportations.Findme(e => e.Id == driverDto.TransportationId);
            if (transport == null) throw new CustomExecption("transpotation is not exist");

            // check transportaion is taken
            var  drivers= unitOFWork.Drivers.FindAll(e => e.TransportationId == driverDto.TransportationId);
            if (drivers.Any()) throw new CustomExecption("transpotation is already with another driver");

            //add driver
            var driver = new Driver()
            {  Id= Guid.NewGuid().ToString(),
                Name = driverDto.Name,
                TransportationId = driverDto.TransportationId
            };

            unitOFWork.Drivers.Addone(driver);
            unitOFWork.Compelet(); 

            return Ok(new { message = "Driver added successfully" });
        }


        [HttpPut]
        public IActionResult UpdateDriver([FromBody] DriverUpdateDto driverDto)
        {
            // check driver exists
            var dbDriver = unitOFWork.Drivers.Findme(e => e.Id == driverDto.Id);
            if (dbDriver == null) return BadRequest(new { message = "Driver not found." });

            // check  updated transportation not exist with another driver
            var drivers = unitOFWork.Drivers.FindAll(e => e.TransportationId == driverDto.TransportationId
            && e.Id != driverDto.Id);
            if (drivers.Any()) throw new CustomExecption("transpotation is already with another driver");

            dbDriver.Name = driverDto.Name;
            dbDriver.TransportationId = driverDto.TransportationId;

            unitOFWork.Compelet(); 

            return Ok(new { message = "Driver updated successfully" });
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteDriver([FromRoute] string id)
        {
            var driver = unitOFWork.Drivers.Findme(e => e.Id == id);
            if (driver == null) return BadRequest(new { message = "Driver not found." });

            unitOFWork.Drivers.Delete(driver);
            unitOFWork.Compelet(); 

            return Ok(new { message = "Driver deleted successfully" });
        }
    }
}
