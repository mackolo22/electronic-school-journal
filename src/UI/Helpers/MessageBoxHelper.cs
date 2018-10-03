using System;
using System.Windows;
using UI.ViewModels;

namespace UI.Helpers
{
    public static class MessageBoxHelper
    {
        public static void ShowMessageBox(string caption, Action okAction, string buttonOkCaption = "OK")
        {
            var viewModel = new MessageBoxViewModel(caption, okAction, buttonOkCaption);
            var messageBox = new MessageBox(viewModel);
            messageBox.ShowDialog();
        }

        public static void ShowErrorMessageBox(string caption, string content)
        {
            var viewModel = new ErrorMessageBoxViewModel(caption, content);
            var messageBox = new ErrorMessageBox(viewModel);
            messageBox.ShowDialog();
        }
    }

    public class MessageBoxViewModel : ViewModelBase
    {
        public MessageBoxViewModel(string caption, Action okAction, string buttonOkCaption)
        {
            Caption = caption;
            OkAction = okAction;
            ButtonOkCaption = buttonOkCaption;
        }

        public string Caption { get; private set; }
        public Action OkAction { get; private set; }
        public string ButtonOkCaption { get; private set; }

        public RelayCommand OkClickCommand
        {
            get => new RelayCommand(ExecuteOkClick, () => true);
        }

        private void ExecuteOkClick(object parameter)
        {
            OkAction?.Invoke();
            if (parameter is Window window)
            {
                window.Close();
            }
        }
    }

    public class ErrorMessageBoxViewModel : ViewModelBase
    {
        public ErrorMessageBoxViewModel(string caption, string content)
        {
            Caption = caption;
            Content = content;
        }

        public string Caption { get; set; }
        public string Content { get; set; }
    }
}
