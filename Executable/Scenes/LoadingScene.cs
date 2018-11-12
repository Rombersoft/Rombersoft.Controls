using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Xna.Framework;
using Rombersoft.Controls;

namespace Executable.Scenes
{
    public class LoadingScene : IScene
    {
        bool _runUpdate;
        int _dx, _dy;
        Slider _slider;

        public LoadingScene()
        {
            _dx = 100;
            _dy = 100;
            try
            {
                _slider = new Slider(100, 100, 300);
                _slider.OnValueChanged += (arg1, arg2) => { Console.WriteLine(arg2); };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public bool Init()
        {
            _runUpdate = true;
            Task.Run(() =>
            {
                try
                {
                    while (_runUpdate)
                    {
                        Task.Delay(30).Wait();
                        _slider.ClickInControl();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
            return true;
        }

        public void Remove()
        {
            _runUpdate = false;
        }

        public void Update(GameTime gameTime)
        {
            //_slider.SetPosition(_dx++, _dy++);
        }

        public void Draw(GameTime gameTime)
        {
            _slider.Draw();
        }
    }
}