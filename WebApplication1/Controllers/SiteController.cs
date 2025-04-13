using IRepositoryService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using WebApplication1.Const;
using WebApplication1.Dtos.Sites;
using WebApplication1.Middlewares.ExceptionFeatures;
using static System.Net.Mime.MediaTypeNames;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SiteController : ControllerBase
    {
        private readonly IUnitOfWork unitOFWork;

        public SiteController(IUnitOfWork unitOFWork)
        {
            this.unitOFWork = unitOFWork;
        }

        [HttpGet]
        public IActionResult GetSites([FromQuery] int pagenumber)
        {
            if(pagenumber <= 0) { return BadRequest(new { message= "not found" }); }

            List<SiteGetDto> sites = new List<SiteGetDto>();
            var dbsites = unitOFWork.Sites.GetAllWith(new[] { "SiteImages" },
                ((pagenumber - 1) * ConstantProject.NumberOfData),
                ConstantProject.NumberOfData);

            foreach (var sit in dbsites)
            {
                var sitedto = new SiteGetDto()
                {
                    Id = sit.SiteId,
                    Name = sit.SiteName,
                    Description = sit.SiteDescription
                };

                foreach(var img in sit.SiteImages)
                {
                    sitedto.siteImages.Add(new SiteImagesGetDto()
                    { Id=img.Id,
                        SiteId=sit.SiteId,
                        Image= $"{Request.Scheme}://{Request.Host}/images/{img.ImageName}"
                    });
                }
                sites.Add(sitedto);
            }

            return Ok(sites);
        }

        [HttpPost]
        public IActionResult AddSites([FromForm] SiteAddDto sitedto)
        {
            if(sitedto.images == null || !sitedto.images.Any()) { return BadRequest(new { message = "images is required" }); }

            // check government exists
            var existgovernment = unitOFWork.Governments.FindAll(e => e.GovernmentId == sitedto.GovernmentId);
            if(!existgovernment.Any()) throw new CustomExecption("government is not exist");

            var site = new Site()
            { SiteId= Guid.NewGuid().ToString(),
                GovernmentId =sitedto.GovernmentId,
            SiteDescription=sitedto.Description,
            SiteName=sitedto.Name
            };
            unitOFWork.Sites.Addone(site);

            unitOFWork.Compelet(); // to get site id first to use in siteimages

            var siteimages =new List<SiteImage>();
            foreach (var image in sitedto.images)
            {
                if (image.Length > 0)
                {
                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                    string imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                    string filePath = Path.Combine(imagesDirectory, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        image.CopyTo(stream);
                    }

                    // Save the file path in the database
                    siteimages.Add(new SiteImage()
                    {
                        ImageName = fileName,
                        SiteId = site.SiteId
                    });
                }
            }
            unitOFWork.SiteImages.AddRange(siteimages);
            unitOFWork.Compelet();

                return Ok(new {message="site added successfully"});
            
        }

        [HttpPut]
        public IActionResult UpdateSite([FromBody] SiteUpdateDto sitedto)
        {
            //there is no images in any places
            if (sitedto.images == null && (sitedto.siteImages == null || !sitedto.siteImages.Any()))
            {
                return BadRequest(new { message = "site image is requried" });
            }
            // check site exists
                var dbsite = unitOFWork.Sites.Findme(e => e.SiteId == sitedto.Id, new[] { "SiteImages" });

            if (dbsite == null) return BadRequest(new { message = "site not found" });

            // check government exists
            var existgovernment = unitOFWork.Governments.FindAll(e => e.GovernmentId == sitedto.GovernmentId);
            if (!existgovernment.Any()) throw new CustomExecption("government is not exist");

            dbsite.SiteName = sitedto.Name;
            dbsite.GovernmentId = sitedto.GovernmentId;
            dbsite.SiteDescription = sitedto.Description;


            string imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(imagesDirectory))
            {
                Directory.CreateDirectory(imagesDirectory);
            }

            // compare exisitng images first 

            if (sitedto.siteImages.Any())
            {
                foreach (var existimg in dbsite.SiteImages)
                {
                    // delete what is in database and not in cominglist
                    var comingimg =sitedto.siteImages.FirstOrDefault(e => e.Id == existimg.Id);
                    if (comingimg == null)
                    {
                        // delete image file from server
                        string fullPath = Path.Combine(imagesDirectory, existimg.ImageName);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }

                        unitOFWork.SiteImages.Delete(existimg);

                    }
                }
            }
            // siteimages is empty , so user want to delete all exisitng
            else
            {
                foreach (var existimg in dbsite.SiteImages)
                {
                    string fullPath = Path.Combine(imagesDirectory, existimg.ImageName);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }

                unitOFWork.SiteImages.DeleteRange(dbsite.SiteImages);
          
 
            }

            // add new images
            if(sitedto.images != null && sitedto.images.Count>0)
            { 
            foreach (var image in sitedto.images)
            {
                if (image.Length > 0)
                {
                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                    string filePath = Path.Combine(imagesDirectory, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        image.CopyTo(stream);
                    }

                    // Save the file path in the database
                    var siteimage = new SiteImage
                    {
                  SiteId=sitedto.Id ,
                  ImageName=fileName
                    };
                    unitOFWork.SiteImages.Addone(siteimage);
                }
            }
            }
            unitOFWork.Compelet();

            return Ok(new { message = "site updated successfully" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteSite([FromRoute] string id)
        {
            var site = unitOFWork.Sites.Findme(e => e.SiteId== id, new[] {"SiteImages"} );
            if (site == null) return BadRequest(new { message = "not found" });

  
            string imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            foreach (var item in site.SiteImages)
            {
                string fullPath = Path.Combine(imagesDirectory, item.ImageName);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            unitOFWork.SiteImages.DeleteRange(site.SiteImages);
            unitOFWork.Sites.Delete(site);

            unitOFWork.Compelet();
            return Ok(new { message = "site deleted successfully" });
        }
    }
}
