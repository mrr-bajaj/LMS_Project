using System.Linq;

namespace LMS_Project.Models
{
    public class LoginRepo : ILoginRepo
    {
        private readonly LMS_ProjectContext _context;
        public LoginRepo(LMS_ProjectContext context)
        {
            _context = context;
        }

        public Account getUserByName(string username) { 
        return _context.Accounts.FirstOrDefault(u => u.UserName == username);
        }
    }
}
