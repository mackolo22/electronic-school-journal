using ApplicationCore.Enums;
using Microsoft.WindowsAzure.Storage.Table;

namespace ApplicationCore.Models
{
    public class Message
    {
        public string From { get; set; }
        public string To { get; set; }
        [IgnoreProperty]
        public string UserToDisplay { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string DateAndTime { get; set; }
        public MessageFolder Folder { get; set; }
    }
}
