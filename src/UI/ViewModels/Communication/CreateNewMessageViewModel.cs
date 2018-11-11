using ApplicationCore.Enums;
using ApplicationCore.Extensions;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UI.Helpers;

namespace UI.ViewModels
{
    public class CreateNewMessageViewModel : BaseViewModel
    {
        private readonly IUsersRepository _usersRepository;
        private readonly LongRunningOperationHelper _longRunningOperationHelper;
        private Window _window;
        private string _to;
        private bool _showMatchingAddressees;
        private User _selectedAddressee;

        public CreateNewMessageViewModel(IUsersRepository usersRepository, LongRunningOperationHelper longRunningOperationHelper)
        {
            _usersRepository = usersRepository;
            _longRunningOperationHelper = longRunningOperationHelper;
            SelectedAddressees = new ObservableCollection<User>();
        }

        public User User { get; set; }
        public List<User> AllUsers { get; set; }
        public string To
        {
            get => _to;
            set
            {
                _to = value;
                if (value.Length >= 1)
                {
                    MatchingAddressees = AllUsers.Where(x =>
                    {
                        bool matchFound = FindMatchingUserByName(x.FirstName, x.LastName, x.FullName, value);
                        return matchFound;
                    })
                    .ToList();

                    if (MatchingAddressees != null && MatchingAddressees.Count > 0)
                    {
                        OnPropertyChanged(nameof(MatchingAddressees));
                        SelectedAddressee = null;
                        ShowMatchingAddressees = true;
                    }
                    else
                    {
                        SelectedAddressee = null;
                        ShowMatchingAddressees = false;
                    }
                }
                else
                {
                    SelectedAddressee = null;
                    ShowMatchingAddressees = false;
                }
            }
        }

        private bool FindMatchingUserByName(string firstName, string lastName, string fullName, string inputName)
        {
            bool matchFound =
                CheckIfStringContainsAnotherString(firstName, inputName) ||
                CheckIfStringContainsAnotherString(lastName, inputName) ||
                CheckIfStringContainsAnotherString(fullName, inputName);

            return matchFound;
        }

        private bool CheckIfStringContainsAnotherString(string firstString, string secondString)
        {
            firstString = firstString.ToLower().RemoveDiacritics();
            secondString = secondString.ToLower().RemoveDiacritics();
            bool contains = firstString.Contains(secondString);
            return contains;
        }

        public List<User> MatchingAddressees { get; set; }
        public bool ShowMatchingAddressees
        {
            get => _showMatchingAddressees;
            set
            {
                _showMatchingAddressees = value;
                OnPropertyChanged(nameof(ShowMatchingAddressees));
            }
        }

        public User SelectedAddressee
        {
            get => _selectedAddressee;
            set
            {
                _selectedAddressee = value;
                OnPropertyChanged(nameof(SelectedAddressee));
                if (value != null)
                {
                    ShowMatchingAddressees = false;
                    MatchingAddressees = null;
                    To = String.Empty;
                    AllUsers.Remove(value);
                    SelectedAddressees.Add(value);
                    OnPropertyChanged(nameof(To));
                    OnPropertyChanged(nameof(MatchingAddressees));
                    OnPropertyChanged(nameof(SelectedAddressees));
                }
            }
        }

        public ObservableCollection<User> SelectedAddressees { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public bool MessageSent { get; set; }

        public RelayCommand LoadedCommand => new RelayCommand(async (parameter) => await ExecuteLoadedAsync(parameter), () => true);
        private async Task ExecuteLoadedAsync(object parameter)
        {
            _window = parameter as Window;

            await _longRunningOperationHelper.ProceedLongRunningOperationAsync(_window, async () =>
            {
                AllUsers = new List<User>();
                var administrators = await _usersRepository.GetAllAsync(nameof(Administrator));
                var students = await _usersRepository.GetAllAsync(nameof(Student));
                var teachers = await _usersRepository.GetAllAsync(nameof(Teacher));
                var parents = await _usersRepository.GetAllAsync(nameof(Parent));
                AllUsers.AddRange(administrators);
                AllUsers.AddRange(students);
                AllUsers.AddRange(teachers);
                AllUsers.AddRange(parents);
                AllUsers.RemoveAll(x => x.Id == User.Id);
            });
        }

        public RelayCommand RemoveAddresseeCommand => new RelayCommand(ExecuteRemoveAddressee, () => true);
        private void ExecuteRemoveAddressee(object parameter)
        {
            User addressee = parameter as User;
            SelectedAddressees.Remove(addressee);
            OnPropertyChanged(nameof(SelectedAddressees));
            AllUsers.Add(addressee);
        }

        public RelayCommand SendMessageCommand => new RelayCommand(async (parameter) => await ExecuteSendMessageAsync(parameter), () => true);
        private async Task ExecuteSendMessageAsync(object parameter)
        {
            await _longRunningOperationHelper.ProceedLongRunningOperationAsync(_window, async () =>
            {
                if (SelectedAddressees.Count > 0)
                {
                    var today = DateTime.Now;
                    string dateAndTime = $"{today.ToShortDateString()}, g. {today.ToShortTimeString()}";
                    Message message = new Message()
                    {
                        From = User.FullName,
                        Subject = Subject,
                        DateAndTime = dateAndTime,
                        Content = Content,
                        Folder = MessageFolder.Received
                    };

                    foreach (var addreessee in SelectedAddressees)
                    {
                        message.To += $"{addreessee.FullName} ";
                        await AddMessageToFolderForGivenUserAsync(addreessee, message);
                    }

                    string addressees = message.To;
                    int index = addressees.Count() - 1;
                    message.To = addressees.Remove(index, 1);
                    message.Folder = MessageFolder.Sent;
                    await AddMessageToFolderForGivenUserAsync(User, message);

                    MessageSent = true;
                    _window.Close();
                }
                else
                {
                    MessageBoxHelper.ShowErrorMessageBox("Musisz wskazać adresata wiadomości.");
                }
            });
        }

        private async Task AddMessageToFolderForGivenUserAsync(User user, Message message)
        {
            var userToUpdate = await _usersRepository.GetAsync(user.PartitionKey, user.RowKey);
            if (userToUpdate != null)
            {
                if (String.IsNullOrWhiteSpace(userToUpdate.SerializedMessages))
                {
                    userToUpdate.Messages = new List<Message>
                    {
                        message
                    };
                }
                else
                {
                    userToUpdate.Messages = JsonConvert.DeserializeObject<List<Message>>(userToUpdate.SerializedMessages);
                    userToUpdate.Messages.Add(message);
                }

                userToUpdate.SerializedMessages = JsonConvert.SerializeObject(userToUpdate.Messages);
                await _usersRepository.InsertOrReplaceAsync(userToUpdate);
            }
        }

        public RelayCommand CancelCommand => new RelayCommand(ExecuteCancel, () => true);
        private void ExecuteCancel(object parameter)
        {
            _window.Close();
        }
    }
}
