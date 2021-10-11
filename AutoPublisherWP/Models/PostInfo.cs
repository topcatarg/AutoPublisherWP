using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPublisherWP.Models
{
    public class PostInfo
    {
        public DateTime Date { get; set; }
        public String Section { get; set; }
        public String Title { get; set; }
        public String From { get; set; }
        public long CharCount { get; set; }
        public String Content { get; set; }
        public List<int> CategoryListId { get; set; }
        public List<string> CategoryListNames { get; set; } = new List<string>();
        public string UserName { get; set; }
        public int Caracteres
        {
            get 
            {
                string resul = Content;
                int pos = resul.IndexOf("<", 0);
                while (pos >= 0)
                {
                    int end = resul.IndexOf(">", pos);
                    resul = resul.Remove(pos, end - pos+1);
                    pos = resul.IndexOf("<", 0);
                } 
                return resul.Replace("\n","").Replace("&nsbp;","").Length;
            }
        }

        public string DayName
        {
            get
            {
                var culture = new System.Globalization.CultureInfo("es-AR");
                return culture.DateTimeFormat.GetDayName(Date.DayOfWeek);
            }
        }
        public string Categories
        {
            get
            {
                return String.Join(",", CategoryListNames.ToArray());
            }
        }
        public override string ToString()
        {
            return $"\"{ Date.ToString()}\",\"{Title}\",\"{Caracteres}\",\"{DayName}\",\"{Categories}\",\"{UserName}\"" ;
        }

    }
}
