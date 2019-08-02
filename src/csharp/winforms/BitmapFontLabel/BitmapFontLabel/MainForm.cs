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

using System.Drawing;
using System.Windows.Forms;

namespace BitmapFontLabel
{
    public class MainForm : Form
    {
        public MainForm()
        {
            Text = "BitmapFontLabel";
            
            // might be necessary to reduce flickr; but try also without
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            // TODO width for the labels
            var label1 = new BitmapFontLabel
            {
                Text = "ABCDEFGHIJKLMNOPQRST"
            };
            label1.Width = 200;
            var label2 = new BitmapFontLabel
            {
                Text = "CDE"
            };
            label2.Location = new Point(0, label1.Height);
            
            Controls.Add(label1);
            Controls.Add(label2);
            
            label1.IsMarquee = true;
            label2.IsBlinking = true;
        }
    }
}
