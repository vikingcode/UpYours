using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using UpYours.Models;
using Microsoft.Win32;
using System.IO;
using PropertyChanged;

namespace UpYours.ViewModels
{
    [Export(typeof(AddFileViewModel))]
    [ImplementPropertyChanged]
    public class AddFileViewModel : Screen
    {
        private ObservableCollection<User> users;
        private readonly IWindowManager _windowManager;
        private string _directory;
        private string _videoname;

        public Video Video { get; set; }

        public ObservableCollection<User> Users { get { return users; } }

        private User _selectedUser;
        public User SelectedUser
        {
            get
            {
                return _selectedUser;
            }
            set
            {
                _selectedUser = value;
                NotifyOfPropertyChange();
                SetUser();
            }
        }

        public AddFileViewModel(ObservableCollection<User> users, IWindowManager windowManager)
        {
            this.users = users;
            _windowManager = windowManager;
            this.ViewAttached += AddFileViewModel_ViewAttached;

        }

        private void AddFileViewModel_ViewAttached(object sender, ViewAttachedEventArgs e)
        {
            OpenFile();
        }

        private void SelectFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == false)
                return;

            Video = new Video();

            var file = openFileDialog.FileName;
            _directory = Path.GetDirectoryName(file);
            _videoname = Path.GetFileNameWithoutExtension(file);


            Video.FilePath = file;
            Video.Bytes = new FileInfo(file).Length;
            Video.ImagePath = Path.Combine(_directory, _videoname + ".jpg");
        }

        private void SetUser()
        {
            Video.User = _selectedUser;
            var metafile = Path.Combine(_directory, _videoname + ".txt");

            if (File.Exists(metafile))
            {
                using (FileStream fs = new FileStream(metafile, FileMode.Open))
                using (StreamReader rdr = new StreamReader(fs))
                {
                    Video.Title = rdr.ReadLine();

                    var cat = rdr.ReadLine();
                    Video.Category = _selectedUser.Categories.Where(c => c.Id == cat || c.Title == cat).FirstOrDefault();

                    Video.Status = (VideoStatus)Enum.Parse(typeof(VideoStatus), rdr.ReadLine(), true);

                    Video.Description = rdr.ReadToEnd();
                }
            }

            TryClose(true);
        }

        public void OpenFile()
        {
            SelectFile();
        }
    }
}
