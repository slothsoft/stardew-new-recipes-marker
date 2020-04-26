using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley;
using StardewValley.Menus;

namespace NewRecipesMarker
{

    public class ModEntry : Mod
    {
        // Note: The recipes are store in these files
        // - "TileSheets/Craftables" for the craftables
        // - "Maps/springobjects" for meals

        private const String ASSET_CRAFTABLES = "TileSheets/Craftables";
        private const String ASSET_MEALS = "Maps/springobjects";

        private const String MOD_SPARKLE = "assets/sparkle.png";

        private IModHelper helper;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="newHelper">Provides simplified APIs for writing mods.</param>

        public override void Entry(IModHelper newHelper)
        {
            this.helper = newHelper;
            this.helper.Events.Display.RenderedActiveMenu += this.HandleRenderedActiveMenu;
        }


        private void HandleRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            if (Game1.activeClickableMenu is GameMenu menu)
            {
                if (menu.pages[menu.currentTab] is CraftingPage craftingPage)
                {
                    var spriteBatch = e.SpriteBatch;

                    // TODO: a better "new" might be in Cursors.png at around 320 / 412

                    Texture2D sourceImage = this.Helper.Content.Load<Texture2D>(MOD_SPARKLE, ContentSource.ModFolder);

                    // the following adds stuff to CraftingPage.draw(SpriteBatch)

                    const float scale = Game1.pixelZoom;
                    const int offsetX = -4;
                    const int offsetY = -4;

                    foreach (ClickableComponent component in craftingPage.currentPageClickableComponents)
                    {
                        if (component is ClickableTextureComponent clickableComponent)
                        {
                            Rectangle targetRect = new Rectangle(clickableComponent.bounds.X + offsetX, clickableComponent.bounds.Y + offsetY, (int)(sourceImage.Width * scale), (int)(sourceImage.Height * scale));

                            // TODO: check periodically if we can access craftingPage.pagesOfCraftingRecipes yet
                            
                            List<Dictionary<ClickableTextureComponent, CraftingRecipe>> pagesOfCraftingRecipes = this.helper.Reflection.GetField<List<Dictionary<ClickableTextureComponent, CraftingRecipe>>>(craftingPage, "pagesOfCraftingRecipes").GetValue();
                            var craftingRecipe = pagesOfCraftingRecipes.Find((dict) => dict.ContainsKey(clickableComponent));
                            if (craftingRecipe != null && craftingRecipe[clickableComponent].timesCrafted <= 0)
                            {
                                // FIXME: we wont to draw BELOW the popups
                                spriteBatch.Draw(sourceImage, targetRect, Color.White);
                            }
                        }
                    }
                }
            }
        }
    }
}
