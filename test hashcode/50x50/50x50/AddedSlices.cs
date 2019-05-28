using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _50x50
{
    class AddedSlices
    {
        public AddedSlices()
        {
            Slices=new List<Slice>();
        }
        public List<Slice> Slices { get; set; }

        public int Order { get; set; }
    }
}
