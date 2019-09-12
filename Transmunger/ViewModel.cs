﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Transmunger
{
    internal class ViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ITransProcessor> _preprocessors;
        private ObservableCollection<ITransProcessor> _postprocessors;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ObservableCollection<ITransProcessor> Preprocessors
            { get => _preprocessors; set { _preprocessors = value; NotifyPropertyChanged(); } }
        public ObservableCollection<ITransProcessor> Postprocessors
            { get => _postprocessors; set { _postprocessors = value; NotifyPropertyChanged(); } }

        public ViewModel()
        {
            this.Preprocessors = new ObservableCollection<ITransProcessor>();
            this.Postprocessors = new ObservableCollection<ITransProcessor>();
        }
    }
}