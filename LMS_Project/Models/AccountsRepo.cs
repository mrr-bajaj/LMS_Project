using System.Linq;

namespace LMS_Project.Models
{
    public class AccountsRepo : IAccountsRepo
    {
        private readonly LMS_ProjectContext _lms_projectcontext;
        public AccountsRepo(LMS_ProjectContext lms_projectcontext)
        {
            _lms_projectcontext = lms_projectcontext;
        }
        public Account GetUserByName(string username)
        {
            var test = _lms_projectcontext.Accounts.FirstOrDefault(u => u.UserName == username);
            System.Console.WriteLine(test);
            return _lms_projectcontext.Accounts.FirstOrDefault(u => u.UserName == username);
        }
    }
}
