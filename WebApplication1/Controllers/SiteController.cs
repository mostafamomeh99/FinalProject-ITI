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
             take: ConstantProject.NumberOfData, skip: ((pagenumber - 1) * ConstantProject.NumberOfData)
            );

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
        [Consumes("multipart/form-data")]
     
        public IActionResult UpdateSite([FromForm] SiteUpdateDto sitedto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Log all validation errors
                    var errors = ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .Select(e => new {
                            Field = e.Key,
                            Errors = e.Value.Errors.Select(error => error.ErrorMessage)
                        }).ToList();

                    Console.WriteLine("Validation errors:");
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"{error.Field}: {string.Join(", ", error.Errors)}");
                    }

                    return BadRequest(new
                    {
                        message = "Validation failed",
                        errors = errors
                    });
                }

                // Check if we have any images at all
                bool hasNewImages = sitedto.Images != null && sitedto.Images.Any();
                bool hasExistingImages = sitedto.SiteImages != null && sitedto.SiteImages.Any();

                if (!hasNewImages && !hasExistingImages)
                {
                    return BadRequest(new { message = "At least one image is required" });
                }

                // check site exists
                var dbsite = unitOFWork.Sites.Findme(e => e.SiteId == sitedto.Id, new[] { "SiteImages" });

                if (dbsite == null) return BadRequest(new { message = "Site not found" });

                // check government exists
                var existgovernment = unitOFWork.Governments.FindAll(e => e.GovernmentId == sitedto.GovernmentId);
                if (!existgovernment.Any())
                    return BadRequest(new { message = "Government does not exist" });

                // Update basic info
                dbsite.SiteName = sitedto.Name;
                dbsite.GovernmentId = sitedto.GovernmentId;
                dbsite.SiteDescription = sitedto.Description;

                string imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(imagesDirectory))
                {
                    Directory.CreateDirectory(imagesDirectory);
                }

                // Handle existing images
                if (hasExistingImages)
                {
                    // Find images to remove (exist in DB but not in incoming list)
                    var imagesToRemove = dbsite.SiteImages
                        .Where(dbImg => !sitedto.SiteImages.Any(si => si.Image == dbImg.ImageName))
                        .ToList();

                    foreach (var imgToRemove in imagesToRemove)
                    {
                        // Delete file
                        string fullPath = Path.Combine(imagesDirectory, imgToRemove.ImageName);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        unitOFWork.SiteImages.Delete(imgToRemove);
                    }
                }
                else
                {
                    // No existing images in request - delete all
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

                // Add new images
                if (hasNewImages)
                {
                    foreach (var image in sitedto.Images)
                    {
                        if (image.Length > 0)
                        {
                            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                            string filePath = Path.Combine(imagesDirectory, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                image.CopyTo(stream);
                            }

                            unitOFWork.SiteImages.Addone(new SiteImage
                            {
                                SiteId = sitedto.Id,
                                ImageName = fileName
                            });
                        }
                    }
                }

                unitOFWork.Compelet();
                return Ok(new { message = "Site updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
