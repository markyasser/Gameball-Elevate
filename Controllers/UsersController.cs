using Gameball_Elevate.Models;
using Gameball_Elevate.Ops;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using ZiggyCreatures.Caching.Fusion;

namespace Gameball_Elevate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserOps _userRepository;
        private readonly TransactionOps _transactionRepository;
        private readonly IFusionCache _cache; // Caching Using FusionCache
        private readonly ILogger<UsersController> _logger;
        public UsersController(UserOps userRepository, TransactionOps transactionRepository, IFusionCache cache, ILogger<UsersController> logger)
        {
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
            _cache = cache;
            _logger = logger;
        }


        // Get All Users
        // GET api/Users
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var stopwatch = Stopwatch.StartNew(); // Start measuring time

            // Define a cache key
            var cacheKey = "all_users";

            // Use FusionCache to get or set the cached data
            var users = _cache.GetOrSet<IEnumerable<User>>(cacheKey, 
                _ => _userRepository.GetAll(),
                TimeSpan.FromMinutes(5) // 5 minutes
            );

            stopwatch.Stop(); // Stop measuring time
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            // Log the response time
            _logger.LogInformation("GetUsers response time: {ElapsedMilliseconds} ms", elapsedMilliseconds);

            return Ok(users);
        }
        // Local function to fetch users
        private async Task<IEnumerable<User>> FetchUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        // Check the balance of any user
        // PUT api/Users/AddPoints/{id}
        [HttpPut("Points/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CheckUserPoints(string id)
        {
            var user = await _userRepository.GetbyIdAsync(id);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            var points = await _userRepository.GetPointsAsync(user);
            return Ok("User '" + user.UserName + "' has "+ points + " remaining points");
        }

        // Add points to any user
        // PUT api/Users/AddPoints/{id}
        [HttpPut("AddPoints/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddPoints(string id, [FromBody] int points)
        {
            var user = await _userRepository.GetbyIdAsync(id);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            await _userRepository.AddPointsAsync(user, points);
            await _transactionRepository.AddTransactionAsync(new Transaction
            {
                UserId = id,
                Points = points,
                CurrentBalance = user.Points,
                CreatedAt = DateTime.UtcNow,
                type = TransactionType.Added
            });

            return Ok("Added " + points + " points for user '" + user.UserName + "'");
        }


        // Check the balance of the current user
        // GET api/Users/MyPoints
        [HttpGet("MyPoints")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> MyPoints()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value; // Get the current user id
            var user = await _userRepository.GetbyIdAsync(userId);
            var points = await _userRepository.GetPointsAsync(user);
            return Ok(points);
        }

        // Buy products to earn points
        // PUT api/Users/BuyProducts
        [HttpPut("BuyProducts")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> BuyProducts([FromBody] int productPrice)
        {
            int points = (int)Math.Round(productPrice * 0.2); // 20% of the product price
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _userRepository.GetbyIdAsync(userId);
            await _userRepository.AddPointsAsync(user, points);
            await _transactionRepository.AddTransactionAsync(new Transaction
            {
                UserId = userId,
                Points = points,
                CurrentBalance = user.Points,
                type = TransactionType.Added,
                CreatedAt = DateTime.UtcNow,
            });

            return Ok("Loyal client, Added " + points + " points");
        }


        // Redeem points from the current user
        // PUT api/Users/RedeemPoints
        [HttpPut("RedeemPoints")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> RedeemPoints([FromBody] int points)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _userRepository.GetbyIdAsync(userId);
            var redeemSuccess = await _userRepository.RedeemPointsAsync(user, points);

            if (!redeemSuccess)
            {
                return BadRequest("Insufficient points, try buying new products to get points");
            }

            var transactionAddSuccess = await _transactionRepository.AddTransactionAsync(new Transaction
            {
                UserId = userId,
                Points = points,
                CurrentBalance = user.Points,
                type = TransactionType.Redeemed,
                CreatedAt = DateTime.UtcNow,
            });

            if (!transactionAddSuccess)
            {
                return BadRequest("Error adding new transaction");
            }

            return Ok("Redeemed "+ points + " points");
        }
    }
}
