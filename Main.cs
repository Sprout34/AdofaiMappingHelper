using ADOFAI;
using ADOFAI.LevelEditor.Controls;
using HarmonyLib;
using MappingHelper.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityModManagerNet;


namespace MappingHelper
{
    /// <summary>
    /// The main class for the mod. Call other parts of your code from this
    /// class.
    /// </summary>
    public static class Main
    {
        
        internal static LevelEvent MappingHelperLevelEvent { get; set; }
        internal static CustomTab MappingHelper { get; set; }
        internal static Localization Localizations { get; set; }
        internal static List<string> propertiesToActive { get; set; }
        internal static bool NowIsOnFocused = false;
        internal static bool showingFakeFloor = false;
        internal static bool affectAtPropertyHasBeenDisabled = false;
        internal static int lastSelectedFloorsCount = 0;
        internal static bool initializeSomething = true;

        /// <summary>
        /// Whether the mod is enabled. This is useful to have as a global
        /// property in case other parts of your mod's code needs to see if the
        /// mod is enabled.
        /// </summary>
        public static bool IsEnabled { get; private set; }

        /// <summary>
        /// UMM's logger instance. Use this to write logs to the UMM settings
        /// window under the "Logs" tab.
        /// </summary>
        public static UnityModManager.ModEntry.ModLogger Logger { get; private set; }

        private static Harmony harmony;

        public static UnityModManager.ModEntry ModEntry { get; private set; }

