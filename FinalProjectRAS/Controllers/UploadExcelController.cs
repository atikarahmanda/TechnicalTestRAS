using FinalProjectRAS.Repositories;
using FinalProjectRAS.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProjectRAS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadExcelController : Controller
    {
        private UploadExcelRepository _uploadexcelRepository;
        private IConfiguration _configuration;
        public UploadExcelController(UploadExcelRepository UploadExcelRepository, IConfiguration configuration)
        {
            _uploadexcelRepository = UploadExcelRepository;
            _configuration = configuration;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult UploadQuestionsFromExcel(IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "No file uploaded." });
            }

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var (message, savedQuestionsCount) = _uploadexcelRepository.SaveQuestionsFromExcel(stream);

                    if (savedQuestionsCount == 0)
                    {
                        return BadRequest(new { Message = message });
                    }

                    return Ok(new { Message = $"{savedQuestionsCount} questions successfully uploaded." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging purposes
                return StatusCode(500, new { Message = "An error occurred while processing the file.", Details = ex.Message });
            }

        }
    }
}
