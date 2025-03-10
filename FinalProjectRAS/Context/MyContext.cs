using FinalProjectRAS.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectRAS.Context
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options) { }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<User> Users { get; set; }

        // event (kek trigger) jadi setiap answer masuk bakal di total, terus kalau udah 25, maka status akan selesai
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var newAnswers = ChangeTracker.Entries<Answer>()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity)
                .ToList();

            foreach (var answer in newAnswers)
            {
                var userResult = await Results
                    .FirstOrDefaultAsync(r => r.User_id == answer.User_id, cancellationToken);

                if (userResult != null)
                {
                    userResult.Score = (userResult.Score ?? 0) + (answer.Point ?? 0);
                    Results.Update(userResult);
                }
                else
                {
                    var newResult = new Result
                    {
                        User_id = answer.User_id,
                        Score = answer.Point
                    };
                    Results.Add(newResult);
                }

                int answerCount = await Answers
                    .CountAsync(a => a.User_id == answer.User_id, cancellationToken);

                if (answerCount == 25)
                {
                    var userRequest = await Requests
                        .FirstOrDefaultAsync(r => r.User_id == answer.User_id, cancellationToken);

                    if (userRequest != null)
                    {
                        userRequest.Status_test = true;
                        Requests.Update(userRequest);
                    }
                    else
                    {
                        var newRequest = new Request
                        {
                            User_id = answer.User_id,
                            Status_test = true
                        };
                        Requests.Add(newRequest);
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
