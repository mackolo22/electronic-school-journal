using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using Infrastructure.Data.AzureStorage;
using Infrastructure.Data.AzureStorage.Tables;
using UI.Helpers;
using UI.ViewModels;
using Unity;

namespace UI
{
    public static class UnityConfiguration
    {
        private static UnityContainer _unityContainer;

        public static void ConfigureUnityContainer()
        {
            _unityContainer = new UnityContainer();

            #region Repositories
            _unityContainer.RegisterType<IUsersRepository, UsersRepository>();
            _unityContainer.RegisterType<IClassesRepository, ClassesRepository>();
            #endregion Repositories

            #region Services
            _unityContainer.RegisterType<IApplicationSettingsService, ApplicationSettingsService>();
            _unityContainer.RegisterType<ILoginService, LoginService>();
            _unityContainer.RegisterSingleton<IUniqueIDGenerator, UniqueIDGenerator>();
            _unityContainer.RegisterType<ITimeTableService, TimeTableService>();
            _unityContainer.RegisterType<IMailingService, MailingService>();
            #endregion Services

            #region ViewModels
            _unityContainer.RegisterType<MainViewModel>();
            _unityContainer.RegisterType<LoginFirstStepViewModel>();
            _unityContainer.RegisterType<LoginSecondStepViewModel>();
            _unityContainer.RegisterType<RecoverPasswordViewModel>();
            _unityContainer.RegisterType<HomeViewModel>();
            _unityContainer.RegisterType<SettingsViewModel>();
            _unityContainer.RegisterType<CreateClassViewModel>();
            _unityContainer.RegisterType<TeachersViewModel>();
            _unityContainer.RegisterType<ManageTimeTablesViewModel>();
            _unityContainer.RegisterType<TimeTableViewModel>();
            _unityContainer.RegisterType<ClassGradesViewModel>();
            _unityContainer.RegisterType<AddGradeViewModel>();
            _unityContainer.RegisterType<StudentGradesViewModel>();
            _unityContainer.RegisterType<ShowGradeViewModel>();
            _unityContainer.RegisterType<ClassFrequencyViewModel>();
            _unityContainer.RegisterType<StudentFrequencyViewModel>();
            _unityContainer.RegisterType<MessagesViewModel>();
            _unityContainer.RegisterType<CreateNewMessageViewModel>();
            #endregion ViewModels

            #region Helpers
            _unityContainer.RegisterSingleton<AzureStorageHelper>();
            _unityContainer.RegisterType<LongRunningOperationHelper>();
            #endregion Helpers
        }

        public static T Resolve<T>()
        {
            T type = _unityContainer.Resolve<T>();
            return type;
        }
    }
}
