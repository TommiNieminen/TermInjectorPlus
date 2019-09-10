using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Transmunger
{
    internal class ViewModel
    {
        public ObservableCollection<TransProcessor> Preprocessors { get; private set; }
        public List<TransProcessor> Postprocessors { get; private set; }

        public ViewModel()
        {
            this.Preprocessors = new ObservableCollection<TransProcessor>();
            this.Postprocessors = new List<TransProcessor>();
        }
    }
}