using System;
using Microsoft.Xna.Framework;

namespace Executable.Scenes
{
    public interface IScene
    {
        bool Init();
        void Remove();
        void Draw(GameTime gameTime);
        void Update(GameTime gameTime);
    }
}
