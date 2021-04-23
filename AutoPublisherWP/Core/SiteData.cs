using AutoPublisherWP.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    }
}
