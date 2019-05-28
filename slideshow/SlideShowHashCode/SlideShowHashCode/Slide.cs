using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlideShowHashCode
{
    class Slide
    {
        public Slide()
        {
            Tags = new string[TagCount];
        }
        public int Id { get; set; }
        public Image Img1 { get; set; }
        public Image Img2 { get; set; }
        public char Type { get; set; }
        public int TagCount { get; set; }
        public string[] Tags { get; set; }
        public bool IsChecked { get; set; }
    }
}
