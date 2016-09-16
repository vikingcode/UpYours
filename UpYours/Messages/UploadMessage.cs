using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpYours.Models;

namespace UpYours.Messages
{
    public class UploadMessage
    {
        public Video Video { get; set; }
        public User User { get; set; }
        public UploadMessage(Video video, User user)
        {
            Video = video;
            User = user;
        }
    }
}
