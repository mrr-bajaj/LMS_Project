using System.Linq;

namespace LMS_Project.Models
{
    public class BookRepo : IBookRepo
    {
        private readonly LMS_ProjectContext _lms_projectcontext;
        public BookRepo(LMS_ProjectContext lms_projectcontext)
        {
            _lms_projectcontext = lms_projectcontext;
        }
        public Book GetBookById(int bookId)
        {
            return _lms_projectcontext.Books.FirstOrDefault(b => b.BookId == bookId);
        }
    }
}
