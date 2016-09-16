using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpYours.Models
{
    [ImplementPropertyChanged]
    public class Category
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }
}
