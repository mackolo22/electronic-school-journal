﻿using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ApplicationCore.Services
{
    public class ApplicationSettingsService : IApplicationSettingsService
    {
        private const string ApplicationSettingsRegistry = @"SOFTWARE\ElektronicznyDziennikSzkolny";

        public void SaveLoggedUserDataInRegistry(string userType, User user)
        {
            var registryKey = Registry.CurrentUser.CreateSubKey(ApplicationSettingsRegistry);
            string serializedUser = JsonConvert.SerializeObject(user);
            registryKey.SetValue(userType, serializedUser);
            registryKey.Close();
        }

        public User GetLoggedUserDataFromRegistry(string userType)
        {
            User user = null;
            var registryKey = Registry.CurrentUser.OpenSubKey(ApplicationSettingsRegistry);
            if (registryKey != null)
            {
                var value = registryKey.GetValue(userType);
                if (value != null)
                {
                    string serializedUser = value.ToString();
                    if (!String.IsNullOrWhiteSpace(serializedUser))
                    {
                        if (userType == "Administrator")
                        {
                            user = JsonConvert.DeserializeObject<Administrator>(serializedUser);
                        }
                        else if (userType == "Student")
                        {
                            user = JsonConvert.DeserializeObject<Student>(serializedUser);
                        }
                        else if (userType == "Parent")
                        {
                            user = JsonConvert.DeserializeObject<Parent>(serializedUser);
                        }
                        else if (userType == "Teacher")
                        {
                            user = JsonConvert.DeserializeObject<Teacher>(serializedUser);
                        }
                    }
                }
            }

            return user;
        }

        public void SaveTimeTableForUserInRegistry(string userType, List<Lesson> lessons)
        {
            var registryKey = Registry.CurrentUser.CreateSubKey(ApplicationSettingsRegistry);
            string nameOfKey = userType + "TimeTable";
            string serializedLessons = JsonConvert.SerializeObject(lessons);
            registryKey.SetValue(nameOfKey, serializedLessons);
            registryKey.Close();
        }

        public List<Lesson> GetTimeTableForUserFromRegistry(string userType)
        {
            List<Lesson> lessons = new List<Lesson>();

            var registryKey = Registry.CurrentUser.OpenSubKey(ApplicationSettingsRegistry);
            if (registryKey != null)
            {
                string nameOfKey = userType + "TimeTable";
                var value = registryKey.GetValue(nameOfKey);
                if (value != null)
                {
                    string serializedLessons = value.ToString();
                    lessons = JsonConvert.DeserializeObject<List<Lesson>>(serializedLessons);
                }
            }

            return lessons;
        }
    }
}
