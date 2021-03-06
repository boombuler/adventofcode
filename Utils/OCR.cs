﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Utils
{
    abstract class OCR
    {
        public delegate bool IsSetDelegate(int x, int y);
        public abstract int CharWidth { get; }
        public abstract int CharHeight { get; }
        public abstract int Spacing { get; }

        /// <param name="charMap">a pixel arrax with first coordinate x and second y</param>
        /// <param name="setPixel"></param>
        /// <returns></returns>
        public string Decode(char[,] charMap, char setPixel = '#')
            => Decode((x,y) => charMap[x, y] == setPixel, charMap.GetLength(0));

        public string Decode(IsSetDelegate testPixel, int totalWidth)
        {
            var lt = GetLookupTable();
            var result = new StringBuilder();

            for (int cx = 0; cx < totalWidth; cx += (CharWidth + Spacing))
            {
                var letter = ExtractLetter(testPixel, cx);
                if (lt.TryGetValue(letter, out char c))
                    result.Append(c);
                else
                    throw new Exception("Unknown character");
            }
            return result.ToString();
        }

        private Dictionary<string, char> GetLookupTable()
        {
            var (letter, drawing) = GetAlphabet();
            drawing = drawing.Replace("\r", string.Empty).Replace("\n", string.Empty);

            int lineWidth = letter.Length * (CharWidth + Spacing) - Spacing;
            if (lineWidth != (drawing.Length / CharHeight))
                throw new Exception("Invalid Font Drawing.");

            IsSetDelegate testChar = (x, y) => drawing[y * lineWidth + x] == '#';

            return letter
                .Select((c, i) => new { Char = c, Drawing = ExtractLetter(testChar, i * (CharWidth + Spacing)) })
                .ToDictionary(x => x.Drawing, x => x.Char);
        }

        private string ExtractLetter(IsSetDelegate test, int startX)
        {
            char[] letter = new char[CharWidth * CharHeight];
            for (int y = 0; y < CharHeight; y++)
                for (int x = 0; x < CharWidth; x++)
                    letter[(y * CharWidth) + x] = test(startX + x, y) ? '#' : '.';
            return new string(letter);
        }

        protected abstract (string letters, string drawing) GetAlphabet();
    }
}
