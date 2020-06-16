using ApplicationCore.Constants;
using ApplicationCore.DTO;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var provider = _providerService.GetProvider(id);

            if (provider != null)
            {
                return new ObjectResult(provider);
            }

            return NotFound();
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
            var provider = ConvertProviderModelToProviderDTO(model);
            _providerService.EditProvider(provider);
            return Ok(model);
        }

        [HttpPost]
        public IActionResult Post(ProviderModel model)
        {
            _providerService.AddProvider(ConvertProviderModelToProviderDTO(model));
            return Ok(model);
        }

        private ProviderDTO ConvertProviderModelToProviderDTO(ProviderModel model)
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
    }
}
