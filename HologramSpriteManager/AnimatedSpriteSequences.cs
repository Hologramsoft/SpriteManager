using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*
using FarseerPhysics.Common;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Common.Decomposition;
*/
using System.Threading.Tasks;
using System.IO;


namespace HologramSpriteManager
{
    public enum AnimationType
    {
        Character,
        Animated
    }

    public class CurrentFrame
    {
        public Rectangle SourceRectangle;
        public Texture2D SpriteMapTexture;
        //public Vertices PhysicsVertices;

    }

    public class SpriteMap
    {
        public string SpriteMapName { get; set; }
        public Texture2D SpriteMapTexture { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }
        private int width;
        private int height;

		public void PopulateTexture(bool fromExternal)
        {
			if (fromExternal) {
				using (var fileStream = File.Open (SpriteMapName,FileMode.Open,FileAccess.Read)) {
					//SpriteMapTexture = Texture2D.FromStream (SamsungKids.SamsungKids.graphics.GraphicsDevice, fileStream); 
				}
			}
			else
				SpriteMapTexture = SpriteManager.ContentShell.Load<Texture2D>(SpriteMapName);
				
            width = SpriteMapTexture.Width / Columns;
            height = SpriteMapTexture.Height / Rows;
        }
        public Rectangle GetRectangle(int iFrameRow,int iFrameColumn)
        {
            return new Rectangle(width * iFrameColumn, height * iFrameRow, width, height);
        }
    }

    public class AnimationFrame
    {
        public int weight { get; set; }
        public int map { get; set; }
        public int row { get; set; }
        public int column { get; set; }
        //public Vertices PhysicsVertices { get; set; }

    }

    public class AnimationSequence
    {
        public string AnimationName { get; set; }
        public float AnimationTime { get; set; }
        public int AnimationLoop { get; set; }
        public List<AnimationFrame> AnimationFrames { get; set; }
        //derived
        public int iTotalWeight=0;
        public float fLastAnimationStartTime;

        public AnimationSequence Clone()
        {
            AnimationSequence ret = new AnimationSequence();

            ret.AnimationName = AnimationName;
            ret.AnimationTime = AnimationTime;
            ret.AnimationLoop = AnimationLoop;
            ret.AnimationFrames = AnimationFrames;
            ret.iTotalWeight = iTotalWeight;


            return ret;
        }

        public void CalcWeight()
        {
            iTotalWeight = 0;
            for (int i=0;i<AnimationFrames.Count(); i++)
            {
                iTotalWeight += AnimationFrames[i].weight;
            }
        }

        public AnimationFrame GetCurrentFrameMeta()
        {

            float fPassedTime = SpriteManager.GameTime - fLastAnimationStartTime;
            if (fPassedTime > AnimationTime)
            {
                if (AnimationLoop == 0)
                {
                    fPassedTime = AnimationTime;
                }
                else
                {
                    fLastAnimationStartTime = SpriteManager.GameTime;
                }
            }

            int iFrame = 0;
            //if zero just use first frame
            if (fPassedTime > 0)
            {
                //weighting!
                //what percentage of total time has passed.
                float fPositionPercent = (fPassedTime / AnimationTime) * 100;
                //what is the value according to out weighting count
                float fTargetValue = (fPositionPercent / 100) * iTotalWeight;
                //loop through and find the frame
                int iCurrentWeight = 0;

                for (int i = 0; i < AnimationFrames.Count(); i++)
                {
                    iCurrentWeight += AnimationFrames[i].weight;
                    if (iCurrentWeight > fTargetValue)
                    {
                        iFrame = i;
                        break;
                    }
                }
            }
            return  AnimationFrames[iFrame];
        }
    }

    public class AnimatedSpriteSequences
    {
        public string Description { get; set; }
        public string sType { get; set; }
        public List<SpriteMap> SpriteMaps { get; set; }
        public List<AnimationSequence> AnimationSequences { get; set; }
        public AnimationType Type;

