using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hashcode
{
    public class Slice
    {
        public List<Piece> Pieces { get; set; }

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
            return $"({Pieces[Pieces.Count - 1].RE},{Pieces[Pieces.Count - 1].CE})";
        }

        public string GetSliceData()
        {
            return $"{Pieces[0].RS} {Pieces[0].CS} {Pieces[Pieces.Count - 1].RE} {Pieces[Pieces.Count - 1].CE}";
        }
    }
}
