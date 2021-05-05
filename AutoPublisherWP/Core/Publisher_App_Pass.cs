using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoPublisherWP.Interfaces;
using AutoPublisherWP.Models;
using WordPressPCL;
using WordPressPCL.Models;
using WordPressPCL.Utility;

namespace AutoPublisherWP.Core
{
    public class Publisher_App_Pass : IPublisher
    {

        public SiteList Connection { get; set; }

        private WordPressClient client;

        public AutoPublisherWP.Models.Post Post { get; set; }

        private MediaItem Image;

        public async Task CheckImage()
        {
            await TestConnection();
            MediaQueryBuilder q = new MediaQueryBuilder
            {
                Slugs = new string[]
                {
                    "2020-09-20-celulares"
                }
            };
            var items = await client.Media.Query(q);
            if (items.Count() > 1)
            {
                Connection.Message = "mas de un elemento encontrado";
                return;
            }
            //var m = items.First();
        }

        public async Task Publish()
        {
            Connection.Message = "Iniciando";
            if (!Post.CanPublish())
            {
                Connection.Message = "Faltan datos";
                return;
            }
            Connection.ProgressValue = 0;
            await TestConnection();
            if (!Connection.Tested)
            {
                Connection.Message = "Fallo el testeo";
                return;
            }
            Connection.Message = "Testeado";
            Connection.ProgressValue = 30;
            if (!Post.WithoutImage)
            {
                await PushImage();
                Connection.Message = "Imagen subida";
            }
            else
            {
                Connection.Message = "Sin imagen";
            }
            Connection.ProgressValue = 60;
            await PublishPost();
            Connection.Message = "Publicación realizada";
            Connection.ProgressValue = 100;
            Connection.State = true;
        }

        private async Task PublishPost()
        {
            var NuevoPost = new WordPressPCL.Models.Post()
            {
                Title = new WordPressPCL.Models.Title(Post.Title),
                Content = new WordPressPCL.Models.Content(Post.Content),
                Date = Post.Date
            };
            if (!Post.WithoutImage)
            {
                NuevoPost.FeaturedMedia = Image.Id;
            }
            if (Connection.CategoryList != null)
            {
                var c = Connection.CategoryList.Where(x => x.Selected).Select(x => x.Id).ToArray();
                if (c.Count() > 0)
                {
                    NuevoPost.Categories = c;
                }
            }
            _ = await client.Posts.Create(NuevoPost);
        }

        private async Task PushImage()
        {
            Image = await client.Media.Create(Post.ImageURI.LocalPath, System.IO.Path.GetFileName(Post.ImageURI.LocalPath));
        }

        public async Task TestConnection()
        {
            Connection.Tested = false;
            Connection.Message = "Iniciando testeo";
            string baseURL = Connection.SiteUrl + "wp-json";
            client = new WordPressClient(baseURL)
            {
                AuthMethod = AuthMethod.ApplicationPassword,
                UserName = Connection.User
            };
            client.SetApplicationPassword(Connection.Password);
            var post = new WordPressPCL.Models.Post()
            {
                Title = new Title("Title 1"),
                Content = new Content("Content PostCreate")
            };
            WordPressPCL.Models.Post postCreated;
            try
            {
                postCreated = await client.Posts.Create(post);
            }
            catch(Exception e)
            {
                Connection.Message = e.Message;
                return ;
            }
            bool deleted;
            try
            {
                deleted = await client.Posts.Delete(postCreated.Id, true);
            }
            catch (Exception e)
            {
                Connection.Message = e.Message;
                return;
            }
            Connection.Tested = true;
            Connection.Message = "Testeo finalizado";
        }
    }
}
