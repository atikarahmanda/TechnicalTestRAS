using FinalProjectRAS.Repositories;
using FinalProjectRAS.Repositories.Interface;
using FinalProjectRAS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace FinalProjectRAS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class QuestionsController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IQuestionsRepository _questionsRepository;
        public QuestionsController(IQuestionsRepository questionRepository, IConfiguration configuration)
        {
            _questionsRepository = questionRepository;
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Candidate")]
        public async Task<ActionResult<List<QuestionsVM>>> GetRandomQuestionsByLevel()
        {
            var exams = await _questionsRepository.GetRandomQuestionsByLevel();
            return Ok(new { Status = "200", Message = "Questions retrieved successfully", Data = exams });
        }

        [HttpGet("QuestionAll")]
        [Authorize(Roles = "Admin,Candidate")]
        public async Task<ActionResult<List<QuestionsVM>>> GetAllQuestion()
        {
            var exams = await _questionsRepository.GetAllQuestion();
            return Ok(new { Status = "200", Message = "Questions retrieved successfully", Data = exams });
        }

        [HttpPost("Answer")]
        [Authorize(Roles = "Admin,Candidate")]
        public async Task<IActionResult> SaveAnswer([FromBody] AnswerVM answerVM)
        {
            if (answerVM == null)
            {
                return BadRequest(new { Status = "400", Message = "Answer is null", Data = (object)null });
            }

            bool isSaved = await _questionsRepository.SaveAnswer(answerVM);

            if (!isSaved)
            {
                return StatusCode(500, new { Status = "500", Message = "An error occurred while saving the answer", Data = (object)null });
            }

            return Ok(new { Status = "200", Message = "Answer saved successfully", Data = (object)null });
        }

        [HttpGet("score/{userId}")]
        [Authorize(Roles = "Admin,Candidate")]
        public async Task<IActionResult> GetTotalScoreByUserId(Guid userId)
        {
            if (string.IsNullOrEmpty(userId.ToString()))
            {
                return BadRequest(new { Status = "400", Message = "User ID is required", Data = (object)null });
            }

            int totalScore = await _questionsRepository.GetTotalScoreByUserId(userId);

            return Ok(new { Status = "200", Message = "Total score retrieved successfully", Data = new { UserId = userId, TotalScore = totalScore } });
        }

        [HttpGet("users/scores")]
        [Authorize(Roles = "Admin,Candidate")]
        public async Task<IActionResult> GetAllUserScores()
        {
            var userScores = await _questionsRepository.GetAllUserScores();

            return Ok(new { Status = "200", Message = "User scores retrieved successfully", Data = userScores });
        }

        [HttpGet("GetStatisticQuestion")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetStatisticQuestion()
        {
            try
            {
                var statistics = _questionsRepository.GetStatisticQuestion();
                return Ok(new
                {
                    status = "success",
                    message = "Statistics retrieved successfully",
                    data = statistics
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = ex.Message
                });
            }
        }

        [HttpDelete("DeleteQuestion/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteQuestion(string id)
        {
            var result = _questionsRepository.DeleteQuestion(id);

            if (result == 0)
            {
                return NotFound(new
                {
                    status = "error",
                    message = "Question not found."
                });
            }

            if (result == 1)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Cannot delete the question because it is referenced to answer data."
                });
            }

            if (result == -1)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "An unexpected error occurred. Please try again later."
                });
            }

            return Ok(new
            {
                status = "success",
                message = "Question deleted successfully."
            });
        }


    }
}