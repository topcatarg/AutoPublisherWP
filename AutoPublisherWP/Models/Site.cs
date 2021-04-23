using AutoPublisherWP.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPublisherWP.Models
{
    public class Site : INotifyMe
    {

        private int id;
        private string siteurl;
        private string user;
        private string password;

        public int Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }
        public string SiteURL
        {
            get => siteurl;
            set
            {
                siteurl = value;
                OnPropertyChanged();
            }
        }
        public string User
        {
            get => user;
            set
            {
                user = value;
                OnPropertyChanged();
            }
        }
        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }
    }
}
