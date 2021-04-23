using AutoPublisherWP.Core;
using AutoPublisherWP.Helpers;
using AutoPublisherWP.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPublisherWP.ViewModels
{
    public class ConfigVM: INotifyMe
    {
        private Site site;
        public Site Site
        {
            get => site;
            set
            {
                site = value;
                OnPropertyChanged();
                OnPropertyChanged("CanEdit");
            }
        }

        private ObservableCollection<Site> sites = new ObservableCollection<Site>();
        public ObservableCollection<Site> Sites
        {
            get => sites;
            set
            {
                sites = value;
                OnPropertyChanged();
            }
        }

        private bool editing = false;
        public bool Editing
        {
            get => editing;
            set
            {
                editing = value;
                OnPropertyChanged();
            }
        }

        public bool Adding { get; set; } = false;
        public Site OldSite { get; set; }

        private bool waitforupdate = true;
        public bool WaitForUpdate
        {
            get => waitforupdate;
            set
            {
                waitforupdate = value;
                OnPropertyChanged();
            }
        }

        public ConfigVM()
        {
            var db = new Database();
            using (var conn = db.GetConnection())
            {
                var r = conn.Query<Site>(
@"select 
id as Id,
URL as SiteURL,
User,
Pass as Password
from sites");
                foreach (Site s in r)
                {
                    Sites.Add(s);
                }
            }
        }

        public async Task Upsert()
        {
            var db = new Database();
            using (var conn = db.GetConnection())
            {
                string Query;
                if (site.Id == 0)
                {
                    Query = @"
insert into sites(Url,User,Pass)
values (@SiteURL,@User,@Password);

select 
id as Id,
URL as SiteURL,
User,
Pass as Password
from sites
where id=
(select max(id) from sites);";
                    Site result = await conn.QueryFirstAsync<Site>(Query, new
                    {
                        site.SiteURL,
                        site.User,
                        site.Password
                    });
                    Sites.Add(result);
                }
                else
                {
                    Query = @"
Update sites 
set Url=@Url, User=@User,Pass=@Pass 
where id=@Id;";
                    _ = await conn.QueryAsync(Query, new
                    {
                        @Url = site.SiteURL,
                        @User = site.User,
                        @Pass = site.Password,
                        @Id = site.Id
                    });
                }


            }
        }

        public async Task Delete()
        {
            string query =
@"delete
from sites
where id = @id;";
            var db = new Database();
            using (var conn = db.GetConnection())
            {
                _ = await conn.QueryAsync(query, new
                {
                    @id = site.Id
                });
                sites.Remove(site);
            }
        }
    }
}
