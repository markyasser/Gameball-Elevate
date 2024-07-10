using Gameball_Elevate.Models;
using Gameball_Elevate.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Gameball_Elevate.Ops
{
    public class TransactionOps : BasicOps<Transaction>
    {
        public TransactionOps(GlobalDbContext dbContext) : base(dbContext) { }

        public async Task<List<Transaction>> GetUserTransactions(string userId)
        {
            return await DbSet.Where(t => t.UserId == userId).ToListAsync();
        }
        public async Task<bool> AddTransactionAsync(Transaction transaction)
        {
            try
            {

                DbSet.Add(transaction);
                await DbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
