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
            var eee = Program._dividers;
            var rer = Program._divideOrder;

            piece.CS = x + col;
            piece.RS = y + row;
            piece.CE = x + col;
            piece.RE = y + row;
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
    }
}
