using Google.Apis.YouTube.v3.Data;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpYours.Models
{
    [ImplementPropertyChanged]
    public class Video
    {
        public User User { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; } 
        public List<string> Tags { get; set; }
        public string Description { get; set; }
        public Category Category { get; set; }// See https://developers.google.com/youtube/v3/docs/videoCategories/list

        public double Progress { get; set; }
        public double Bytes { get; set; }
        public string ImagePath { get; set; }

        public VideoStatus Status { get; set; }
        public string id { get; set; }

        public bool IsUploading { get; set; }
        public Video()
        {
            Tags = new List<string>();
            IsUploading = true;
            Progress = 0;
        }

    }

}
