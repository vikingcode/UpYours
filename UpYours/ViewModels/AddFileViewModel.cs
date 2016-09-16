using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpYours.ViewModels
{
    [Export(typeof(AddFileViewModel))]
    public class AddFileViewModel : PropertyChangedBase, IViewModelBase
    {
    }
}
