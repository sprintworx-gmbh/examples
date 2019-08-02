// License: MIT
// see LICENSE file in this folder, if this file is not preset refer to
// https://github.com/sprintworx-gmbh/examples/blob/master/LICENSE
// https://opensource.org/licenses/MIT
// https://mit-license.org/
//
// The following copyright applies
//
// Copyright © 2019 SprintWORX GmbH, authors: Matthias Fritsch and Daniel Bişar
//
// <>< he is alive

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace BitmapFontLabel
{
    /// <summary>
    /// Loads files from a folder. Format
    /// NUMBER.png where number is the index of a character in the character table
    /// if a specific char is not defined fallback.png is used if present in the folder.
    ///
    /// The folder Fonts/Font1 is an example font. Just a few pictures.
    /// If you want to use anything else than png modify the FilePattern below
    /// 
    /// </summary>
    public class BitmapFont : IDisposable
    {
        private const string FilePattern = "*.png";
        
        private readonly List<Image> _glyphs;
        private Image _fallbackBitmap;
        
        public int FontHeight { get; private set; }
        public int FontWidth {get; private set;}
        
        public BitmapFont()
        {
            _glyphs = new List<Image>();
        }

        public void LoadFrom(string directory)
        {
            FontHeight = 0;
            FontWidth = 0;
            
            // know limitations: FontWidth and FontHeight is not influenced by fallback image
            
            DisposeBitmaps();
            _glyphs.Clear();
            
            var files = Directory.GetFiles(directory, FilePattern);

            foreach (var fileName in files)
            {
                var name = Path.GetFileNameWithoutExtension(fileName);
                
                // is the name a number
                if (int.TryParse(name, out var n))
                {
                    AssureGlyphsCount(n);
                    var image = LoadBitmap(fileName);
                    _glyphs[n] = image;
                    
                    if(FontHeight < image.Height)
                        FontHeight = image.Height;
                    
                    if(FontWidth < image.Width)
                        FontWidth = image.Width;
                }
                else if(name == "fallback")
                    _fallbackBitmap = LoadBitmap(fileName);
            }
        }

        private void AssureGlyphsCount(int index)
        {
            while(_glyphs.Count <= index)
                _glyphs.Add(null); // add an empty entry, so that we later will be able to access the list just
                                        // via the index (numeric value) of a char
        }

        private void DisposeBitmaps()
        {
            // ? after bitmap means: call function only if object is not null
            foreach(var bitmap in _glyphs)
                bitmap?.Dispose();
            
            _fallbackBitmap?.Dispose();
        }

        private Image LoadBitmap(string fileName)
        {
            // you could also modify the bitmap here or do some setup
            var image = Image.FromFile(fileName);
            return image;
        }

        public void DrawText(Graphics g, string text, int x = 0)
        {
            // iteration over list and array with index is slightly faster than foreach
            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];
                var image = GetImage(c);

                // Debug is not include in release builds, so the if will be removed too
                if (image == null)
                    Debug.WriteLine("Could not render character: " + c + " - no fallback image");
                else
                {
                    g.DrawImageUnscaled(image, x, 0);
                    x += FontWidth;
                }
            }
        }

        private Image GetImage(char c)
        {
            var n = (int)c;
            
            if(n >= _glyphs.Count)
                return _fallbackBitmap;
            
            // return the image from glyphs, if the value is null take _fallbackBitmap
            return _glyphs[n] ?? _fallbackBitmap;
        }

        public void Dispose()
        {
            DisposeBitmaps();
        }

        public int MeasureWidth(string text)
        {
            return text.Length * FontWidth;
        }
    }
}
