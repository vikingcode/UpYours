using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UpYours.Models
{
    [ImplementPropertyChanged]
    public class User
    {
        private UserCredential _credential;
        private YouTubeService _youtubeService;

        public ObservableCollection<Category> Categories { get; set; }

        public UserCredential Credentials { get { return _credential; } }
        public YouTubeService YoutubeService { get { return _youtubeService; } }
        private string userid;
        private IDataStore _userstorage;

        public User(string id, IDataStore UserStorage)
        {
            Categories = new ObservableCollection<Category>();
            userid = id;
            _userstorage = UserStorage;
        }
        public async void Login()
        {
            UserCredential credential;
            using (var stream = new FileStream("client_id.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { YouTubeService.Scope.YoutubeUpload, YouTubeService.Scope.Youtube },
                    userid,
                    CancellationToken.None,
                    _userstorage
                );
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });

            _credential = credential;
            _youtubeService = youtubeService;
        }

        public async Task GetDetailsUser()
        {
            var channelsListRequest = _youtubeService.Channels.List("contentDetails, snippet");
            channelsListRequest.Mine = true;
            var channelsListResponse = await channelsListRequest.ExecuteAsync();
            var userName = channelsListResponse.Items[0].Snippet.Title;
        }


    }
}
