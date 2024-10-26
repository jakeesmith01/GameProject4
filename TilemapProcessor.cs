using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GameProject4
{
    public class TilemapData
    {
        public string Tileset { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public int[] Tiles { get; set; }
    }

    public class TilemapImporter
    {
        public TilemapData Import(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Tilemap file not found", filePath);

            var jsonData = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<TilemapData>(jsonData);
        }
    }
}
