using System;
using System.Threading.Tasks;
using UI.Views;

namespace UI.Helpers
{
    public class LongRunningOperationHelper
    {
        public async Task ProceedLongRunningOperationAsync(Func<Task> operation)
        {
            var dialog = new OperationInProgressDialog();
            dialog.Show();
            await operation();
            dialog.Close();
        }
    }
}
