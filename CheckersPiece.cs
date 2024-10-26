using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject4
{
    public class CheckersPiece
    {
        /// <summary>
        /// The texture of the checker piece
        /// </summary>
        private Texture2D _texture;

        private string _textureName;

        /// <summary>
        /// The position of the checker piece
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Determines if the piece is a king ( should it draw the regular texture or the king texture )
        /// </summary>
        public bool IsKing { get; private set; }

        /// <summary>
        /// Determines if the piece is selected
        /// </summary>
        public bool Selected { get; set; }

        public CheckersPiece(Vector2 Position, string textureName)
        {
            _textureName = textureName;
            this.Position = Position;
            IsKing = false;
        }
        
        /// <summary>
        /// Loads the texture for the specified piece
        /// </summary>
        /// <param name="content">The content manager to load the checker piece with</param>
        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>(_textureName);
        }

        public void Promote()
        {
            IsKing = true;
        }
        
        /// <summary>
        /// The draw method for the checker piece
        /// </summary>
        /// <param name="gameTime">The current gametime (not used, at least yet)</param>
        /// <param name="spriteBatch">The spritebatch to draw with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(IsKing)
            {
                if(!Selected) spriteBatch.Draw(_texture, Position, new Rectangle(16, 0, 16, 16), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                else spriteBatch.Draw(_texture, Position, new Rectangle(16, 0, 16, 16), Color.Yellow, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
            else
            {
                if(!Selected) spriteBatch.Draw(_texture, Position, new Rectangle(0, 0, 16, 16), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                else spriteBatch.Draw(_texture, Position, new Rectangle(0, 0, 16, 16), Color.Yellow, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
        }

        


    }
}
