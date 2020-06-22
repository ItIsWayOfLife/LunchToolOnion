using System;
using System.Collections.Generic;
using System.Security.Claims;
using ApplicationCore.DTO;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAPI.Controllers.Identity;
using WebAPI.Models;


namespace WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogService _сatalogService;
        private readonly IUserHelper _userHelper;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(ICatalogService сatalogService,
             IUserHelper userHelper,
             ILogger<CatalogController> logger)
        {
            _сatalogService = сatalogService;
            _userHelper = userHelper;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                IEnumerable<CatalogDTO> сatalogDTOs = _сatalogService.GetСatalogs();

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[catalog/get]:[info:get catalogs]");

                return new ObjectResult(сatalogDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[catalog/get]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var catalog = _сatalogService.GetСatalog(id);

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[catalog/get/{id}]:[info:get catalog {id}]");

                return new ObjectResult(catalog);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[catalog/get/{id}]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpGet, Route("provider/{providerid}")]
        public IActionResult GetByProviderId(int providerid)
        {
            try
            {
                var catalog = _сatalogService.GetСatalogs(providerid);

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[catalog/provider/{providerid}]:[info:get catalog by provider {providerid}]");

                return new ObjectResult(catalog);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[catalog/provider/{providerid}]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                _сatalogService.DeleteСatalog(id);

                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[catalog/delete/{id}]:[info:delete catalog {id}]:[user:{userId}]");


                return Ok(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[catalog/delete/{id}]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public IActionResult Put(CatalogModel model)
        {
            try
            {
                _сatalogService.EditСatalog(ConvertCatalogModelToCatalogDTO(model));

                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[catalog/put]:[info:edit catalog {model.Id}]:[user:{userId}]");

                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[catalog/put]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Post(CatalogModel model)
        {
            try
            {
                _сatalogService.AddСatalog(ConvertCatalogModelToCatalogDTO(model));

                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[catalog/post]:[info:create new catalog name {model.Name}]:[user:{userId}]");

                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[catalog/post]:[error:{ex}]");

                return BadRequest();
            }
        }

        private CatalogDTO ConvertCatalogModelToCatalogDTO(CatalogModel model)
        {
            return new CatalogDTO()
            {
                Id = model.Id,
                Info = model.Info,
                Name = model.Name,
                ProviderId = model.ProviderId
            };
        }
    }
}
