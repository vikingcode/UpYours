using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.IO;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3.Data;
using UpYours.Models;
using System.Collections.ObjectModel;

namespace UpYours.ViewModels
{
    [Export(typeof(MainViewModel))]
    public class MainViewModel : PropertyChangedBase
    {
        private readonly IWindowManager _windowManager;

        public static string DataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        public static IDataStore UserStorage = new FileDataStore(DataDir);

        public ObservableCollection<VideoViewModel> Videos { get; set; }

        public ObservableCollection<User> Users { get; set; }

        public string Title { get { return "UpYours";  } }

        [ImportingConstructor]
        public MainViewModel(IWindowManager windowManager)
        {

            _windowManager = windowManager;
            Videos = new ObservableCollection<VideoViewModel>();
            Users = new ObservableCollection<User>();

            Login();
        }

        public void AddVideo()
        {
            var addfile = new AddFileViewModel(Users, _windowManager);
            if (_windowManager.ShowDialog(addfile) == false)
                return;

            Videos.Add(new VideoViewModel(addfile.Video));
        }

        public void AddUser()
        {
            NewUser();
        }

        private void NewUser()
        {
            var x = new User(Guid.NewGuid().ToString(), UserStorage);
            Users.Add(x);
            x.Login();
        }

        public async Task Login()
        {
            List<string> users = new List<string>();
            foreach (var f in Directory.GetFiles(DataDir))
            {
                if (f.Contains("Google.Apis.Auth.OAuth2.Responses.TokenResponse-"))
                {
                    users.Add(f.Substring(f.LastIndexOf("TokenResponse-") + 14));
                }
            }

            if (users.Count > 0)
                foreach (var u in users)
                {
                    var x = new User(u, UserStorage);
                    Users.Add(x);
                    x.Login();
                    await GetCategories(x);
                }
            else
            {
                NewUser();
            }
            return;
        }

        public async Task GetCategories(User user)
        {
            var searchListRequest = user.YoutubeService.VideoCategories.List("snippet");
            searchListRequest.RegionCode = "AU";
            var searchListResponse = await searchListRequest.ExecuteAsync();

            foreach (var c in searchListResponse.Items)
            {
                user.Categories.Add(new Category { Id = c.Id, Title = c.Snippet.Title });
            }
            return;
        }

    }
}

