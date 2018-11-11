using System;
using System.Threading.Tasks;
using System.Windows.Controls;
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

        // TODO: wszystkie widoki mają wywołać tę metodę zamiast ^
        public async Task ProceedLongRunningOperationAsync(Control caller, Func<Task> operation)
        {
            caller.IsEnabled = false;
            var dialog = new OperationInProgressDialog();
            dialog.Show();
            await operation();
            await Task.Delay(5000);
            dialog.Close();
            caller.IsEnabled = true;
        }
    }
}