        /// <summary>
        /// Perform any initial setup with the mod here.
        /// </summary>
        /// <param name="modEntry">UMM's mod entry for the mod.</param>
        internal static void Setup(UnityModManager.ModEntry modEntry) {
            Logger = modEntry.Logger;
            // Add hooks to UMM event methods
            modEntry.OnToggle = OnToggle;
            ModEntry = modEntry;
            Localizations = new Localization(Path.Combine(modEntry.Path, "Localizations.json"));
            propertiesToActive = PrefabProperties.TrackDisappearAnimationToActive;

            MappingHelper = new CustomTab
            {
                icon = CreateSprite(Path.Combine(modEntry.Path, "MappingHelperIcon.png")),
                type = 801,
                name = "MappingHelperSettings",
                title = "Mapping Helper",
                index = -1,
                properties = PrefabProperties.Properties.Select(property => property.ToData()).ToList(),
                onFocused = () => {
                    NowIsOnFocused = true;
                    ShowOrHideFakeFloor();

                    if (initializeSomething)
                    {
                        initializeLyricDecoration();
                        initializeSomething = false;
                    }
                },
                onUnFocused = () => {
                    NowIsOnFocused = false;
                    Patches.FakeFloor.enabled = false;
                    ShowOrHideFakeFloor();
                },
                onChange = (levelEvent, key, oldValue, newValue) =>
                {
                    Features feature = levelEvent.Get<Features>("FeaturesOption");
                    TrackFeatures trackFeatures = levelEvent.Get<TrackFeatures>("TrackFeatures");
                    TrackAnimation trackAnimation = levelEvent.Get<TrackAnimation>("TrackAnimation");
                    FileType fileType = levelEvent.Get<FileType>("FileType");
                    MagicShapeFeature magicShapeFeature = levelEvent.Get<MagicShapeFeature>("magicShapeFeature");
                    bool previewMagicShape = levelEvent.Get<bool>("previewMagicShape");
                    bool previewTrack = levelEvent.Get<bool>("previewTrack");
                    bool lyricDisappearAnimation = levelEvent.Get<bool>("lyricDisappearAnimation");
                    LyricGeneratedAs lyricGeneratedAs = levelEvent.Get<LyricGeneratedAs>("lyricGeneratedAs");

                    ShowOrHideFakeFloor();

                    if (key.Equals("useReverseAngle") || key.Equals("affectTileRangeFrom") || key.Equals("affectTileRangeTo") || key.Equals("affectAt") || key.Equals("vertexCount") || key.Equals("previewTrack") || key.Equals("generationCount") || key.Equals("trackAngleData"))
                    {
                        if (feature == Features.MagicShape)
                        {
                            if (previewMagicShape && showingFakeFloor)
                            {
                                scnEditor.instance.RemakePath();
                            }
                        }
                        else if (feature == Features.GenerateTrack)
                        {
                            if (previewTrack && showingFakeFloor)
                            {
                                scnEditor.instance.RemakePath();
                            }
                        }
                    }

                    if (key.Equals("FeaturesOption"))
                    {
                        switch (feature)
                        {
                            case Features.TrackDisappearAnimation:
                                propertiesToActive = PrefabProperties.TrackDisappearAnimationToActive;
                                
                                break;
                            case Features.TrackAppearAnimation:
                                propertiesToActive = PrefabProperties.TrackAppearAnimationToActive;
                                
                                break;
                            case Features.MultipleTracks:
                                switch (trackFeatures)
                                {
                                    case TrackFeatures.CreateTrack:
                                        propertiesToActive = PrefabProperties.MultipleTrackToActive;
                                        break;
                                    case TrackFeatures.CreateAnimation:
                                        switch (trackAnimation)
                                        {
                                            case TrackAnimation.DisappearAnimation:
                                                propertiesToActive = PrefabProperties.MultipleTrackDisappearToActive;
                                                initializePropertyControlForTrackAnimation(trackAnimation);
                                                break;
                                            case TrackAnimation.AppearAnimation:
                                                propertiesToActive = PrefabProperties.MultipleTrackAppearToActive;
                                                initializePropertyControlForTrackAnimation(trackAnimation);
                                                break;
                                        }
                                        break;
                                }
                                break;
                            case Features.DynamicDecoration:
                                switch (fileType)
                                {
                                    case FileType.Image:
                                        propertiesToActive = PrefabProperties.DynamicDecorationSelectImageToActive;
                                        break;
                                    case FileType.Video:
                                        propertiesToActive = PrefabProperties.DynamicDecorationSelectVideoToActive;
                                        break;
                                }
                                break;
                            case Features.Decoration3D:
                                propertiesToActive = PrefabProperties.Decoration3DToActive;
                                break;
                            case Features.MagicShape:
                                switch (magicShapeFeature)
                                {
                                    case MagicShapeFeature.CreateMagicShape:
                                        propertiesToActive = PrefabProperties.MagicShapeCreateToActive;
                                        break;
                                    case MagicShapeFeature.SynchronizeBpm:
                                        propertiesToActive = PrefabProperties.MagicShapeBpmToActive;
                                        break;
                                    case MagicShapeFeature.RotateMagicShape:
                                        propertiesToActive = PrefabProperties.MagicShapeRotateToActive;
                                        break;
                                }
                                break;
                            case Features.TrackSizeChange:
                                propertiesToActive = PrefabProperties.TrackSizeChangeToActive;
                                break;
                            case Features.TrackExplosionAnimation:
                                propertiesToActive = PrefabProperties.TrackExplosionAnimationToActive;
                                break;
                            case Features.Lyric:
                                switch (lyricGeneratedAs)
                                {
                                    case LyricGeneratedAs.BuiltInText:
                                        if (lyricDisappearAnimation) propertiesToActive = PrefabProperties.LyricToActiveWithDisapperAnimation;
                                        else propertiesToActive = PrefabProperties.LyricToActive;
                                        break;
                                    case LyricGeneratedAs.Decoration:
                                        if (lyricDisappearAnimation) propertiesToActive = PrefabProperties.LyricToActiveWithDisapperAnimation_Decoration;
                                        else propertiesToActive = PrefabProperties.LyricToActive_Decoration;
                                        break;
                                }
                                break;
                            case Features.GenerateTrack:
                                propertiesToActive = PrefabProperties.GenerateTrackToActive;
                                break;
                        }
                        initializePropertyControlForFeatures(feature);
                    }

                    if (key.Equals("TrackFeatures"))
                    {
                        switch (trackFeatures)
                        {
                            case TrackFeatures.CreateTrack:
                                propertiesToActive = PrefabProperties.MultipleTrackToActive;
                                break;
                            case TrackFeatures.CreateAnimation:
                                switch (trackAnimation)
                                {
                                    case TrackAnimation.DisappearAnimation:
                                        propertiesToActive = PrefabProperties.MultipleTrackDisappearToActive;
                                        initializePropertyControlForTrackAnimation(trackAnimation);
                                        break;
                                    case TrackAnimation.AppearAnimation:
                                        propertiesToActive = PrefabProperties.MultipleTrackAppearToActive;
                                        initializePropertyControlForTrackAnimation(trackAnimation);
                                        break;
                                }
                                break;
                        }
                    }

                    if (key.Equals("TrackAnimation"))
                    {
                        switch (trackAnimation)
                        {
                            case TrackAnimation.DisappearAnimation:
                                propertiesToActive = PrefabProperties.MultipleTrackDisappearToActive;
                                initializePropertyControlForTrackAnimation(trackAnimation);
                                break;
                            case TrackAnimation.AppearAnimation:
                                propertiesToActive = PrefabProperties.MultipleTrackAppearToActive;
                                initializePropertyControlForTrackAnimation(trackAnimation);
                                break;
                        }
                    }

                    if (key.Equals("FileType"))
                    {
                        switch (fileType)
                        {
                            case FileType.Image:
                                propertiesToActive = PrefabProperties.DynamicDecorationSelectImageToActive;
                                break;
                            case FileType.Video:
                                propertiesToActive = PrefabProperties.DynamicDecorationSelectVideoToActive;
                                break;
                        }
                    }

                    if (key.Equals("magicShapeFeature"))
                    {
                        switch (magicShapeFeature)
                        {
                            case MagicShapeFeature.CreateMagicShape:
                                propertiesToActive = PrefabProperties.MagicShapeCreateToActive;
                                break;
                            case MagicShapeFeature.SynchronizeBpm :
                                propertiesToActive = PrefabProperties.MagicShapeBpmToActive;
                                break;
                            case MagicShapeFeature.RotateMagicShape:
                                propertiesToActive = PrefabProperties.MagicShapeRotateToActive;
                                break;
                        }
                    }

                    if (key.Equals("lyricDisappearAnimation") || key.Equals("lyricGeneratedAs"))
                    {
                        switch (lyricGeneratedAs)
                        {
                            case LyricGeneratedAs.BuiltInText:
                                if (lyricDisappearAnimation) propertiesToActive = PrefabProperties.LyricToActiveWithDisapperAnimation;
                                else propertiesToActive = PrefabProperties.LyricToActive;
                                break;
                            case LyricGeneratedAs.Decoration:
                                if (lyricDisappearAnimation) propertiesToActive = PrefabProperties.LyricToActiveWithDisapperAnimation_Decoration;
                                else propertiesToActive = PrefabProperties.LyricToActive_Decoration;
                                break;
                        }
                    }


                    activeChilden();
                    return true;
                },
                saveSetting = true
            };


        }

