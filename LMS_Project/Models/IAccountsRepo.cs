namespace LMS_Project.Models
{
    public interface IAccountsRepo
    {
        Account GetUserByName(string username);
    }
}
