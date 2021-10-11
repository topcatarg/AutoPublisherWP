using AutoPublisherWP.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using WordPressPCL;

namespace AutoPublisherWP.Core
{
    public class SiteData
    {
        public SiteList Connection { get; set; }

        private WordPressClient client;

        public async Task<List<WordPressPCL.Models.Category>> GetCategories()
        {
            Connection.Message = "Obteniendo categorias";
            string baseURL = Connection.SiteUrl + "wp-json";
            client = new WordPressClient(baseURL)
            {
                AuthMethod = WordPressPCL.Models.AuthMethod.JWTAuth
            };
            var cat = await client.Categories.GetAll().ConfigureAwait(false);
            Connection.Message = "Categorias obtenidas";
            return cat.ToList();
        }

        public async Task<string> GetPostsAsync()
        {
            List<PostInfo> postInfos = new List<PostInfo>();
            Connection.Message = "Conectado";
            string baseURL = Connection.SiteUrl + "wp-json";
            client = new WordPressClient(baseURL)
            {
                AuthMethod = WordPressPCL.Models.AuthMethod.ApplicationPassword,
                UserName = Connection.User
            };
            client.SetApplicationPassword(Connection.Password);
            Connection.Message = "Obteniendo categorias";
            var categories = await client.Categories.GetAll(false, true);
            var Users = await client.Users.GetAll(false, true);
            WordPressPCL.Utility.PostsQueryBuilder PB = new WordPressPCL.Utility.PostsQueryBuilder();
            PB.After = new DateTime(2019, 5, 1);
            PB.Before = DateTime.Today;
            PB.PerPage = 100;
            PB.Page = -1;
            PB.Order = WordPressPCL.Models.Order.ASC;
            int postcount = 0;
            do
            {
                Connection.Message = $"Bajando pagina {PB.Page}";
                PB.Page += 1;
                var posts = await client.Posts.Query(PB, true);
                postcount = posts.Count();
                foreach(var p in posts)
                {
                    PostInfo post = new PostInfo()
                    {
                        Date = p.Date,
                        Title = p.Title.Rendered,
                        Content = p.Content.Rendered,
                        CategoryListId = p.Categories.ToList<int>()
                    };
                    foreach(int cat in p.Categories)
                    {
                        var found = categories.First(x => x.Id == cat);
                        if (found!= null)
                        {
                            post.CategoryListNames.Add(found.Name);
                        }
                    }
                    post.UserName = Users.First(x => x.Id == p.Author).Slug;
                    postInfos.Add(post);
                }

            } while (postcount == 100);
            int i = 0;
            StreamWriter wt = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "novedades.csv"));
            foreach (var p in postInfos)
            {
                Connection.Message = $"Grabando: {i}";
                wt.WriteLine(p.ToString());
                i++;
            }
            wt.Close();
            Connection.Message = "Finalizado";
            return "";
        }
    }
}
