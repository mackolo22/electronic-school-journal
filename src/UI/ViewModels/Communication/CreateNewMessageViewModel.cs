using ApplicationCore.Enums;
using ApplicationCore.Extensions;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private bool _addresseeSelected;

        public CreateNewMessageViewModel(IUsersRepository usersRepository, LongRunningOperationHelper longRunningOperationHelper)
        {
            _usersRepository = usersRepository;
            _longRunningOperationHelper = longRunningOperationHelper;
        }

        public User User { get; set; }
        public List<User> AllUsers { get; set; }
        public string To
        {
            get => _to;
            set
            {
                _to = value;
                if (_addresseeSelected && _selectedAddressee.FullName == _to)
                {
                    OnPropertyChanged(nameof(To));
                    return;
                }
                else if (_addresseeSelected && _selectedAddressee.FullName != _to)
                {
                    _addresseeSelected = false;
                }

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
                    _addresseeSelected = true;
                    To = value.FullName;
                    OnPropertyChanged(nameof(MatchingAddressees));
                }
                else
                {
                    _addresseeSelected = false;
                }
            }
        }

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

        public RelayCommand CancelCommand => new RelayCommand(ExecuteCancel, () => true);
        private void ExecuteCancel(object parameter)
        {
            _window.Close();
        }

        public RelayCommand SendMessageCommand => new RelayCommand(async (parameter) => await ExecuteSendMessageAsync(parameter), () => true);
        private async Task ExecuteSendMessageAsync(object parameter)
        {
            // TODO: dodać możliwość wyboru wielu userów i np. całej klasy
            // i rozbić to na mniejsze metody
            await _longRunningOperationHelper.ProceedLongRunningOperationAsync(async () =>
            {
                if (_selectedAddressee != null)
                {
                    var today = DateTime.Now;
                    string dateAndTime = $"{today.ToShortDateString()}, g. {today.ToShortTimeString()}";
                    Message message = new Message()
                    {
                        From = User.FullName,
                        To = _selectedAddressee.FullName,
                        Subject = Subject,
                        DateAndTime = dateAndTime,
                        Content = Content,
                        Folder = MessageFolder.Received
                    };

                    var receiver = await _usersRepository.GetAsync(_selectedAddressee.PartitionKey, _selectedAddressee.RowKey);
                    if (receiver != null)
                    {
                        if (String.IsNullOrWhiteSpace(receiver.SerializedMessages))
                        {
                            receiver.Messages = new List<Message>
                            {
                                message
                            };
                        }
                        else
                        {
                            receiver.Messages = JsonConvert.DeserializeObject<List<Message>>(receiver.SerializedMessages);
                            receiver.Messages.Add(message);
                        }

                        receiver.SerializedMessages = JsonConvert.SerializeObject(receiver.Messages);
                        await _usersRepository.InsertOrReplaceAsync(receiver);

                        var sender = await _usersRepository.GetAsync(User.PartitionKey, User.RowKey);
                        if (sender != null)
                        {
                            message.Folder = MessageFolder.Sent;
                            if (String.IsNullOrWhiteSpace(sender.SerializedMessages))
                            {
                                sender.Messages = new List<Message>
                                {
                                    message
                                };
                            }
                            else
                            {
                                sender.Messages = JsonConvert.DeserializeObject<List<Message>>(sender.SerializedMessages);
                                sender.Messages.Add(message);
                            }

                            sender.SerializedMessages = JsonConvert.SerializeObject(sender.Messages);
                            await _usersRepository.InsertOrReplaceAsync(sender);
                        }

                        MessageSent = true;
                        _window.Close();
                    }
                }
                else
                {
                    MessageBoxHelper.ShowErrorMessageBox("Musisz wskazać adresata wiadomości.");
                }
            });
        }
    }
}
