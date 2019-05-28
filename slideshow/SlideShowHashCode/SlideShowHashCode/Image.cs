using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlideShowHashCode
{
    class Image
    {
        public Image()
        {
            Tags = new string[TagCount];
        }

        public int Id { get; set; }
        public char Orientation { get; set; }
        public int TagCount { get; set; }
        public string[] Tags { get; set; }
        public bool IsChecked { get; set; }
    }
}
