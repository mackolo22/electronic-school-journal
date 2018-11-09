using ApplicationCore.Enums;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Helpers;
using UI.Views;

namespace UI.ViewModels
{
    public class MessagesViewModel : BaseViewModel
    {
        private readonly IUsersRepository _usersRepository;
        private readonly LongRunningOperationHelper _longRunningOperationHelper;

        public MessagesViewModel(IUsersRepository usersRepository, LongRunningOperationHelper longRunningOperationHelper)
        {
            _usersRepository = usersRepository;
            _longRunningOperationHelper = longRunningOperationHelper;
            Messages = new List<Message>();
        }

        public User User { get; set; }
        public List<Message> Messages { get; set; }
        public string SelectedFolder { get; set; } = "Received";
        public string SenderReceiver { get; set; }

        public RelayCommand LoadedCommand => new RelayCommand(async (parameter) => await ExecuteLoadedAsync(parameter), () => true);
        private async Task ExecuteLoadedAsync(object parameter)
        {
            SenderReceiver = "Od";
            OnPropertyChanged(nameof(SenderReceiver));
            await ChangeSelectedFolderAsync(SelectedFolder);
        }

        public RelayCommand MessageSelectedCommand => new RelayCommand(ExecuteMessageSelected, () => true);
        private void ExecuteMessageSelected(object parameter)
        {
            Message message = parameter as Message;
            // TODO: wyświetlić dialog z wiadomością
            MessageBoxHelper.ShowMessageBox(message.Subject);
        }

        public RelayCommand CreateNewMessageCommand => new RelayCommand(ExecuteCreateNewMessage, () => true);
        private void ExecuteCreateNewMessage(object parameter)
        {
            var viewModel = new CreateNewMessageViewModel(_usersRepository, _longRunningOperationHelper)
            {
                User = User
            };

            var dialog = new CreateNewMessageDialog(viewModel);
            dialog.ShowDialog();

            if (viewModel.MessageSent)
            {
                MessageBoxHelper.ShowMessageBox("Twoja wiadomość została wysłana.");
            }
        }

        public RelayCommand ChangeSelectedFolderCommand => new RelayCommand(async (parameter) => await ExecuteChangeSelectedFolderAsync(parameter), () => true);
        private async Task ExecuteChangeSelectedFolderAsync(object parameter)
        {
            string folder = parameter as string;
            if (folder == SelectedFolder)
            {
                return;
            }

            await ChangeSelectedFolderAsync(folder);
        }

        private async Task ChangeSelectedFolderAsync(string folder)
        {
            await _longRunningOperationHelper.ProceedLongRunningOperationAsync(async () =>
            {
                SelectedFolder = folder;
                List<Message> allMessages = null;
                var user = await _usersRepository.GetAsync(User.PartitionKey, User.RowKey);
                if (user != null && !String.IsNullOrWhiteSpace(user.SerializedMessages))
                {
                    allMessages = JsonConvert.DeserializeObject<List<Message>>(user.SerializedMessages);
                    if (allMessages.Count == 0)
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }

                if (folder == "Received")
                {
                    Messages = allMessages.Where(x => x.Folder == MessageFolder.Received).ToList();
                    foreach (var message in Messages)
                    {
                        message.UserToDisplay = message.From;
                    }

                    SenderReceiver = "Od";
                }
                else if (folder == "Sent")
                {
                    Messages = allMessages.Where(x => x.Folder == MessageFolder.Sent).ToList();
                    foreach (var message in Messages)
                    {
                        message.UserToDisplay = message.To;
                    }

                    SenderReceiver = "Do";
                }
                else if (folder == "Trash")
                {
                    Messages = allMessages.Where(x => x.Folder == MessageFolder.Trash).ToList();
                    foreach (var message in Messages)
                    {
                        message.UserToDisplay = message.From;
                    }

                    SenderReceiver = "Od";
                }

                OnPropertyChanged(nameof(SenderReceiver));
                OnPropertyChanged(nameof(Messages));
            });
        }
    }
}
