using System;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace ShareUX
{
    public sealed partial class MainPage : Page
    {
        private ShareOperation operation;
        private StorageFile selectedFile;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                operation = e.Parameter as ShareOperation;
                if (operation.Data.Contains(StandardDataFormats.StorageItems))
                {
                    var files = await operation.Data.GetStorageItemsAsync();
                    selectedFile = (files.FirstOrDefault()) as StorageFile;
                    var stream = await selectedFile.OpenReadAsync();
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                    {
                        BitmapImage image = new BitmapImage();
                        await image.SetSourceAsync(stream);
                        img.Source = image;
                    });
                }
            }
        }

        private async void ShareBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await selectedFile.CopyAsync(ApplicationData.Current.LocalFolder, "header.jpg", NameCollisionOption.ReplaceExisting);
            operation.ReportCompleted();
        }
    }
}
