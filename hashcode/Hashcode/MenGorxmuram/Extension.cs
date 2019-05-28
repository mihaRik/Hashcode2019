using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hashcode
{
    public static class Extension
    {
        public static bool PieceCreator(this Piece piece, int row, int col, int y, int x, ref Slice slice)
        {
            var pizza = Program.pizza;

            piece.CS = x + col;
            piece.RS = y + row;
            piece.Value = pizza[y + row, x + col];

            if (piece.Value != 'X')
            {
                slice.Pieces.Add(piece);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int IngridientCount(this List<Slice> slices, Func<Piece, bool> predicate)
        {
            var count = 0;
            foreach (var slice in slices)
            {
                count += slice.Pieces.Count(predicate);
            }
            return count;
        }
    }
}
