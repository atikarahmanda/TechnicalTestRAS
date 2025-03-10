using FinalProjectRAS.Context;
using FinalProjectRAS.Models;
using FinalProjectRAS.Repositories.Interface;
using FinalProjectRAS.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FinalProjectRAS.Repositories
{
    public class UploadExcelRepository : IUploadExcelRepository
    {
        private readonly MyContext _myContext;

        // Perbaikan path relatif untuk folder Question dan Option
        private readonly string _questionImageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), @"..\client\src\assets\images\Question");
        private readonly string _optionImageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), @"..\client\src\assets\images\Option");

        public UploadExcelRepository(MyContext myContext)
        {
            _myContext = myContext;

            // Memastikan folder Question ada
            if (!Directory.Exists(_questionImageFolderPath))
            {
                Console.WriteLine("Folder gambar pertanyaan tidak ditemukan, membuat folder...");
                Directory.CreateDirectory(_questionImageFolderPath);
            }
            else
            {
                Console.WriteLine("Folder gambar pertanyaan ditemukan.");
            }

            // Memastikan folder Option ada
            if (!Directory.Exists(_optionImageFolderPath))
            {
                Console.WriteLine("Folder gambar opsi tidak ditemukan, membuat folder...");
                Directory.CreateDirectory(_optionImageFolderPath);
            }
        }

        public (string Message, int SavedQuestionsCount) SaveQuestionsFromExcel(Stream fileStream)
        {
            var questions = new List<Question>();

            var lastQuestionId = _myContext.Questions.OrderByDescending(q => q.Question_id).FirstOrDefault()?.Question_id;
            var lastOptionId = _myContext.Options.OrderByDescending(o => o.Option_id).FirstOrDefault()?.Option_id;

            int nextQuestionNumber = lastQuestionId == null ? 1 : int.Parse(lastQuestionId.Substring(1)) + 1;
            int nextOptionNumber = lastOptionId == null ? 1 : int.Parse(lastOptionId.Substring(1)) + 1;

            using (var package = new ExcelPackage(fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var questionText = worksheet.Cells[row, 1].Value?.ToString();
                    if (string.IsNullOrWhiteSpace(questionText))
                    {
                        return ($"Question text is missing in row {row}", 0);
                    }

                    var options = new List<Option>();
                    for (int col = 3; col <= 6; col++)
                    {
                        string optionText = worksheet.Cells[row, col].Value?.ToString();
                        string optionImagePath = null;

                        // Cek apakah ada gambar pada sel tertentu
                        foreach (var drawing in worksheet.Drawings)
                        {
                            if (drawing is ExcelPicture picture && picture.From.Row + 1 == row && picture.From.Column + 1 == col)
                            {
                                var fileName = $"{Guid.NewGuid()}.png";
                                var fullPath = Path.Combine(_optionImageFolderPath, fileName);

                                using (var imageFileStream = new FileStream(fullPath, FileMode.Create))
                                {
                                    imageFileStream.Write(picture.Image.ImageBytes, 0, picture.Image.ImageBytes.Length);
                                }

                                // Menyimpan path gambar
                                optionImagePath = Path.Combine(@"images\Option", fileName);
                                break;
                            }
                        }

                        // Menambahkan opsi
                        options.Add(new Option
                        {
                            Option_id = $"O{nextOptionNumber:D4}",
                            Option_Text = optionImagePath == null ? optionText : null,
                            Option_Image = optionImagePath ?? null,
                            Is_Correct = worksheet.Cells[row, 7].Value?.ToString() == $"opsi {col - 2}"
                        });

                        nextOptionNumber++;
                    }

                    // Parsing kategori dan level
                    var categoryStr = worksheet.Cells[row, 8].Value?.ToString();
                    if (string.IsNullOrWhiteSpace(categoryStr) || !Enum.TryParse(categoryStr, true, out Category parsedCategory))
                    {
                        return ($"Category is missing or invalid in row {row}", 0);
                    }

                    var levelStr = worksheet.Cells[row, 9].Value?.ToString();
                    if (string.IsNullOrWhiteSpace(levelStr) || !Enum.TryParse(levelStr, true, out Level parsedLevel))
                    {
                        return ($"Level is missing or invalid in row {row}", 0);
                    }

                    string questionImagePath = null;
                    foreach (var drawing in worksheet.Drawings)
                    {
                        if (drawing is ExcelPicture picture && picture.From.Row + 1 == row && picture.From.Column + 1 == 2)
                        {
                            var fileName = $"{Guid.NewGuid()}.png";
                            var fullPath = Path.Combine(_questionImageFolderPath, fileName);

                            using (var imageFileStream = new FileStream(fullPath, FileMode.Create))
                            {
                                imageFileStream.Write(picture.Image.ImageBytes, 0, picture.Image.ImageBytes.Length);
                            }

                            //questionImagePath = Path.Combine(@"images\Question", fileName);
                            questionImagePath = Path.Combine(@"images\Question", fileName);
                            break;
                        }
                    }

                    // Menyimpan pertanyaan
                    var question = new Question
                    {
                        Question_id = $"Q{nextQuestionNumber:D4}",
                        Question_text = questionText,
                        Category = parsedCategory,
                        Level = parsedLevel,
                        ImagePath = questionImagePath,
                        Options = options
                    };

                    foreach (var option in options)
                    {
                        option.Question_id = question.Question_id;
                    }

                    questions.Add(question);
                    nextQuestionNumber++;
                }
            }

            _myContext.Questions.AddRange(questions);
            _myContext.SaveChanges();
            Console.WriteLine("Data berhasil disimpan ke database.");

            return ("Questions successfully uploaded", questions.Count);
        }
    }
}