        /// <summary>
        /// Handler for toggling the mod on/off.
        /// </summary>
        /// <param name="modEntry">UMM's mod entry for the mod.</param>
        /// <param name="value">
        /// <c>true</c> if the mod is being toggled on, <c>false</c> if the mod
        /// is being toggled off.
        /// </param>
        /// <returns><c>true</c></returns>
        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value) {
            IsEnabled = value;
            if (value) 
            {
                StartMod(modEntry);
            } else {
                StopMod(modEntry);
            }
            return true;
        }

        /// <summary>
        /// Start the mod up. You can create Unity GameObjects, patch methods,
        /// etc.
        /// </summary>
        /// <param name="modEntry">UMM's mod entry for the mod.</param>
        private static void StartMod(UnityModManager.ModEntry modEntry) {
            // Patch everything in this assembly
            harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Stop the mod by cleaning up anything that you created in
        /// <see cref="StartMod(UnityModManager.ModEntry)"/>.
        /// </summary>
        /// <param name="modEntry">UMM's mod entry for the mod.</param>
        private static void StopMod(UnityModManager.ModEntry modEntry) {
            // Unpatch everything
            harmony.UnpatchAll(modEntry.Info.Id);
        }


        public static void activeChilden()
        {
            List<PropertiesPanel> panelsList = scnEditor.instance.settingsPanel.panelsList;
            Transform content = panelsList.Find(p => p.name == "MappingHelperSettings").transform.Find("viewport").Find("content");
            if (content == null) return;

            for (int i = 0; i < content.childCount; i++)
            {
                content.GetChild(i).gameObject.SetActive(false);
            }

            for (int i = 0; i < propertiesToActive.Count; i++)
            {
                string childName = propertiesToActive[i];
                Transform child = content.Find(childName);
                child?.gameObject.SetActive(true);
                child?.SetSiblingIndex(i);
            }
        }

        public static Transform FindChild(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                    return child;
                Transform result = FindChild(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }


        public static LevelEvent GetEvent(LevelEventType type)
        {
            if (scnEditor.instance == null || (int)type != MappingHelper.type || (!MappingHelper.saveSetting && scnEditor.instance.settingsPanel.selectedEventType != type))
                return null;
            if (scnEditor.instance.settingsPanel.selectedEventType == type)
                return scnEditor.instance.settingsPanel.selectedEvent;
            return Patches.InspectorPanelShowPanelPatch.saves.TryGetValue(type, out LevelEvent value) ? value : null;
        }

        internal static Sprite CreateSprite(string path)
        {
            Sprite result = null;
            if (File.Exists(path))
            {
                byte[] data = File.ReadAllBytes(path);

                Texture2D texture = new Texture2D(0, 0);

                if (texture.LoadImage(data))
                {
                    result = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f)
                    );
                    return result;
                }
            }
            return result;
        }

