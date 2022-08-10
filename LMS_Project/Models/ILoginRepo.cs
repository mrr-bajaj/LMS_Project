namespace LMS_Project.Models
{
    public interface ILoginRepo
    {
        Account getUserByName(string userName);
    }
}
