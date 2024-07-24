using Gameball_Elevate.Utils;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Gameball_Elevate.Models
{
    public class User : IdentityUser
    {
        public int Points { get; set; }
    }
}
