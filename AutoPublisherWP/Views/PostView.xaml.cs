using AutoPublisherWP.ViewModels;
using Microsoft.Win32;
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
using System.Xml;
using WK.Libraries.SharpClipboardNS;
using AutoPublisherWP.Helpers;

namespace AutoPublisherWP.Views
{
    /// <summary>
    /// Lógica de interacción para PostView.xaml
    /// </summary>
    public partial class PostView : UserControl
    {
        public MainWindowVM ThisViewModel = null;
        SharpClipboard Clip = new SharpClipboard();

        public PostView()
        {
            InitializeComponent();
            Clip.MonitorClipboard = true;
            Clip.ObservableFormats.Files = false;
            Clip.ObservableFormats.Images = false;
            Clip.ObservableFormats.Others = false;
            Clip.ObservableFormats.Texts = true;
            Clip.ClipboardChanged += Clip_ClipboardChanged;
        }

        private void Clip_ClipboardChanged(object sender, SharpClipboard.ClipboardChangedEventArgs e)
        {
            var Value = e.Content.ToString();
            FormatText();
        }

        private void FormatText()
        {
            string ClipText = Clipboard.GetText(TextDataFormat.Html).Trim() ;
            if (ClipText.Length == 0 || !ClipText.Contains("<!--StartFragment-->"))
            {
                return;
            }
            ClipText = ClipText.Substring(ClipText.IndexOf("<!--StartFragment-->") + 20);
            ClipText = ClipText.Substring(0, ClipText.IndexOf("<!--EndFragment-->"));
            ClipText =  NormalizeXML(ClipText);
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.LoadXml($"<div>{ClipText}</div>");
            }
            catch (Exception)
            {
                return;
            }
            string Title = "";
            xml.IterateThroughAllNodes(
                delegate (XmlNode node)
                {
                    if (node.Attributes != null)
                    {
                        node.Attributes.RemoveAll();
                    }
                    if (string.IsNullOrEmpty(Title))
                    {
                        if (!string.IsNullOrEmpty(node.Value))
                        {
                            Title = node.Value;
                            node.Value = "";
                        }
                    }
                });
            TitleText.Document = new FlowDocument();
            TitleText.AppendText(Title);
            ContentText.Document = new FlowDocument();
            ContentText.AppendText(xml.InnerXml);
        }

        private string NormalizeXML(string XML)
        {
            string ReplaceTo = "<br></br>";
            string ReplaceFrom = "";
            string TextToSearch = "<br ";
            int start = 0;
            start = XML.IndexOf(TextToSearch);
            while(start > -1)
            {
                ReplaceFrom  = XML.Substring(start, XML.IndexOf(">", start) - start +1);
                XML = XML.Replace(ReplaceFrom, ReplaceTo);
                start = XML.IndexOf(TextToSearch);
            }
            return XML;
        }

        public void SetDataContext(MainWindowVM VM)
        {
            ThisViewModel = VM;
            this.DataContext = VM;
            VM.ChangeImage += ChangeImage;
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            var Op = new OpenFileDialog();
            var result = Op.ShowDialog();
            if ((bool)result)
            {
                ChangeImage(Op.FileName);
            }
        }

