using ApplicationCore.Constants;
using ApplicationCore.DTO;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using WebAPI.Controllers.Identity;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderController : ControllerBase
    {
        private readonly IProviderService _providerService;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly ILogger<ProviderController> _logger;
        private readonly IUserHelper _userHelper;

        private readonly string _path;

        public ProviderController(IProviderService providerService,
            IWebHostEnvironment appEnvironment,
            ILogger<ProviderController> logger,
            IUserHelper userHelper)
        {
            _providerService = providerService;
            _appEnvironment = appEnvironment;
            _logger = logger;
            _userHelper = userHelper;
            _path = _path = PathConstants.APIURL + PathConstants.pathForAPI;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                IEnumerable<ProviderDTO> providersDtos = _providerService.GetProviders();
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<ProviderDTO, ProviderModel>()).CreateMapper();
                var providers = mapper.Map<IEnumerable<ProviderDTO>, List<ProviderModel>>(providersDtos);

                foreach (var pr in providers)
                {
                    pr.Path = _path + pr.Path;
                    pr.TimeWorkTo = providersDtos.FirstOrDefault(p => p.Id == pr.Id).TimeWorkTo.ToShortTimeString();
                    pr.TimeWorkWith = providersDtos.FirstOrDefault(p => p.Id == pr.Id).TimeWorkWith.ToShortTimeString();
                }

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[provider/get]:[info:get providers]");

                return new ObjectResult(providers);
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[provider/get]:[error:{ex.Property}, {ex.Message}]");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[provider/get]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var provider = _providerService.GetProvider(id);

                if (provider != null)
                {
                    _logger.LogInformation($"[{DateTime.Now.ToString()}]:[provider/get/{id}]:[info:get provider {id}]");

                    return new ObjectResult(provider);
                }

                return NotFound();
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[provider/get/{id}]:[error:{ex.Property}, {ex.Message}]");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[provider/get/{id}]:[error:{ex}]");

                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Post(ProviderModel model)
        {
            try
            {
                _providerService.AddProvider(ConvertProviderModelToProviderDTO(model));

                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[provider/post]:[info:add provider]:[user:{userId}]");

                return Ok(model);
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[provider/post]:[error:{ex.Property}, {ex.Message}]");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[provider/post]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public IActionResult Put(ProviderModel model)
        {
            try
            {
                var provider = ConvertProviderModelToProviderDTO(model);
                _providerService.EditProvider(provider);

                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[provider/put]:[info:edit provider {model.Id}]:[user:{userId}]");

                return Ok(model);
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[provider/put]:[error:{ex.Property}, {ex.Message}]");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[provider/put]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                _providerService.DeleteProvider(id);

                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[provider/delete/{id}]:[info:delete provider {id}]:[user:{userId}]");

                return Ok(id);
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[provider/delete/{id}]:[error:{ex.Property}, {ex.Message}]");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[provider/delete/{id}]:[error:{ex}]");

                return BadRequest();
            }
        }

        private ProviderDTO ConvertProviderModelToProviderDTO(ProviderModel model)
        {
            try
            {
                ProviderDTO providerDto = new ProviderDTO()
                {
                    Id = model.Id,
                    Email = model.Email,
                    Info = model.Info,
                    IsActive = model.IsActive,
                    IsFavorite = model.IsFavorite,
                    Name = model.Name,
                    Path = model.Path.Replace(_path, ""),
                    TimeWorkTo = Convert.ToDateTime(model.TimeWorkTo),
                    TimeWorkWith = Convert.ToDateTime(model.TimeWorkWith),
                    WorkingDays = model.WorkingDays
                };

                return providerDto;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
