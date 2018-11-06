using ApplicationCore.Models;

namespace ApplicationCore.Interfaces
{
    public interface IApplicationSettingsService
    {
        void SaveLoggedUserDataInRegistry(string userType, User user);
        User GetLoggedUserDataFromRegistry(string userType);
    }
}