        public AnimatedSpriteSequences Clone()
        {
            AnimatedSpriteSequences ret = new AnimatedSpriteSequences();
            ret.Description =Description;
            ret.sType = sType;
            ret.SpriteMaps = SpriteMaps;
            ret.AnimationSequences = new List<AnimationSequence>();
            for (int i = 0; i < AnimationSequences.Count();i++ )
            {
                ret.AnimationSequences.Add( AnimationSequences[i].Clone());
            }
            ret.Type = Type;

            return ret;
        
        }

		public void PopulateSequence(bool fromExternal)
        {
            //load sprite maps:
            for (int i = 0; i < SpriteMaps.Count(); i++)
            {
				SpriteMaps[i].PopulateTexture(fromExternal);
            }
            //add weights
            for (int i = 0; i < AnimationSequences.Count(); i++)
            {
                AnimationSequences[i].CalcWeight();
            }

            //set enums
            switch(sType)
            {
                case "Character":
                    Type = AnimationType.Character;
                    break;
                case "Animated" :
                    Type = AnimationType.Animated;
                    break;
            }

            //set physics vertices
            //Physics experiment
            /*
            //loop through sequences
            for (int iSequence = 0; iSequence < AnimationSequences.Count(); iSequence++)
            {
                AnimationSequence asCurrentSequence = AnimationSequences[iSequence];

                //loop through each frame
                for (int iFrame = 0; iFrame < asCurrentSequence.AnimationFrames.Count(); iFrame++)
                {
                    AnimationFrame afCurrentFrame = asCurrentSequence.AnimationFrames[iFrame];
                    
                    Rectangle SourceRectangle = SpriteMaps[afCurrentFrame.map].GetRectangle(afCurrentFrame.row,afCurrentFrame.column);

                    Texture2D SubTexture = new Texture2D(SpriteManager._GraphicsDeviceManager.GraphicsDevice,SourceRectangle.Width,SourceRectangle.Height);

                    Color[] ColorData = new Color[SourceRectangle.Width*SourceRectangle.Height];

                    SpriteMaps[afCurrentFrame.map].SpriteMapTexture.GetData(0, SourceRectangle, ColorData, 0, ColorData.Length);


                    SubTexture.SetData(ColorData);

                    uint[] data = new uint[SubTexture.Width * SubTexture.Height];
                    
                    //SubTexture.GetData(data);
                    //Vertices textureVertices = PolygonTools.CreatePolygon(data, SubTexture.Width, false);
                    //afCurrentFrame.PhysicsVertices = textureVertices;
                    //Console.WriteLine("Texture vertices: " + textureVertices.ToString());
                    
                }
            }*/
            
        }


        string sCurrentSequence = "idle";
        public void SetAnimation(string sSequence)
        {
            sCurrentSequence = sSequence;
            for (int i = 0; i < AnimationSequences.Count(); i++)
            {
                if (AnimationSequences[i].AnimationName == sCurrentSequence)
                {
                    AnimationSequences[i].fLastAnimationStartTime = SpriteManager.GameTime;
                    break;
                }
            }
        }

		public async void SetAnimation(string startAnim,string endAnim,int duration)
		{
			SetAnimation (startAnim);
			await Task.Delay(duration > 0 ? duration : (int)AnimationSequences.Find(a => a.AnimationName == startAnim).AnimationTime);
			SetAnimation (endAnim);
		}


        public CurrentFrame GetCurrentFrame()
        {
            //get sequence
            //zeroth one is standard (in case of spelling errors
            AnimationSequence CurrentSequence = AnimationSequences[0];
            for (int i = 0; i < AnimationSequences.Count(); i++)
            {
                if (AnimationSequences[i].AnimationName == sCurrentSequence)
                {
                    CurrentSequence = AnimationSequences[i];
                    break;
                }
            }
            AnimationFrame frameMeta =  CurrentSequence.GetCurrentFrameMeta();

            CurrentFrame frame = new CurrentFrame();
            frame.SpriteMapTexture = SpriteMaps[frameMeta.map].SpriteMapTexture;
            frame.SourceRectangle = SpriteMaps[frameMeta.map].GetRectangle(frameMeta.row,frameMeta.column);
            //frame.PhysicsVertices = frameMeta.PhysicsVertices;

            return frame;
        }
    }
}
