using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Submit
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
        private static List<Slice> _slices = new List<Slice>();
        private static List<Slice> _archive = new List<Slice>();
        private static int r, c, l, h;
        public static char[,] _pizza;
        private static StreamReader _stream;
        public static int _divideOrder = 0;
        public static List<int> _dividers;
        private static int _maxTCount;
        private static int _maxMCount;
        private static int constH = h;
        private static string _text;
        private static double _sliceCount;
        private static int _mCount;
        private static int _tCount;
        private static string _path;

        static void Main(string[] args)
        {
            Console.WriteLine("1. Example");
            Console.WriteLine("2. Small");
            Console.WriteLine("3. Medium");
            Console.WriteLine("4. Big");
            Console.Write("Select input file(1, 2, 3, 4): ");
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

            var rules = _stream.ReadLine();
            var a = rules.Split(' ');
            r = Convert.ToInt32(a[0]);
            c = Convert.ToInt32(a[1]);
            l = Convert.ToInt32(a[2]);
            h = Convert.ToInt32(a[3]);

            constH = h;
            _sliceCount = r * c / (double)h;

            _text = _stream.ReadToEnd();
            _text = _text.Replace("\n", null);
            _pizza = new char[r, c];

            _mCount = _text.Count(x => x == 'M');
            _tCount = _text.Count(x => x == 'T');

            CreatePizza();
            SetIngridientsCount();

            for (int y = 0; y < r; y++)
            {
                for (int x = 0; x < c; x++)
                {
                    if (_pizza[y, x] == 'T' || _pizza[y, x] == 'M')
                    {
                        Calculate(x, y);
                    }
                }
            }


            _stream.Close();

            ShowCorrectResult();
        }

        private static void SetIngridientsCount()
        {
            if (_slices.Count != 0)
            {
                _mCount -= _slices[_slices.Count - 1].Pieces.Count(x => x.Value == 'M');
                _tCount -= _slices[_slices.Count - 1].Pieces.Count(x => x.Value == 'T');
            }

            if (_mCount < _tCount)
            {
                var m = (int)Math.Round(_mCount / (_sliceCount - _slices.Count));
                _maxTCount = h - l;
                _maxMCount = m + (m - l);
            }
            else
            {
                var t = (int)Math.Round(_tCount / (_sliceCount - _slices.Count));
                _maxMCount = h - l;
                _maxTCount = t + (t - l);
            }
        }

        private static void ShowCorrectResult()
        {
            using (StreamWriter stream = File.CreateText(_path))
            {
                stream.WriteLine(_slices.Count);
                foreach (var item in _slices)
                {
                    stream.WriteLine(item.GetSliceData());
                }
            }
        }

        private static void Calculate(int x, int y)
        {
            var slice = PickMostValidSlice(x, y);

            h = constH;

            if (slice != null)
            {
                _slices.Add(slice);
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
                var maxColSize = _archive.Max(s => s.ColSize);
                firstMaxSlice = _archive
                                .Where(s => s.ColSize == maxColSize)
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
            var lastSlice = _slices[_slices.Count - 1];
            lastSlice.Pieces.ForEach(s => _pizza[s.RS, s.CS] = 'X');
        }

        private static void CreatePizza()
        {
            for (int y = 0; y < r; y++)
            {
                for (int x = 0; x < c; x++)
                {
                    _pizza[y, x] = _text[y * c + x];
                }
            }
        }

        private static void FileSelector(Category category)
        {
            switch (category)
            {
                case Category.Small:
                    _stream = new StreamReader(Path.GetFullPath("b_small.in"));
                    _path = Path.GetFullPath("b_small.out");
                    break;
                case Category.Medium:
                    _stream = new StreamReader(Path.GetFullPath("c_medium.in"));
                    _path = Path.GetFullPath("c_medium.out");
                    break;
                case Category.Big:
                    _stream = new StreamReader(Path.GetFullPath("d_big.in"));
                    _path = Path.GetFullPath("d_big.out");
                    break;
                case Category.Example:
                    _stream = new StreamReader(Path.GetFullPath("a_example.in"));
                    _path = Path.GetFullPath("a_example.out");
                    break;
                default:
                    break;
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
