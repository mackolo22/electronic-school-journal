using ApplicationCore.Enums;
using ApplicationCore.Models;

namespace UI.ViewModels
{
    public class ShowMessageViewModel : BaseViewModel
    {
        public ShowMessageViewModel(Message message)
        {
            DateAndTime = $"Data wysłania: {message.DateAndTime}";

            if (message.Folder == MessageFolder.Sent)
            {
                FromOrTo = $"Do: {message.To}";
            }
            else
            {
                FromOrTo = $"Od: {message.From}";
            }

            Subject = $"Temat: {message.Subject}";
            Content = message.Content;
        }

        public string DateAndTime { get; set; }
        public string FromOrTo { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}
