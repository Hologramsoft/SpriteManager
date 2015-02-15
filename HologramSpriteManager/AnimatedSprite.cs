using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace HologramSpriteManager
{

    class AnimatedSprite
    {
        //setup
        public AnimatedSpriteSequences Sequences;

        //public string CurrentAnimation = "idle";

        public Vector2 Position;

        public Vector2 Movement { get; set; }

        public AnimatedSprite Clone()
        {
            AnimatedSprite ret = new AnimatedSprite();
            ret.Sequences = Sequences.Clone();
            return ret;
        }


        public AnimatedSprite(AnimatedSpriteSequences Meta)
        {
            Sequences = Meta;
        }
        public AnimatedSprite()
        {
        }

        public void ChangeAnimation(string sAnim)
        { 
            Sequences.SetAnimation(sAnim);

        }

		/// <summary>
		/// Changes the animation.
		/// </summary>
		/// <param name="startAnim">Start animation.</param>
		/// <param name="endAnim">End animation.</param>
		/// <param name="duration">Duration in miliseconds.Default value is 0. If not set, it'll use the duration from json.</param>
		public void ChangeAnimation(string startAnim,string endAnim,int duration = 0)
		{
			Sequences.SetAnimation(startAnim,endAnim,duration);
		}


        public void Draw()
        {

            CurrentFrame frame = Sequences.GetCurrentFrame();
            //Console.WriteLine(frame.SourceRectangle);
            Rectangle destinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, frame.SourceRectangle.Width, frame.SourceRectangle.Height);
            SpriteManager.spriteBatch.Draw(frame.SpriteMapTexture, destinationRectangle, frame.SourceRectangle, Color.White);

        }

		public void DrawWithAlpha(Color color)
		{
			CurrentFrame frame = Sequences.GetCurrentFrame();
			//Console.WriteLine(frame.SourceRectangle);
			Rectangle destinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, frame.SourceRectangle.Width, frame.SourceRectangle.Height);
			SpriteManager.spriteBatch.Draw(frame.SpriteMapTexture, destinationRectangle, frame.SourceRectangle, color);

		}

        public Rectangle Bounds
        {
            get
            {
                CurrentFrame current = Sequences.GetCurrentFrame();
                return new Rectangle((int)Position.X, (int)Position.Y, current.SourceRectangle.Width, current.SourceRectangle.Height);
                         // width, height);
            }
        }

    }
}
