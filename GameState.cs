using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;

namespace GameProject4
{
    [Serializable]
    public class GameState
    {
        /// <summary>
        /// The black pieces on the board
        /// </summary>
        public List<PieceState> BlackPieces { get; set; }

        /// <summary>
        /// The wood pieces on the board
        /// </summary>
        public List<PieceState> WoodPieces { get; set; }
    }

    [Serializable]
    public class PieceState
    {
        /// <summary>
        /// The position of the piece on the board
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// If the piece is a king or not
        /// </summary>
        public bool IsKing { get; set; }
    }
}
