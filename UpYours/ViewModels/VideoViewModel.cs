using Caliburn.Micro;
using Google.Apis.Upload;
using Google.Apis.YouTube.v3.Data;
using PropertyChanged;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using UpYours.Models;

namespace UpYours.ViewModels
{
    [Export(typeof(VideoViewModel))]
    [ImplementPropertyChanged]
    public class VideoViewModel : Screen
    {
        public Models.Video Model { get; set; }

        public VideoViewModel(Models.Video video)
        {
            Model = video;
        }

        public void Upload()
        {
            Upload(Model);
        }


        public async Task Upload(Models.Video videomodel)
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
                var videosInsertRequest = videomodel.User.YoutubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");

                videosInsertRequest.ProgressChanged += (progress) => VideoUploadProgressChanged(progress, videomodel);
                videosInsertRequest.ResponseReceived += (response) => VideoUploadResponseReceived(response, videomodel, videomodel.User);

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
