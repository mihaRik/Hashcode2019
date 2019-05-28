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
        Example,
        Small,
        Medium,
        Big
    }

    class Program
    {
        static List<Slice> slices = new List<Slice>();
        static List<Slice> _archive = new List<Slice>();
        static int r, c, l, h;
        public static char[,] pizza;
        private static StreamReader stream;
        public static int _divideOrder = 0;
        public static List<int> _dividers;
        private static int _maxTCount;
        private static int _maxMCount;
        private static string path;
        static Random rc = new Random();
        static Random gc = new Random();
        static Random bc = new Random();
        static int constH = h;
        static string text;
        static double _sliceCount;
        static int mCount;
        static int tCount;

        static void Main(string[] args)
        {
            Console.Write("Select curban(1,2,3,4): ");
            var choice = Console.ReadLine();
            Console.WriteLine();
            switch (choice)
            {
                case "1":
                    FileSelector(Category.Example);
                    break;
                case "2":
                    FileSelector(Category.Small);
                    break;
                case "3":
                    FileSelector(Category.Medium);
                    break;
                case "4":
                    FileSelector(Category.Big);
                    break;
                default:
                    break;
            }

            var rules = stream.ReadLine();
            var a = rules.Split(' ');
            r = Convert.ToInt32(a[0]);
            c = Convert.ToInt32(a[1]);
            l = Convert.ToInt32(a[2]);
            h = Convert.ToInt32(a[3]);

            constH = h;
            _sliceCount = r * c / (double)h;

            text = stream.ReadToEnd();
            text = text.Replace("\n", null);
            pizza = new char[r, c];

            mCount = text.Count(x => x == 'M');
            tCount = text.Count(x => x == 'T');

            CreatePizza();
            SetIngridientsCount();

            _dividers = FigureAreaDetector().ToList();

            for (int y = 0; y < r; y++)
            {
                for (int x = 0; x < c; x++)
                {
                    if (pizza[y, x] == 'T' || pizza[y, x] == 'M')
                    {
                        Calculate(x, y);
                    }
                }
            }

            stream.Close();

            DisplayInfo();
            //ShowSlicePos();
            //HtmlCreator();
            //ShowCorrectResult();
        }

        private static void SetIngridientsCount()
        {
            if (slices.Count != 0)
            {
                mCount -= slices[slices.Count - 1].Pieces.Count(x => x.Value == 'M');
                tCount -= slices[slices.Count - 1].Pieces.Count(x => x.Value == 'T');
            }

            if (mCount < tCount)
            {
                var m = (int)Math.Round(mCount / (_sliceCount - slices.Count));
                _maxTCount = h - l;
                _maxMCount = m + (m - l);
            }
            else
            {
                var t = (int)Math.Round(tCount / (_sliceCount - slices.Count));
                _maxMCount = h - l;
                _maxTCount = t + (t - l);
            }
        }

        private static void ShowCorrectResult()
        {
            Console.WriteLine(slices.Count);
            foreach (var item in slices)
            {
                Console.WriteLine(item.GetSliceData());
            }
        }

        private static void Calculate(int x, int y)
        {
            var slice = new Slice();

            slice = PickMostValidSlice(x, y);

            h = constH;

            if (slice != null)
            {
                _divideOrder = 0;
                slices.Add(slice);
                _archive.Clear();
                CuttingPizza();
            }
        }

        private static Slice PickMostValidSlice(int x, int y)
        {
            for (int area = h; area >= l * 2; area--)
            {
                h = area;
                _dividers = FigureAreaDetector().ToList();

                for (_divideOrder = 0; _divideOrder < _dividers.Count; _divideOrder++)
                {
                    if (x + h / _dividers[_divideOrder] <= c &&
                        y + _dividers[_divideOrder] <= r)
                    {
                        var slice = new Slice();
                        CuttingSlice(x, y, slice);
                    }
                }
            }

            var firstMaxSlice = new Slice();

            if (_archive.Count != 0)
            {
                var maxLengthSlice = _archive.Max(s => s.Pieces.Count);
                firstMaxSlice = _archive
                                .Where(s => s.Pieces.Count == maxLengthSlice)
                                .ToList()[0];
            }

            return firstMaxSlice.Pieces.Count != 0 ? firstMaxSlice : null;
        }

        private static void CuttingSlice(int x, int y, Slice slice)
        {
            var result = false;
            var mCountInSlice = 0;
            var tCountInSlice = 0;
            for (int row = 0; row < _dividers[_divideOrder]; row++)
            {
                for (int col = 0; col < h / _dividers[_divideOrder]; col++)
                {
                    var piece = new Piece();
                    result = piece.PieceCreator(row, col, y, x, ref slice);

                    mCountInSlice = slice.Pieces.Count(p => p.Value == 'M');
                    tCountInSlice = slice.Pieces.Count(p => p.Value == 'T');

                    if (!result)
                    {
                        if (slice.Pieces.Count > h &&
                            slice.Pieces.Count < l * 2 &&
                            mCountInSlice <= _maxMCount &&
                            tCountInSlice <= _maxTCount &&
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

            if (slice.Pieces.Count <= h &&
                slice.Pieces.Count >= l * 2 &&
                mCountInSlice <= _maxMCount &&
                tCountInSlice <= _maxTCount &&
                mCountInSlice >= l &&
                tCountInSlice >= l)
            {
                slice.RowSize = _dividers[_divideOrder];
                slice.ColSize = h / _dividers[_divideOrder];
                _archive.Add(slice);
            }
            slice = new Slice();

            _divideOrder++;

            if (_divideOrder < _dividers.Count &&
                x + h / _dividers[_divideOrder] <= c &&
                y + _dividers[_divideOrder] <= r)
            {
                CuttingSlice(x, y, slice);
            }
        }

        private static void CuttingPizza()
        {
            var lastSlice = slices[slices.Count - 1];
            lastSlice.Pieces.ForEach(s => pizza[s.RS, s.CS] = 'X');
        }

        private static void CreatePizza()
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
            var xCountInfo = 0;
            var tCountInfo = 0;
            var mCountInfo = 0;
            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    if (pizza[i, j] == 'X')
                    {
                        xCountInfo++;
                    }
                    if (pizza[i, j] == 'M')
                    {
                        mCountInfo++;
                    }
                    if (pizza[i, j] == 'T')
                    {
                        tCountInfo++;
                    }
                }
            }
            Console.WriteLine($"M {mCountInfo}, T {tCountInfo}, X {xCountInfo}");

            Console.WriteLine(slices.Count);
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
                case Category.Example:
                    stream = new StreamReader(@"C:\Users\mihaRik\Desktop\hashcode\a_example.in");
                    path = @"C:\Users\mihaRik\Desktop\hashcode\example.html";
                    break;
                default:
                    break;
            }
        }

        private static void HtmlCreator()
        {
            var checkList = new List<Piece>();
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
                        checkList.Add(pi);
                    }
                    sw.WriteLine("</div>");
                }

                sw.WriteLine("<div class='slice'>");
                for (int i = 0; i < r; i++)
                {
                    for (int j = 0; j < c; j++)
                    {
                        if (!checkList.Any(x => x.CS == j && x.RS == i))
                        {
                            sw.WriteLine($@"<span style='position: absolute; top: {i * 15}px; left: {j * 15}px; background: rgb(123,53,231)'>{text[i * c + j]}</span>");
                        }
                    }
                }
                sw.WriteLine("</div>");

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

        private static int GetGCD(int x, int y)
        {
            while (Math.Max(x, y) % Math.Min(x, y) != 0)
            {
                int tmp = Math.Max(x, y) % Math.Min(x, y);
                if (x < y) y = tmp;
                else x = tmp;
            }

            return Math.Min(x, y);
        }
    }
}
