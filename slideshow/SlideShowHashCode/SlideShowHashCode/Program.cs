using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SlideShowHashCode
{
    enum SlideShows
    {
        Example,
        Landscapes,
        Moments,
        Pet,
        Selfie
    }

    class Program
    {
        private static StreamReader _stream;
        private static Image[] _input;
        private static List<Slide> _readyVerticalSlide;
        private static List<Slide> _output;
        private static string _path;

        static void Main(string[] args)
        {
            Console.Write("Choose file: ");
            var choice = Convert.ToInt32(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    ChooseFile(SlideShows.Example);
                    break;
                case 2:
                    ChooseFile(SlideShows.Landscapes);
                    break;
                case 3:
                    ChooseFile(SlideShows.Moments);
                    break;
                case 4:
                    ChooseFile(SlideShows.Pet);
                    break;
                case 5:
                    ChooseFile(SlideShows.Selfie);
                    break;
                default:
                    break;
            }
            var imageCount = Convert.ToInt32(_stream.ReadLine());
            _input = new Image[imageCount];
            _readyVerticalSlide = new List<Slide>();
            _output = new List<Slide>();
            var file = _stream.ReadToEnd();
            var eachRow = file.Split('\n');

            for (int i = 0; i < imageCount; i++)
            {
                var photoProperties = eachRow[i].Split(' ');
                var photoOrientation = Convert.ToChar(photoProperties[0]);
                var photoTagCount = Convert.ToInt32(photoProperties[1]);
                var tagsArr = photoProperties.Skip(2).ToArray();
                var image = new Image()
                {
                    Id = i,
                    Orientation = photoOrientation,
                    TagCount = photoTagCount,
                    Tags = tagsArr,
                };
                _input[i] = image;
            }
            var verticalImages = _input.Where(i => i.Orientation == 'V').ToList();

            for (int i = 0; i < verticalImages.Count; i++)
            {
                for (int j = i; j < verticalImages.Count; j++)
                {
                    if (!verticalImages[j].IsChecked)
                    {
                        VerticalsToHorizontal(verticalImages[i], verticalImages[j]);
                    }
                }
            }

            var horizontalImages = _input.Where(i => i.Orientation == 'H').ToList();

            foreach (var img in horizontalImages)
            {
                var horImg = new Slide
                {
                    Id = img.Id,
                    Img1 = img,
                    TagCount = img.TagCount,
                    Tags = img.Tags,
                    Type = 'H',
                };
                _readyVerticalSlide.Add(horImg);
            }

            _readyVerticalSlide[0].IsChecked = true;
            _output.Add(_readyVerticalSlide[0]);

            for (int i = 0; i < _readyVerticalSlide.Count; i++)
            {
                for (int j = i; j < _readyVerticalSlide.Count; j++)
                {
                    if (!_readyVerticalSlide[j].IsChecked)
                    {
                        if (InterestFactor(_output.Last(), _readyVerticalSlide[j]) > 1)
                        {
                            _readyVerticalSlide[j].IsChecked = true;
                            _output.Add(_readyVerticalSlide[j]);
                        }
                    }
                }
            }

            foreach (var item in _output)
            {
                if (item.Type == 'V')
                {
                    Console.WriteLine(item.Img1.Id + " " + item.Img2.Id);
                }
                else
                {
                    Console.WriteLine(item.Img1.Id);
                }
            }

            _stream.Close();

            GenerateOutput();
        }

        static void ChooseFile(SlideShows slideShows)
        {
            switch (slideShows)
            {
                case SlideShows.Example:
                    _stream = new StreamReader(Path.GetFileName("a_example.txt"));
                    _path = Path.GetFullPath("a_example.out");
                    break;
                case SlideShows.Landscapes:
                    _stream = new StreamReader(Path.GetFileName("b_lovely_landscapes.txt"));
                    _path = Path.GetFullPath("b_lovely_landscapes.out");
                    break;
                case SlideShows.Moments:
                    _stream = new StreamReader(Path.GetFileName("c_memorable_moments.txt"));
                    _path = Path.GetFullPath("c_memorable_moments.out");
                    break;
                case SlideShows.Pet:
                    _stream = new StreamReader(Path.GetFileName("d_pet_pictures.txt"));
                    _path = Path.GetFullPath("d_pet_pictures.out");
                    break;
                case SlideShows.Selfie:
                    _stream = new StreamReader(Path.GetFileName("e_shiny_selfies.txt"));
                    _path = Path.GetFullPath("e_shiny_selfies.out");
                    break;
                default:
                    break;
            }
        }

        static int InterestFactor(Image img1, Image img2)
        {
            var img1Tags = img1.Tags;
            var img2Tags = img2.Tags;
            var img1OwnTags = img1.Tags.ToList();
            var img2OwnTags = img2.Tags.ToList();

            var commonTags = new List<string>();

            for (int i = 0; i < img1Tags.Length; i++)
            {
                for (int j = 0; j < img2Tags.Length; j++)
                {
                    if (img1Tags[i] == img2Tags[j])
                    {
                        commonTags.Add(img2Tags[j]);
                        img1OwnTags.Remove(img1Tags[i]);
                        img2OwnTags.Remove(img2Tags[j]);
                    }
                }
            }

            var min = new int[] { commonTags.Count, img1OwnTags.Count, img2OwnTags.Count };
            return min.Min();
        }

        static int InterestFactor(Slide slide1, Slide slide2)
        {
            var slide1Tags = slide1.Tags;
            var slide2Tags = slide2.Tags;
            var slide1OwnTags = slide1.Tags.ToList();
            var slide2OwnTags = slide2.Tags.ToList();

            var commonTags = new List<string>();

            for (int i = 0; i < slide1Tags.Length; i++)
            {
                for (int j = 0; j < slide2Tags.Length; j++)
                {
                    if (slide1Tags[i] == slide2Tags[j])
                    {
                        commonTags.Add(slide2Tags[j]);
                        slide1OwnTags.Remove(slide1Tags[i]);
                        slide2OwnTags.Remove(slide2Tags[j]);
                    }
                }
            }

            var min = new int[] { commonTags.Count, slide1OwnTags.Count, slide2OwnTags.Count };
            return min.Min();
        }

        static void VerticalsToHorizontal(Image img1, Image img2)
        {
            if (img1.Orientation == 'V' && img2.Orientation == 'V')
            {
                var img1Tags = img1.Tags;
                var img2Tags = img2.Tags;
                var img1OwnTags = img1.Tags.ToList();
                var img2OwnTags = img2.Tags.ToList();

                var commonTags = new List<string>();

                for (int i = 0; i < img1Tags.Length; i++)
                {
                    for (int j = 0; j < img2Tags.Length; j++)
                    {
                        if (img1Tags[i] == img2Tags[j])
                        {
                            commonTags.Add(img2Tags[j]);
                            img1OwnTags.Remove(img1Tags[i]);
                            img2OwnTags.Remove(img2Tags[j]);
                        }
                    }
                }
                var tagInfo = new int[] { commonTags.Count, img1OwnTags.Count, img2OwnTags.Count };
                var tags = new List<string>();

                tags.AddRange(img1OwnTags);
                tags.AddRange(img2OwnTags);
                tags.AddRange(commonTags);

                if (InterestFactor(img1, img2) > 1)
                {
                    var slide = new Slide()
                    {
                        Id = img1.Id,
                        Img1 = img1,
                        Img2 = img2,
                        Type = 'V',
                        TagCount = tagInfo.Sum(),
                        Tags = tags.ToArray(),
                        IsChecked = true
                    };
                    _readyVerticalSlide.Add(slide);
                };
            }
        }

        static void GenerateOutput()
        {
            using (StreamWriter stream = File.CreateText(_path))
            {
                stream.WriteLine(_output.Count);
                foreach (var output in _output)
                {
                    if (output.Type == 'V')
                    {
                        stream.WriteLine(output.Img1.Id + " " + output.Img2.Id);
                    }
                    else
                    {
                        stream.WriteLine(output.Img1.Id);
                    }
                }
            }
        }
    }
}