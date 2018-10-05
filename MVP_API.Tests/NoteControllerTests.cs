using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MVP_API.Controllers;
using Xunit;

namespace MVP_API.Tests
{
    public class NoteControllerTests
    {
        [Fact]
        public void VerifySuccessfulUserIdAndBodyControllerCall()
        {            
            // arrange
            var repository = new Mock<IRepository>();
            repository.Setup(x => x.QueryNotes(It.IsAny<Expression<Func<Note, bool>>>()))
                .Returns(new List<Note>
                {
                    new Note {UserId = 1, Body = "Body1"},
                    new Note {UserId = 2, Body = "Body2"},
                    new Note {UserId = 3, Body = "Body3"}
                });
            var logger = new Mock<ILogger<NotesController>>(); 

            var controller = new NotesController(repository.Object, logger.Object);

            // act
            var result = controller.Post(new NoteRequest {UserId = 1, Body = "Body"}) as OkObjectResult;

            // assert
            Assert.Equal(200, result.StatusCode);

            var noteResponse = result.Value as NoteResponse;
            Assert.Equal(3, noteResponse.Notes.Count());
        }

        [Fact]
        public void VerifySuccessfulUserIdControllerCall()
        {
            // arrange
            var repository = new Mock<IRepository>();
            repository.Setup(x => x.QueryNotes(It.IsAny<Expression<Func<Note, bool>>>()))
                .Returns(new List<Note>
                {
                    new Note {UserId = 1, Body = "Body1"},
                });
            var logger = new Mock<ILogger<NotesController>>();

            var controller = new NotesController(repository.Object, logger.Object);

            // act
            var result = controller.Post(new NoteRequest { UserId = 1, Body = string.Empty}) as OkObjectResult;

            // assert
            Assert.Equal(200, result.StatusCode);

            var noteResponse = result.Value as NoteResponse;
            Assert.Equal(1, noteResponse.Notes.Count());
        }

        [Fact]
        public void VerifySuccessfulBodyControllerCall()
        {
            // arrange
            var repository = new Mock<IRepository>();
            repository.Setup(x => x.QueryNotes(It.IsAny<Expression<Func<Note, bool>>>()))
                .Returns(new List<Note>
                {
                    new Note {UserId = 0, Body = "Body3"}
                });
            var logger = new Mock<ILogger<NotesController>>();

            var controller = new NotesController(repository.Object, logger.Object);

            // act
            var result = controller.Post(new NoteRequest { Body = "Body" }) as OkObjectResult;

            // assert
            Assert.Equal(200, result.StatusCode);

            var noteResponse = result.Value as NoteResponse;
            Assert.Equal(1, noteResponse.Notes.Count());
        }

        [Fact]
        public void VerifyFailedControllerCall()
        {
            // arrange
            var errorMessageReturned = string.Empty;

            var repository = new Mock<IRepository>();
            repository.Setup(x => x.QueryNotes(It.IsAny<Expression<Func<Note, bool>>>()))
                .Throws(new Exception("This is an error"));
            var fakeLog = new FakeLog();
            var controller = new NotesController(repository.Object, fakeLog);

            // act
            var result = controller.Post(new NoteRequest { UserId = 1, Body = "Body" }) as ObjectResult;

            // assert
            Assert.Equal(500, result.StatusCode);
            Assert.True(fakeLog.LoggedMessage.Contains("This is an error"));
        }

        public class FakeLog : ILogger<NotesController>
        {
            public string LoggedMessage { get; set; }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                LoggedMessage = state.ToString();
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return false;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }
        }
    }
}
