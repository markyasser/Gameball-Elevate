using Gameball_Elevate.Models;
using Microsoft.EntityFrameworkCore;
using Gameball_Elevate.DbContexts;

namespace Gameball_Elevate.Ops
{
    public class UserOps : BasicOps<User>
    {
        public UserOps(GlobalDbContext dbContext) : base(dbContext) { }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await DbSet.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task CreateCustomerAsync(User user)
        {
            Create(user);
            await DbContext.SaveChangesAsync();
        }

        public async Task AddPointsAsync(User user, int points)
        {
            if (user != null)
            {
                user.Points += points;
                Edit(user);
                await DbContext.SaveChangesAsync();
            }
        }

        public async Task<int> GetPointsAsync(User user)
        {
            return user?.Points ?? 0;
        }

        public async Task<bool> RedeemPointsAsync(User user, int points)
        {
            if (user != null && user.Points >= points)
            {
                user.Points -= points;
                Edit(user);
                await DbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<User>> GetUsersWithLessThan100PointsAsync()
        {
            var lessThan100Points = await DbSet.Where(u => u.Points < 100 && u.Email != null).ToListAsync();
            Console.WriteLine($"Found {lessThan100Points.Count} users with less than 100 points.");
            return lessThan100Points;
        }
    }
}
