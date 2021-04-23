using AutoPublisherWP.Core;
using AutoPublisherWP.Helpers;
using AutoPublisherWP.Models;
using Dapper;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AutoPublisherWP.ViewModels
{
    public class PublishVM:INotifyMe
    {
        private SiteList site;
        public SiteList Site
        {
            get => site;
            set
            {
                site = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<SiteList> sites = new ObservableCollection<SiteList>();
        public ObservableCollection<SiteList> Sites
        {
            get => sites;
            set
            {
                sites = value;
                OnPropertyChanged();
            }
        }

        public PublishVM()
        {
        }

        public async Task RefreshMe()
        {
            Sites.Clear();
            var db = new Database();
            using (var conn = db.GetConnection())
            {
                var r = await conn.QueryAsync<SiteList>(
@"select 
URL as SiteURL,
User,
Pass as Password
from sites");
                foreach (SiteList s in r)
                {
                    Sites.Add(s);
                }
            }
        }

    }
}
