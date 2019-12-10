using System;
using System.IO;
using System.Collections.Generic;

namespace EvenFinancial
{
    class Program
    {
        static void Main(string[] args)
        {

            //Convert text file into "image"
            var image = ReadInFile(args[0]);

            int xBound = image.GetUpperBound(0);
            int yBound = image.GetUpperBound(1);

            printImage(image);

            var usedPoints = new List<Tuple<int, int>>();
            var blobs = new List<List<Tuple<int, int>>>();

            //Retrieve Blobs from image
            for (int y = 0; y <= yBound; y++)
            {
                for (int x = 0; x <= xBound; x++)
                {
                    var b = new List<Tuple<int, int>>();
                    if (image[x, y] == 1 && !usedPoints.Contains(Tuple.Create(x, y)))
                    {
                        var g = findAdjacentPixels(image, x, y, b);
                        blobs.Add(g);
                        usedPoints.AddRange(g);
                    }
                }
            }

            //Convert the blobs into boxes
            var boxesList = new List<Tuple<Tuple<int, int>, Tuple<int, int>>>();
            foreach(var blob in blobs)
            {
                boxesList.Add(GetBox(blob));
            }

            //Remove Overlapping Boxes
            var retList = new List<Tuple<Tuple<int, int>, Tuple<int, int>>>();
            for (int i = 0; i < boxesList.Count; i++)
            {
                var checkList = new List<Tuple<Tuple<int, int>, Tuple<int, int>>>(boxesList);
                checkList.RemoveAt(i);

                if (isPixel(boxesList[i]) || !haveAnyOverlaps(checkList, boxesList[i]))
                {
                    retList.Add(boxesList[i]);
                }
            }

            //Find Boxes with largest area
            var areas = new List<int>();
            foreach(var box in retList)
            {
                areas.Add(getArea(box));
            }

            areas.Sort();
            areas.Reverse();

            if(retList.Count > 0)
            {
                var largestArea = areas[0];
                for(int i = 0; i < retList.Count; i++)
                {
                    if(getArea(retList[i]) == largestArea)
                    {
                        Console.WriteLine($"({retList[i].Item1.Item1 + 1}, {retList[i].Item1.Item2 + 1}) ({retList[i].Item2.Item1 + 1}, {retList[i].Item2.Item2 + 1})");
                    }
                }
            }
        }
        
        //Retrieves the area of a box given two points, located in top left and bottom right
        public static int getArea(Tuple<Tuple<int, int>, Tuple<int, int>> t)
        {
            var w = (t.Item2.Item1 - t.Item1.Item1) + 1;
            var l = (t.Item2.Item2 - t.Item1.Item2) + 1;

            return l * w;
        }

        //Determines if given coordinates are a single pixel
        private static bool isPixel(Tuple<Tuple<int, int>, Tuple<int, int>> c)
        {
            if (c.Item1.Item1 == c.Item2.Item1 && c.Item1.Item2 == c.Item2.Item2)
                return true;
            else
                return false;
        }

        //Determines whether 2 given boxes overlap or not
        private static bool doOverlap(Tuple<int, int> l1, Tuple<int, int> r1, Tuple<int, int> l2, Tuple<int, int> r2)
        {
            if (l1.Item1 > r2.Item1 || l2.Item1 > r1.Item1)
            {
                return false;
            }

            if (l1.Item2 > r2.Item2 || l2.Item2 > r1.Item2)
            {
                return false;
            }

            return true;
        }

        //Check across all provided boxes to see if there are any overlaps
        private static bool haveAnyOverlaps(List<Tuple<Tuple<int, int>, Tuple<int, int>>> checkList, Tuple<Tuple<int, int>, Tuple<int, int>> coodinate )
        {
            var topLeft = coodinate.Item1;
            var botRight = coodinate.Item2;

            foreach (var box in checkList)
            {
                var boxTopLeft = box.Item1;
                var boxBotRight = box.Item2;

                if(doOverlap(topLeft, botRight, boxTopLeft, boxBotRight))
                {
                    return true;
                }
            }
            return false;
        }

        //Takes a blob and creates a minimum-bounding box for it
        public static Tuple<Tuple<int,int>, Tuple<int, int>> GetBox(List<Tuple<int, int>> blob)
        {
            var xMin = blob[0].Item1;
            var yMin = blob[0].Item2;

            var xMax = blob[0].Item1;
            var yMax = blob[0].Item2;

            foreach(var c in blob)
            {
                if(c.Item1 < xMin)
                {
                    xMin = c.Item1;
                }
                if (c.Item2 < yMin)
                {
                    yMin = c.Item2;
                }
                if (c.Item1 > xMax)
                {
                    xMax = c.Item1;
                }
                if (c.Item2 > yMax)
                {
                    yMax = c.Item2;
                }
            }

            return Tuple.Create(
                       Tuple.Create(xMin, yMin),
                       Tuple.Create(xMax, yMax));
        }

        //Prints the provided grid
        public static void printImage(int[,] image)
        {
                int xBound = image.GetUpperBound(0);
                int yBound = image.GetUpperBound(1);

                for (int y = 0; y <= yBound; y++)
                {
                    for (int x = 0; x <= xBound; x++)
                    {
                        Console.Write(image[x, y]);
                    }
                    Console.WriteLine();
                }
        }

        //Finds all of the touching/adjacent pixels given a coordinate 
        public static List<Tuple<int, int>> findAdjacentPixels(int[,] image, int x, int y, List<Tuple<int, int>> list)
        {
            list.Add(Tuple.Create(x, y));

            var xBound = image.GetUpperBound(0);
            var yBound = image.GetUpperBound(1);

            //Right
            if (x+1 <= xBound)
            {
                if (image[x+1,y] == 1 && !list.Contains(Tuple.Create(x + 1, y)))
                {
                    findAdjacentPixels(image, x + 1, y, list);
                }
            }//Left
            if (x - 1 >= 0)
            {
                if (image[x - 1, y] == 1 && !list.Contains(Tuple.Create(x - 1, y)))
                {
                    findAdjacentPixels(image, x - 1, y, list);
                }
            }//Below
            if ( y + 1 <= yBound)
            {
                if (image[x, y + 1] == 1 && !list.Contains(Tuple.Create(x, y + 1)))
                {
                    findAdjacentPixels(image, x, y + 1, list);
                }
            }//Below
            if (y - 1 >= 0)
            {
                if (image[x, y - 1] == 1 && !list.Contains(Tuple.Create(x, y - 1)))
                {
                    findAdjacentPixels(image, x, y - 1, list);
                }
            }

            return list;
        }

        //Reads in a file of '-'s and '*'s and converts it into an "image"
        public static int[,] ReadInFile(string fileLocation)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var fullPath = path + "\\" + fileLocation;

            if (!File.Exists(fullPath))
            {
                throw new Exception("Unable to find given file. Please check if file exists within program directory.");
            }

            string[] lines = System.IO.File.ReadAllLines(fullPath);

            int[,] image = new int[lines[0].Length, lines.Length];

            int y = 0;
            foreach (var line in lines)
            {
                int x = 0;
                foreach (var c in line)
                {
                    if (c == '*')
                    {
                        image[x, y] = 1;
                    }
                    x++;
                }
                y++;
            }

            return image;
        }
    }
}