        public static void initializePropertyControlForFeatures(Features feature)
        {
            Dictionary<string, Property> properties = scnEditor.instance.settingsPanel.panelsList.FirstOrDefault(panel => panel.name == "MappingHelperSettings").properties;

            switch (feature)
            {
                case Features.TrackDisappearAnimation:
                    (properties["startTile"].control as PropertyControl_Tile).tileValue = new Tuple<int, TileRelativeTo>(-1, TileRelativeTo.ThisTile);
                    MappingHelperLevelEvent["startTile"] = new Tuple<int, TileRelativeTo>(-1, TileRelativeTo.ThisTile);
                    (properties["endTile"].control as PropertyControl_Tile).tileValue = new Tuple<int, TileRelativeTo>(-1, TileRelativeTo.ThisTile);
                    MappingHelperLevelEvent["endTile"] = new Tuple<int, TileRelativeTo>(-1, TileRelativeTo.ThisTile);
                    (properties["opacity"].control as PropertyControl_Text).text = "0";
                    MappingHelperLevelEvent["opacity"] = 0f;
                    (properties["angleOffset"].control as PropertyControl_Text).text = "0";
                    MappingHelperLevelEvent["angleOffset"] = 0f;
                    break;
                case Features.TrackAppearAnimation:
                    (properties["startTile"].control as PropertyControl_Tile).tileValue = new Tuple<int, TileRelativeTo>(8, TileRelativeTo.ThisTile);
                    MappingHelperLevelEvent["startTile"] = new Tuple<int, TileRelativeTo>(8, TileRelativeTo.ThisTile);
                    (properties["endTile"].control as PropertyControl_Tile).tileValue = new Tuple<int, TileRelativeTo>(8, TileRelativeTo.ThisTile);
                    MappingHelperLevelEvent["endTile"] = new Tuple<int, TileRelativeTo>(8, TileRelativeTo.ThisTile);
                    (properties["opacity"].control as PropertyControl_Text).text = "100";
                    MappingHelperLevelEvent["opacity"] = 100f;
                    (properties["angleOffset"].control as PropertyControl_Text).text = "0";
                    MappingHelperLevelEvent["angleOffset"] = 0f;
                    break;
                case Features.TrackExplosionAnimation:
                    (properties["startTile"].control as PropertyControl_Tile).tileValue = new Tuple<int, TileRelativeTo>(-4, TileRelativeTo.ThisTile);
                    MappingHelperLevelEvent["startTile"] = new Tuple<int, TileRelativeTo>(-4, TileRelativeTo.ThisTile);
                    (properties["endTile"].control as PropertyControl_Tile).tileValue = new Tuple<int, TileRelativeTo>(4, TileRelativeTo.ThisTile);
                    MappingHelperLevelEvent["endTile"] = new Tuple<int, TileRelativeTo>(4, TileRelativeTo.ThisTile);
                    (properties["angleOffset"].control as PropertyControl_Text).text = "0";
                    MappingHelperLevelEvent["angleOffset"] = 0f;
                    break;
                case Features.DynamicDecoration:
                    (properties["angleOffset"].control as PropertyControl_Text).text = "90";
                    MappingHelperLevelEvent["angleOffset"] = 90f;
                    break;
                case Features.Lyric:
                    (properties["opacity"].control as PropertyControl_Text).text = "100";
                    MappingHelperLevelEvent["opacity"] = 100f;
                    (properties["angleOffset"].control as PropertyControl_Text).text = "0";
                    MappingHelperLevelEvent["angleOffset"] = 0f;
                    break;
            }
        }

