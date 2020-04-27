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
                    // crafting page in the main menu
                    AddSparklesToCraftingPage(craftingPage, e.SpriteBatch);
                }
            }
            else if (Game1.activeClickableMenu is CraftingPage craftingPage)
            {
                // crafting page for the stove and the workbench
                AddSparklesToCraftingPage(craftingPage, e.SpriteBatch);
            }
        }

        private void AddSparklesToCraftingPage(CraftingPage craftingPage, SpriteBatch spriteBatch)
        {
            // TODO: a better "new" might be in Cursors.png at around 144 / 440

            Texture2D sourceImage = this.Helper.Content.Load<Texture2D>(MOD_SPARKLE, ContentSource.ModFolder);

            // the following adds stuff to CraftingPage.draw(SpriteBatch)

            const float scale = Game1.pixelZoom;
            int offsetX = -sourceImage.Width / 2;
            int offsetY = -sourceImage.Height / 2;

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
                        bool mealWasNotCooked = (craftingRecipe[clickableComponent].isCookingRecipe && !Game1.player.recipesCooked.ContainsKey(craftingRecipe[clickableComponent].getIndexOfMenuView()));
                        bool craftWasNotCrafted = (!craftingRecipe[clickableComponent].isCookingRecipe && craftingRecipe[clickableComponent].timesCrafted <= 0);

                        if (mealWasNotCooked || craftWasNotCrafted)
                        {
                            // FIXME: we want to draw BELOW the popups
                            spriteBatch.Draw(sourceImage, targetRect, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                        }
                    }
                }
            }
        }
    }
}
