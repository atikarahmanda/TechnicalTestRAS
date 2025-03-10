using FinalProjectRAS.Context;
using FinalProjectRAS.Models;
using FinalProjectRAS.Repositories.Interface;
using FinalProjectRAS.Services;
using FinalProjectRAS.Utils;
using FinalProjectRAS.ViewModels;
using Latihan_1.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FinalProjectRAS.Repositories
{
    public class UserRepository : IUserRepository
    {
        public readonly MyContext _myContext;
        private readonly IConfiguration _configuration;
        public UserRepository(MyContext myContext, IConfiguration configuration)
        {
            _myContext = myContext;
            _configuration = configuration;
        }
        private static int EMAIL_REGISTERED = -1, NOT_FOUND = -1;
        private static int CORRECT_EMAIL_ONLY = 0;
        private static int CORRECT_EMAIL_PASSWORD = 1;
        private static int FAILED_SEND_EMAIL = -2;

        public (int, UserVM) Login(LoginVM account)
        {
            var emptyUserVM = new UserVM
            {
                UserId = null,
                Email = "",
                Fullname = "",
                Role = "",
                IsCompleted = false,
            };

            //EMAIL PASSWORD SALAH
            var emailMatch = _myContext.Users.FirstOrDefault(u => u.Email == account.Email);
            if (emailMatch == null) return (NOT_FOUND, emptyUserVM);

            //PASSWORD SALAH
            bool isValidPassword = Hashing.ValidatePassword(account.Password, emailMatch.Password);
            if (!isValidPassword) return (CORRECT_EMAIL_ONLY, emptyUserVM);

            //AMBIL DATA USER
            var selectedUser = _myContext.Users
                .Include(u => u.Request) 
                .FirstOrDefault(u => u.Email == account.Email);

            //KADALUARSA
            //if (selectedUser?.Deadline < DateTime.UtcNow) return (EXPIRED_USER, emptyUserVM);
            var userCompleted = selectedUser?.Request?.Status_test;
            var userDeadline = !(userCompleted ?? false) && (selectedUser?.Deadline < DateTime.UtcNow);
            var userVM = new UserVM
            {
                UserId = selectedUser?.User_id,
                Email = selectedUser?.Email,
                Fullname = selectedUser?.Fullname,
                Role = selectedUser?.Role.ToString(),
                IsCompleted = userCompleted,
                Deadline = userDeadline
            };
            return (CORRECT_EMAIL_PASSWORD, userVM);
        }

        public int Register(RegisterVM registerVM)
        {
            if (_myContext.Users.Any(u => u.Email == registerVM.Email)) return EMAIL_REGISTERED;

            var updatedUserId = Guid.NewGuid();
            string randomPassword = PasswordGenerator.GeneratePassword();
            //string randomPassword = "12345";
            string password = Hashing.HashPassword(randomPassword);
            var deadline = DateTime.Now.AddDays(7);

            // Membuat transaksi untuk memastikan data hanya disimpan setelah email berhasil dikirim
            using (var transaction = _myContext.Database.BeginTransaction())
            {
                try
                {
                    // Membuat user baru
                    var user = new User
                    {
                        User_id = updatedUserId,
                        Email = registerVM.Email,
                        Password = password,
                        Fullname = registerVM.FirstName + " " + registerVM.LastName,
                        Role = (Role?)1,
                        Created_at = DateTime.Now,
                        Deadline = deadline,
                    };
                    _myContext.Users.Add(user);

                    // Membuat request untuk user
                    var request = new Request
                    {
                        User_id = updatedUserId,
                        Status_test = false,
                    };
                    _myContext.Requests.Add(request);

                    // Membuat result untuk user
                    var result = new Result
                    {
                        User_id = updatedUserId,
                        Score = 0,
                    };
                    _myContext.Results.Add(result);

                    // Mengirim email
                    string formattedDeadline = deadline.ToString("dd MMMM yyyy") + " pukul 23.59 WIB";
                    var data = new DataEmailVM
                    {
                        Email = registerVM.Email,
                        Password = randomPassword,
                        Deadline = formattedDeadline,
                        Url = "http://localhost:5173/"
                    };

                    var emailService = new EmailService(_configuration);
                    emailService.SendEmail(registerVM.Email, "Undangan Technical Test Online", data);

                    // Jika email berhasil dikirim, simpan perubahan ke database
                    int changes = _myContext.SaveChanges();

                    // Jika semuanya berhasil, commit transaksi
                    transaction.Commit();

                    return changes;
                }
                catch (Exception ex)
                {
                    // Jika ada kesalahan, rollback transaksi dan kembalikan -1
                    transaction.Rollback();
                    return FAILED_SEND_EMAIL;
                }
            }
        }


        public IEnumerable<CandidateVM> GetCandidateData()
        {
            var details = _myContext.Users
                .Include(e => e.Request)
                .Where(e => e.Role == Role.Candidate)
                .OrderByDescending(e => e.Created_at)
                .Select(e => new CandidateVM
                {
                    User_id = e.User_id,
                    Email = e.Email,
                    Fullname = e.Fullname,
                    Deadline = e.Deadline,
                    StatusTest = e.Request.Status_test
                })
                .ToList();

            return details;
        }


        public IEnumerable<CandidateVM> GetCandidatePastDeadline()
        {
            var currentDate = DateTime.Now.Date;

            var details = _myContext.Users
                .Include(e => e.Request)
                .Where(e => e.Role == Role.Candidate && e.Deadline <= currentDate && e.Request.Status_test == false)
                .Select(e => new CandidateVM
                {
                    User_id = e.User_id,
                    Email = e.Email,
                    Fullname = e.Fullname,
                    Deadline = e.Deadline,
                    StatusTest = e.Request.Status_test
                })
                .ToList();

            return details;
        }

        public User GetUserById(Guid User_id)
        {
            return _myContext.Users.Find(User_id);
        }


        public int UpdateDeadline(Guid User_id, int newDeadlineDays)
        {
            var user = GetUserById(User_id);
            if (user == null)
            {
                return 0;
            }
            user.Deadline = user.Deadline?.AddDays(newDeadlineDays);
            _myContext.Users.Update(user);
            _myContext.SaveChanges();
            return 1;
        }

        public int DeleteUser(Guid userId)
        {
            var user = _myContext.Users.Include(u => u.Request)
                                       .Include(u => u.Result)
                                       .FirstOrDefault(u => u.User_id == userId);

            if (user == null)
            {
                return 0;
            }

            var answers = _myContext.Answers.Where(a => a.User_id == userId);
            _myContext.Answers.RemoveRange(answers);
            _myContext.Requests.Remove(user.Request);
            _myContext.Results.Remove(user.Result);
            _myContext.Users.Remove(user);
            _myContext.SaveChanges();
            return 1;
        }

        public bool UpdateRequestStatus(Guid userId, bool newStatus)
        {
            var request = _myContext.Requests.Find(userId);
            if (request == null)
            {
                return false;
            }

            request.Status_test = newStatus;

            _myContext.SaveChanges();
            return true;
        }

        public StatisticVM GetStatisticCandidate()
        {
            int totalCandidate = _myContext.Users.Count(u => u.Role == Role.Candidate);

            int totalComplete = _myContext.Users
                .Where(u => u.Role == Role.Candidate && u.Request != null && u.Request.Status_test == true)
                .Count();

            int totalIncomplete = _myContext.Users
                .Where(u => u.Role == Role.Candidate && u.Request != null && u.Request.Status_test == false)
                .Count();

            return new StatisticVM
            {
                TotalCandidate = totalCandidate,
                TotalComplete = totalComplete,
                TotalUncomplete = totalIncomplete
            };
        }
    }
}
