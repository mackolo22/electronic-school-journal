﻿using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using Infrastructure.Data.AzureStorage.Tables;
using UI.Dialogs;
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
            _unityContainer.RegisterType<ITableStorageRepository, TableStorageRepository>();
            #endregion Repositories

            #region Services
            _unityContainer.RegisterType<ILoginService, LoginService>();
            _unityContainer.RegisterType<IUniqueIDGenerator, UniqueIDGenerator>();
            _unityContainer.RegisterType<IClassService, ClassService>();
            _unityContainer.RegisterType<IPersonService, PersonService>();
            #endregion Services

            #region ViewModels
            _unityContainer.RegisterType<MainViewModel>();
            _unityContainer.RegisterType<HomeViewModel>();
            _unityContainer.RegisterType<AddClassViewModel>();
            _unityContainer.RegisterType<AddStudentViewModel>();
            _unityContainer.RegisterType<AddTeacherViewModel>();
            _unityContainer.RegisterType<AddParentViewModel>();
            _unityContainer.RegisterType<AddLessonViewModel>();
            _unityContainer.RegisterType<AddTermViewModel>();
            #endregion ViewModels
        }

        public static T Resolve<T>()
        {
            T type = _unityContainer.Resolve<T>();
            return type;
        }
    }
}