        private static void initializePropertyControlForTrackAnimation(TrackAnimation trackAnimation)
        {
            Dictionary<string, Property> properties = scnEditor.instance.settingsPanel.panelsList.FirstOrDefault(panel => panel.name == "MappingHelperSettings").properties;

            switch (trackAnimation)
            {
                case TrackAnimation.DisappearAnimation:
                    (properties["startTile"].control as PropertyControl_Tile).tileValue = new Tuple<int, TileRelativeTo>(-1, TileRelativeTo.ThisTile);
                    MappingHelperLevelEvent["startTile"] = new Tuple<int, TileRelativeTo>(-1, TileRelativeTo.ThisTile);
                    (properties["endTile"].control as PropertyControl_Tile).tileValue = new Tuple<int, TileRelativeTo>(-1, TileRelativeTo.ThisTile);
                    MappingHelperLevelEvent["endTile"] = new Tuple<int, TileRelativeTo>(-1, TileRelativeTo.ThisTile);
                    (properties["opacity"].control as PropertyControl_Text).text = "0";
                    MappingHelperLevelEvent["opacity"] = 0f;
                    break;
                case TrackAnimation.AppearAnimation:
                    (properties["startTile"].control as PropertyControl_Tile).tileValue = new Tuple<int, TileRelativeTo>(8, TileRelativeTo.ThisTile);
                    MappingHelperLevelEvent["startTile"] = new Tuple<int, TileRelativeTo>(8, TileRelativeTo.ThisTile);
                    (properties["endTile"].control as PropertyControl_Tile).tileValue = new Tuple<int, TileRelativeTo>(8, TileRelativeTo.ThisTile);
                    MappingHelperLevelEvent["endTile"] = new Tuple<int, TileRelativeTo>(8, TileRelativeTo.ThisTile);
                    (properties["opacity"].control as PropertyControl_Text).text = "100";
                    MappingHelperLevelEvent["opacity"] = 100f;
                    break;
            }
        }

        private static void initializeLyricDecoration()
        {
            Dictionary<string, Property> properties = scnEditor.instance.settingsPanel.panelsList.FirstOrDefault(panel => panel.name == "MappingHelperSettings").properties;

            (properties["strokeSize"].control as PropertyControl_Slider).UpdateSliderValue(MappingHelperLevelEvent["strokeSize"]);
            (properties["shadowSpread"].control as PropertyControl_Slider).UpdateSliderValue(MappingHelperLevelEvent["shadowSpread"]);
            (properties["shadowDensity"].control as PropertyControl_Slider).UpdateSliderValue(MappingHelperLevelEvent["shadowDensity"]);
        }


        public static void ShowOrHideFakeFloor()
        {
            LevelEvent levelEvent = GetEvent((LevelEventType)MappingHelper.type);
            if (levelEvent == null)
                return;
            Features feature = levelEvent.Get<Features>("FeaturesOption");

            if (feature == Features.MagicShape)
            {
                MagicShapeFeature magicShapeFeature = levelEvent.Get<MagicShapeFeature>("magicShapeFeature");
                bool previewMagicShape = levelEvent.Get<bool>("previewMagicShape");
                if (NowIsOnFocused && magicShapeFeature == MagicShapeFeature.CreateMagicShape && feature==Features.MagicShape)
                {
                    Patches.FakeFloor.enabled = true;
                    if (previewMagicShape)
                    {
                        if (!showingFakeFloor)
                        {
                            scnEditor.instance.RemakePath();
                            showingFakeFloor = true;
                        }
                    }
                    else
                    {
                        if (showingFakeFloor)
                        {
                            scnEditor.instance.RemakePath();
                            showingFakeFloor = false;
                        }
                    }
                }
                else
                {
                    Patches.FakeFloor.enabled = false;
                    if (showingFakeFloor)
                    {
                        scnEditor.instance.RemakePath();
                        showingFakeFloor = false;
                    }
                }
            }
            else if (feature == Features.GenerateTrack)
            {
                bool previewTrack = levelEvent.Get<bool>("previewTrack");
                if (NowIsOnFocused && feature==Features.GenerateTrack)
                {
                    Patches.FakeFloor.enabled = true;
                    if (previewTrack)
                    {
                        if (!showingFakeFloor)
                        {
                            scnEditor.instance.RemakePath();
                            showingFakeFloor = true;
                        }
                    }
                    else
                    {
                        if (showingFakeFloor)
                        {
                            scnEditor.instance.RemakePath();
                            showingFakeFloor = false;
                        }
                    }
                }
                else
                {
                    Patches.FakeFloor.enabled = false;
                    if (showingFakeFloor)
                    {
                        scnEditor.instance.RemakePath();
                        showingFakeFloor = false;
                    }
                }
            }
            else
            {
                Patches.FakeFloor.enabled = false;
                if (showingFakeFloor)
                {
                    scnEditor.instance.RemakePath();
                    showingFakeFloor = false;
                }
            }
        }

    }
}
