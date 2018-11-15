using System.Collections.Generic;
using ApplicationCore.Models;

namespace ApplicationCore.Interfaces
{
    public interface IApplicationSettingsService
    {
        void SaveLoggedUserDataInRegistry(string userType, User user);
        User GetLoggedUserDataFromRegistry(string userType);
        void SaveTimeTableForUserInRegistry(string userType, List<Lesson> lessons);
        List<Lesson> GetTimeTableForUserFromRegistry(string userType);
    }
}
