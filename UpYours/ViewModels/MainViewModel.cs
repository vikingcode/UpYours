using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3.Data;
using UpYours.Models;
using System.Collections.ObjectModel;
using Microsoft.Win32;

namespace UpYours.ViewModels
{
    [InheritedExport]
    public interface IViewModelBase
    {

    }

    [InheritedExport]
    public interface IMainViewModel
    {

    }

    public class MainViewModel : PropertyChangedBase, IViewModelBase, IMainViewModel
    {
        private readonly IWindowManager _windowManager;

        public static string DataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        public static IDataStore UserStorage = new FileDataStore(DataDir);

        public ObservableCollection<Models.Video> Videos { get; set; }

        public ObservableCollection<User> Users { get; set; }

        //public RelayCommand UploadCommand { get; set; }
        [ImportingConstructor]
        public MainViewModel(IWindowManager windowManager)
        {

            _windowManager = windowManager;
            Videos = new ObservableCollection<Models.Video>();
            Users = new ObservableCollection<User>();

            Login();

            // Messenger.Default.Register<UploadMessage>(this, c => Upload(c.Video, Users[0]));
        }

        public void Add()
        {
            _windowManager.ShowWindow(new AddFileViewModel());
            //OpenFile();
        }

        public void OpenFile()
        {
            //TODO: Move this to AddFileViewModel
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var video = new Models.Video();

                var file = openFileDialog.FileName;
                var directory = Path.GetDirectoryName(file);
                var name = Path.GetFileNameWithoutExtension(file);
                var metafile = Path.Combine(directory, name + ".txt");

                video.FilePath = file;
                video.Bytes = new FileInfo(file).Length;
                video.User = Users[0];

                video.ImagePath = Path.Combine(directory, name + ".jpg");

                if (File.Exists(metafile))
                {
                    using (FileStream fs = new FileStream(metafile, FileMode.Open))
                    using (StreamReader rdr = new StreamReader(fs))
                    {
                        video.Title = rdr.ReadLine();

                        var cat = rdr.ReadLine();
                        video.Category = Users[0].Categories.Where(c => c.Id == cat || c.Title == cat).FirstOrDefault();

                        video.Status = (Models.VideoStatus)Enum.Parse(typeof(Models.VideoStatus), rdr.ReadLine(), true);

                        video.Description = rdr.ReadToEnd();
                    }
                }

                Videos.Add(video);
            }
        }

        public async Task Login()
        {
            List<string> users = new List<string>();
            foreach (var f in Directory.GetFiles(DataDir))
            {
                if (f.Contains("Google.Apis.Auth.OAuth2.Responses.TokenResponse-"))
                {
                    users.Add(f.Substring(f.LastIndexOf('-') + 1));
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
                var x = new User("user", UserStorage);
                Users.Add(x);
                x.Login();
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

        public async Task Upload(Models.Video videomodel, User user)
        {

            var video = new Google.Apis.YouTube.v3.Data.Video
            {
                Snippet = new VideoSnippet
                {
                    Title = videomodel.Title,
                    Description = videomodel.Description,
                    Tags = videomodel.Tags.ToArray(),
                    CategoryId = videomodel.Category?.Id ?? "1",
                },
                Status = new Google.Apis.YouTube.v3.Data.VideoStatus
                {
                    PrivacyStatus = videomodel.Status.ToString().ToLower()
                },
            };

            using (var fileStream = new FileStream(videomodel.FilePath, FileMode.Open))
            {
                var videosInsertRequest = user.YoutubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");

                videosInsertRequest.ProgressChanged += (progress) => VideoUploadProgressChanged(progress, videomodel);
                videosInsertRequest.ResponseReceived += (response) => VideoUploadResponseReceived(response, videomodel, user);

                videomodel.IsUploading = false;

                await videosInsertRequest.UploadAsync();
            }
        }

        public async Task SetThumbnail(string ImageFilePath, string VideoId, User user)
        {
            try
            {
                using (var fileStream = new FileStream(ImageFilePath, FileMode.Open, FileAccess.Read))
                {
                    var upload = user.YoutubeService.Thumbnails.Set(VideoId, fileStream, "image/jpeg");

                    upload.ProgressChanged += thumbnailUpload_ProgressChanged;
                    upload.ResponseReceived += (o) => thumbnailUploadResponseReceived(o, VideoId);

                    await upload.UploadAsync();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void thumbnailUploadResponseReceived(ThumbnailSetResponse obj, string videoId)
        {
            Console.WriteLine("Thumbnail for '{0}' was successfully uploaded.", videoId);
        }

        private void thumbnailUpload_ProgressChanged(IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    Console.WriteLine("{0} bytes sent.", progress.BytesSent);
                    break;

                case UploadStatus.Failed:
                    Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                    break;
            }
        }

        private void VideoUploadProgressChanged(IUploadProgress progress, Models.Video video)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    Console.WriteLine("{0} bytes sent.", progress.BytesSent);
                    video.Progress = (progress.BytesSent / video.Bytes) * 100;
                    Console.WriteLine("{0}%  uploaded.", video.Progress);
                    break;

                case UploadStatus.Failed:
                    Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                    break;
            }
        }

        private void VideoUploadResponseReceived(Google.Apis.YouTube.v3.Data.Video video, Models.Video videoModel, User user)
        {
            videoModel.id = video.Id;
            Console.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);
            if (File.Exists(videoModel.ImagePath))
                SetThumbnail(videoModel.ImagePath, videoModel.id, user);
        }
    }
}

