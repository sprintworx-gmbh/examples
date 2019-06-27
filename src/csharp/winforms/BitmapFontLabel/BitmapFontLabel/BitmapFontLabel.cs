using System;
using System.Windows.Forms;

namespace BitmapFontLabel
{
    /// <summary>
    /// Renders a text from bitmaps. The font property is ignored.
    /// load a bitmap font via: 
    /// </summary>
    public class BitmapFontLabel : Label
    {
        private readonly BitmapFont _bitmapFont;
        private readonly Timer _blinkTimer;
        private readonly Timer _marqueeTimer;
        private bool _renderText; // used for blinking
        private int _marqueeDist; // distance for text scrolling in pixel
        
        private bool _isBlinking;
        public bool IsBlinking
        {
            get { return _isBlinking; }
            set
            {
                _isBlinking = value;
                
                if(value)
                    _blinkTimer.Start();
                else
                {
                    _blinkTimer.Stop();
                    _renderText = true;
                }
            }
        }
        
        private bool _isMarquee;
        public bool IsMarquee
        {
            get { return _isMarquee; }
            set
            {
                _isMarquee = value; 
                
                if(value)
                    _marqueeTimer.Start();
                else
                {
                    _marqueeTimer.Stop();
                    _marqueeDist = 0;
                }
            }
        }

        public BitmapFontLabel()
        {
            _renderText = true;
            
            _blinkTimer = new Timer();
            _blinkTimer.Tick += OnBlinkTimerTicked;
            _blinkTimer.Interval = 500; // in ms

            _marqueeTimer = new Timer();
            _marqueeTimer.Tick += OnMarqueeTimerTicked;
            _marqueeTimer.Interval = 50;
            
            // on timer: Invalidate(true)
            _bitmapFont = new BitmapFont();
            
            // TODO modify for your needs
            _bitmapFont.LoadFrom("Fonts/Font1");
            
            Height = _bitmapFont.FontHeight;
        }

        private void OnBlinkTimerTicked(object sender, EventArgs e)
        {
            _renderText = !_renderText;

            // cause repaint
            Invalidate(true);
        }

        private void OnMarqueeTimerTicked(object sender, EventArgs e)
        {
            // TODO potential optimization: measure could be executed only if Text changed
            if(_marqueeDist < -_bitmapFont.MeasureWidth(Text))
                _marqueeDist = Width;
            
            _marqueeDist--;
            // cause repaint
            Invalidate(true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if(!_renderText)
                return;
            
            var g = e.Graphics;
            
            // TODO eventually: g.Clear(BackColor); and set BackColor to the same color as the fonts background

            _bitmapFont.DrawText(g, Text, _marqueeDist);
            
            // would render the label with normal font rendering
            //base.OnPaint(e);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _bitmapFont?.Dispose();
        }
    }
}