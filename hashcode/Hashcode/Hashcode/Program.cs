using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hashcode
{
    public enum Category
    {
        Small,
        Medium,
        Big
    }

    class Program
    {
        static List<Slice> slices = new List<Slice>();
        static int _maxMCount = 6;
        static int _maxTCount = 7;
        //static int _maxMCount = 3;
        //static int _maxTCount = 4;
        static int r, c, l, h;
        public static char[,] pizza;
        private static StreamReader stream;
        public static int _divideOrder = 0;
        public static List<int> _dividers;
        private static string path;
        static Random rc = new Random();
        static Random gc = new Random();
        static Random bc = new Random();

        static void Main(string[] args)
        {
            FileSelector(Category.Medium);

            var rules = stream.ReadLine();
            var a = rules.Split(' ');
            r = Convert.ToInt32(a[0]);
            c = Convert.ToInt32(a[1]);
            l = Convert.ToInt32(a[2]);
            h = Convert.ToInt32(a[3]);

            var text = stream.ReadToEnd();
            text = text.Replace("\n", null);
            pizza = new char[r, c];

            CreatePizza(text);

            _dividers = FigureAreaDetector().ToList();

            for (int y = 0; y < r; y++)
            {
                for (int x = 0; x < c; x++)
                {
                    if (pizza[y, x] == 'T' || pizza[y, x] == 'M')
                    {
                        for (_divideOrder = 0; _divideOrder < _dividers.Count; _divideOrder++)
                        {
                            if (x + h / _dividers[_divideOrder] <= c &&
                                y + _dividers[_divideOrder] <= r)
                            {
                                Hesabla(x, y);
                                break;
                            }
                        }
                    }
                }
            }

            stream.Close();

            //FileWriter();
            DisplayInfo();
            //ShowSlicePos();
            //HtmlCreator();
            //Console.WriteLine(slices.Count);
            //foreach (var item in slices)
            //{
            //    Console.WriteLine(item.GetSliceData());
            //}
        }

        private static void Hesabla(int x, int y)
        {
            var slice = new Slice();

            slice = CuttingSlice(x, y, slice);

            var mCountInSlice = slice.Pieces.Count(p => p.Value == 'M');
            var tCountInSlice = slice.Pieces.Count(p => p.Value == 'T');

            if (slice.Pieces.Count <= h &&
                slice.Pieces.Count >= l * 2 &&
                //mCountInSlice <= _maxMCount &&
                //tCountInSlice <= _maxTCount &&
                mCountInSlice >= l &&
                tCountInSlice >= l)
            {
                _divideOrder = 0;
                slices.Add(slice);
                CuttingPizza(y, x);
            }
            else
            {
                _divideOrder++;

                if (_divideOrder < _dividers.Count &&
                    x + h / _dividers[_divideOrder] <= c &&
                    y + _dividers[_divideOrder] <= r)
                {
                    Hesabla(x, y);
                }
            }
        }

        private static Slice CuttingSlice(int x, int y, Slice slice)
        {
            var result = false;
            for (int row = 0; row < _dividers[_divideOrder]; row++)
            {
                for (int col = 0; col < h / _dividers[_divideOrder]; col++)
                {
                    var piece = new Piece();
                    result = piece.PieceCreator(row, col, y, x, ref slice);

                    var mCountInSlice = slice.Pieces.Count(p => p.Value == 'M');
                    var tCountInSlice = slice.Pieces.Count(p => p.Value == 'T');

                    if (!result)
                    {
                        if (slice.Pieces.Count > h &&
                            slice.Pieces.Count < l * 2 &&
                            mCountInSlice < l &&
                            tCountInSlice < l)
                        {
                            slice = new Slice();
                        }
                        break;
                    }
                }

                if (!result)
                {
                    break;
                }
            }

            return slice;
        }

        private static void CuttingPizza(int y, int x)
        {
            var lastSlice = slices[slices.Count - 1];
            lastSlice.Pieces.ForEach(s => pizza[s.RS, s.CS] = 'X');
        }

        private static void CreatePizza(string text)
        {
            for (int y = 0; y < r; y++)
            {
                for (int x = 0; x < c; x++)
                {
                    pizza[y, x] = text[y * c + x];
                }
            }
        }

        private static void ShowSlicePos()
        {
            foreach (var item in slices)
            {
                Console.WriteLine(item.GetStartPoint() + " " + item.GetEndPoint());
            }
        }

        private static void DisplayInfo()
        {
            var mCount = 0;
            var tCount = 0;
            var xCount = 0;
            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    if (pizza[i, j] == 'M')
                    {
                        mCount++;
                    }
                    if (pizza[i, j] == 'T')
                    {
                        tCount++;
                    }
                    if (pizza[i, j] == 'X')
                    {
                        xCount++;
                    }
                }
            }
            Console.WriteLine($"M {mCount}, T {tCount}, X {xCount}");

            Console.WriteLine(slices.Count);
        }

        private static void FileWriter()
        {
            var path = @"C:\Users\mihaRik\Desktop\hashcode\output.in";
            string line = "";
            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    line += pizza[i, j];
                    if (j == c - 1)
                    {
                        line += "\n";
                    }
                }
            }
            File.WriteAllText(path, line);
        }

        private static void FileSelector(Category category)
        {
            switch (category)
            {
                case Category.Small:
                    stream = new StreamReader(@"C:\Users\mihaRik\Desktop\hashcode\b_small.in");
                    path = @"C:\Users\mihaRik\Desktop\hashcode\small.html";
                    break;
                case Category.Medium:
                    stream = new StreamReader(@"C:\Users\mihaRik\Desktop\hashcode\c_medium.in");
                    path = @"C:\Users\mihaRik\Desktop\hashcode\medium.html";
                    break;
                case Category.Big:
                    stream = new StreamReader(@"C:\Users\mihaRik\Desktop\hashcode\d_big.in");
                    path = @"C:\Users\mihaRik\Desktop\hashcode\big.html";
                    break;
                default:
                    break;
            }
        }

        private static void HtmlCreator()
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine($@"<!DOCTYPE html>
                                <html lang='en'>
                                <head>
                                    <meta charset='UTF-8'>
                                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                                   <meta http-equiv='X-UA-Compatible' content='ie=edge'>
                                    <link rel='stylesheet' href='style.css'>
                                   <title>Title</title>
                                </head>
                                <body>");
                foreach (var item in slices)
                {
                    var color = $@"rgb({rc.Next(255)},{gc.Next(255)},{bc.Next(255)})";
                    sw.WriteLine("<div class='slice'>");
                    foreach (var pi in item.Pieces)
                    {
                        sw.WriteLine($@"<span style='position: absolute; top: {pi.RS * 15}px; left: {pi.CS * 15}px; background: {color}'>{pi.Value}</span>");
                    }
                    sw.WriteLine("</div>");
                }
                sw.WriteLine(@"</body >
                               </html>");
            }
        }

        private static IEnumerable<int> FigureAreaDetector()
        {
            for (int i = 1; i <= h; i++)
            {
                if (h % i == 0)
                {
                    yield return i;
                }
            }
        }
    }
}
