using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Newtonsoft.Json;

//using Android.Content.Res;
using Android.App;

using HologramSpriteManager;


namespace HologramSpriteManager
{
    class AnimationSpecReader
    {
		public static AnimatedSprite PopulateAnimations(string sFile,bool fromExternal = false)
        {

            #if __ANDROID__
			StreamReader Reader;
			if(fromExternal)
				Reader = new StreamReader(sFile);
			else
				Reader = new StreamReader(Application.Context.Assets.Open(sFile));

            string Content = Reader.ReadToEnd();
            AnimatedSpriteSequences Meta = JsonConvert.DeserializeObject<AnimatedSpriteSequences>(Content);
            
			if(fromExternal){
				foreach (var spriteMap in Meta.SpriteMaps) {
					spriteMap.SpriteMapName = sFile.Remove(sFile.IndexOf(".json"));
					spriteMap.SpriteMapName = spriteMap.SpriteMapName.Insert(spriteMap.SpriteMapName.Length,".png");
				}
			}	

            #else

            string text = System.IO.File.ReadAllText(sFile);
            AnimatedSpriteSequences Meta = JsonConvert.DeserializeObject<AnimatedSpriteSequences>(text);

            #endif

			Meta.PopulateSequence(fromExternal);

            if (Meta.Type == AnimationType.Animated)
            {

                var ctor = typeof(T).GetConstructor(new Type[] { typeof(AnimatedSpriteSequences) });
                return (T)ctor.Invoke(new object[] { Meta });
            }



            //Console.WriteLine("spec output: " + text);

            return null;

        }
    }
}
