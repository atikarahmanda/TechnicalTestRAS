using FinalProjectRAS.Context;
using FinalProjectRAS.Models;
using FinalProjectRAS.Repositories.Interface;
using FinalProjectRAS.Utils;
using FinalProjectRAS.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SkiaSharp;

namespace FinalProjectRAS.Repositories
{
    public class QuestionsRepository : IQuestionsRepository
    {
        private readonly MyContext _myContext;
        public QuestionsRepository(MyContext myContext)
        {
            _myContext = myContext;
        }

        public async Task<bool> SaveAnswer(AnswerVM answerVM)
        {
            var option = await _myContext.Options
                .FirstOrDefaultAsync(o => o.Option_id == answerVM.Option_id);
            var question = await _myContext.Questions
                .FirstOrDefaultAsync(q => q.Question_id == answerVM.Question_id);


            int points = (bool)option.Is_Correct ? 1 : 0;

            var existingAnswer = await _myContext.Answers
                .FirstOrDefaultAsync(a => a.User_id == answerVM.User_id && a.Question_id == answerVM.Question_id);

            if (existingAnswer != null)
            {
                existingAnswer.Option_id = answerVM.Option_id;
                existingAnswer.Point = points;

                _myContext.Answers.Update(existingAnswer);
            }
            else
            {
                var answer = new Answer
                {
                    User_id = answerVM.User_id,
                    Question_id = answerVM.Question_id,
                    Option_id = answerVM.Option_id,
                    Point = points
                };

                _myContext.Answers.Add(answer);
            }

            int result = await _myContext.SaveChangesAsync();

            return result > 0;
        }

        public async Task<int> GetTotalScoreByUserId(Guid userId)
        {
            var totalScore = await _myContext.Answers
                .Where(a => a.User_id == userId)
                .SumAsync(a => a.Point ?? 0);

            return totalScore;
        }
        public async Task<List<UserScoreVM>> GetAllUserScores()
        {
            var userScores = await _myContext.Answers
                .GroupBy(a => new { a.User_id, a.User.Fullname, a.User.Email })
                .Select(g => new UserScoreVM
                {
                    UserId = g.Key.User_id,
                    Fullname = g.Key.Fullname,
                    Email = g.Key.Email,
                    TotalScore = g.Sum(a => a.Point ?? 0)*4
                })
                .ToListAsync();

            return userScores;
        }

