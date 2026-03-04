using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Oqtane.Controllers;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Shared;
using RootNamespace.Models;
using RootNamespace.Services;

namespace RootNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleNameController : ModuleControllerBase
    {
        private readonly IModuleNameService _moduleNameService;

        public ModuleNameController(IModuleNameService moduleNameService, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
        {
            _moduleNameService = moduleNameService;
        }

        // GET api/<controller>?moduleid=x
        [HttpGet]
        public async Task<IEnumerable<ModuleName>> Get(int moduleId)
        {
            if (IsAuthorizedEntityId(EntityNames.Module, moduleId))
            {
                return await _moduleNameService.GetModuleNamesAsync(moduleId);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ModuleName Get Attempt For Module {ModuleId}", moduleId);
                HttpContext.Response.StatusCode = 401;
                return null;
            }
        }

        // GET api/<controller>/5/1
        [HttpGet("{id}/{moduleId}")]
        public async Task<ModuleName> Get(int id, int moduleId)
        {
            if (IsAuthorizedEntityId(EntityNames.Module, moduleId))
            {
                return await _moduleNameService.GetModuleNameAsync(id, moduleId);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ModuleName Get Attempt {ModuleNameId} For Module {ModuleId}", id, moduleId);
                HttpContext.Response.StatusCode = 401;
                return null;
            }
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<ModuleName> Post([FromBody] ModuleName moduleName)
        {
            if (ModelState.IsValid && IsAuthorizedEntityId(EntityNames.Module, moduleName.ModuleId))
            {
                moduleName = await _moduleNameService.AddModuleNameAsync(moduleName);
                _logger.Log(LogLevel.Information, this, LogFunction.Create, "ModuleName Added {ModuleName}", moduleName);
                return moduleName;
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ModuleName Post Attempt {ModuleName}", moduleName);
                HttpContext.Response.StatusCode = 401;
                return null;
            }
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public async Task<ModuleName> Put(int id, [FromBody] ModuleName moduleName)
        {
            if (ModelState.IsValid && IsAuthorizedEntityId(EntityNames.Module, moduleName.ModuleId) && id == moduleName.ModuleNameId)
            {
                moduleName = await _moduleNameService.UpdateModuleNameAsync(moduleName);
                _logger.Log(LogLevel.Information, this, LogFunction.Update, "ModuleName Updated {ModuleName}", moduleName);
                return moduleName;
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ModuleName Put Attempt {ModuleName}", moduleName);
                HttpContext.Response.StatusCode = 401;
                return null;
            }
        }

        // DELETE api/<controller>/5/1
        [HttpDelete("{id}/{moduleId}")]
        public async Task Delete(int id, int moduleId)
        {
            if (IsAuthorizedEntityId(EntityNames.Module, moduleId))
            {
                await _moduleNameService.DeleteModuleNameAsync(id, moduleId);
                _logger.Log(LogLevel.Information, this, LogFunction.Delete, "ModuleName Deleted {ModuleNameId}", id);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ModuleName Delete Attempt {ModuleNameId} For Module {ModuleId}", id, moduleId);
                HttpContext.Response.StatusCode = 401;
            }
        }
    }
}
