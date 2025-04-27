using IRepositoryService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using WebApplication1.Dtos.Transportations;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransportationController : ControllerBase
    {
        private readonly IUnitOfWork unitOFWork;

        public TransportationController(IUnitOfWork unitOFWork)
        {
            this.unitOFWork = unitOFWork;
        }

        [HttpGet]
        public IActionResult GetTransportations()
        {

            List<TransportationGetDto> transportations = new List<TransportationGetDto>();
            var dbTransportations = unitOFWork.Transportations.GetAllWith();

            foreach (var trans in dbTransportations)
            {
                transportations.Add(new TransportationGetDto()
                {
                    Id = trans.Id,
                    Name = trans.Name
                });
            }

            return Ok(transportations);
        }


        [HttpPost]
        public IActionResult AddTransportation([FromBody] TransportationAddDto transportationDto)
        {

            var transportation = new Transportation()
            {   
                Id = transportationDto.Id,
                Name = transportationDto.Name
            };

            unitOFWork.Transportations.Addone(transportation);
            unitOFWork.Compelet(); 

            return Ok(new { message = "Transportation added successfully" });
        }


        [HttpPut]
        public IActionResult UpdateTransportation([FromBody] TransportationUpdateDto transportationDto)
        {
            var dbtransportation = unitOFWork.Transportations.Findme(e => e.Id == transportationDto.Id);
            if (dbtransportation == null) 
            { return BadRequest(new { message = "Transportation not found." }); }

            dbtransportation.Name = transportationDto.Name;

            unitOFWork.Compelet(); 

            return Ok(new { message = "Transportation updated successfully" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTransportation([FromRoute] string id)
        {
            var transportation = unitOFWork.Transportations.Findme(e => e.Id == id);
            if (transportation == null) return BadRequest(new { message = "Transportation not found." });

            unitOFWork.Transportations.Delete(transportation);
            unitOFWork.Compelet(); 

            return Ok(new { message = "Transportation deleted successfully" });
        }
    }
}
