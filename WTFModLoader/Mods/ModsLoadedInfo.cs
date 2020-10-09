using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using CoOpSpRpG;
using System.Reflection;
using System.IO;
using WTFModLoader.Manager;
using WTFModLoader.Config;

namespace WTFModLoader.Mods
{
    public class ModsLoadedInfo : IWTFMod
    {


        public ModLoadPriority Priority => ModLoadPriority.Low;
        public void Initialize()
        {
            Harmony harmony = new Harmony("blacktea.ModsLoaded");
            harmony.PatchAll();
        }

    }
        
        
        [HarmonyPatch(typeof(RootMenuRev2), "createElements")]
        public class RootMenuRev2_createElements
        {
          [HarmonyPostfix]
            private static void Postfix(ref List<GuiElement> ___baseCanvas, ref String ___disclaimer, ref int ___screenWidth, ref int ___screenHeight)//, ref GuiElement ___declaimerCanv)
             {
            bool CanvasFound = false;
            ___disclaimer = "Mod Loader installed";



            String ModsLogFile = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), System.IO.Path.Combine(@"Mods/WTFModLoader.log")));

          
            if (System.IO.File.Exists(ModsLogFile))
            {
                string text = File.ReadAllText(ModsLogFile);
                string[] source = text.Split(new char[] { '.', '?', '!', ' ', ';', ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
                var matchSuccessful = from word in source
                                 where word.ToLowerInvariant() == "Successfully".ToLowerInvariant()
                                 select word;
                int SuccCount = matchSuccessful.Count();
                var matchFailed = from word in source
                                      where word.ToLowerInvariant() == "failed".ToLowerInvariant()
                                      select word;
                int FailCount = matchFailed.Count();

                ___disclaimer = "Wayward Terran Frontier Mod Loader";

                if (SuccCount > 0)
                ___disclaimer = ___disclaimer + " \n " + SuccCount.ToString() + " mods successfully loaded";

                if (FailCount > 0)
                ___disclaimer = ___disclaimer + " \n " + FailCount.ToString() + " mods failed to load";

            }


            foreach (var entry in ___baseCanvas)
                  {
                    if (entry.name == "Declaimer under")
                    {
                    entry.elementList.Clear();
                    entry.AddTextBox(___disclaimer, SCREEN_MANAGER.FF14, 0, 0, 520, 160, new Microsoft.Xna.Framework.Color(196, 250, 255, 210), VerticalAlignment.top, 18, 1f, 10f, 1f);
                    CanvasFound = true;
                    }
                  }

                    if (!CanvasFound)
                    {
                        ___baseCanvas.Add(new Canvas("Declaimer under", SCREEN_MANAGER.white, ___screenWidth / 2 + 200, ___screenHeight / 2 - 400, 0, 0, 520, 160, SortType.vertical, new Microsoft.Xna.Framework.Color(0, 0, 0, 110)));
                        ___baseCanvas.Last<GuiElement>().AddTextBox(___disclaimer, SCREEN_MANAGER.FF14, 0, 0, 520, 160, new Microsoft.Xna.Framework.Color(196, 250, 255, 210), VerticalAlignment.top, 18, 1f, 10f, 1f);

                    }
             }

         }


        [HarmonyPatch(typeof(RootMenuRev2), "resize")]
        public class RootMenuRev2_resize
        {
            [HarmonyPostfix]
            private static void Postfix(ref List<GuiElement> ___baseCanvas, ref int ___screenWidth, ref int ___screenHeight)//, ref GuiElement ___declaimerCanv)
            {
                foreach (var entry in ___baseCanvas)
                {
                    if (entry.name == "Declaimer under")
                    {
                        entry.reposition(___screenWidth / 2 - 260, ___screenHeight / 2 - 160 - 140, false); 
                    }
                }
            }

        }
}