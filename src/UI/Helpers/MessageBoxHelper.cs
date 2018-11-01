using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UI.ViewModels;

namespace UI.Helpers
{
    public static class MessageBoxHelper
    {
        public static void ShowMessageBox(string content, Action okAction = null, string caption = "Informacja", string buttonOkCaption = "OK")
        {
            var viewModel = new MessageBoxViewModel(caption, content, okAction, buttonOkCaption);
            var messageBox = new MessageBox(viewModel);
            Icon icon = SystemIcons.Information;
            ImageSource iconImageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            messageBox.Icon = iconImageSource;
            messageBox.ShowDialog();
        }

        public static void ShowErrorMessageBox(string content, Action okAction = null, string caption = "Błąd", string buttonOkCaption = "OK")
        {
            var viewModel = new MessageBoxViewModel(caption, content, okAction, buttonOkCaption);
            var messageBox = new MessageBox(viewModel);
            Icon icon = SystemIcons.Error;
            ImageSource iconImageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            messageBox.Icon = iconImageSource;

            messageBox.ShowDialog();
        }
    }

    public class MessageBoxViewModel : BaseViewModel
    {
        public MessageBoxViewModel(string caption, string content, Action okAction, string buttonOkCaption)
        {
            Caption = caption;
            Content = content;
            OkAction = okAction;
            ButtonOkCaption = buttonOkCaption;
        }

        public string Caption { get; private set; }
        public string Content { get; private set; }
        public Action OkAction { get; private set; }
        public string ButtonOkCaption { get; private set; }

        public RelayCommand OkClickCommand => new RelayCommand(ExecuteOkClick, () => true);
        private void ExecuteOkClick(object parameter)
        {
            OkAction?.Invoke();
            if (parameter is Window window)
            {
                window.Close();
            }
        }
    }
}
