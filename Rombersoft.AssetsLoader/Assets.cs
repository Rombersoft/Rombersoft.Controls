using System;

namespace Rombersoft.AssetsLoader
{
    public class Asset
    {
        public AssetsType AssetType { get; set; }
        public string PathLocation { get; set; }

        public Asset(AssetsType type, string path)
        {
            AssetType = type;
            PathLocation = path;
        }
    }

    public enum AssetsType
    {
        Animation,
        Character,
        Effect,
        Font,
        Model,
        Sound,
        Textures
    }
}
