using AutoPublisherWP.Core;
using AutoPublisherWP.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoPublisherWP.Views
{
    /// <summary>
    /// Lógica de interacción para PublishView.xaml
    /// </summary>
    public partial class PublishView : UserControl
    {
        private readonly PublishVM ThisVM = new PublishVM();
        public Models.Post Post;

        public PublishView()
        {
            InitializeComponent();
            this.DataContext = ThisVM;
            ProgressControl.Visibility = Visibility.Hidden;
        }

        public async void ConfigMe(Models.Post p)
        {
            Post = p;
            LblPublishDate.DataContext = p;
            await RefreshView();
            Post.PostModified += Post_PostModified;
        }

        private void Post_PostModified()
        {
            foreach (var v in ThisVM.Sites)
            {
                v.Message = "";
            }
        }

        private void BtnCheckAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in ThisVM.Sites)
            {
                item.IsChecked = true;
            }

        }

        private void BtnUncheckAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in ThisVM.Sites)
            {
                item.IsChecked = false;
            }
        }

        private async void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            ButtonEnable(false);
            List<Task> Tasks = new List<Task>();
            foreach (var v in ThisVM.Sites)
            {
                if (v.IsChecked)
                {
                    //Publisher p = new Publisher()
                    Publisher_App_Pass p = new Publisher_App_Pass()
                    {
                        Connection = v
                    };
                    Task t = Task.Run(async () => await (Task)p.TestConnection());
                    Tasks.Add(t);
                }
            }
            await Task.WhenAll(Tasks.ToArray());
            ButtonEnable(true);
        }

        public async Task RefreshView()
        {
            await ThisVM.RefreshMe();
        }

        public void ButtonEnable(bool Enable)
        {
            BtnCheckAll.IsEnabled = Enable;
            BtnUncheckAll.IsEnabled = Enable;
            BtnPublish.IsEnabled = Enable;
            BtnTest.IsEnabled = Enable;
            dgSites.IsEnabled = Enable;
            ProgressControl.Visibility = Enable ? Visibility.Hidden : Visibility.Visible;
        }

        private async void BtnPublish_Click(object sender, RoutedEventArgs e)
        {
            ButtonEnable(false);
            List<Task> Tasks = new List<Task>();
            foreach (var v in ThisVM.Sites)
            {
                if (v.IsChecked)
                {
                    //Publisher p = new Publisher()
                    Publisher_App_Pass p = new Publisher_App_Pass()
                    {
                        Connection = v,
                        Post = Post
                    };
                    Task t = Task.Run(async () => await (Task)p.Publish());
                    Tasks.Add(t);
                }
            }
            await Task.WhenAll(Tasks.ToArray());
            ButtonEnable(true);
        }

        private async void BtnCategorias_Click(object sender, RoutedEventArgs e)
        {
            ButtonEnable(false);
            List<Task> Tasks = new List<Task>();
            List<Tuple<Models.SiteList, List<WordPressPCL.Models.Category>>> UpdateList = new List<Tuple<Models.SiteList, List<WordPressPCL.Models.Category>>>();
            foreach (Models.SiteList v in ThisVM.Sites)
            {
                v.CategoryList.Clear();
                if (v.IsChecked)
                {
                    SiteData sd = new SiteData() { Connection = v };
                    Task t = Task.Run(async () =>
                    {
                        var list = await sd.GetCategories();
                        UpdateList.Add(new Tuple<Models.SiteList, List<WordPressPCL.Models.Category>>(v, list));
                    });
                    Tasks.Add(t);
                }
            }
            await Task.WhenAll(Tasks.ToArray());
            foreach (var item in UpdateList)
            {
                foreach (var cat in item.Item2)
                {
                    item.Item1.CategoryList.Add(new Models.Category()
                    {
                        Id = cat.Id,
                        Name = cat.Name
                    });
                }
            }
            ButtonEnable(true);
        }
    }
}
