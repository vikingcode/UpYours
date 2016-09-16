using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpYours.Models
{
    public enum VideoStatus
    {
        [Description("Unlisted")]
        Unlisted,
        [Description("Public")]
        Public,
        [Description("Private")]
        Private,
        [Description("Scheduled")]
        Scheduled
    }
}
