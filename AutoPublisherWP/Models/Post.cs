using AutoPublisherWP.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPublisherWP.Models
{
    public class Post: INotifyMe
    {
        private Uri imageuri;
        public Uri ImageURI
        {
            get => imageuri;
            set
            {
                imageuri = value;
                OnPropertyChanged();
                PostModified?.Invoke();
            }
        }
        private string title;
        public string Title {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged();
                PostModified?.Invoke();
            }
        }
        private string content;
        public string Content {
            get => content;
            set
            {
                content = value;
                OnPropertyChanged();
                PostModified?.Invoke();
            }
        }

        private DateTime date;
        public DateTime Date
        {
            get => date;
            set
            {
                date = value;
                OnPropertyChanged();
                PostModified?.Invoke();
            }
        }

        private bool withoutimage = false;
        public bool WithoutImage
        {
            get => withoutimage;
            set
            {
                withoutimage = value;
                OnPropertyChanged();
            }
        }
        public Post()
        {
            ImageURI = null;
            Title = "";
            Content = "";
            Date = System.DateTime.Now;
        }

        public bool CanPublish()
        {
            return (System.IO.File.Exists(ImageURI?.LocalPath) || WithoutImage) && !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Content);
        }

        public event Action PostModified;

        


    }
}
