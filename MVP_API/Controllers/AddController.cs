using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MVP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AddController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly ILogger<AddController> _logger;

        public AddController(IRepository repository, ILogger<AddController> logger)
        {
            _repository = repository;
             _logger = logger;
        }

        // GET api/Add/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            var noteToAdd = new Note { UserId = id, Body = $"Body for user {id}.", UpdatedOn = DateTime.Now };
            _repository.Save(noteToAdd);
            return id + " Added.";
        }
    }
}