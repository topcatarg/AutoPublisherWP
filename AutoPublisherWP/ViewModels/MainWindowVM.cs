using AutoPublisherWP.Core;
using AutoPublisherWP.Helpers;
using AutoPublisherWP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using WK.Libraries.SharpClipboardNS;

namespace AutoPublisherWP.ViewModels
{
    public class MainWindowVM: INotifyMe
    {
        #region Properties

        private string footer;

        private Post post;
        public Post Post
        {
            get => post;
            set
            {
                post = value;
                OnPropertyChanged();
            }
        }

        public string Footer
        {
            get { return footer; }
            set
            {
                footer = value;
                OnPropertyChanged();
            }
        }

        private bool enabled = false;
        public bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
                OnPropertyChanged();
            }
        }

        private Visibility working = Visibility.Visible;
        public Visibility Working
        {
            get => working;
            set
            {
                working = value;
                OnPropertyChanged();
            }
        }

        private Boolean autoimage = false;
        public Boolean AutoImage
        {
            get => autoimage;
            set
            {
                autoimage = value;
                OnPropertyChanged();
                RefreshAutoImageState();
            }
        }

        private string watchedfolder = "Seleccionar una carpeta";
        public string WatchedFolder
        {
            get => watchedfolder;
            set
            {
                watchedfolder = value;
                OnPropertyChanged();
                RefreshAutoImageState();
            }
        }

        public System.Windows.Controls.Image ImageControl { get; set; }
        #endregion

        public Action<String> ChangeImage;

        public MainWindowVM()
        {
            Post = new Post();
            Footer = "";
            
        }

        public async Task CheckDatabase()
        {
            var db = new Database();
            Footer = await db.CheckDatabase();
        }

       

        private FileSystemWatcher Fs = null;
        public void RefreshAutoImageState()
        {
            if (Fs == null && autoimage && Directory.Exists(watchedfolder))
            {
                //Start the watcher
                Fs = new FileSystemWatcher(watchedfolder)
                {
                    NotifyFilter = NotifyFilters.FileName |
                                    NotifyFilters.LastWrite
                };
                Fs.Created += OnCreated;
                Fs.Filter = "";
                Fs.EnableRaisingEvents = true;

            }
            else if(!autoimage && Fs != null)
            {
                //kill the watcher
                Fs.Created -= OnCreated;
                Fs.Dispose();
                Fs = null;
            }
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            
            string Extension = Path.GetExtension(e.FullPath).ToLower();
            if (Extension == ".jpg" || Extension == ".jpeg" || Extension == ".png")
            {
                FileInfo fInfo = new FileInfo(e.FullPath);
                while (IsFileLocked(fInfo))
                {
                    Thread.Sleep(500);
                }
                ChangeImage?.Invoke(e.FullPath);
            }
        }

        private bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }

    }
}
