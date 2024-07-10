using Gameball_Elevate.Models;
using Gameball_Elevate.Ops;
using Gameball_Elevate.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Gameball_Elevate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly UserOps _userRepository;
        private readonly TransactionOps _transactionRepository;
        public TransactionsController(TransactionOps transactionRepository, UserOps userRepository)
        {
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
        }


        // Get All Transactions
        // GET api/Transactions
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTransactions()
        {
            var transactions = await _transactionRepository.GetAllAsync();
            return Ok(transactions);
        }
        // Get the transactions of any user
        // GET api/Transactions/{id}
        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserTransactions(string userId)
        {
            var user = await _userRepository.GetbyIdAsync(userId);
            if (user == null)
            {
                return BadRequest("User not found.");
            }
            try
            {
                var transactions = await _transactionRepository.GetUserTransactions(userId);
                var transactionDtos = await GetTransactionsDto(transactions, user);
                return Ok(transactionDtos);
            }
            catch
            {
                return BadRequest("Error fetching transactions.");
            }
        }


        private async Task<List<TransactionDto>> GetTransactionsDto(List<Transaction> transactions, User user)
        {
            // return TransactionDto where points in the transaction are displayed as a string if the transaction is of type "Added" and as a negative number if the transaction is of type "Redeemed"
            List<TransactionDto> transactionDtos = new List<TransactionDto>();
            foreach (var transaction in transactions)
            {
                var transactionDto = new TransactionDto
                {
                    Username = user.UserName,
                    Points = transaction.type == TransactionType.Added ? '+' + transaction.Points.ToString() : '-' + transaction.Points.ToString(),
                    CurrentBalance = transaction.CurrentBalance,
                    CreatedAt = transaction.CreatedAt
                };
                transactionDtos.Add(transactionDto);
            }
            return transactionDtos;
        }
    }
}
