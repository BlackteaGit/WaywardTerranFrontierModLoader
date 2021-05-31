using CoOpSpRpG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WTFModLoader
{
    public static class WTFRootMenu
    {
        public static WidgetMods mods;
        public static RootMenuRev2 instance;
        public static void closeSettings(object sender)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
            typeof(RootMenuRev2).GetField("popupActive", flags).SetValue(instance, false);
        }
        public static void actionMods(object sender)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
            SCREEN_MANAGER.GameSounds[1].play();
            mods.OpenSettings();
            typeof(RootMenuRev2).GetField("popupActive", flags).SetValue(instance, true);
            //this.popupDelete.isVisible = false;
        }
    }
}
