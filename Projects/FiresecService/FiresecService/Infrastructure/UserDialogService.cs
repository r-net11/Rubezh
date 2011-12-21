using Infrastructure.Common;

namespace FiresecService.Infrastructure
{
    public static class UserDialogService
    {
        public static bool ShowModalWindow(IDialogContent model)
        {
            try
            {
                var dialog = new DialogWindow();
                dialog.SetContent(model);

                bool? result = dialog.ShowDialog();
                if (result == null)
                    return false;

                return (bool)result;
            }
            catch
            {
                return false;
            }
        }
    }
}
