using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MVP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize(Policy = "SubjectNames")]
    public class NotesController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly ILogger<NotesController> _logger;

        public NotesController(IRepository repository, ILogger<NotesController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // POST api/note
        [HttpPost]
        public IActionResult Post(NoteRequest value)
        {
            try
            {
                _logger.LogInformation($"QUERY: Id: {value.UserId}, Body: {value.Body}.");

                IEnumerable<Note> foundNotes = null;
                if (value.UserId != 0 && value.Body.Any()) foundNotes = _repository.QueryNotes(x => x.UserId == value.UserId && x.Body.Contains(value.Body)).ToList();
                if (value.UserId != 0 && !value.Body.Any()) foundNotes = _repository.QueryNotes(x => x.UserId == value.UserId).ToList();
                if (value.UserId == 0 && value.Body.Any()) foundNotes = _repository.QueryNotes(x => x.Body.Contains(value.Body)).ToList();

                var noteResponse = new NoteResponse
                {
                    Notes = foundNotes
                };

                return Ok(noteResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR:{ex}.");

                var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An exception occurred, see log.")
                };

                return StatusCode(500, response);
            }
        }
    }
}
