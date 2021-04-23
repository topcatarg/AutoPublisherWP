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
using AutoPublisherWP.ViewModels;
using AutoPublisherWP.Views;
using MahApps.Metro.Controls;

namespace AutoPublisherWP
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        readonly MainWindowVM ThisVM = new MainWindowVM();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = ThisVM;
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await ThisVM.CheckDatabase();
            PostViewControl.SetDataContext(ThisVM);
            ConfigViewControl.ConfigStart(PublishViewControl);
            PublishViewControl.ConfigMe(ThisVM.Post);
            ThisVM.Enabled = true;
            ThisVM.Working = Visibility.Hidden;

        }
    }
}
