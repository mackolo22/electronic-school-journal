namespace ApplicationCore.Interfaces
{
    public interface ILoginService
    {
        string GenerateLogin(string firstName, string lastName);
        string GeneratePassword(int length = 8);
    }
}
