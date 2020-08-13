using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Security.Claims;
using WebAPI.Controllers.Identity;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UploadController:ControllerBase
    {
        private readonly ILogger<UploadController> _logger;
        private readonly IUserHelper _userHelper;

        public UploadController(ILogger<UploadController> logger,
            IUserHelper userHelper)
        {
            _logger = logger;
            _userHelper = userHelper;
        }

        [HttpPost, DisableRequestSizeLimit]
        public IActionResult Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("wwwroot", "files", "images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                 
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                    string userId = _userHelper.GetUserId(currentEmail);

                    if (userId == null)
                    {
                        return NotFound("User not found");
                    }

                    _logger.LogInformation($"[{DateTime.Now.ToString()}]:[upload/post]:[info:upload file {fileName}]:[user:{userId}]");

                    return Ok(new { fileName });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[upload/post]:[error:{ex}]");

                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}
