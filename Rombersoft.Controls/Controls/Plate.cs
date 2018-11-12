using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rombersoft.Controls;
using Microsoft.Xna.Framework.Graphics;

namespace Rombersoft.Controls
{
    public class Plate: Control
    {
        private int _x, _y, _width, _height; //1689,346,357,100
        private Rectangle _rectSource1, _rectSource2, _rectSource3, _rectSource4, _rectSource5, _rectSource6, _rectSource7, _rectSource8, _rectSource9;
        private Rectangle[] _rectDest;
        private byte _density;
        public bool FixedSize { get; private set; }
        public object Tag { get; set; }
        private Point[] _regionScope;
        private Rectangle _directedRegion;
        public Rectangle DirectedRegion 
        { 
            get { return _directedRegion; } 
            set
            {
                _directedRegion = value; 
                _regionScope = new Point[4]{new Point(Rect.X, Rect.Y), new Point(Rect.X + Rect.Width, Rect.Y), 
                    new Point(Rect.X + Rect.Width, Rect.Y + Rect.Height),
                    new Point(Rect.X, Rect.Y + Rect.Height)};
            } 
        }

        public Plate(int x, int y, int width, int height, bool fixedSize, byte density) : base()
        {
            FixedSize = fixedSize;
            _x = x;
            _y = y;
            _width = width < 21 ? 21 : width;
            _height = height < 21 ? 21 : height;
            _rectSource1 = new Rectangle(1689, 346, 10, 10);
            _rectSource2 = new Rectangle(1699, 346, 337, 10);
            _rectSource3 = new Rectangle(2036, 346, 10, 10);
            _rectSource4 = new Rectangle(1689, 356, 10, 80);
            _rectSource5 = new Rectangle(1699, 356, 337, 80);
            _rectSource6 = new Rectangle(2036, 356, 10, 80);
            _rectSource7 = new Rectangle(1689, 436, 10, 10);
            _rectSource8 = new Rectangle(1699, 436, 337, 10);
            _rectSource9 = new Rectangle(2036, 436, 10, 10);

            _rectDest = new Rectangle[9];
            _rectDest[0] = new Rectangle(_x, _y, 10, 10);
            _rectDest[1] = new Rectangle(_x + 10, _y, _width - 20, 10);
            _rectDest[2] = new Rectangle(_x + _width - 10, _y, 10, 10);
            _rectDest[3] = new Rectangle(_x, _y + 10, 10, _height - 20);
            _rectDest[4] = new Rectangle(_x+10, _y+10, _width-20, _height -20);
            _rectDest[5] = new Rectangle(_x + _width - 10, y + 10, 10, _height - 20);
            _rectDest[6] = new Rectangle(_x, _y + _height - 10, 10, 10);
            _rectDest[7] = new Rectangle(_x + 10, _y + height - 10, _width - 20, 10);
            _rectDest[8] = new Rectangle(_x + _width - 10, _y + _height - 10, 10, 10);
            Rect = new Rectangle(x, y, width, height);
            DirectedRegion = GraphicInstance.Window;
            _density = density;
        }

        public byte DensityBG
        {
            get { return _density; }
            set { _density = value; }
        }


        public void Resize(int deltaWidth, int deltaHeight)
        {
            _width += deltaWidth;
            _height += deltaHeight;
            _rectDest[3] = new Rectangle(_x, _y + 10, 10, _height - 20);
            _rectDest[4] = new Rectangle(_x+10, _y+10, _width-20, _height -20);
            _rectDest[5] = new Rectangle(_x + _width - 10, _y + 10, 10, _height - 20);
            _rectDest[6] = new Rectangle(_rectDest[6].X + deltaWidth, _rectDest[6].Y + deltaHeight, _rectDest[6].Width, _rectDest[6].Height);
            _rectDest[7] = new Rectangle(_rectDest[7].X + deltaWidth, _rectDest[7].Y + deltaHeight, _rectDest[7].Width+deltaWidth, _rectDest[7].Height);
            _rectDest[8] = new Rectangle(_rectDest[8].X + deltaWidth, _rectDest[8].Y + deltaHeight, _rectDest[8].Width, _rectDest[8].Height);
            Rect = new Rectangle(_x, _y, _width, _height);
        }

        public override bool ClickInControl()
        {
            if(RectInDirectedRegion())
                return ClickInternal();
            return false;
        }

        private bool RectInDirectedRegion()
        {
            return (DirectedRegion.Contains(_regionScope[0]) || DirectedRegion.Contains(_regionScope[1]) 
                || DirectedRegion.Contains(_regionScope[2]) || DirectedRegion.Contains(_regionScope[3]));
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (byte i = 0; i < _density; i++)
            {
                spriteBatch.Draw(ControlResources.Atlas, _rectDest[0], _rectSource1, Color.White);
                spriteBatch.Draw(ControlResources.Atlas, _rectDest[1], _rectSource2, Color.White);
                spriteBatch.Draw(ControlResources.Atlas, _rectDest[2], _rectSource3, Color.White);
                spriteBatch.Draw(ControlResources.Atlas, _rectDest[3], _rectSource4, Color.White);
                spriteBatch.Draw(ControlResources.Atlas, _rectDest[4], _rectSource5, Color.White);
                spriteBatch.Draw(ControlResources.Atlas, _rectDest[5], _rectSource6, Color.White);
                spriteBatch.Draw(ControlResources.Atlas, _rectDest[6], _rectSource7, Color.White);
                spriteBatch.Draw(ControlResources.Atlas, _rectDest[7], _rectSource8, Color.White);
                spriteBatch.Draw(ControlResources.Atlas, _rectDest[8], _rectSource9, Color.White);
            }
            DrawInternal(spriteBatch);
        }

        public override void Offset(int x, int y)
        {
            _x += x;
            _y += y;
            for(int i=0; i<_rectDest.Length; i++)
            {
                _rectDest[i].Offset(x, y);
            }
            for (ushort i = 0; i < Controls.Count; i++)
                Controls[i].Offset(x, y);
            Rect = new Rectangle(_x, _y, _width, _height);
            _regionScope = new Point[4]{new Point(Rect.X, Rect.Y), new Point(Rect.X + Rect.Width, Rect.Y), new Point(Rect.X + Rect.Width, Rect.Y + Rect.Height),
                new Point(Rect.X, Rect.Y + Rect.Height)};
        }

        public override void SetPosition(int x, int y)
        {
            int diffX = x - _x;
            int diffY = y - _y;
            for (int i = 0; i < _rectDest.Length; i++)
            {
                _rectDest[i].Offset(diffX, diffY);
            }
            for (ushort i = 0; i < Controls.Count; i++)
                Controls[i].Offset(diffX, diffY);
            Rect = new Rectangle(x, y, _width, _height);
            _regionScope = new Point[4]{new Point(Rect.X, Rect.Y), new Point(Rect.X + Rect.Width, Rect.Y), new Point(Rect.X + Rect.Width, Rect.Y + Rect.Height),
                new Point(Rect.X, Rect.Y + Rect.Height)};
            _x = x;
            _y = y;
        }
    }
}
