using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureImageUploader.Models
{
    public class HomePageModel
    {
        public string ImageList { get; set; }
      
        public string ImageFilter { get; set; }
        
        public List<string> Images = new List<string>();
    }
}