        private void ChangeImage(string ImageSource)
        {
            this.Dispatcher.Invoke(() =>
            {
                
                ImageControl.Source = null;
                BitmapImage image = new BitmapImage();
                try
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = new Uri(ImageSource);
                    image.EndInit();
                }
                catch (System.IO.IOException)
                {
                    ThisViewModel.Post.ImageURI = null;
                    return;
                }
                ImageControl.Source = image;
                ThisViewModel.Post.ImageURI = new Uri(ImageSource);
                //Now we try to get the date from the file
                string FileName = System.IO.Path.GetFileName(ImageSource);
                DateTime dt;
                if (int.TryParse(FileName.Substring(0, 4), out int result))
                {
                    int year = result;
                    if (int.TryParse(FileName.Substring(5, 2), out result))
                    {
                        int month = result;
                        if (int.TryParse(FileName.Substring(8, 2), out result))
                        {
                            dt = new DateTime(year, month, result);
                            ThisViewModel.Post.Date = dt;
                            CalendarControl.DisplayDate = dt;
                        }
                    }
                }
            });
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            ImageControl.Source = null;
            ThisViewModel.Post.ImageURI = null;
        }

        private void TitleText_TextChanged(object sender, TextChangedEventArgs e)
        {
            ThisViewModel.Post.Title = new TextRange(TitleText.Document.ContentStart, TitleText.Document.ContentEnd).Text;
        }

        private void ContentText_TextChanged(object sender, TextChangedEventArgs e)
        {
            ThisViewModel.Post.Content = new TextRange(ContentText.Document.ContentStart, ContentText.Document.ContentEnd).Text;
        }

        private void DeleteTitleButton_Click(object sender, RoutedEventArgs e)
        {
            TitleText.Document = new FlowDocument();
            ThisViewModel.Post.Title = string.Empty;
        }

        private void DeleteContentButton_Click(object sender, RoutedEventArgs e)
        {
            ContentText.Document = new FlowDocument();
            ThisViewModel.Post.Content = string.Empty;
        }

        private void CalendarControl_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThisViewModel is null)
                return;
            //ThisViewModel.Post.Date = (DateTime)CalendarControl.SelectedDate;
        }

        private void DeleteAndCopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                TitleText.Document = new FlowDocument();
                TitleText.AppendText(Clipboard.GetText());
                /*
                string temp = Clipboard.GetText(TextDataFormat.Html);
                if (temp.Length > 0)
                {
                    TitleText.Document = new FlowDocument();
                    temp = temp.Substring(temp.IndexOf("<!--StartFragment-->") + 20);
                    temp = temp.Substring(0, temp.IndexOf("<!--EndFragment-->"));
                    TitleText.AppendText(temp);
                }*/
            }
        }

        private void DeleteAndCopyContentButton_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                string temp = Clipboard.GetText(TextDataFormat.Html);
                if (temp.Length > 0)
                {
                    ContentText.Document = new FlowDocument();
                    temp = temp.Substring(temp.IndexOf("<!--StartFragment-->") + 20);
                    temp = temp.Substring(0, temp.IndexOf("<!--EndFragment-->"));
                    //now take out every style
                    while (temp.IndexOf("style=") > 0)
                    {
                        int startindex = temp.IndexOf("style=") ;
                        int endindex = temp.IndexOf('"', startindex+7);
                        temp = temp.Substring(0, startindex-1) + temp.Substring(endindex + 1);
                        //temp = temp.Substring(startindex, endindex - startindex);
                    }
                    temp = temp.Replace("<span>", "").Replace("</span>","").Replace("<p><br></p>","");
                    ContentText.AppendText(temp);
                }
            }
        }

        private void CopyAllText_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                string temp = Clipboard.GetText(TextDataFormat.Html);
                if (temp.Length > 0)
                {
                    temp = temp.Substring(temp.IndexOf("<!--StartFragment-->") + 20);
                    temp = temp.Substring(0, temp.IndexOf("<!--EndFragment-->"));
                    var lines = temp.Split(separator: new char[] { '>' },options: StringSplitOptions.RemoveEmptyEntries);
                    //get the title first
                    int count = 0;
                    int closed = 0;
                    do
                    {
                        if (!lines[count].StartsWith("</") && !lines[count].StartsWith("<")  )
                        {
                            TitleText.Document = new FlowDocument();
                            TitleText.AppendText(lines[count].Substring(0,lines[count].IndexOf("<")));
                            closed--;
                        }
                        else if (lines[count].StartsWith("</"))
                        {
                            closed--;
                        }
                        else
                        {
                            closed++;
                        }
                        count++;
                    } while (closed > 0);
                    StringBuilder sb = new StringBuilder();
                    for (; count < lines.Length;count++)
                    {
                        if (lines[count].StartsWith("<"))
                        {
                            sb.Append(GetText(lines[count]));
                        }
                        else
                        {
                            sb.Append(lines[count].Substring(0,lines[count].IndexOf("<")));
                            sb.Append(GetText(lines[count].Substring(lines[count].IndexOf("<"))));
                        }
                    }
                    ContentText.Document = new FlowDocument();
                    ContentText.AppendText(sb.ToString().Replace("<p><br></p>", ""));
                }
            }
        }

        private string GetText(string s)
        {
            if (s.StartsWith("<p"))
            {
                return("<p>");
            }
            else if (s.StartsWith("</p"))
            {
                return ("</p>");
            }
            else if (s.StartsWith("<br"))
            {
                return ("<br>");
            }
            else if (s.StartsWith("</br"))
            {
                return ("</br>");
            }
            else if (s.StartsWith("<b"))
            {
                return ("<b>");
            }
            else if (s.StartsWith("</b"))
            {
                return ("</b>");
            }
            return string.Empty;
        }

        private void BtnRutaImagenes_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ThisViewModel.WatchedFolder = fd.SelectedPath;
            }
            else
            {
                ThisViewModel.WatchedFolder = "Seleccione una carpeta";
            }
            
        }
    }
}
