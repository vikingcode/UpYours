using PropertyChanged;

namespace UpYours.Models
{
    [ImplementPropertyChanged]
    public class Category
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }
}
