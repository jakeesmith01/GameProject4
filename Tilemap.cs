using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace GameProject4
{
    public class Tilemap
    {
        private Texture2D _tilesetTexture;
        private Rectangle[] _tiles;
        private int _tileWidth, _tileHeight, _mapWidth, _mapHeight;
        public int[] _map;

        public void LoadContent(ContentManager content, TilemapData data)
        {
            // Load the tileset texture
            _tilesetTexture = content.Load<Texture2D>(data.Tileset);
            _tileWidth = data.TileWidth;
            _tileHeight = data.TileHeight;
            _mapWidth = data.MapWidth;
            _mapHeight = data.MapHeight;
            _map = data.Tiles;

            // Create rectangles for each tile in the tileset
            int tilesetColumns = _tilesetTexture.Width / _tileWidth;
            int tilesetRows = _tilesetTexture.Height / _tileHeight;
            _tiles = new Rectangle[tilesetColumns * tilesetRows];

            for (int y = 0; y < tilesetRows; y++)
            {
                for (int x = 0; x < tilesetColumns; x++)
                {
                    int index = y * tilesetColumns + x;
                    _tiles[index] = new Rectangle(x * _tileWidth, y * _tileHeight, _tileWidth, _tileHeight);
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    int index = _map[y * _mapWidth + x] - 1; // Subtracting 1 to account for 1-based tile indexes

                    if (index == -1) continue;

                    spriteBatch.Draw(
                        _tilesetTexture,
                        new Vector2(x * _tileWidth, y * _tileHeight),
                        _tiles[index],
                        Color.White
                    );
                }
            }
        }
    }
}
