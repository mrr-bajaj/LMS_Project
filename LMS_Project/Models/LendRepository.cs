using System.Linq;
namespace LMS_Project.Models
{
    public class LendRepository : ILendRepository
    {
        private readonly LMS_ProjectContext _librarycontext;
        public LendRepository(LMS_ProjectContext librarycontext) { 
        _librarycontext = librarycontext;
        }
        public LendRequest GetLendRequest(int id)
        {

            return _librarycontext.LendRequests.FirstOrDefault(s => s.LendId == id);
        }
    }
}
