using System;
using System.Collections.Generic;
using ApplicationCore.DTO;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;


namespace WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogService _сatalogService;

        public CatalogController(ICatalogService сatalogService)
        {
            _сatalogService = сatalogService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                IEnumerable<CatalogDTO> сatalogDTOs = _сatalogService.GetСatalogs();           
                return new ObjectResult(сatalogDTOs);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var catalog = _сatalogService.GetСatalog(id);
                return new ObjectResult(catalog);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet, Route("provider/{providerid}")]
        public IActionResult GetByProviderId(int providerid)
        {
            try
            {
                var catalog = _сatalogService.GetСatalogs(providerid);
                return new ObjectResult(catalog);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _сatalogService.DeleteСatalog(id);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        public IActionResult Put(CatalogModel model)
        {
            try
            {
                _сatalogService.EditСatalog(ConvertCatalogModelToCatalogDTO(model));
                return Ok(model);
            }
            catch (Exception ex)
            {

                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Post(CatalogModel model)
        {
            try
            {
                _сatalogService.AddСatalog(ConvertCatalogModelToCatalogDTO(model));
                return Ok(model);
            }
            catch (Exception ex)
            {
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
