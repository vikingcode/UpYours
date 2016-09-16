//using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Windows.Controls;
//using UpYours.Messages;
using UpYours.Models;

namespace UpYours.Views
{
    public partial class VideoView : UserControl
    {
        public VideoView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var video = this.DataContext as Video;
         //   Messenger.Default.Send(new UploadMessage(video, null));
        }
    }
}