        public async Task<List<QuestionsVM>> GetRandomQuestionsByLevel()
        {
            var totalQuestions = 25; // Total number of questions to return
            var easyCount = (int)(totalQuestions * 0.4); // 40% easy questions
            var mediumCount = (int)(totalQuestions * 0.4); // 40% medium questions
            var highCount = (int)(totalQuestions * 0.2); // 20% hard questions

            // Fetch questions based on level
            var easyQuestions = await _myContext.Questions
                .Where(q => q.Level == Level.Easy)
                .OrderBy(q => Guid.NewGuid())
                .Take(easyCount)
                .Include(q => q.Options)
                .Select(q => new QuestionsVM
                {
                    Question_id = q.Question_id,
                    Question_text = q.Question_text,
                    ImagePath = q.ImagePath,
                    Options = q.Options.Select(o => new OptionVM
                    {
                        Option_Id = o.Option_id,
                        Option_Text = o.Option_Text,
                        Option_Image = o.Option_Image,
                    }).ToList()
                })
                .ToListAsync();

            var mediumQuestions = await _myContext.Questions
                .Where(q => q.Level == Level.Medium)
                .OrderBy(q => Guid.NewGuid())
                .Take(mediumCount)
                .Include(q => q.Options)
                .Select(q => new QuestionsVM
                {
                    Question_id = q.Question_id,
                    Question_text = q.Question_text,
                    ImagePath = q.ImagePath,
                    Options = q.Options.Select(o => new OptionVM
                    {
                        Option_Id = o.Option_id,
                        Option_Text = o.Option_Text,
                        Option_Image = o.Option_Image,
                    }).ToList()
                })
                .ToListAsync();

            var highQuestions = await _myContext.Questions
                .Where(q => q.Level == Level.Hard)
                .OrderBy(q => Guid.NewGuid())
                .Take(highCount)
                .Include(q => q.Options)
                .Select(q => new QuestionsVM
                {
                    Question_id = q.Question_id,
                    Question_text = q.Question_text,
                    ImagePath = q.ImagePath,
                    Options = q.Options.Select(o => new OptionVM
                    {
                        Option_Id = o.Option_id,
                        Option_Text = o.Option_Text,
                        Option_Image = o.Option_Image,
                    }).ToList()
                })
                .ToListAsync();

            // Fill missing questions if any level doesn't have enough questions
            var totalFetched = easyQuestions.Count + mediumQuestions.Count + highQuestions.Count;

            // If not enough questions are fetched, attempt to fill the gap from other levels
            var remainingQuestionsNeeded = totalQuestions - totalFetched;

            if (remainingQuestionsNeeded > 0)
            {
                // Try to fill from the levels that have fewer questions than their initial count
                var allLevels = new List<IQueryable<Question>> {
            _myContext.Questions.Where(q => q.Level == Level.Easy),
            _myContext.Questions.Where(q => q.Level == Level.Medium),
            _myContext.Questions.Where(q => q.Level == Level.Hard)
        };

                // Exclude already fetched questions from each level
                var alreadyFetchedIds = easyQuestions.Concat(mediumQuestions).Concat(highQuestions)
                                                      .Select(q => q.Question_id).ToList();

                foreach (var levelQuery in allLevels)
                {
                    var remainingQuestions = await levelQuery
                        .Where(q => !alreadyFetchedIds.Contains(q.Question_id))
                        .OrderBy(q => Guid.NewGuid()) // Shuffle
                        .Take(remainingQuestionsNeeded)
                        .Include(q => q.Options)
                        .Select(q => new QuestionsVM
                        {
                            Question_id = q.Question_id,
                            Question_text = q.Question_text,
                            ImagePath = q.ImagePath,
                            Options = q.Options.Select(o => new OptionVM
                            {
                                Option_Id = o.Option_id,
                                Option_Text = o.Option_Text,
                                Option_Image = o.Option_Image,
                            }).ToList()
                        })
                        .ToListAsync();

                    // Add the fetched questions and break if we've filled the gap
                    if (remainingQuestions.Any())
                    {
                        if (easyQuestions.Count < easyCount)
                            easyQuestions.AddRange(remainingQuestions);
                        else if (mediumQuestions.Count < mediumCount)
                            mediumQuestions.AddRange(remainingQuestions);
                        else
                            highQuestions.AddRange(remainingQuestions);

                        remainingQuestionsNeeded -= remainingQuestions.Count;
                    }

                    if (remainingQuestionsNeeded <= 0)
                        break;
                }
            }

            // Combine all questions from all levels
            var allQuestions = easyQuestions.Concat(mediumQuestions).Concat(highQuestions).ToList();

            // If still less than 25, just shuffle and take the first 25 questions
            if (allQuestions.Count < totalQuestions)
            {
                allQuestions = allQuestions.OrderBy(q => Guid.NewGuid()).Take(totalQuestions).ToList();
            }

            return allQuestions;
        }
        public async Task<List<QuestionsVM>> GetAllQuestion()
        {
            var allQuestions = await _myContext.Questions
                .Include(q => q.Options)
                .Select(q => new QuestionsVM
                {
                    Question_id = q.Question_id,
                    Question_text = q.Question_text,
                    ImagePath = q.ImagePath,
                    Options = q.Options.Select(o => new OptionVM
                    {
                        Option_Id = o.Option_id,
                        Option_Text = o.Option_Text,
                        Option_Image = o.Option_Image,
                    }).ToList()
                })
                .ToListAsync();

            return allQuestions;
        }


        public StatisticQuestionVM GetStatisticQuestion()
        {
            var statistics = new StatisticQuestionVM
            {
                TotalQuestion = _myContext.Questions.Count(),
                LevelEasy = _myContext.Questions.Count(q => q.Level == Level.Easy),
                LevelMedium = _myContext.Questions.Count(q => q.Level == Level.Medium),
                LevelHard = _myContext.Questions.Count(q => q.Level == Level.Hard),
                CategoryPemograman = _myContext.Questions.Count(q => q.Category == Category.pemograman),
                CategoryDatabase =  _myContext.Questions.Count(q => q.Category == Category.database),
                CategoryCodingan = _myContext.Questions.Count(q => q.Category == Category.codingan),
                CategoryAnalogi = _myContext.Questions.Count(q => q.Category == Category.analogi),
                CategoryLogika = _myContext.Questions.Count(q => q.Category == Category.logika)
            };

            return statistics;
        }

        public int DeleteQuestion(string questionId)
        {
            try
            {
                var question = _myContext.Questions
                    .Include(q => q.Options) 
                    .FirstOrDefault(q => q.Question_id == questionId);

                if (question == null)
                {
                    return 0; 
                }

                var hasReferences = _myContext.Answers
                    .Any(a => question.Options.Select(o => o.Option_id).Contains(a.Option_id));
                if (hasReferences)
                {
                    return 1; 
                }

                _myContext.Options.RemoveRange(question.Options);

                _myContext.Questions.Remove(question);

                return _myContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting question: {ex.Message}");
                return -1;
            }
        }

    }
}