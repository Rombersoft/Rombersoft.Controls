using System;

namespace Rombersoft.Controls
{
    internal interface ITextEditor
    {
        bool Focused { get; set; }
        void Append(char symbol);
        void DeleteOneSymbol();
        void BackSpaceOneSymbol();
        void Return();
        void MoveUp();
        void MoveDown();
        void MoveLeft();
        void MoveRight();
        void MoveHome();
        void MoveEnd();
    }
}