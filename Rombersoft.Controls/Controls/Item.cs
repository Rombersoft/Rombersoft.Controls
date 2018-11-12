using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rombersoft.Controls
{
    public class Item
    {
        public string Key { get; set; }
        public object Value { get; set; }

        public Item(string key, string value)
        {
            Key = key;
            Value = Value;
        }
        public override string ToString()
        {
            return string.Format("Key = {0}, Value = {1}", Key, Value);
        }
    }

    public class CheckBoxItem
    {
        public uint Id { get; set; }
        public string Key { get; set; }
        public CheckBox CheckBoxExecuted { get; set; }
        public Button ButtonExecuted { get; set; }
        public TextBox Comment { get; set; }
        public Label LabelAction { get; set; } 
        public object Value { get; set; }
        public Rectangle Rectangle { get; set; }

        public void ClickControl()
        {
            ButtonExecuted.ClickInControl();
            Comment.ClickInControl();
            //CheckBoxExecuted.ClickInControl();
        }

        public void Draw()
        {
            CheckBoxExecuted.Draw();
            LabelAction.Draw();
            Comment.Draw();
        }
    }

    enum SortingType
    {
        Asc,
        Des
    }
}
