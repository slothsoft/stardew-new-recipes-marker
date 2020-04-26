using System;
using Microsoft.Xna.Framework;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley;
using StardewValley.Menus;

namespace NewRecipesMarker
{ 
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += this.doMagic;
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void doMagic(object sender, MenuChangedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            if (e.NewMenu == null)
                return;

            if (Game1.activeClickableMenu is GameMenu menu)
            {
                foreach (IClickableMenu page in menu.pages)
                {
                    switch (page)
                    {
                        case CraftingPage craftingPage:
                            this.Monitor.Log($"Found crafting page.", LogLevel.Debug);
                            doStuffToPage(craftingPage);

                            break;
                    }

                }
            }
        }

        private void doStuffToPage(CraftingPage craftingPage)
        {
//            craftingPage.draw = drawPlus;
        }

        public override void drawPlus(SpriteBatch b)
        {

        }
    }
}
