using ApplicationCore.Constants;
using ApplicationCore.DTO;
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
            _path = _pathConstants.pathProvider;
            _APIURL = _pathConstants.APIURL;
        }

        [HttpGet]
        public ActionResult Get()
        {
            IEnumerable<ProviderDTO> providersDtos = _providerService.GetProviders();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<ProviderDTO, ProviderModel>()).CreateMapper();
            var providers = mapper.Map<IEnumerable<ProviderDTO>, List<ProviderModel>>(providersDtos);

            foreach (var pr in providers)
            {
                pr.Path = _APIURL +_path + pr.Path;
                pr.TimeWorkTo = providersDtos.FirstOrDefault(p=>p.Id==pr.Id).TimeWorkTo.ToShortTimeString();
                pr.TimeWorkWith = providersDtos.FirstOrDefault(p => p.Id == pr.Id).TimeWorkWith.ToShortTimeString();
            }

            return new ObjectResult(providers);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
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
    }
}
