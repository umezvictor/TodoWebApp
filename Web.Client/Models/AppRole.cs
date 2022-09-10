using Microsoft.AspNetCore.Identity;

namespace Web.Client.Models
{

    public class AppRole : IdentityRole<Guid>
    {
        public AppRole()
        {
            Id = Guid.NewGuid();
            ConcurrencyStamp = Guid.NewGuid().ToString("N");
        }


    }
}
