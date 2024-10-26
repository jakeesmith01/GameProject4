using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject4
{
    public class GameSerializer
    {
        private readonly string _filePath;

        public GameSerializer(string filePath)
        {
            _filePath = filePath;
        }

        public void SaveGame(List<CheckersPiece> blackPieces, List<CheckersPiece> woodPieces)
        {
            GameState state = new GameState
            {
                BlackPieces = blackPieces.Select(p => new PieceState { Position = p.Position, IsKing = p.IsKing }).ToList(),
                WoodPieces = woodPieces.Select(p => new PieceState { Position = p.Position, IsKing = p.IsKing }).ToList()
            };

            string json = JsonConvert.SerializeObject(state, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public GameState LoadGame()
        {
            if (!File.Exists(_filePath))
                throw new FileNotFoundException("Save file not found!");

            string json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<GameState>(json);
        }
    }
}
