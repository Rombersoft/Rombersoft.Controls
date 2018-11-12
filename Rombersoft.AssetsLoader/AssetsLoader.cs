using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Rombersoft.AssetsLoader
{
    public class AssetsLoader
    {
        private Dictionary<string, SpriteFont> _listFonts;
        private Dictionary<string, Texture2D> _listTextures;
        private Dictionary<string, Rectangle> _atlasRegions;
        private Dictionary<string, Song> _sounds;

        private GraphicsDevice _device { get; set; }
        public string[] FilesLogo { get; private set; }
        public Texture2D Atlas { get; private set; }
        public Texture2D Brush { get; private set; }
        private ContentManager _contentManager;
        public Queue<Asset> _assets;
        private byte _logoLoadingSpeed;
        private bool _assetsCreated;
        public event Action<ushort> StateLoadChanged;
        public event Action AssetsLoaded;
        public static AssetsLoader Instance = new AssetsLoader();

        public Dictionary<string, Rectangle> AtlasRegions
        {
            get { return _atlasRegions; }
        }

        public Dictionary<string, SpriteFont> Fonts
        {
            get { return _listFonts; }
        }

        private AssetsLoader()
        {
            _listFonts = new Dictionary<string, SpriteFont>();
            _listTextures = new Dictionary<string, Texture2D>();
            _atlasRegions = new Dictionary<string, Rectangle>();
            _sounds = new Dictionary<string, Song>();
        }

        public void Init(GraphicsDevice device, ContentManager contentManager, byte logoLoadingSpeed)
        {
            _device = device;
            _contentManager = contentManager;
            _logoLoadingSpeed = logoLoadingSpeed;
            _assetsCreated = false;
            _assets = new Queue<Asset>(5000);
            try
            {
                Brush = new Texture2D(device, 1, 1);
                Brush.SetData<Color>(new Color[1] { Color.FromNonPremultiplied(255, 255, 255, 255)});
                using (StreamReader reader = new StreamReader(_contentManager.RootDirectory + "/Controls/Atlas.txt"))
                {
                    string line = null;
                    while (true)
                    {
                        line = reader.ReadLine();
                        if (String.IsNullOrEmpty(line)) break;
                        string[] para = line.Split(' ');
                        string[] dataForRect = para[1].Split(',');
                        _atlasRegions.Add(para[0], new Rectangle(Int32.Parse(dataForRect[0]), Int32.Parse(dataForRect[1]), Int32.Parse(dataForRect[2]), Int32.Parse(dataForRect[3])));
                    }
                }
                Atlas = _contentManager.Load<Texture2D>("Controls/Atlas");
                DirectoryInfo directoryInfo = new DirectoryInfo(_contentManager.RootDirectory + "/Controls/Fonts");
                FileInfo[] fileFonts = directoryInfo.GetFiles();
                byte length = (byte)fileFonts.Length;
                for (byte i = 0; i < length; i++)
                {
                    string name = fileFonts[i].Name.Split('.')[0];
                    _listFonts.Add(name, _contentManager.Load<SpriteFont>(("Controls/Fonts/" + name)));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public int CreateAssets(string level)
        {
            DirectoryInfo[] dirs = new DirectoryInfo(_contentManager.RootDirectory + "/" + level).GetDirectories();
            int dirLength = dirs.Length;
            int filesCount = 0;
            FileInfo[] fileInfos;
            for (byte i = 0; i < dirLength; i++)
            {
                fileInfos = dirs[i].GetFiles();
                filesCount = fileInfos.Length;
                switch (dirs[i].Name)
                {
                    case "Songs":
                        for (int j = 0; j < filesCount; j++)
                        {
                            _assets.Enqueue(new Asset(AssetsType.Sound, level + "/" + dirs[j].Name));
                        }
                        break;
                    case "Textures":
                        for (int j = 0; j < filesCount; j++)
                        {
                            _assets.Enqueue(new Asset(AssetsType.Textures, level + "/" + dirs[j].Name));
                        }
                        break;
                    case "Fonts":
                        for (int j = 0; j < filesCount; j++)
                        {
                            _assets.Enqueue(new Asset(AssetsType.Font, level + "/" + dirs[j].Name));
                        }
                        break;
                    case "Animations":
                        for (int j = 0; j < filesCount; j++)
                        {
                            _assets.Enqueue(new Asset(AssetsType.Animation, level + "/" + dirs[j].Name));
                        }
                        break;
                }
            }
            _assetsCreated = true;
            return _assets.Count;
        }

        public void LoadAssets()
        {
            if (_assets.Count == 0) return;
            bool isLimitReached = false;
            byte count = 0;
            while (_assets.Count > 0 && !isLimitReached)
            {
                Asset asset = _assets.Dequeue();
                switch (asset.AssetType)
                {
                    case AssetsType.Animation:
                        isLimitReached = (count == 1);
                        break;
                    case AssetsType.Character:
                        isLimitReached = (count == 1);
                        break;
                    case AssetsType.Effect:
                        isLimitReached = (count == 1);
                        break;
                    case AssetsType.Font:
                        isLimitReached = (count == _logoLoadingSpeed);
                        break;
                    case AssetsType.Model:
                        isLimitReached = (count == 1);
                        break;
                    case AssetsType.Sound:
                        isLimitReached = (count == 1);
                        break;
                }
                count++;
            }
            if (StateLoadChanged != null) StateLoadChanged((ushort)_assets.Count);
            if (_assets.Count == 0 && _assetsCreated)
            {
                _assetsCreated = false;
                if (AssetsLoaded != null) AssetsLoaded();
            }
        }

        public Texture2D GetTexture(string name)
        {
            try
            {
                return _listTextures[name];
            }
            catch (Exception)
            {
                throw new Exception("Нет текстуры " + name);
            }
        }

        public SpriteFont RequestFont(string name)
        {
            try
            {
                return _listFonts[name];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return _listFonts["Roboto14"];
            }
        }
    }
}