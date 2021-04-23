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
    /// Lógica de interacción para ConfigView.xaml
    /// </summary>
    public partial class ConfigView : UserControl
    {
        ConfigVM thisVM;
        public PublishView PublishView;

        public ConfigView()
        {
            InitializeComponent();
        }

        public void ConfigStart(PublishView Pv)
        {
            thisVM = new ConfigVM();
            this.DataContext = thisVM;
            if (dgSites.Items.Count > 0)
                dgSites.SelectedIndex = 0;
            ProgressControl.Visibility = Visibility.Hidden;
            ChangeView(false);
            PublishView = Pv;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            ChangeView(true);
            thisVM.OldSite = thisVM.Site;
            thisVM.Site = new Models.Site();
            thisVM.Adding = true;
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (thisVM.Site == null)
                return;
            ProgressControl.Visibility = Visibility.Visible;
            await thisVM.Delete();
            ProgressControl.Visibility = Visibility.Hidden;
            dgSites.SelectedIndex = dgSites.Items.Count - 1;
            await PublishView.RefreshView();
        }

        private async void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            //Must add
            ProgressControl.Visibility = Visibility.Visible;
            thisVM.WaitForUpdate = !thisVM.WaitForUpdate;
            await thisVM.Upsert();
            if (thisVM.Adding)
                dgSites.SelectedIndex = dgSites.Items.Count - 1;
            thisVM.Adding = false;
            thisVM.WaitForUpdate = !thisVM.WaitForUpdate;
            ProgressControl.Visibility = Visibility.Hidden;
            ChangeView(false);
            await PublishView.RefreshView();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (thisVM.Site == null)
                return;
            thisVM.OldSite = new Models.Site
            {
                Id = thisVM.Site.Id,
                SiteURL = thisVM.Site.SiteURL,
                Password = thisVM.Site.Password,
                User = thisVM.Site.User
            };
            ChangeView(true);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            thisVM.Adding = false;
            ChangeView(false);
            thisVM.Site.Id = thisVM.OldSite.Id;
            thisVM.Site.User = thisVM.OldSite.User;
            thisVM.Site.SiteURL = thisVM.OldSite.SiteURL;
            thisVM.Site.Password = thisVM.OldSite.Password;
        }

        private void ChangeView(bool Editing)
        {
            dgSites.IsEnabled = !Editing;
            thisVM.Editing = Editing;
            Visibility ButtonsEditing;
            Visibility ButtonsAdding;
            if (Editing)
            {
                ButtonsEditing = Visibility.Visible;
                ButtonsAdding = Visibility.Hidden;
            }
            else
            {
                ButtonsEditing = Visibility.Hidden;
                ButtonsAdding = Visibility.Visible;
            }
            BtnAdd.Visibility = ButtonsAdding;
            BtnEdit.Visibility = ButtonsAdding;
            BtnDelete.Visibility = ButtonsAdding;
            BtnOk.Visibility = ButtonsEditing;
            BtnCancel.Visibility = ButtonsEditing;
        }
    }
}
