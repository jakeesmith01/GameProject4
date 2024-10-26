using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace GameProject4
{

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Tilemap _tilemap;
        private List<CheckersPiece> _woodPieces;
        private List<CheckersPiece> _blackPieces;
        private List<Vector2> _legalMoves;
        private CheckersPiece _selectedPiece;
        private bool _mousePreviouslyPressed;
        private SpriteFont _font;

        private GameSerializer _gameSerializer;

        private GameStatus status = GameStatus.Ongoing;

        private bool HasSave = false;

        private bool Restart = false;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = 128;
            _graphics.PreferredBackBufferWidth = 128;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Initializes the different components of the game, if a game save is detected it will load this state.
        /// </summary>
        protected override void Initialize()
        {
            _gameSerializer = new GameSerializer("gameSave.dat");
            // TODO: Add your initialization logic here
            _tilemap = new Tilemap();
            TilemapImporter importer = new TilemapImporter();
            TilemapData data = importer.Import("Content/map.json");
            _tilemap.LoadContent(Content, data);
            _woodPieces = new List<CheckersPiece>();
            _blackPieces = new List<CheckersPiece>();
            _legalMoves = new List<Vector2>();

            if (File.Exists("gameSave.dat") && !Restart)
            {
                HasSave = true;
                try
                {
                    LoadGame();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                HasSave = false;
                Restart = false;
            }

            base.Initialize();
        }

        /// <summary>
        /// Overloads the OnExiting function to save the game state before exiting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnExiting(object sender, EventArgs args)
        {
            try
            {
                SaveGame();
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            base.OnExiting(sender, args);
        }

        /// <summary>
        /// Saves the game current state
        /// </summary>
        public void SaveGame()
        {
            _gameSerializer.SaveGame(_blackPieces, _woodPieces);
        }

        /// <summary>
        /// Loads the game's previous state
        /// </summary>
        public void LoadGame()
        {
            GameState loadedState = _gameSerializer.LoadGame();

            _blackPieces.Clear();
            foreach(var pieceState in loadedState.BlackPieces)
            {
                CheckersPiece p = new CheckersPiece(pieceState.Position, "checkersBlack");
                if(pieceState.IsKing)
                {
                    p.Promote();
                }
                _blackPieces.Add(p);
            }

            _woodPieces.Clear();
            foreach(var pieceState in loadedState.WoodPieces)
            {
                CheckersPiece p = new CheckersPiece(pieceState.Position, "checkersWood");
                if (pieceState.IsKing)
                {
                    p.Promote();
                }
                _woodPieces.Add(p);
            }

            foreach (var c in _blackPieces)
            {
                c.LoadContent(Content);
            }

            foreach (var c in _woodPieces)
            {
                c.LoadContent(Content);
            }
        }

        /// <summary>
        /// Loads content for the game and sets up the board with the initial pieces
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _font = Content.Load<SpriteFont>("PressStart2P");

            if (!HasSave)
            {
                _woodPieces.Clear();
                _blackPieces.Clear();

                for (int row = 0; row < 3; row++)
                {
                    for (int col = 0; col < 8; col++)
                    {
                        if (_tilemap._map[row * 8 + col] == 1)
                        {
                            _woodPieces.Add(new CheckersPiece(new Vector2(col * 16, row * 16), "checkersWood"));
                        }
                    }
                }

                for (int row = 5; row < 8; row++)
                {
                    for (int col = 0; col < 8; col++)
                    {
                        if (_tilemap._map[row * 8 + col] == 1)
                        {
                            _blackPieces.Add(new CheckersPiece(new Vector2(col * 16, row * 16), "checkersBlack"));
                        }
                    }
                }

                foreach (var c in _blackPieces)
                {
                    c.LoadContent(Content);
                }

                foreach (var c in _woodPieces)
                {
                    c.LoadContent(Content);
                }
            }
        }

        /// <summary>
        /// Updates the game logic
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Q) || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            if(_blackPieces.Count == 0)
            {
                status = GameStatus.WoodWins;
            }
            else if (_woodPieces.Count == 0)
            {
                status = GameStatus.BlackWins;
            }
            else
            {
                status = GameStatus.Ongoing;
            }

            if(status == GameStatus.WoodWins || status == GameStatus.BlackWins)
            {
                KeyboardState kbState = Keyboard.GetState();

                if(kbState.IsKeyDown(Keys.R))
                {
                    _woodPieces.Clear();
                    _blackPieces.Clear();

                    _selectedPiece = null;
                    _legalMoves.Clear();
                    status = GameStatus.Ongoing;
                    HasSave = false;
                    Restart = true;

                    Initialize();
                    LoadContent();
                }
            }

            // TODO: Add your update logic here
            MouseState mouseState = Mouse.GetState();
            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);

            if (mouseState.LeftButton == ButtonState.Pressed && !_mousePreviouslyPressed && status == GameStatus.Ongoing)
            {

                if (_selectedPiece == null)
                {
                    foreach(var piece in _blackPieces)
                    {
                        Rectangle pieceBounds = new Rectangle((int)piece.Position.X, (int)piece.Position.Y, 16, 16);
                        if(pieceBounds.Contains(mousePosition))
                        {
                            _selectedPiece = piece;
                            piece.Selected = true;
                            _legalMoves = GetLegalMoves(piece);
                            break;
                        }
                    }
                }
                else
                {
                    bool moved = false;

                    foreach(var move in _legalMoves)
                    {
                        Rectangle moveBounds = new Rectangle((int)move.X, (int)move.Y, 16, 16);

                        if(moveBounds.Contains(mousePosition))
                        {
                            MovePiece(_selectedPiece, move);
                            moved = true;
                            ComputerMove();

                            break;
                        }
                    }

                    if (!moved)
                    {
                        foreach (var piece in _blackPieces)
                        {
                            Rectangle pieceBounds = new Rectangle((int)piece.Position.X, (int)piece.Position.Y, 16, 16);
                            if (pieceBounds.Contains(mousePosition))
                            {
                                _selectedPiece = piece;
                                piece.Selected = true;
                                _legalMoves = GetLegalMoves(piece);
                                break;
                            }
                        }
                    }
                }
            }

            foreach(var p in _blackPieces)
            {
                if(p != _selectedPiece)
                {
                    p.Selected = false;
                }
            }

            _mousePreviouslyPressed = mouseState.LeftButton == ButtonState.Pressed;

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the various components of the game to the screen
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();
            _tilemap.Draw(gameTime, _spriteBatch);

            if (status == GameStatus.BlackWins)
            {
                Vector2 shadowOffset = new Vector2(1, 1);
                Vector2 shadowOffset2 = new Vector2(2, 2);

                // shadow for the text to make it more legible
                _spriteBatch.DrawString(_font, "You win!", new Vector2(30, 40) + shadowOffset2, Color.Black);
                _spriteBatch.DrawString(_font, "You win!", new Vector2(30, 40) + shadowOffset, Color.Black);

                // shadow for the text to make it more legible
                _spriteBatch.DrawString(_font, "'R' to restart!", new Vector2(10, 60) + shadowOffset2, Color.Black);
                _spriteBatch.DrawString(_font, "'R' to restart!", new Vector2(10, 60) + shadowOffset, Color.Black);

                // the text to draw
                _spriteBatch.DrawString(_font, "You win!", new Vector2(30, 40), Color.LightBlue);
                _spriteBatch.DrawString(_font, "'R' to restart!", new Vector2(10, 60), Color.LightBlue);
            }
            else if (status == GameStatus.WoodWins)
            {
                Vector2 shadowOffset = new Vector2(1, 1);
                Vector2 shadowOffset2 = new Vector2(2, 2);

                // shadow for the text to make it more legible
                _spriteBatch.DrawString(_font, "You lose!", new Vector2(30, 40) + shadowOffset2, Color.Black);
                _spriteBatch.DrawString(_font, "You lose!", new Vector2(30, 40) + shadowOffset, Color.Black);

                // shadow for the text to make it more legible
                _spriteBatch.DrawString(_font, "'R' to restart!", new Vector2(10, 60) + shadowOffset2, Color.Black);
                _spriteBatch.DrawString(_font, "'R' to restart!", new Vector2(10, 60) + shadowOffset, Color.Black);

                // the text to draw
                _spriteBatch.DrawString(_font, "You lose!", new Vector2(30, 40), Color.LightBlue);
                _spriteBatch.DrawString(_font, "'R' to restart!", new Vector2(10, 60), Color.LightBlue);
            }
            else
            {
                foreach (var c in _woodPieces)
                {
                    c.Draw(gameTime, _spriteBatch);
                }

                foreach (var c in _blackPieces)
                {
                    c.Draw(gameTime, _spriteBatch);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private CheckersPiece GetPieceAt(Vector2 position)
        {
            return _woodPieces.Concat(_blackPieces).FirstOrDefault(piece => piece.Position == position);
        }

        /// <summary>
        /// Determines all legal moves for the passed CheckersPiece
        /// </summary>
        /// <param name="piece">The piece to check for legal moves</param>
        /// <returns>A list of all moves that are legal for the current piece</returns>
        private List<Vector2> GetLegalMoves(CheckersPiece piece)
        {
            List<Vector2> moves = new List<Vector2>();
            int[] directions = piece.IsKing ? new int[] { -1, 1 } : new int[] { _blackPieces.Contains(piece) ? -1 : 1 };

            foreach(int direction in directions)
            {
                Vector2 forwardLeft = piece.Position + new Vector2(-16, direction * 16);
                Vector2 forwardRight = piece.Position + new Vector2(16, direction * 16);

                if (IsTileEmpty(forwardLeft) && IsWithinBoardBounds(forwardLeft))
                {
                    moves.Add(forwardLeft);
                }

                if (IsTileEmpty(forwardRight) && IsWithinBoardBounds(forwardRight))
                {
                    moves.Add(forwardRight);
                }

                Vector2 jumpLeft = piece.Position + new Vector2(-32, direction * 32);
                Vector2 jumpRight = piece.Position + new Vector2(32, direction * 32);

                if (IsJumpMoveValid(piece.Position, forwardLeft, jumpLeft) && IsWithinBoardBounds(jumpLeft))
                {
                    moves.Add(jumpLeft);
                }

                if (IsJumpMoveValid(piece.Position, forwardRight, jumpRight) && IsWithinBoardBounds(jumpRight))
                {
                    moves.Add(jumpRight);
                }
            }

            return moves;
        }

        /// <summary>
        /// Determines if the tile is empty or not
        /// </summary>
        /// <param name="position">The position to check for</param>
        /// <returns>true if it is empty, false otherwise</returns>
        private bool IsTileEmpty(Vector2 position)
        {
            foreach (var piece in _woodPieces.Concat(_blackPieces))
            {
                if (piece.Position == position) return false;
            }

            return true;
        }

        /// <summary>
        /// Determines if the move is valid
        /// </summary>
        /// <param name="start"></param>
        /// <param name="over"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private bool IsJumpMoveValid(Vector2 start, Vector2 over, Vector2 end)
        {
            var startPiece = GetPieceAt(start);
            var middlePiece = GetPieceAt(over);

            bool isJumpDistance = Math.Abs(end.X - start.X) == 32 && Math.Abs(end.Y - start.Y) == 32;

            if (middlePiece != null && isJumpDistance &&
                 ((startPiece != null && _blackPieces.Contains(startPiece) && _woodPieces.Contains(middlePiece)) ||
                    (startPiece != null && _woodPieces.Contains(startPiece) && _blackPieces.Contains(middlePiece))))
            {
                return IsTileEmpty(end);
            }

            return false;
        }

        /// <summary>
        /// Moves the specified piece to its new position
        /// </summary>
        /// <param name="piece">The piece to move</param>
        /// <param name="newPosition">The new position of the piece</param>
        private void MovePiece(CheckersPiece piece, Vector2 newPosition)
        {
            // Check if this is a jump move by finding the middle position
            Vector2 middle = (piece.Position + newPosition) / 2;

            bool isJump = Math.Abs(newPosition.X - piece.Position.X) == 32 && Math.Abs(newPosition.Y - piece.Position.Y) == 32;

            // Remove opponent piece in case of jump
            if (isJump)
            {
                var opponent = GetPieceAt(middle);
                if(opponent != null && (_blackPieces.Contains(opponent) && _woodPieces.Contains(piece)) || (_woodPieces.Contains(opponent) && _blackPieces.Contains(piece)))
                {
                    if (_blackPieces.Contains(opponent))
                    {
                        _blackPieces.Remove(opponent);
                    }
                    else
                    {
                        _woodPieces.Remove(opponent);
                    }
                }
            }

            // Move the piece to the new position
            piece.Position = newPosition;

            // Check if the piece should be promoted to king (reaching opposite end of the board)
            if ((newPosition.Y == 0 && _blackPieces.Contains(piece)) ||
                (newPosition.Y == 7 * 16 && _woodPieces.Contains(piece)))
            {
                piece.Promote();
            }

            // Deselect the piece and clear legal moves
            _selectedPiece = null;
            _legalMoves.Clear();
        }

        /// <summary>
        /// Determines if the position is within the bounds of the board
        /// </summary>
        /// <param name="position">The position to check</param>
        /// <returns>true if its within the bounds, false otherwise</returns>
        public bool IsWithinBoardBounds(Vector2 position)
        {
            return position.X >= 0 && position.X < 128 && position.Y >= 0 && position.Y < 128;
        }

        /// <summary>
        /// Controls the 'computer player' by making a random, legal move. The computer will prioritize moves that capture a piece.
        /// </summary>
        private void ComputerMove()
        {
            List<(CheckersPiece, Vector2)> capturingMoves = new List<(CheckersPiece, Vector2)>();
            List<(CheckersPiece, Vector2)> legalMoves = new List<(CheckersPiece, Vector2)>();

            foreach(var piece in _woodPieces)
            {
                List<Vector2> pieceMoves = GetLegalMoves(piece);

                foreach(var move in pieceMoves)
                {
                    Vector2 middle = (piece.Position + move) / 2;
                    if(IsJumpMoveValid(piece.Position, middle, move))
                    {
                        capturingMoves.Add((piece, move));
                    }
                    else
                    {
                        legalMoves.Add((piece, move));
                    }
                }
            }

            if(capturingMoves.Count > 0)
            {
                var randomCapturingMove = capturingMoves[new Random().Next(capturingMoves.Count)];
                MovePiece(randomCapturingMove.Item1, randomCapturingMove.Item2);
            }
            else if(legalMoves.Count > 0)
            {
                var randomLegalMove = legalMoves[new Random().Next(legalMoves.Count)];
                MovePiece(randomLegalMove.Item1, randomLegalMove.Item2);
            }
        }

    }

        
}
