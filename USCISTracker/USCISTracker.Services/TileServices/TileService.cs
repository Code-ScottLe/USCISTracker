using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationsExtensions;
using NotificationsExtensions.Tiles;
using USCISTracker.Data;

namespace USCISTracker.Services.TileServices
{
    public class TileService
    {
        /// <summary>
        /// Create the main tile notification with the given case.
        /// </summary>
        /// <param name="currentCase"></param>
        /// <returns></returns>
        public static TileContent CreateAdaptiveMainTileContent(Case currentCase)
        {
            //Medium Tile
            var MediumTile = new TileBinding()
            {
                Content = new TileBindingContentAdaptive()
                {
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = currentCase.Status,
                            HintStyle = AdaptiveTextStyle.Caption,
                            HintWrap = true
                        },

                        new AdaptiveText()
                        {
                            Text = currentCase.ReceiptNumber.ReceiptNumber,
                            HintStyle = AdaptiveTextStyle.CaptionSubtle
                        }
                    }
                },

                DisplayName = currentCase.Name
            };

            //Wide Tile
            var WideTile = new TileBinding()
            {
                Content = new TileBindingContentAdaptive()
                {
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = currentCase.Status,
                            HintStyle = AdaptiveTextStyle.Body,
                            HintWrap = true
                        },

                        new AdaptiveText()
                        {
                            Text = $"Receipt #: {currentCase.ReceiptNumber.ReceiptNumber}",
                            HintStyle = AdaptiveTextStyle.Caption,
                            HintWrap = true
                        },

                        new AdaptiveText()
                        {
                            Text = $"Refresh on: {currentCase.LastRefresh}",
                            HintStyle = AdaptiveTextStyle.CaptionSubtle,
                            HintWrap = true
                        }
                    }
                },

                DisplayName = currentCase.Name
            };

            //small tile

            //Large tile
            var LargeTile = new TileBinding()
            {
                Content = new TileBindingContentAdaptive()
                {
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = currentCase.Status,
                            HintStyle = AdaptiveTextStyle.Subtitle,
                            HintWrap = true
                        },

                        new AdaptiveText()
                        {
                            Text = $"Receipt #: {currentCase.ReceiptNumber.ReceiptNumber}",
                            HintStyle = AdaptiveTextStyle.Base,
                            HintWrap = true
                        },

                        new AdaptiveText()
                        {
                            Text = $"Last Update: {currentCase.LastCaseUpdate.ToString(@"MM\/dd\/yyyy")}",
                            HintStyle = AdaptiveTextStyle.BodySubtle,
                            HintWrap = true
                        },

                        new AdaptiveText()
                        {
                            Text = $"Refresh on: {currentCase.LastRefresh}",
                            HintStyle = AdaptiveTextStyle.BodySubtle,
                            HintWrap = true
                        }
                    }
                },

                DisplayName = currentCase.Name
            };


            //Wrap it all together
            TileContent tileContent = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = MediumTile,
                    TileWide = WideTile,
                    TileLarge = LargeTile,
                    Branding = TileBranding.NameAndLogo
                }
            };


            return tileContent;
        }

    }
}
