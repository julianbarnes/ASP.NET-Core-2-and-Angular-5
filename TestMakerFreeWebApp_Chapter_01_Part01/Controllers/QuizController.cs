using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TestMakerFreeWebApp.ViewModels;
using System.Collections.Generic;
using System.Linq;
using TestMakerFreeWebApp;
using Mapster;
using TestMakerFreeWebApp.Data;

namespace TestMakerFreeWebApp.Controllers
{
    [Route("api/[controller]")]
    public class QuizController : Controller
    {
        #region Private Fields
        private ApplicationDbContext DbContext;
        #endregion
        #region Constructor 
        public QuizController(ApplicationDbContext context)
        {
            //Instantiate the ApplicationDbContext through DI
            DbContext = context;
        }
        #endregion Constructor
        #region RESTful conventions methods
        /// <summary>
        /// GET: api/quiz/{id}
        /// Retrieves the Quiz with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Quiz</param>
        /// <returns>the Quiz with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            // create a sample quiz to match the given request
            var quiz = DbContext.Quizzes.Where(i => i.Id == id).FirstOrDefault();
            if (quiz == null)
            {
                return NotFound(new
                {
                    Error = String.Format("QuizID {0} has not been found", id)
                });
            }
            // output the result in JSON format
            return new JsonResult(
                quiz.Adapt<QuizViewModel>(),
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented
                });
        }

        /// <summary>
        /// Adds a new Quiz to the Database
        /// </summary>
        /// <param name="model">The QuizViewModel containing the data to insert</param>
        [HttpPut]
        public IActionResult Put(QuizViewModel model)
        {
            //return a generit HTTP Status 500 (Server Error)
            //if the client payload is invalid.
            if (model == null)
            {
                return new StatusCodeResult(500);
            }
            //handle the insert without object mapping
            var quiz = new Quiz();
            //properties taken from the request
            quiz.Title = model.Title;
            quiz.Description = model.Description;
            quiz.Text = model.Text;
            quiz.Notes = model.Notes;

            //properties set from server-side
            quiz.CreatedDate = DateTime.Now;
            quiz.LastModifiedDate = quiz.CreatedDate;

            //Set a temporary author using the Admin user's userId
            quiz.UserId = DbContext.Users
                .Where(u => u.UserName == "Admin").FirstOrDefault().Id;
            DbContext.Quizzes.Add(quiz);
            DbContext.SaveChanges();

            //return the newly-created Quiz to the client.
            return new JsonResult(quiz.Adapt<QuizViewModel>(),
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented
                });
        }

        /// <summary>
        /// Edit the Quiz with the given {id}
        /// </summary>
        /// <param name="model">The QuizViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post(QuizViewModel model)
        {
            if(model == null)
            {
                return new StatusCodeResult(500);
            }
            var quiz = DbContext.Quizzes.Where(q => q.Id == model.Id).FirstOrDefault();
            if(quiz == null)
            {
                return NotFound(new
                {
                    Error = String.Format("QuizID{0}has not been found",
                    model.Id)
                });
            }

            quiz.Title = model.Title;
            quiz.Description = model.Description;
            quiz.Text = model.Text;
            quiz.Notes = model.Notes;

            quiz.LastModifiedDate = quiz.CreatedDate;
            DbContext.SaveChanges();
            return new JsonResult(quiz.Adapt<QuizViewModel>(),
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented
                });
        }

        /// <summary>
        /// Deletes the Quiz with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var quiz = DbContext.Quizzes.Where(q => q.Id == id).FirstOrDefault();
            if (quiz == null)
            {
                return NotFound(new
                {
                    Error = String.Format("QuizID{0}has not been found",
                   id)
                });
            }
            DbContext.Quizzes.Remove(quiz);
            DbContext.SaveChanges();

            return new OkResult();
        }
        #endregion

        #region Attribute-based routing methods
        /// <summary>
        /// GET: api/quiz/latest
        /// Retrieves the {num} latest Quizzes
        /// </summary>
        /// <param name="num">the number of quizzes to retrieve</param>
        /// <returns>the {num} latest Quizzes</returns>
        [HttpGet("Latest/{num:int?}")]
        public IActionResult Latest(int num = 10)
        {
            var latest = DbContext.Quizzes.OrderByDescending(q => q.CreatedDate).Take(num).ToArray();

            // output the result in JSON format
            return new JsonResult(
                latest.Adapt<QuizViewModel[]>(),
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented
                });
        }

        /// <summary>
        /// GET: api/quiz/ByTitle
        /// Retrieves the {num} Quizzes sorted by Title (A to Z)
        /// </summary>
        /// <param name="num">the number of quizzes to retrieve</param>
        /// <returns>{num} Quizzes sorted by Title</returns>
        [HttpGet("ByTitle/{num:int?}")]
        public IActionResult ByTitle(int num = 10)
        {
            var byTitle = DbContext.Quizzes.OrderBy(q => q.Title).Take(num).ToArray();

            return new JsonResult(
                byTitle.Adapt<QuizViewModel[]>(),
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented
                });
        }

        /// <summary>
        /// GET: api/quiz/mostViewed
        /// Retrieves the {num} random Quizzes
        /// </summary>
        /// <param name="num">the number of quizzes to retrieve</param>
        /// <returns>{num} random Quizzes</returns>
        [HttpGet("Random/{num:int?}")]
        public IActionResult Random(int num = 10)
        {
            var random = DbContext.Quizzes.OrderBy(q => Guid.NewGuid())
                .Take(num)
                .ToArray();

            return new JsonResult(
                random.Adapt<QuizViewModel[]>(),
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented
                });
        }
        #endregion
    }
}