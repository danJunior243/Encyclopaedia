using Encyclopaedia.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace Encyclopaedia.Core.Entities
{
    public class ApplicationUser: IdentityUser<int>
    {
         public string DisplayName { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Reader;
        public bool IsActive { get; set; } = true;

         public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        

    }
}
