using System;
using Microsoft.Xna.Framework.Input;

namespace Rombersoft.Controls
{
    public struct MyMouseState
    {
        public int X { get; private set;}
        public int Y { get; private set;}
        public int ScrollWheelValue { get; private set;}
        public ButtonState LeftButton { get; private set;}
        public ButtonState RightButton { get; private set;}
        public ButtonState MiddleButton { get; private set;}

        public MyMouseState(int x, int y, ButtonState leftButton, ButtonState rightButton, ButtonState middleButton, int scrollWheelValue)
        {
            X = x;
            Y = y;
            LeftButton = leftButton;
            RightButton = rightButton;
            MiddleButton = middleButton;
            ScrollWheelValue = scrollWheelValue;
        }
    }
}

