using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Rombersoft.Controls
{
    public class ListBox : Control
    {
        private ScrollBar _scrollBar;
        private float _step = 0;
        private int _selectedIndex, _itemHeight, _firstItemIndex, _lastItemIndex, _itemsVisibledQuantity, _itemsCount;
        private Rectangle[] _rectFrameSources, _rectFrameDests, _rectSelectedItemSources, _rectSelectedItemDests;
        private List<Item> _list;
        private bool _isPressed = false;
        private Item _selectedItem;
        private Action _syncInvoker;
        public event Action<object, int> OnCnahgeSelectedItem;
        public Color ForeColor { get; set; }
        public Color FrameColor { get; set; }
        public Color GuideColor
        {
            get { return _scrollBar.GuideColor; }
            set 
            { 
                _scrollBar.GuideColor = value; 
            }
        }

        public Color ThumbColor 
        {
            get { return _scrollBar.ButtonsColor; }
            set { _scrollBar.ButtonsColor = value; } 
        }

        public Color SelectedItemColor { get; set; }

        public ListBox(int x, int y, int width, int height)
        {
            _selectedIndex = -1;
            _selectedItem = null;
            _firstItemIndex = 0;
            _lastItemIndex = 0;
            _list = new List<Item>();
            _syncInvoker = null;
            Rect = new Rectangle(x, y, width, height);
            CorrectSizes();
            ForeColor = FrameColor = ThumbColor = SelectedItemColor = ControlResources.DefaultColor;
        }

        public List<Item> Items
        {
            get { return _list; }
            set { _list = value; }
        }

        public void CorrectSizes()
        {
            _itemHeight = 1 + (int)Font.MeasureString("A").Y;
            if (Rect.Height + 2 % _itemHeight > 0)
            {
                if (Rect.Height % _itemHeight > _itemHeight / 2) Rect.Height = (Rect.Height / _itemHeight) * _itemHeight + _itemHeight + 2;
                else Rect.Height = Rect.Height / _itemHeight * _itemHeight + 2;
            }
            _itemsVisibledQuantity = Rect.Height / _itemHeight;
            _rectSelectedItemSources = SpliteRect(ControlResources.AtlasRegions["RoundSquare"], 12);
            _rectSelectedItemDests = SpliteRect(new Rectangle(0, 0, Rect.Width - 4, _itemHeight), 12);
            _rectFrameSources = SpliteRect(ControlResources.AtlasRegions["Frame"], 12);
            _rectFrameDests = SpliteRect(Rect, 12);
            if (_scrollBar != null)
            {
                Color scrollButtonsColor = _scrollBar.ButtonsColor;
                Color scrollGuideColor = _scrollBar.GuideColor;
                _scrollBar = new ScrollBar(Rect.X + Rect.Width - _itemHeight / 3, Rect.Y, _itemHeight*2/3, Rect.Height);
                _scrollBar.ButtonsColor = scrollButtonsColor;
                _scrollBar.GuideColor = scrollGuideColor;
            }
            else _scrollBar = new ScrollBar(Rect.X + Rect.Width - _itemHeight / 3, Rect.Y, _itemHeight*2/3, Rect.Height);
            _scrollBar.OnValueChanged += ScrollBar_OnValueChanged;
        }

        [MethodImplAttribute(MethodImplOptions.Synchronized)]
        void ScrollBar_OnValueChanged(int val)
        {
            _firstItemIndex = (int)Math.Round(_step * val, MidpointRounding.AwayFromZero);
            _lastItemIndex = _firstItemIndex + _itemsVisibledQuantity - 1;
            _rectSelectedItemDests = SpliteRect(new Rectangle(Rect.X + 3, Rect.Y + 1 + _itemHeight * (_selectedIndex - _firstItemIndex), Rect.Width - 4, _itemHeight - 1), 12);
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (value >= 0 && value > _list.Count - 1)
                    throw new Exception(String.Format("Selected index ({0}) not exist in list from {1} elements", value, _list.Count));
                _selectedIndex = value;
                CreateSelectedItemRect();
            }
        }

        public Item SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                Item temp = _list.SingleOrDefault(x => x.Key == value.Key);
                if (temp == null) throw new Exception(String.Format("Selected item ({0}) not exist in list", value));
                _selectedItem = value;
                _selectedIndex = _list.FindIndex(x=>x==temp);
                Rectangle selectedRect = new Rectangle(Rect.X + 2, (_selectedIndex - _firstItemIndex) * _itemHeight + Rect.Y + 1, Rect.Width - 4, _itemHeight - 1);
                _rectSelectedItemDests = SpliteRect(selectedRect, 12);
                if (OnCnahgeSelectedItem != null) OnCnahgeSelectedItem(this, _selectedIndex);
            }
        }

        private void CreateSelectedItemRect()
        {
            Rectangle selectedRect = new Rectangle(Rect.X + 2, (_selectedIndex - _firstItemIndex) * _itemHeight + Rect.Y + 1, Rect.Width - 4, _itemHeight - 1);
            _rectSelectedItemDests = SpliteRect(selectedRect, 12);
            if (_selectedIndex < 0) _selectedItem = null;
            else _selectedItem = _list[_selectedIndex];
            if (OnCnahgeSelectedItem != null) OnCnahgeSelectedItem(this, _selectedIndex);
        }

        #region Collection

        public void Add(string key)
        {
            Item item;
            if (_list.Count > 0) item = new Item(key, null);
            else item = new Item(key, null);
            _list.Add(item);
            Interlocked.Increment(ref _itemsCount);
            if (_list.Count <= _itemsVisibledQuantity)
            {
                _lastItemIndex = _list.Count - 1;
            }
            else
            {
                _firstItemIndex++;
                _lastItemIndex++;
                _step = (_itemsCount - _itemsVisibledQuantity) / (float)_scrollBar.Max;
                _scrollBar.Value = _scrollBar.Max;
            }
        }

        public void Add(string key, string value)
        {
            Item item;
            if (_list.Count > 0) item = new Item(key, value);
            else item = new Item(key, value);
            _list.Add(item);
            if (_list.Count <= _itemsVisibledQuantity)
            {
                _lastItemIndex = _list.Count - 1;
            }
            else
            {
                _firstItemIndex++;
                _lastItemIndex++;
                _step = (_list.Count - _itemsVisibledQuantity) / (float)_scrollBar.Max;
                _scrollBar.Value = _scrollBar.Max;
            }
            Interlocked.Increment(ref _itemsCount);
        }

        public void RemoveAt(int index)
        {
            _syncInvoker = () =>
            {
                _list.RemoveAt(index);
                Interlocked.Decrement(ref _itemsCount);
                OffsetItemsAfterDeleting(index);
            };
        }

        public void Remove(Item element)
        {
            int index = _list.FindIndex(x => x == element);
            if (index >= 0)
                RemoveAt(index);
        }

        private void OffsetItemsAfterDeleting(int deletingIndex)
        {
            if (deletingIndex == 0 && _firstItemIndex == 0)
            {
                if (_itemsCount < _lastItemIndex + 1) _lastItemIndex--;
            }
            else if (deletingIndex <= _firstItemIndex)
            {
                _firstItemIndex--;
                _lastItemIndex--;
            }
            else if (deletingIndex <= _lastItemIndex) //if index > _firstItemIndex;
            {
                if (_itemsCount < _lastItemIndex + 1)
                {
                    _firstItemIndex--;
                    _lastItemIndex--;
                }
            }
            if (_selectedIndex >= 0)
            {
                _selectedIndex = -1;
                _selectedItem = null;
                if (OnCnahgeSelectedItem != null) OnCnahgeSelectedItem(this, _selectedIndex);
            }
            _step = (_itemsCount - _itemsVisibledQuantity) / (float)_scrollBar.Max;
        }

        public void Clear()
        {
            _syncInvoker = () =>
            {
                _selectedIndex = -1;
                _selectedItem = null;
                _firstItemIndex = _lastItemIndex = 0;
                _list.Clear();
                _itemsCount = 0;
            };
        }

        private void Sort()
        {
            bool flag = true;
            Item temp;
            Item[] items = _list.ToArray();
            while (flag)
            {
                flag = false;
                for (int i = 1; i < items.Length; i++)
                {
                    if (items[i].Key.CompareTo(items[i - 1].Key) < 0)
                    {
                        temp = items[i - 1];
                        items[i - 1] = items[i];
                        items[i] = temp;
                        flag = true;
                    }
                }
            }
        }

        #endregion

        public void SetFont(SpriteFont font)
        {
            if (_itemsCount > 0) throw new InvalidOperationException("You have to execute this operation before list filling");
            Font = font;
            CorrectSizes();
        }

        public override void Draw()
        {
            Draw(GraphicInstance.SpriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            int end = _itemsVisibledQuantity;
            if (_selectedIndex > -1 && _rectSelectedItemDests[0].Y >= Rect.Y && _rectSelectedItemDests[0].Y < Rect.Y + Rect.Height - _itemHeight)
            {
                spriteBatch.Draw(ControlResources.Atlas, _rectSelectedItemDests[0], _rectSelectedItemSources[0], SelectedItemColor);
                spriteBatch.Draw(ControlResources.Atlas, _rectSelectedItemDests[1], _rectSelectedItemSources[1], SelectedItemColor);
                spriteBatch.Draw(ControlResources.Atlas, _rectSelectedItemDests[2], _rectSelectedItemSources[2], SelectedItemColor);
                spriteBatch.Draw(ControlResources.Atlas, _rectSelectedItemDests[3], _rectSelectedItemSources[3], SelectedItemColor);
                spriteBatch.Draw(ControlResources.Atlas, _rectSelectedItemDests[4], _rectSelectedItemSources[4], SelectedItemColor);
                spriteBatch.Draw(ControlResources.Atlas, _rectSelectedItemDests[5], _rectSelectedItemSources[5], SelectedItemColor);
                spriteBatch.Draw(ControlResources.Atlas, _rectSelectedItemDests[6], _rectSelectedItemSources[6], SelectedItemColor);
                spriteBatch.Draw(ControlResources.Atlas, _rectSelectedItemDests[7], _rectSelectedItemSources[7], SelectedItemColor);
                spriteBatch.Draw(ControlResources.Atlas, _rectSelectedItemDests[8], _rectSelectedItemSources[8], SelectedItemColor);
            }
            for (byte i = 1; i < end; i++)
            {
                spriteBatch.Draw(ControlResources.Brush, new Rectangle(Rect.X + 1, Rect.Y + _itemHeight * i, Rect.Width - 2, 1), FrameColor);
            }
            spriteBatch.Draw(ControlResources.Atlas, _rectFrameDests[0], _rectFrameSources[0], FrameColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectFrameDests[1], _rectFrameSources[1], FrameColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectFrameDests[2], _rectFrameSources[2], FrameColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectFrameDests[3], _rectFrameSources[3], FrameColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectFrameDests[4], _rectFrameSources[4], FrameColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectFrameDests[5], _rectFrameSources[5], FrameColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectFrameDests[6], _rectFrameSources[6], FrameColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectFrameDests[7], _rectFrameSources[7], FrameColor);
            spriteBatch.Draw(ControlResources.Atlas, _rectFrameDests[8], _rectFrameSources[8], FrameColor);

            if (_itemsCount > 0)
                for (int i = _firstItemIndex; i <= _lastItemIndex; i++)
                {
                    if (i == _selectedIndex) spriteBatch.DrawString(Font, _list[i].Key, new Vector2(Rect.X + _itemHeight / 3, Rect.Y + 2 + _itemHeight * (i - _firstItemIndex)),
                                                                    GetInverse(ForeColor));
                    else spriteBatch.DrawString(Font, _list[i].Key, new Vector2(Rect.X + _itemHeight / 3, Rect.Y + 2 + _itemHeight * (i - _firstItemIndex)), ForeColor);
                }
            if (_list.Count > _itemsVisibledQuantity)
                _scrollBar.Draw(spriteBatch);
            if (_syncInvoker != null)
            {
                _syncInvoker.Invoke();
                _syncInvoker = null;
            }
        }

        public override bool ClickInControl()
        {
            if (!Enabled) return false;
            if (_list.Count > _itemsVisibledQuantity && _scrollBar.ClickInControl()) return true;
            if (InputManager.Instance.MouseState.LeftButton == ButtonState.Pressed)
            {
                if (!_isPressed && Rect.Contains(InputManager.Instance.GetMousePositionToVector2()))
                {
                    _isPressed = true;
                    int mouseY = InputManager.Instance.MouseState.Y;
                    int residue = (mouseY - Rect.X) % _itemHeight;
                    if (residue > 0)
                    {
                        int temp = _firstItemIndex + (mouseY - Rect.X) / _itemHeight;
                        if (temp <= _list.Count - 1)
                        {
                            _selectedIndex = temp;
                            CreateSelectedItemRect();
                        }
                    }
                }
            }
            else _isPressed = false;
            return _isPressed;
        }

        public override void SetPosition(int x, int y)
        {
            Offset(x - Rect.X, y - Rect.Y);
        }

        public override void Offset(int x, int y)
        {
            Rect.Offset(x, y);
            _rectFrameDests[0].Offset(x, y);
            _rectFrameDests[1].Offset(x, y);
            _rectFrameDests[2].Offset(x, y);
            _rectFrameDests[3].Offset(x, y);
            _rectFrameDests[4].Offset(x, y);
            _rectFrameDests[5].Offset(x, y);
            _rectFrameDests[6].Offset(x, y);
            _rectFrameDests[7].Offset(x, y);
            _rectFrameDests[8].Offset(x, y);
            _scrollBar.Offset(x,y);
        }
    }
}