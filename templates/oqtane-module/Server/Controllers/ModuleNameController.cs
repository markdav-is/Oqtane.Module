using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Infrastructure;
using RootNamespace.Models;
using RootNamespace.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RootNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleNameController : Controller
    {
        private readonly IModuleNameService _moduleNameService;
        private readonly ILogManager _logger;

        public ModuleNameController(IModuleNameService moduleNameService, ILogManager logger)
        {
            _moduleNameService = moduleNameService;
            _logger = logger;
        }

        // GET api/<controller>?moduleId=x
        [HttpGet]
        [Authorize(Policy = "ViewModule")]
        public async Task<IEnumerable<ModuleName>> Get(int moduleId)
        {
            return await _moduleNameService.GetModuleNamesAsync(moduleId);
        }

        // GET api/<controller>/id
        [HttpGet("{id}")]
        [Authorize(Policy = "ViewModule")]
        public async Task<ModuleName> Get(int id)
        {
            return await _moduleNameService.GetModuleNameAsync(id);
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Policy = "EditModule")]
        public async Task<ModuleName> Post([FromBody] ModuleName moduleName)
        {
            return await _moduleNameService.AddModuleNameAsync(moduleName);
        }

        // PUT api/<controller>/id
        [HttpPut("{id}")]
        [Authorize(Policy = "EditModule")]
        public async Task<ModuleName> Put(int id, [FromBody] ModuleName moduleName)
        {
            return await _moduleNameService.UpdateModuleNameAsync(moduleName);
        }

        // DELETE api/<controller>/id
        [HttpDelete("{id}")]
        [Authorize(Policy = "EditModule")]
        public async Task Delete(int id)
        {
            await _moduleNameService.DeleteModuleNameAsync(id);
        }
    }
}
