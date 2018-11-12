using System;
using Microsoft.Xna.Framework;

namespace Executable.Scenes
{
    public class SceneManager
    {
        public Action<GameTime> Draw, Update;
        IScene _scene;
        private static SceneManager _instance = new SceneManager();
        public static SceneManager Instance { get { return _instance; } }
            
        private SceneManager()
        {
            _scene = new LoadingScene();
            _scene.Init();
            Draw = _scene.Draw;
            Update = _scene.Update;
        }
    }

    public enum Scenes
    {
        Loading = 0
    }
}
