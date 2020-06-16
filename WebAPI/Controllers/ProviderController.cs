using ApplicationCore.Constants;
using ApplicationCore.DTO;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Models.Provider;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderController : ControllerBase
    {
        private readonly IProviderService _providerService;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly PathConstants _pathConstants;

        private readonly string _path;
        private readonly string _APIURL;

        public ProviderController(IProviderService providerService,
            IWebHostEnvironment appEnvironment)
        {
            _providerService = providerService;
            _appEnvironment = appEnvironment;
            _pathConstants = new PathConstants();
            _path = _pathConstants.pathForAPI;
            _APIURL = _pathConstants.APIURL;
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
                pr.Path = _APIURL + _path + pr.Path;
                pr.TimeWorkTo = providersDtos.FirstOrDefault(p => p.Id == pr.Id).TimeWorkTo.ToShortTimeString();
                pr.TimeWorkWith = providersDtos.FirstOrDefault(p => p.Id == pr.Id).TimeWorkWith.ToShortTimeString();
            }

            return new ObjectResult(providers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
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
                return new ObjectResult(provider);
            }

            return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _providerService.DeleteProvider(id);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        public  IActionResult Put(ProviderModel model)
        {
            try
            { 
            var provider = ConvertProviderModelToProviderDTO(model);
            _providerService.EditProvider(provider);
            return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public IActionResult Post(ProviderModel model)
        {
            try
            { 
            _providerService.AddProvider(ConvertProviderModelToProviderDTO(model));
            return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
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
                Path = model.Path.Replace(_APIURL+_path, ""),
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
