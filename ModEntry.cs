using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley;
using StardewValley.Menus;

namespace NewRecipesMarker
{

    public class ModEntry : Mod, IAssetEditor
    {
        // Note: The recipes are store in these files
        // - "TileSheets/Craftables" for the craftables
        // - "Maps/springobjects" for meals

        private const String ASSET_CRAFTABLES = "TileSheets/Craftables";
        private const String ASSET_MEALS = "Maps/springobjects";

        private const String MOD_SPARKLE = "assets/sparkle.png";

        private IModHelper helper;
        private Boolean addSparklesToCraftables;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="newHelper">Provides simplified APIs for writing mods.</param>

        public override void Entry(IModHelper newHelper)
        {
            this.helper = newHelper;
            this.helper.Events.Display.MenuChanged += this.HandleMenuChanged;
        }


        private void HandleMenuChanged(object sender, MenuChangedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // the game menu was opened -> add sparkles
            if (e.NewMenu is GameMenu menu)
            {
                addSparklesToCraftables = true;
                this.helper.Content.InvalidateCache(ASSET_CRAFTABLES);
            }


            // the game menu was opened -> remove sparkles
            if (e.NewMenu == null)
            {
                addSparklesToCraftables = false;
                this.helper.Content.InvalidateCache(ASSET_CRAFTABLES);
            }
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals(ASSET_CRAFTABLES) && addSparklesToCraftables)
            {
                return true;
            }

            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals(ASSET_CRAFTABLES))
            {
                this.Monitor.Log($"Loaded craftables asset with sparkles.", LogLevel.Debug);

                IAssetDataForImage editor = asset.AsImage();
                PatchTileSheetImage(editor, Game1.bigCraftableSpriteSheet, 16, 32);
            }
        }

        private void PatchTileSheetImage(IAssetDataForImage toBePatched, Texture2D tileSheet, int tilePosition, int width = -1, int height = -1)
        {
            Texture2D sourceImage = this.Helper.Content.Load<Texture2D>(MOD_SPARKLE, ContentSource.ModFolder);

            foreach (int key in Game1.bigCraftablesInformation.Keys)
            {
                CraftingRecipe craftingRecipe = new CraftingRecipe(Game1.bigCraftablesInformation[key], false);
                var timesCrafted = Game1.player.craftingRecipes.ContainsKey(Game1.bigCraftablesInformation[key]) ? Game1.player.craftingRecipes[Game1.bigCraftablesInformation[key]] : 0;

                if (timesCrafted == 0)
                {
                    Rectangle spriteRect = GetSourceRectForStandardTileSheet(128, key, 16, 32);
                    Rectangle targetRect = new Rectangle(spriteRect.X, spriteRect.Y, sourceImage.Width, sourceImage.Height);
                    toBePatched.PatchImage(sourceImage, targetArea: targetRect, patchMode: PatchMode.Overlay); // 16, 32
                    this.Monitor.Log($"Added sparkles to {spriteRect.X} | {spriteRect.Y}", LogLevel.Debug);
                } else
                {
                    this.Monitor.Log($"No sparkles for {key}, was crafted {craftingRecipe.timesCrafted} times", LogLevel.Debug);
                }
            }
        }

        // This method is a modified copy of Game1.getSourceRectForStandardTileSheet(...).
        // Modified because the method doesn't really need the Texture2D, only its width (and I don't have the texture yet).

        private static Rectangle GetSourceRectForStandardTileSheet(int tileSheetWidth, int tilePosition, int tileWidth = -1, int tileHeight = -1)
        {
            if (tileWidth == -1)
                tileWidth = Game1.tileSize;
            if (tileHeight == -1)
                tileHeight = Game1.tileSize;
            return new Rectangle(tilePosition * tileWidth % tileSheetWidth, tilePosition * tileWidth / tileSheetWidth * tileHeight, tileWidth, tileHeight);
        }
    }
}
