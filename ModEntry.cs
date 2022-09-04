using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Menus;

namespace NewRecipesMarker
{
    public class ModEntry : Mod
    {
        private const string ModSparkle = "assets/sparkle.png";

        private IModHelper _helper;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="newHelper">Provides simplified APIs for writing mods.</param>

        public override void Entry(IModHelper newHelper)
        {
            _helper = newHelper;
            _helper.Events.Display.RenderedActiveMenu += HandleRenderedActiveMenu;
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

            var sourceImage = Helper.Content.Load<Texture2D>(ModSparkle);

            // the following adds stuff to CraftingPage.draw(SpriteBatch)

            const float scale = Game1.pixelZoom;
            var offsetX = -sourceImage.Width / 2;
            var offsetY = -sourceImage.Height / 2;

            foreach (var component in craftingPage.currentPageClickableComponents)
            {
                if (component is ClickableTextureComponent clickableComponent)
                {
                    var targetRect = new Rectangle(clickableComponent.bounds.X + offsetX, clickableComponent.bounds.Y + offsetY, (int)(sourceImage.Width * scale), (int)(sourceImage.Height * scale));

                    // TODO: check periodically if we can access craftingPage.pagesOfCraftingRecipes yet

                    var pagesOfCraftingRecipes = _helper.Reflection.GetField<List<Dictionary<ClickableTextureComponent, CraftingRecipe>>>(craftingPage, "pagesOfCraftingRecipes").GetValue();
                    var craftingRecipe = pagesOfCraftingRecipes.Find((dict) => dict.ContainsKey(clickableComponent));
                    if (craftingRecipe != null && craftingRecipe[clickableComponent].timesCrafted <= 0)
                    {
                        var mealWasNotCooked = (craftingRecipe[clickableComponent].isCookingRecipe && !Game1.player.recipesCooked.ContainsKey(craftingRecipe[clickableComponent].getIndexOfMenuView()));
                        var craftWasNotCrafted = (!craftingRecipe[clickableComponent].isCookingRecipe && craftingRecipe[clickableComponent].timesCrafted <= 0);

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
