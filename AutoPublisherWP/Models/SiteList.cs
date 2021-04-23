using AutoPublisherWP.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPublisherWP.Models
{
    public class SiteList:INotifyMe
    {
        private bool ischecked;
        public bool IsChecked
        {
            get => ischecked;
            set
            {
                ischecked = value;
                OnPropertyChanged();
            }
        }

        private string siteurl;
        public string SiteUrl
        {
            get => siteurl;
            set
            {
                siteurl = value;
                OnPropertyChanged();
            }
        }

        private bool tested = false;
        public bool Tested
        {
            get => tested;
            set
            {
                tested = value;
                OnPropertyChanged();
            }
        }

        private bool state = false;
        public bool State
        {
            get => state;
            set

            {
                state = value;
                OnPropertyChanged();
            }
        }

        private int progressvalue = 0;
        public int ProgressValue
        {
            get => progressvalue;
            set
            {
                progressvalue = value;
                OnPropertyChanged();
            }
        }

        public string User { get; set; }

        public string Password { get; set; }

        private string message;
        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Category> categorylist = new ObservableCollection<Category>();
        public ObservableCollection<Category> CategoryList
        {
            get => categorylist;
            set
            {
                categorylist = value;
                OnPropertyChanged();
            }
        }

    }
}
