using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _50x50
{
    public class Slice
    {
        public List<Piece> Pieces { get; set; }

        public int RowSize { get; set; }

        public int ColSize { get; set; }

        public int Order { get; set; }

        public Slice()
        {
            Pieces = new List<Piece>();
        }
        public string GetStartPoint()
        {
            return $"({Pieces[0].RS},{Pieces[0].CS})";
        }

        public string GetEndPoint()
        {
            return $"({Pieces[Pieces.Count - 1].RS},{Pieces[Pieces.Count - 1].CS})";
        }

        public string GetSliceData()
        {
            return $"{Pieces[0].RS} {Pieces[0].CS} {Pieces[Pieces.Count - 1].RS} {Pieces[Pieces.Count - 1].CS}";
        }
    }
}
