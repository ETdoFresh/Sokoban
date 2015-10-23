using System;
using System.Collections.Generic;
using System.IO;

class LevelReader
{
    public static char[,] ReadLevel(string filename)
    {
        char[,] level = null;
        List<string> lines = new List<string>();
        using (StreamReader reader = new StreamReader(filename))
        {
            int height = 0;
            int width = 0;
            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                height++;
                width = Math.Max(width, line.Length);
                lines.Add(line);
            }

            level = new char[width, height];
            for (int y = 0; y < lines.Count; y++)
                for (int x = 0; x < lines[y].Length; x++)
                    level[x, y] = lines[y][x];
        }
        return level;
    }
}
