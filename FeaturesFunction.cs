using ADOFAI;
using ADOFAI.LevelEditor.Controls;
using DG.Tweening;
using MappingHelper.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace MappingHelper
{
    internal static class FeaturesFunction
    {
        public static void create()
        {
            if (ADOBase.lm.isOldLevel)
            {
                Popup.ShowMessage(Main.Localizations.GetValue("mh.theLevelVersionIsTooOld"));
                return;
            }

            using (new SaveStateScope(scnEditor.instance))
            {
                LevelEvent dataPanel = Main.GetEvent((LevelEventType)Main.MappingHelper.type);

                Features feature = (Features)dataPanel["FeaturesOption"];
                List<scrFloor> listFloors = scrLevelMaker.instance.listFloors;
                float[] floorAngles = scrLevelMaker.instance.floorAngles;

                Dictionary<int, Dictionary<LevelEventType, List<LevelEvent>>> floorEvents = new Dictionary<int, Dictionary<LevelEventType, List<LevelEvent>>>();
                foreach (var e in scnEditor.instance.events)
                {
                    if (e.floor <= 0 || e.floor > floorAngles.Length + 1)
                        continue;

                    if (!floorEvents.ContainsKey(e.floor))
                    {
                        floorEvents[e.floor] = new Dictionary<LevelEventType, List<LevelEvent>>();
                    }
                    if (!floorEvents[e.floor].ContainsKey(e.eventType))
                    {
                        floorEvents[e.floor][e.eventType] = new List<LevelEvent>();
                    }
                    floorEvents[e.floor][e.eventType].Add(e);
                }

                TrackData[] trackDatas = getTrackDatas(floorEvents);

                scnEditor editor = scnEditor.instance;
                if (editor == null) return;

                Tuple<int, int> affectedRange = getAffectedRange();
                int affectTileRangeFrom = affectedRange.Item1;
                int affectTileRangeTo = affectedRange.Item2;
                if (affectTileRangeFrom == -1 || affectTileRangeTo == -1) return;

                Tuple<int, TileRelativeTo> startTile = dataPanel.Get<Tuple<int, TileRelativeTo>>("startTile");
                Tuple<int, TileRelativeTo> endTile = dataPanel.Get<Tuple<int, TileRelativeTo>>("endTile");
                float duration = dataPanel.Get<float>("duration");

                Tuple<float, float> xPosOffsetRange = dataPanel.Get<Tuple<float, float>>("xPosOffsetRange");
                Tuple<float, float> yPosOffsetRange = dataPanel.Get<Tuple<float, float>>("yPosOffsetRange");
                Tuple<float, float> rotationOffsetRange = dataPanel.Get<Tuple<float, float>>("rotationOffsetRange");
                Tuple<float, float> scaleRange = dataPanel.Get<Tuple<float, float>>("scaleRange");

                float scaleRevertTo = dataPanel.Get<float>("scaleRevertTo");

                Tuple<float, float> parallaxRange = dataPanel.Get<Tuple<float, float>>("parallaxRange");
                float parallaxRevertTo = dataPanel.Get<float>("parallaxRevertTo");

                float opacity = dataPanel.Get<float>("opacity");
                float angleOffset = dataPanel.Get<float>("angleOffset");

                Ease ease = dataPanel.Get<Ease>("ease");

                string tag = dataPanel.Get<string>("tag");

                TrackDistribution trackDistribution = dataPanel.Get<TrackDistribution>("TrackDistribution");
                TrackAnimation trackAnimation = dataPanel.Get<TrackAnimation>("TrackAnimation");
                TrackFeatures trackFeatures = dataPanel.Get<TrackFeatures>("TrackFeatures");
                FileType fileType = dataPanel.Get<FileType>("FileType");

                string levelPath = scnEditor.instance.customLevel.levelPath;

                string directoryPath = dataPanel.Get<string>("selectDirectory");
                string imagePath = dataPanel.Get<string>("selectImage");
                string videoPath = dataPanel.Get<string>("selectVideo");

                int imageStart = dataPanel.Get<int>("imageStart");
                int imageEnd = dataPanel.Get<int>("imageEnd");

                string eventTag = dataPanel.Get<string>("eventTag");
                bool selectFrame = dataPanel.Get<bool>("selectFrame");

                Vector2 positionStartValue = dataPanel.Get<Vector2>("positionStartValue");
                Vector2 positionEndValue = dataPanel.Get<Vector2>("positionEndValue");

                Vector2 pivotStartValue = dataPanel.Get<Vector2>("pivotStartValue");
                Vector2 pivotEndValue = dataPanel.Get<Vector2>("pivotEndValue");

                float rotationStartValue = dataPanel.Get<float>("rotationStartValue");
                float rotationEndValue = dataPanel.Get<float>("rotationEndValue");

                Vector2 scaleStartValue = dataPanel.Get<Vector2>("scaleStartValue");
                Vector2 scaleEndValue = dataPanel.Get<Vector2>("scaleEndValue");

                Vector2 parallaxStartValue = dataPanel.Get<Vector2>("parallaxStartValue");
                Vector2 parallaxEndValue = dataPanel.Get<Vector2>("parallaxEndValue");

                float opacityStartValue = dataPanel.Get<float>("opacityStartValue");
                float opacityEndValue = dataPanel.Get<float>("opacityEndValue");

                int depthStartValue = dataPanel.Get<int>("depthStartValue");
                int depthEndValue = dataPanel.Get<int>("depthEndValue");

                string colorStartValue = "#" + dataPanel.Get<string>("colorStartValue");
                string colorEndValue = "#" + dataPanel.Get<string>("colorEndValue");

                int decoCount = dataPanel.Get<int>("decoCount");

                List<string> colorList = ColorGradientUtil.SplitGradientHex(
                    colorStartValue,
                    colorEndValue,
                    decoCount,
                    true,
                    true
                );

                bool useReverseAngle = dataPanel.Get<bool>("useReverseAngle");

                int vertexCount = dataPanel.Get<int>("vertexCount");

                MagicShapeFeature magicShapeFeature = dataPanel.Get<MagicShapeFeature>("magicShapeFeature");
                TwirlStyle twirlStyle = dataPanel.Get<TwirlStyle>("twirlStyle");

                float bpmValue = dataPanel.Get<float>("bpmValue");
                float multiplierValue = dataPanel.Get<float>("multiplierValue");

                float magicShapeRotateValue = dataPanel.Get<float>("magicShapeRotateValue");

                ShowedEvent showedEvent = dataPanel.Get<ShowedEvent>("showedEvent");

                Tuple<float, float> positionTrackScale = dataPanel.Get<Tuple<float, float>>("positionTrackScale");
                Tuple<float, float> scaleRadiusScale = dataPanel.Get<Tuple<float, float>>("scaleRadiusScale");
                Tuple<float, float> scalePlanetsScale = dataPanel.Get<Tuple<float, float>>("scalePlanetsScale");

                float initialAngleOffset = dataPanel.Get<float>("initialAngleOffset");

                ImageFormat imageFormat = dataPanel.Get<ImageFormat>("imageFormat");

                TrackDistribution planetAnimationDistribution = dataPanel.Get<TrackDistribution>("planetAnimationDistribution");

                Tuple<float, float> parallaxChange = dataPanel.Get<Tuple<float, float>>("parallaxChange");

                TrackDistribution eventDistribution = dataPanel.Get<TrackDistribution>("eventDistribution");

                SpeedTypeMH speedTypeMH = dataPanel.Get<SpeedTypeMH>("speedTypeMH");

                bool durationMatchBpm = dataPanel.Get<bool>("durationMatchBpm");

                string lyric = dataPanel.Get<string>("lyric");

                Delimiter delimiter = dataPanel.Get<Delimiter>("delimiter");

                Tuple<float, float> xPivotOffsetRange = dataPanel.Get<Tuple<float, float>>("xPivotOffsetRange");
                Tuple<float, float> yPivotOffsetRange = dataPanel.Get<Tuple<float, float>>("yPivotOffsetRange");

                Tuple<float, float> xParallaxOffsetRange = dataPanel.Get<Tuple<float, float>>("xParallaxOffsetRange");
                Tuple<float, float> yParallaxOffsetRange = dataPanel.Get<Tuple<float, float>>("yParallaxOffsetRange");

                bool lyricDisappearAnimation = dataPanel.Get<bool>("lyricDisappearAnimation");
                float lyricDisappearAfter = dataPanel.Get<float>("lyricDisappearAfter");
                float lyricDisappearDuration = dataPanel.Get<float>("lyricDisappearDuration");

                Tuple<float, float> lyricDisappearXPosOffsetRange = dataPanel.Get<Tuple<float, float>>("lyricDisappearXPosOffsetRange");
                Tuple<float, float> lyricDisappearYPosOffsetRange = dataPanel.Get<Tuple<float, float>>("lyricDisappearYPosOffsetRange");

                Tuple<float, float> lyricDisappearXPivotOffsetRange = dataPanel.Get<Tuple<float, float>>("lyricDisappearXPivotOffsetRange");
                Tuple<float, float> lyricDisappearYPivotOffsetRange = dataPanel.Get<Tuple<float, float>>("lyricDisappearYPivotOffsetRange");

                Tuple<float, float> lyricDisappearRotationOffsetRange = dataPanel.Get<Tuple<float, float>>("lyricDisappearRotationOffsetRange");

                Tuple<float, float> lyricDisappearScaleRange = dataPanel.Get<Tuple<float, float>>("lyricDisappearScaleRange");

                float lyricDisappearOpacity = dataPanel.Get<float>("lyricDisappearOpacity");

                Tuple<float, float> lyricDisappearParallaxRange = dataPanel.Get<Tuple<float, float>>("lyricDisappearParallaxRange");

                Tuple<float, float> lyricDisappearXParallaxOffsetRange = dataPanel.Get<Tuple<float, float>>("lyricDisappearXParallaxOffsetRange");
                Tuple<float, float> lyricDisappearYParallaxOffsetRange = dataPanel.Get<Tuple<float, float>>("lyricDisappearYParallaxOffsetRange");

                LyricGenerationMode lyricGenerationMode = dataPanel.Get<LyricGenerationMode>("lyricGenerationMode");

                Ease lyricDisappearEase = dataPanel.Get<Ease>("lyricDisappearEase");

                Vector2 positionInterval = dataPanel.Get<Vector2>("positionInterval");

                float timeInterval = dataPanel.Get<float>("timeInterval");

                LyricGeneratedAs lyricGeneratedAs = dataPanel.Get<LyricGeneratedAs>("lyricGeneratedAs");

                Font font = dataPanel.Get<Font>("font");

                bool useStroke = dataPanel.Get<bool>("useStroke");
                int strokeSize = dataPanel.Get<int>("strokeSize");
                string strokeColor = dataPanel.Get<string>("strokeColor");

                bool useShadow = dataPanel.Get<bool>("useShadow");

                Tuple<float, float> shadowOffset = dataPanel.Get<Tuple<float, float>>("shadowOffset");

                int shadowSpread = dataPanel.Get<int>("shadowSpread");
                float shadowDensity = dataPanel.Get<float>("shadowDensity");

                string shadowColor = dataPanel.Get<string>("shadowColor");

                bool useCustomFont = dataPanel.Get<bool>("useCustomFont");

                string fontPath = dataPanel.Get<string>("selectFont");

                string color = dataPanel.Get<string>("color");

                string trackAngleData = dataPanel.Get<string>("trackAngleData");

                bool previewTrack = dataPanel.Get<bool>("previewTrack");

                int generationCount = dataPanel.Get<int>("generationCount");

                void removeEvents(int floor, LevelEventType eventType)
                {
                    if (floorEvents.TryGetValue(floor, out Dictionary<LevelEventType, List<LevelEvent>> events))
                    {
                        if (events.TryGetValue(eventType, out List<LevelEvent> type_events))
                        {
                            foreach (LevelEvent levelevent in type_events)
                            {
                                editor.events.Remove(levelevent);
                            }
                        }
                    }
                }
                Dictionary<string, Property> properties = scnEditor.instance.settingsPanel.panelsList.FirstOrDefault(panel => panel.name == "MappingHelperSettings").properties;

                switch (feature)
                {
                    case Features.TrackDisappearAnimation:
                        for (int floor = affectTileRangeFrom; floor <= affectTileRangeTo; floor++)
                        {
                            LevelEvent levelEvent = new LevelEvent(floor, LevelEventType.MoveTrack);


                            levelEvent.disabled["positionOffset"] = dataPanel.disabled["xPosOffsetRange"] && dataPanel.disabled["yPosOffsetRange"];
                            levelEvent.disabled["rotationOffset"] = dataPanel.disabled["rotationOffsetRange"];
                            levelEvent.disabled["scale"] = dataPanel.disabled["scaleRange"];
                            levelEvent.disabled["opacity"] = dataPanel.disabled["opacity"];

                            levelEvent["startTile"] = startTile;
                            levelEvent["endTile"] = endTile;
                            levelEvent["duration"] = durationMatchBpm ? duration * (trackDatas[floor].bpm / trackDatas[affectTileRangeFrom].bpm) : duration;
                            levelEvent["positionOffset"] = new Vector2(dataPanel.disabled["xPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(xPosOffsetRange.Item1, xPosOffsetRange.Item2), dataPanel.disabled["yPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(yPosOffsetRange.Item1, yPosOffsetRange.Item2));
                            levelEvent["rotationOffset"] = UnityEngine.Random.Range(rotationOffsetRange.Item1, rotationOffsetRange.Item2);
                            float scale = UnityEngine.Random.Range(scaleRange.Item1, scaleRange.Item2);
                            levelEvent["scale"] = new Vector2(scale, scale);
                            levelEvent["opacity"] = opacity;
                            levelEvent["angleOffset"] = angleOffset;
                            levelEvent["ease"] = ease;
                            editor.events.Add(levelEvent);
                        }

                        editor.RemakePath(true, true);
                        editor.DeselectFloors();
                        editor.SelectFloor(listFloors[affectTileRangeFrom]);
                        break;

                    case Features.TrackAppearAnimation:
                        LevelEvent initialization_levelEvent = new LevelEvent(affectTileRangeFrom, LevelEventType.MoveTrack);
                        initialization_levelEvent.disabled["opacity"] = false;

                        initialization_levelEvent["startTile"] = new Tuple<int, TileRelativeTo>(affectTileRangeFrom + startTile.Item1, TileRelativeTo.Start);
                        initialization_levelEvent["endTile"] = new Tuple<int, TileRelativeTo>(affectTileRangeTo, TileRelativeTo.Start);
                        initialization_levelEvent["duration"] = 0f;
                        initialization_levelEvent["opacity"] = 0f;
                        editor.events.Add(initialization_levelEvent);

                        for (int floor = affectTileRangeFrom; floor <= affectTileRangeTo - (endTile.Item1 >= 0 ? endTile.Item1 : 0); floor++)
                        {
                            LevelEvent levelEvent1 = new LevelEvent(floor, LevelEventType.MoveTrack);
                            LevelEvent levelEvent2 = new LevelEvent(floor, LevelEventType.MoveTrack);

                            levelEvent1.disabled["positionOffset"] = dataPanel.disabled["xPosOffsetRange"] && dataPanel.disabled["yPosOffsetRange"];
                            levelEvent1.disabled["rotationOffset"] = dataPanel.disabled["rotationOffsetRange"];
                            levelEvent1.disabled["scale"] = dataPanel.disabled["scaleRange"];
                            levelEvent1.disabled["opacity"] = true;
                            levelEvent2.disabled["rotationOffset"] = dataPanel.disabled["rotationOffsetRange"];
                            levelEvent2.disabled["scale"] = dataPanel.disabled["scaleRevertTo"];
                            levelEvent2.disabled["opacity"] = dataPanel.disabled["opacity"];

                            levelEvent1["startTile"] = startTile;
                            levelEvent1["endTile"] = endTile;
                            levelEvent1["duration"] = 0f;
                            levelEvent1["positionOffset"] = new Vector2(dataPanel.disabled["xPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(xPosOffsetRange.Item1, xPosOffsetRange.Item2), dataPanel.disabled["yPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(yPosOffsetRange.Item1, yPosOffsetRange.Item2));
                            levelEvent1["rotationOffset"] = UnityEngine.Random.Range(rotationOffsetRange.Item1, rotationOffsetRange.Item2);
                            float scale = UnityEngine.Random.Range(scaleRange.Item1, scaleRange.Item2);
                            levelEvent1["scale"] = new Vector2(scale, scale);

                            levelEvent2["startTile"] = startTile;
                            levelEvent2["endTile"] = endTile;
                            levelEvent2["duration"] = durationMatchBpm ? duration * (trackDatas[floor].bpm / trackDatas[affectTileRangeFrom].bpm) : duration;
                            levelEvent2["positionOffset"] = new Vector2(dataPanel.disabled["xPosOffsetRange"] ? float.NaN : 0f, dataPanel.disabled["yPosOffsetRange"] ? float.NaN : 0f);
                            levelEvent2["rotationOffset"] = 0f;
                            levelEvent2["scale"] = new Vector2(scaleRevertTo, scaleRevertTo);
                            levelEvent2["opacity"] = opacity;
                            levelEvent2["angleOffset"] = angleOffset;
                            levelEvent2["ease"] = ease;
                            editor.events.Add(levelEvent1);
                            editor.events.Add(levelEvent2);
                        }

                        editor.RemakePath(true, true);
                        editor.DeselectFloors();
                        editor.SelectFloor(listFloors[affectTileRangeFrom]);
                        break;

                    case Features.MultipleTracks:
                        bool isCentralized = (trackDistribution == TrackDistribution.Centralized);
                        float prevHead = (trackDatas[affectTileRangeFrom].tail + 180) % 360;
                        float pivotTrackOffset = isCentralized ? (float)dataPanel["trackRotation"] : 0;
                        float startBpm = trackDatas[affectTileRangeFrom].bpm;
                        int trackCount = affectTileRangeTo - affectTileRangeFrom + 1;

                        if ((bool)dataPanel["usePlanet"] && trackFeatures == TrackFeatures.CreateTrack)
                        {
                            LevelEvent levelEvent_BluePlanet = new LevelEvent(affectTileRangeFrom, LevelEventType.AddObject);
                            LevelEvent levelEvent_RedPlanet = new LevelEvent(affectTileRangeFrom, LevelEventType.AddObject);

                            levelEvent_BluePlanet["relativeTo"] = DecPlacementType.Tile;
                            levelEvent_BluePlanet["objectType"] = ObjectDecorationType.Planet;
                            levelEvent_BluePlanet["planetColorType"] = PlanetDecorationColorType.Custom;
                            levelEvent_BluePlanet["planetColor"] = "0000ffff";
                            levelEvent_BluePlanet["planetTailColor"] = "0000ff00";
                            levelEvent_BluePlanet["position"] = new Vector2(0, 0);
                            levelEvent_BluePlanet["pivotOffset"] = new Vector2(-Mathf.Cos(pivotTrackOffset * Mathf.Deg2Rad), -Mathf.Sin(pivotTrackOffset * Mathf.Deg2Rad));
                            levelEvent_BluePlanet["tag"] = tag.IsNullOrEmpty() ? $"_BluePlanet" : $"{tag} {tag}_BluePlanet";
                            levelEvent_BluePlanet["depth"] = (bool)dataPanel["useIncreasingDepth"] ? (int)dataPanel["initialDepth"] - 1 : -2;
                            levelEvent_BluePlanet["scale"] = new Vector2(100 - parallaxChange.Item1, 100 - parallaxChange.Item1);
                            levelEvent_BluePlanet["parallax"] = new Vector2(parallaxChange.Item1, parallaxChange.Item1);

                            levelEvent_RedPlanet["relativeTo"] = DecPlacementType.Tile;
                            levelEvent_RedPlanet["objectType"] = ObjectDecorationType.Planet;
                            levelEvent_RedPlanet["planetColorType"] = PlanetDecorationColorType.Custom;
                            levelEvent_RedPlanet["planetColor"] = "ff0000ff";
                            levelEvent_RedPlanet["planetTailColor"] = "ff000000";
                            levelEvent_RedPlanet["position"] = new Vector2(Mathf.Cos(pivotTrackOffset * Mathf.Deg2Rad), Mathf.Sin(pivotTrackOffset * Mathf.Deg2Rad));
                            levelEvent_RedPlanet["pivotOffset"] = new Vector2(-Mathf.Cos(pivotTrackOffset * Mathf.Deg2Rad), -Mathf.Sin(pivotTrackOffset * Mathf.Deg2Rad));
                            levelEvent_RedPlanet["tag"] = tag.IsNullOrEmpty() ? $"_RedPlanet" : $"{tag} {tag}_RedPlanet";
                            levelEvent_RedPlanet["depth"] = (bool)dataPanel["useIncreasingDepth"] ? (int)dataPanel["initialDepth"] - 1 : -2;
                            levelEvent_RedPlanet["scale"] = new Vector2(100 - parallaxChange.Item1, 100 - parallaxChange.Item1);
                            levelEvent_RedPlanet["parallax"] = new Vector2(parallaxChange.Item1, parallaxChange.Item1);

                            editor.levelData.decorations.Add(levelEvent_BluePlanet);
                            scrDecorationManager.instance.CreateDecoration(levelEvent_BluePlanet, out _, -1);
                            editor.levelData.decorations.Add(levelEvent_RedPlanet);
                            scrDecorationManager.instance.CreateDecoration(levelEvent_RedPlanet, out _, -1);
                        }

                        bool moveRedPlanet = false;
                        int trackDepth = (int)dataPanel["initialDepth"];
                        for (int floor = affectTileRangeFrom, i = 1; floor <= affectTileRangeTo; floor++, i++)
                        {
                            switch (trackFeatures)
                            {
                                case TrackFeatures.CreateTrack:
                                    Vector2 curFloorPos = trackDatas[floor].position - trackDatas[affectTileRangeFrom].position;
                                    Vector2 nextFloorPos = trackDatas[floor + 1 >= trackDatas.Length ? floor : floor + 1].position - trackDatas[affectTileRangeFrom].position;
                                    int nextFloor = floor + 1 >= trackDatas.Length ? floor : floor + 1;

                                    float trackRotation;
                                    LevelEvent levelEvent = new LevelEvent(isCentralized ? affectTileRangeFrom : floor, LevelEventType.AddObject);
                                    levelEvent["relativeTo"] = DecPlacementType.Tile;
                                    levelEvent["position"] = Vector2.zero;
                                    if (listFloors[floor].midSpin)
                                    {
                                        levelEvent["trackType"] = FloorDecorationType.Midspin;
                                    }
                                    else
                                    {
                                        levelEvent["trackAngle"] = trackDatas[floor].angle;
                                    }

                                    trackRotation = listFloors[floor].midSpin ? trackDatas[floor].head : getMidDirection(trackDatas[floor].tail, trackDatas[floor].head, listFloors[floor].isCCW, true) - getMidDirection(180, (360 - ((trackDatas[floor].angle + 180) % 360)) % 360, false, true);
                                    levelEvent["rotation"] = trackRotation;

                                    levelEvent["rotation"] = (float)levelEvent["rotation"] + trackDatas[floor].rotation;

                                    if (!tag.IsNullOrEmpty())
                                    {
                                        levelEvent["tag"] = $"{tag} {tag}{i}";
                                    }

                                    if (isCentralized)
                                    {
                                        levelEvent["pivotOffset"] = GetPivotOffset(curFloorPos, trackRotation + trackDatas[floor].rotation);
                                    }


                                    if ((bool)dataPanel["useIncreasingDepth"])
                                    {
                                        levelEvent["depth"] = trackDepth;
                                        trackDepth += (int)dataPanel["increasingValue"];
                                    }


                                    if ((bool)dataPanel["usePlanet"])
                                    {
                                        if ((bool)dataPanel["changeParallax"] && (bool)dataPanel["affectPlanet"])
                                        {
                                            LevelEvent levelEvent_adjustPlanet;
                                            if (planetAnimationDistribution == TrackDistribution.Distributed)
                                            {
                                                levelEvent_adjustPlanet = new LevelEvent(floor, LevelEventType.MoveDecorations);
                                            }
                                            else
                                            {
                                                levelEvent_adjustPlanet = new LevelEvent(affectTileRangeFrom, LevelEventType.MoveDecorations);
                                                if (!listFloors[floor].midSpin)
                                                {
                                                    float time = trackDatas[floor].arrivalTime - trackDatas[affectTileRangeFrom].arrivalTime;
                                                    float angleOffsetValue = time / (60f / trackDatas[affectTileRangeFrom].bpm) * 180;
                                                    levelEvent_adjustPlanet["angleOffset"] = angleOffsetValue;
                                                }
                                            }
                                            levelEvent_adjustPlanet.disabled["positionOffset"] = true;
                                            levelEvent_adjustPlanet.disabled["scale"] = false;
                                            levelEvent_adjustPlanet.disabled["parallax"] = false;
                                            levelEvent_adjustPlanet["duration"] = 0f;
                                            levelEvent_adjustPlanet["tag"] = $"{tag}_RedPlanet {tag}_BluePlanet";
                                            float t = (i - 1) / (float)(trackCount - 1);
                                            float parallaxValue = Mathf.Lerp(parallaxChange.Item1, parallaxChange.Item2, t);
                                            float scaleValue = 100f - parallaxValue;
                                            levelEvent_adjustPlanet["scale"] = new Vector2(scaleValue, scaleValue);
                                            levelEvent_adjustPlanet["parallax"] = new Vector2(parallaxValue, parallaxValue);
                                            editor.events.Add(levelEvent_adjustPlanet);
                                        }

                                        LevelEvent levelEvent_MovePlanetRed1;
                                        LevelEvent levelEvent_MovePlanetRed2;
                                        LevelEvent levelEvent_MovePlanetBlue1;
                                        LevelEvent levelEvent_MovePlanetBlue2;

                                        if (planetAnimationDistribution == TrackDistribution.Distributed)
                                        {
                                            levelEvent_MovePlanetRed1 = new LevelEvent(floor, LevelEventType.MoveDecorations);
                                            levelEvent_MovePlanetRed2 = new LevelEvent(floor, LevelEventType.MoveDecorations);
                                            levelEvent_MovePlanetBlue1 = new LevelEvent(floor, LevelEventType.MoveDecorations);
                                            levelEvent_MovePlanetBlue2 = new LevelEvent(floor, LevelEventType.MoveDecorations);
                                        }
                                        else
                                        {
                                            levelEvent_MovePlanetRed1 = new LevelEvent(affectTileRangeFrom, LevelEventType.MoveDecorations);
                                            levelEvent_MovePlanetRed2 = new LevelEvent(affectTileRangeFrom, LevelEventType.MoveDecorations);
                                            levelEvent_MovePlanetBlue1 = new LevelEvent(affectTileRangeFrom, LevelEventType.MoveDecorations);
                                            levelEvent_MovePlanetBlue2 = new LevelEvent(affectTileRangeFrom, LevelEventType.MoveDecorations);
                                            if (!listFloors[floor].midSpin)
                                            {
                                                float time = trackDatas[floor].arrivalTime - trackDatas[affectTileRangeFrom].arrivalTime;
                                                float angleOffsetValue = time / (60f / trackDatas[affectTileRangeFrom].bpm) * 180;

                                                levelEvent_MovePlanetRed1["angleOffset"] = angleOffsetValue;
                                                levelEvent_MovePlanetRed2["angleOffset"] = angleOffsetValue;
                                                levelEvent_MovePlanetBlue1["angleOffset"] = angleOffsetValue;
                                                levelEvent_MovePlanetBlue2["angleOffset"] = angleOffsetValue;
                                            }
                                        }

                                        levelEvent_MovePlanetRed1.disabled["positionOffset"] = false;
                                        levelEvent_MovePlanetRed1.disabled["rotationOffset"] = false;
                                        levelEvent_MovePlanetRed2.disabled["positionOffset"] = false;
                                        levelEvent_MovePlanetRed2.disabled["rotationOffset"] = false;

                                        levelEvent_MovePlanetBlue1.disabled["positionOffset"] = false;
                                        levelEvent_MovePlanetBlue1.disabled["rotationOffset"] = false;
                                        levelEvent_MovePlanetBlue2.disabled["positionOffset"] = false;
                                        levelEvent_MovePlanetBlue2.disabled["rotationOffset"] = false;

                                        levelEvent_MovePlanetRed1["tag"] = $"{tag}_RedPlanet";
                                        levelEvent_MovePlanetRed2["tag"] = $"{tag}_RedPlanet";
                                        levelEvent_MovePlanetBlue1["tag"] = $"{tag}_BluePlanet";
                                        levelEvent_MovePlanetBlue2["tag"] = $"{tag}_BluePlanet";



                                        if (planetAnimationDistribution == TrackDistribution.Distributed)
                                        {
                                            levelEvent_MovePlanetRed1["duration"] = 0f;
                                            levelEvent_MovePlanetRed2["duration"] = trackDatas[floor].angle / 180f;
                                            levelEvent_MovePlanetBlue1["duration"] = 0f;
                                            levelEvent_MovePlanetBlue2["duration"] = trackDatas[floor].angle / 180f;
                                            if (trackDatas[floor].isPause)
                                            {
                                                levelEvent_MovePlanetRed2["duration"] = (float)levelEvent_MovePlanetRed2["duration"] + trackDatas[floor].pauseDuration;
                                                levelEvent_MovePlanetBlue2["duration"] = (float)levelEvent_MovePlanetBlue2["duration"] + trackDatas[floor].pauseDuration;
                                            }
                                            if (trackDatas[floor].isHold)
                                            {
                                                levelEvent_MovePlanetRed2["duration"] = (float)levelEvent_MovePlanetRed2["duration"] + trackDatas[floor].holdDuration * 2;
                                                levelEvent_MovePlanetBlue2["duration"] = (float)levelEvent_MovePlanetBlue2["duration"] + trackDatas[floor].holdDuration * 2;
                                            }
                                        }
                                        else
                                        {
                                            levelEvent_MovePlanetRed1["duration"] = 0f;
                                            levelEvent_MovePlanetRed2["duration"] = trackDatas[floor].angle / 180f * startBpm / trackDatas[floor].bpm;
                                            levelEvent_MovePlanetBlue1["duration"] = 0f;
                                            levelEvent_MovePlanetBlue2["duration"] = trackDatas[floor].angle / 180f * startBpm / trackDatas[floor].bpm;
                                            if (trackDatas[floor].isPause)
                                            {
                                                levelEvent_MovePlanetRed2["duration"] = (float)levelEvent_MovePlanetRed2["duration"] + trackDatas[floor].pauseDuration * startBpm / trackDatas[floor].bpm;
                                                levelEvent_MovePlanetBlue2["duration"] = (float)levelEvent_MovePlanetBlue2["duration"] + trackDatas[floor].pauseDuration * startBpm / trackDatas[floor].bpm;
                                            }
                                            if (trackDatas[floor].isHold)
                                            {
                                                levelEvent_MovePlanetRed2["duration"] = (float)levelEvent_MovePlanetRed2["duration"] + trackDatas[floor].holdDuration * 2 * startBpm / trackDatas[floor].bpm;
                                                levelEvent_MovePlanetBlue2["duration"] = (float)levelEvent_MovePlanetBlue2["duration"] + trackDatas[floor].holdDuration * 2 * startBpm / trackDatas[floor].bpm;
                                            }
                                        }

                                        bool mambo = false;
                                        if (moveRedPlanet)
                                        {
                                            levelEvent_MovePlanetRed1["positionOffset"] = GetPivotOffset(new Vector2(curFloorPos.x - 1, curFloorPos.y), -pivotTrackOffset);
                                            levelEvent_MovePlanetRed1["rotationOffset"] = trackDatas[floor].tail - 180;
                                            levelEvent_MovePlanetRed2["rotationOffset"] = trackDatas[floor].tail - 180 + (listFloors[floor].isCCW ? 1 : -1) * trackDatas[floor].angle;
                                            levelEvent_MovePlanetBlue1["positionOffset"] = GetPivotOffset(new Vector2(curFloorPos.x + 1, curFloorPos.y), -pivotTrackOffset);

                                            if (trackDatas[floor].isPause)
                                            {
                                                levelEvent_MovePlanetRed2["rotationOffset"] = (float)levelEvent_MovePlanetRed2["rotationOffset"] + (listFloors[floor].isCCW ? 1 : -1) * Mathf.Floor(trackDatas[floor].pauseDuration / 2.0f);
                                            }

                                            if (trackDatas[floor].isHold)
                                            {
                                                levelEvent_MovePlanetRed2["rotationOffset"] = (float)levelEvent_MovePlanetRed2["rotationOffset"] + (listFloors[floor].isCCW ? 1 : -1) * trackDatas[floor].holdDuration * 360f;
                                                levelEvent_MovePlanetRed2["positionOffset"] = GetPivotOffset(new Vector2(nextFloorPos.x + Mathf.Cos(trackDatas[nextFloor].tail * Mathf.Deg2Rad) - 1, nextFloorPos.y + Mathf.Sin(trackDatas[nextFloor].tail * Mathf.Deg2Rad)), -pivotTrackOffset);
                                                levelEvent_MovePlanetBlue2["positionOffset"] = GetPivotOffset(new Vector2(nextFloorPos.x + Mathf.Cos(trackDatas[nextFloor].tail * Mathf.Deg2Rad) + 1, nextFloorPos.y + Mathf.Sin(trackDatas[nextFloor].tail * Mathf.Deg2Rad)), -pivotTrackOffset);
                                                mambo = true;
                                            }

                                            if (!listFloors[floor].midSpin)
                                            {
                                                editor.events.Add(levelEvent_MovePlanetBlue1);
                                                editor.events.Add(levelEvent_MovePlanetRed1);
                                                editor.events.Add(levelEvent_MovePlanetRed2);
                                                if (mambo)
                                                {
                                                    mambo = false;
                                                    editor.events.Add(levelEvent_MovePlanetBlue2);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            levelEvent_MovePlanetBlue1["positionOffset"] = GetPivotOffset(curFloorPos, -pivotTrackOffset);
                                            levelEvent_MovePlanetBlue1["rotationOffset"] = trackDatas[floor].tail - 180;
                                            levelEvent_MovePlanetBlue2["rotationOffset"] = trackDatas[floor].tail - 180 + (listFloors[floor].isCCW ? 1 : -1) * trackDatas[floor].angle;
                                            levelEvent_MovePlanetRed1["positionOffset"] = GetPivotOffset(curFloorPos, -pivotTrackOffset);

                                            if (trackDatas[floor].isPause)
                                            {
                                                levelEvent_MovePlanetBlue2["rotationOffset"] = (float)levelEvent_MovePlanetBlue2["rotationOffset"] + (listFloors[floor].isCCW ? 1 : -1) * Mathf.Floor(trackDatas[floor].pauseDuration / 2.0f);
                                            }

                                            if (trackDatas[floor].isHold)
                                            {
                                                levelEvent_MovePlanetBlue2["rotationOffset"] = (float)levelEvent_MovePlanetBlue2["rotationOffset"] + (listFloors[floor].isCCW ? 1 : -1) * trackDatas[floor].holdDuration * 360f;
                                                levelEvent_MovePlanetRed2["positionOffset"] = GetPivotOffset(new Vector2(nextFloorPos.x + Mathf.Cos(trackDatas[nextFloor].tail * Mathf.Deg2Rad), nextFloorPos.y + Mathf.Sin(trackDatas[nextFloor].tail * Mathf.Deg2Rad)), -pivotTrackOffset);
                                                levelEvent_MovePlanetBlue2["positionOffset"] = GetPivotOffset(new Vector2(nextFloorPos.x + Mathf.Cos(trackDatas[nextFloor].tail * Mathf.Deg2Rad), nextFloorPos.y + Mathf.Sin(trackDatas[nextFloor].tail * Mathf.Deg2Rad)), -pivotTrackOffset);
                                                mambo = true;
                                            }

                                            if (!listFloors[floor].midSpin)
                                            {
                                                editor.events.Add(levelEvent_MovePlanetRed1);
                                                editor.events.Add(levelEvent_MovePlanetBlue1);
                                                editor.events.Add(levelEvent_MovePlanetBlue2);
                                                if (mambo)
                                                {
                                                    mambo = false;
                                                    editor.events.Add(levelEvent_MovePlanetRed2);
                                                }
                                            }
                                        }

                                        moveRedPlanet = !moveRedPlanet;
                                    }

                                    switch (listFloors[floor].floorIcon)
                                    {
                                        case FloorIcon.None:
                                            levelEvent["trackIcon"] = CustomFloorIcon.None;
                                            break;
                                        case FloorIcon.Snail:
                                            levelEvent["trackIcon"] = CustomFloorIcon.Snail;
                                            levelEvent["trackIconAngle"] = 360f - trackRotation;
                                            break;
                                        case FloorIcon.AnimatedSnail:
                                            levelEvent["trackIcon"] = CustomFloorIcon.Snail;
                                            levelEvent["trackIconAngle"] = 360f - trackRotation;
                                            break;
                                        case FloorIcon.DoubleSnail:
                                            levelEvent["trackIcon"] = CustomFloorIcon.DoubleSnail;
                                            levelEvent["trackIconAngle"] = 360f - trackRotation;
                                            break;
                                        case FloorIcon.AnimatedDoubleSnail:
                                            levelEvent["trackIcon"] = CustomFloorIcon.DoubleSnail;
                                            levelEvent["trackIconAngle"] = 360f - trackRotation;
                                            break;
                                        case FloorIcon.Rabbit:
                                            levelEvent["trackIcon"] = CustomFloorIcon.Rabbit;
                                            levelEvent["trackIconAngle"] = 360f - trackRotation;
                                            break;
                                        case FloorIcon.AnimatedRabbit:
                                            levelEvent["trackIcon"] = CustomFloorIcon.Rabbit;
                                            levelEvent["trackIconAngle"] = 360f - trackRotation;
                                            break;
                                        case FloorIcon.DoubleRabbit:
                                            levelEvent["trackIcon"] = CustomFloorIcon.DoubleRabbit;
                                            levelEvent["trackIconAngle"] = 360f - trackRotation;
                                            break;
                                        case FloorIcon.AnimatedDoubleRabbit:
                                            levelEvent["trackIcon"] = CustomFloorIcon.DoubleRabbit;
                                            levelEvent["trackIconAngle"] = 360f - trackRotation;
                                            break;
                                        case FloorIcon.Swirl:
                                            levelEvent["trackIcon"] = CustomFloorIcon.Swirl;
                                            levelEvent["trackRedSwirl"] = listFloors[floor].midSpin || trackDatas[floor].angle < 180;
                                            levelEvent["trackIconFlipped"] = listFloors[floor].isCCW;
                                            levelEvent["trackIconAngle"] = listFloors[floor].midSpin ? trackDatas[floor].tail - (90 + trackRotation) : getMidDirection(trackDatas[floor].tail, trackDatas[floor].head, listFloors[floor].isCCW, true) - (90 + trackRotation);
                                            break;
                                        case FloorIcon.Checkpoint:
                                            levelEvent["trackIcon"] = CustomFloorIcon.Checkpoint;
                                            break;
                                        case FloorIcon.HoldArrowShort:
                                            levelEvent["trackIcon"] = CustomFloorIcon.HoldArrowLong;
                                            break;
                                        case FloorIcon.HoldArrowLong:
                                            levelEvent["trackIcon"] = CustomFloorIcon.HoldArrowLong;
                                            break;
                                        case FloorIcon.HoldReleaseShort:
                                            levelEvent["trackIcon"] = CustomFloorIcon.HoldReleaseShort;
                                            break;
                                        case FloorIcon.HoldReleaseLong:
                                            levelEvent["trackIcon"] = CustomFloorIcon.HoldReleaseLong;
                                            break;
                                        case FloorIcon.MultiPlanetTwo:
                                            levelEvent["trackIcon"] = CustomFloorIcon.MultiPlanetTwo;
                                            levelEvent["trackIconAngle"] = 360f - trackRotation;
                                            break;
                                        case FloorIcon.MultiPlanetThreeMore:
                                            levelEvent["trackIcon"] = CustomFloorIcon.MultiPlanetThreeMore;
                                            levelEvent["trackIconAngle"] = 360f - trackRotation;
                                            break;
                                        case FloorIcon.Portal:
                                            levelEvent["trackIcon"] = CustomFloorIcon.Portal;
                                            break;
                                    }
                                    if (isCentralized) levelEvent["rotation"] = (float)levelEvent["rotation"] + (float)dataPanel["trackRotation"];
                                    prevHead = trackDatas[floor].head;

                                    if ((bool)dataPanel["changeParallax"])
                                    {
                                        float t = (i - 1) / (float)(trackCount - 1);
                                        float parallaxValue = Mathf.Lerp(parallaxChange.Item1, parallaxChange.Item2, t);
                                        float scaleValue = 100 - parallaxValue;
                                        levelEvent["scale"] = new Vector2(scaleValue, scaleValue);
                                        levelEvent["parallax"] = new Vector2(parallaxValue, parallaxValue);
                                    }

                                    editor.levelData.decorations.Add(levelEvent);
                                    scrDecorationManager.instance.CreateDecoration(levelEvent, out _, -1);
                                    break;
                                case TrackFeatures.CreateAnimation:
                                    if (!tag.IsNullOrEmpty())
                                    {
                                        LevelEvent levelEvent_MD1;
                                        LevelEvent levelEvent_MD2;
                                        if (eventDistribution == TrackDistribution.Distributed)
                                        {
                                            levelEvent_MD1 = new LevelEvent(floor, LevelEventType.MoveDecorations);
                                            levelEvent_MD2 = new LevelEvent(floor, LevelEventType.MoveDecorations);
                                        }
                                        else
                                        {
                                            levelEvent_MD1 = new LevelEvent(affectTileRangeFrom, LevelEventType.MoveDecorations);
                                            levelEvent_MD2 = new LevelEvent(affectTileRangeFrom, LevelEventType.MoveDecorations);
                                        }

                                        switch (trackAnimation)
                                        {
                                            case TrackAnimation.DisappearAnimation:
                                                if (i + startTile.Item1 > 0)
                                                {
                                                    if (eventDistribution == TrackDistribution.Distributed)
                                                    {
                                                        levelEvent_MD2["angleOffset"] = angleOffset;
                                                    }
                                                    else
                                                    {
                                                        float time = trackDatas[floor - 1].departureTime - trackDatas[affectTileRangeFrom].arrivalTime;
                                                        float angleOffsetValue = time / (60f / trackDatas[affectTileRangeFrom].bpm) * 180;
                                                        levelEvent_MD2["angleOffset"] = angleOffset + angleOffsetValue;
                                                    }

                                                    levelEvent_MD2.disabled["positionOffset"] = dataPanel.disabled["xPosOffsetRange"] && dataPanel.disabled["yPosOffsetRange"];
                                                    levelEvent_MD2.disabled["rotationOffset"] = dataPanel.disabled["rotationOffsetRange"];
                                                    levelEvent_MD2.disabled["scale"] = dataPanel.disabled["scaleRange"];
                                                    levelEvent_MD2.disabled["opacity"] = dataPanel.disabled["opacity"];
                                                    levelEvent_MD2.disabled["parallax"] = dataPanel.disabled["parallaxRange"];

                                                    levelEvent_MD2["tag"] = $"{tag}{i + startTile.Item1}";
                                                    levelEvent_MD2["duration"] = duration;
                                                    levelEvent_MD2["positionOffset"] = new Vector2(dataPanel.disabled["xPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(xPosOffsetRange.Item1, xPosOffsetRange.Item2), dataPanel.disabled["yPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(yPosOffsetRange.Item1, yPosOffsetRange.Item2));
                                                    levelEvent_MD2["rotationOffset"] = UnityEngine.Random.Range(rotationOffsetRange.Item1, rotationOffsetRange.Item2);
                                                    float parallax = UnityEngine.Random.Range(parallaxRange.Item1, parallaxRange.Item2);
                                                    levelEvent_MD2["parallax"] = new Vector2(parallax, parallax);
                                                    float scale = UnityEngine.Random.Range(scaleRange.Item1, scaleRange.Item2);
                                                    levelEvent_MD2["scale"] = new Vector2(scale, scale);
                                                    levelEvent_MD2["opacity"] = opacity;
                                                    levelEvent_MD2["ease"] = ease;
                                                    editor.events.Add(levelEvent_MD2);
                                                }
                                                break;
                                            case TrackAnimation.AppearAnimation:
                                                if (floor <= affectTileRangeTo - (startTile.Item1 >= 0 ? startTile.Item1 : 0))
                                                {
                                                    if (eventDistribution == TrackDistribution.Distributed)
                                                    {
                                                        levelEvent_MD1["angleOffset"] = angleOffset;
                                                        levelEvent_MD2["angleOffset"] = angleOffset;
                                                    }
                                                    else
                                                    {
                                                        float time = trackDatas[floor].arrivalTime - trackDatas[affectTileRangeFrom].arrivalTime;
                                                        float angleOffsetValue = time / (60f / trackDatas[affectTileRangeFrom].bpm) * 180;
                                                        levelEvent_MD1["angleOffset"] = angleOffset + angleOffsetValue;
                                                        levelEvent_MD2["angleOffset"] = angleOffset + angleOffsetValue;
                                                    }

                                                    levelEvent_MD1.disabled["positionOffset"] = dataPanel.disabled["xPosOffsetRange"] && dataPanel.disabled["yPosOffsetRange"];
                                                    levelEvent_MD1.disabled["rotationOffset"] = dataPanel.disabled["rotationOffsetRange"];
                                                    levelEvent_MD1.disabled["scale"] = dataPanel.disabled["scaleRange"];
                                                    levelEvent_MD1.disabled["opacity"] = dataPanel.disabled["opacity"];

                                                    levelEvent_MD2.disabled["positionOffset"] = dataPanel.disabled["xPosOffsetRange"] && dataPanel.disabled["yPosOffsetRange"];
                                                    levelEvent_MD2.disabled["rotationOffset"] = dataPanel.disabled["rotationOffsetRange"];
                                                    levelEvent_MD2.disabled["scale"] = dataPanel.disabled["scaleRevertTo"];
                                                    levelEvent_MD2.disabled["opacity"] = dataPanel.disabled["opacity"];
                                                    levelEvent_MD1.disabled["parallax"] = dataPanel.disabled["parallaxRange"];
                                                    levelEvent_MD2.disabled["parallax"] = dataPanel.disabled["parallaxRevertTo"];

                                                    levelEvent_MD1["tag"] = $"{tag}{i + startTile.Item1}";
                                                    levelEvent_MD1["duration"] = 0f;
                                                    levelEvent_MD1["positionOffset"] = new Vector2(dataPanel.disabled["xPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(xPosOffsetRange.Item1, xPosOffsetRange.Item2), dataPanel.disabled["yPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(yPosOffsetRange.Item1, yPosOffsetRange.Item2));
                                                    levelEvent_MD1["rotationOffset"] = UnityEngine.Random.Range(rotationOffsetRange.Item1, rotationOffsetRange.Item2);
                                                    float parallax = UnityEngine.Random.Range(parallaxRange.Item1, parallaxRange.Item2);
                                                    levelEvent_MD1["parallax"] = new Vector2(parallax, parallax);
                                                    float scale = UnityEngine.Random.Range(scaleRange.Item1, scaleRange.Item2);
                                                    levelEvent_MD1["scale"] = new Vector2(scale, scale);
                                                    levelEvent_MD1["opacity"] = 0f;

                                                    levelEvent_MD2["tag"] = $"{tag}{i + startTile.Item1}";
                                                    levelEvent_MD2["duration"] = duration;
                                                    levelEvent_MD2["positionOffset"] = new Vector2(dataPanel.disabled["xPosOffsetRange"] ? float.NaN : 0f, dataPanel.disabled["yPosOffsetRange"] ? float.NaN : 0f);
                                                    levelEvent_MD2["rotationOffset"] = 0f;
                                                    levelEvent_MD2["parallax"] = new Vector2(parallaxRevertTo, parallaxRevertTo);
                                                    levelEvent_MD2["scale"] = new Vector2(scaleRevertTo, scaleRevertTo);
                                                    levelEvent_MD2["opacity"] = opacity;
                                                    levelEvent_MD2["ease"] = ease;

                                                    editor.events.Add(levelEvent_MD1);
                                                    editor.events.Add(levelEvent_MD2);
                                                }
                                                break;
                                        }

                                    }
                                    else
                                    {
                                        Popup.ShowMessage(Main.Localizations.GetValue("mh.inputTheTagFirst"));
                                        return;
                                    }

                                    break;

                            }


                        }

                        editor.RemakePath(true, true);
                        if (trackFeatures == TrackFeatures.CreateAnimation && !tag.IsNullOrEmpty())
                        {
                            editor.DeselectFloors();
                            editor.SelectFloor(listFloors[affectTileRangeFrom]);
                        }
                        break;

                    case Features.DynamicDecoration:
                        if (levelPath.IsNullOrEmpty())
                        {
                            Popup.ShowMessage(Main.Localizations.GetValue("mh.saveTheLevelFirst"));
                            return;
                        }
                        if ((fileType == FileType.Image && directoryPath.IsNullOrEmpty()) || (fileType == FileType.Video && videoPath.IsNullOrEmpty())) return;


                        {
                            string levelFolderPath = Path.GetDirectoryName(levelPath);
                            string imageNameWithoutExtension;
                            string imageOutputrPath;
                            string[] images;

                            if (fileType == FileType.Image)
                            {
                                imageNameWithoutExtension = Path.GetFileNameWithoutExtension(directoryPath);
                                imageOutputrPath = Path.Combine(levelFolderPath, imageNameWithoutExtension);
                                images = Directory.GetFiles(imageOutputrPath);
                                for (int i = 0; i < images.Length; i++)
                                {
                                    images[i] = Path.GetFileName(images[i]);
                                }
                                images = images.NaturalSort();

                                LevelEvent levelEvent = new LevelEvent(affectTileRangeFrom, LevelEventType.AddDecoration);
                                levelEvent["decorationImage"] = Path.Combine(imageNameWithoutExtension, images[0]);
                                levelEvent["tag"] = $"{tag}";
                                levelEvent["relativeTo"] = DecPlacementType.Tile;
                                levelEvent["depth"] = 1;

                                editor.levelData.decorations.Add(levelEvent);
                                scrDecorationManager.instance.CreateDecoration(levelEvent, out _, -1);
                                int step = affectTileRangeFrom <= affectTileRangeTo ? 1 : -1;
                                for (int floor = affectTileRangeFrom; floor <= affectTileRangeTo; floor++)
                                {
                                    float curAngleOffset = initialAngleOffset;

                                    int start = selectFrame ? imageStart : 1;
                                    int end = selectFrame ? imageEnd : images.Length;
                                    start = Mathf.Clamp(start, 1, images.Length);
                                    end = Mathf.Clamp(end, 1, images.Length);
                                    if (imageStart <= imageEnd)
                                    {
                                        for (int i = start; i <= end; i++)
                                        {
                                            LevelEvent levelEvent_MD = new LevelEvent(floor, LevelEventType.MoveDecorations);
                                            levelEvent_MD.disabled["decorationImage"] = false;
                                            levelEvent_MD.disabled["positionOffset"] = true;

                                            levelEvent_MD["duration"] = 0f;
                                            levelEvent_MD["tag"] = $"{tag}";
                                            levelEvent_MD["decorationImage"] = Path.Combine(imageNameWithoutExtension, images[i - 1]);
                                            levelEvent_MD["angleOffset"] = curAngleOffset;
                                            levelEvent_MD["eventTag"] = eventTag;
                                            curAngleOffset += angleOffset;

                                            editor.events.Add(levelEvent_MD);
                                        }
                                    }
                                    else if (imageStart > imageEnd)
                                    {
                                        for (int i = start; i >= end; i--)
                                        {
                                            LevelEvent levelEvent_MD = new LevelEvent(floor, LevelEventType.MoveDecorations);
                                            levelEvent_MD.disabled["decorationImage"] = false;
                                            levelEvent_MD.disabled["positionOffset"] = true;

                                            levelEvent_MD["duration"] = 0f;
                                            levelEvent_MD["tag"] = $"{tag}";
                                            levelEvent_MD["decorationImage"] = Path.Combine(imageNameWithoutExtension, images[i - 1]);
                                            levelEvent_MD["angleOffset"] = curAngleOffset;
                                            levelEvent_MD["eventTag"] = eventTag;
                                            curAngleOffset += angleOffset;

                                            editor.events.Add(levelEvent_MD);
                                        }
                                    }
                                }
                                editor.ApplyEventsToFloors();
                                editor.RemakePath(true, true);
                                editor.DeselectFloors();
                                editor.SelectFloor(listFloors[affectTileRangeFrom]);
                            }
                            else
                            {
                                videoPath = Path.Combine(levelFolderPath, (string)dataPanel["selectVideo"]);

                                // 检查文件是否存在
                                if (string.IsNullOrEmpty(videoPath) || !File.Exists(videoPath))
                                {
                                    Popup.ShowMessage(Main.Localizations.GetValue("mh.fileDoesNotExist"));
                                    return;
                                }

                                imageNameWithoutExtension = Path.GetFileNameWithoutExtension(videoPath);
                                imageOutputrPath = Path.Combine(levelFolderPath, imageNameWithoutExtension);

                                if (!Directory.Exists(imageOutputrPath))
                                    Directory.CreateDirectory(imageOutputrPath);

                                GameObject runnerObj = new GameObject("VideoCoroutineRunner");
                                CoroutineRunner runner = runnerObj.AddComponent<CoroutineRunner>();
                                ExtraVideo(runner, videoPath, imageOutputrPath, imageFormat);
                            }
                        }
                        break;
                    case Features.Decoration3D:
                        if (levelPath.IsNullOrEmpty())
                        {
                            Popup.ShowMessage(Main.Localizations.GetValue("mh.saveTheLevelFirst"));
                            return;
                        }

                        string completeImagePath = Path.Combine(Path.GetDirectoryName(levelPath), imagePath);
                        // 检查文件是否存在
                        if (string.IsNullOrEmpty(completeImagePath) || !File.Exists(completeImagePath))
                        {
                            Popup.ShowMessage(Main.Localizations.GetValue("mh.fileDoesNotExist"));
                            return;
                        }


                        for (int floor = affectTileRangeFrom; floor <= affectTileRangeTo; floor++)
                        {
                            float xPositionValue = positionStartValue.x;
                            float yPositionValue = positionStartValue.y;
                            float xPivotValue = pivotStartValue.x;
                            float yPivotValue = pivotStartValue.y;
                            float rotationValue = rotationStartValue;
                            float xScaleValue = scaleStartValue.x;
                            float yScaleValue = scaleStartValue.y;
                            string colorValue = colorStartValue;
                            float opacityValue = opacityStartValue;
                            int depthValue = depthStartValue;
                            float xParallaxValue = parallaxStartValue.x;
                            float yParallaxValue = parallaxStartValue.y;

                            for (int i = 0; i < decoCount; i++)
                            {
                                xPositionValue = positionStartValue.x + (positionEndValue.x - positionStartValue.x) / ((decoCount - 1) == 0 ? 1 : (decoCount - 1)) * i;
                                yPositionValue = positionStartValue.y + (positionEndValue.y - positionStartValue.y) / ((decoCount - 1) == 0 ? 1 : (decoCount - 1)) * i;
                                xPivotValue = pivotStartValue.x + (pivotEndValue.x - pivotStartValue.x) / ((decoCount - 1) == 0 ? 1 : (decoCount - 1)) * i;
                                yPivotValue = pivotStartValue.y + (pivotEndValue.y - pivotStartValue.y) / ((decoCount - 1) == 0 ? 1 : (decoCount - 1)) * i;
                                rotationValue = rotationStartValue + (rotationEndValue - rotationStartValue) / ((decoCount - 1) == 0 ? 1 : (decoCount - 1)) * i;
                                xScaleValue = scaleStartValue.x + (scaleEndValue.x - scaleStartValue.x) / ((decoCount - 1) == 0 ? 1 : (decoCount - 1)) * i;
                                yScaleValue = scaleStartValue.y + (scaleEndValue.y - scaleStartValue.y) / ((decoCount - 1) == 0 ? 1 : (decoCount - 1)) * i;
                                colorValue = colorList[i];
                                opacityValue = opacityStartValue + (opacityEndValue - opacityStartValue) / ((decoCount - 1) == 0 ? 1 : (decoCount - 1)) * i;
                                depthValue = depthStartValue + (int)((depthEndValue - depthStartValue) / (float)((decoCount - 1) == 0 ? 1 : (decoCount - 1)) * i);
                                xParallaxValue = parallaxStartValue.x + (parallaxEndValue.x - parallaxStartValue.x) / ((decoCount - 1) == 0 ? 1 : (decoCount - 1)) * i;
                                yParallaxValue = parallaxStartValue.y + (parallaxEndValue.y - parallaxStartValue.y) / ((decoCount - 1) == 0 ? 1 : (decoCount - 1)) * i;

                                LevelEvent levelEvent = new LevelEvent(floor, LevelEventType.AddDecoration);

                                levelEvent["decorationImage"] = imagePath;
                                levelEvent["tag"] = tag.IsNullOrEmpty() ? string.Empty : $"{tag} {tag}{i + 1}";
                                levelEvent["relativeTo"] = DecPlacementType.Tile;
                                levelEvent["position"] = new Vector2(xPositionValue, yPositionValue);
                                levelEvent["pivotOffset"] = new Vector2(xPivotValue, yPivotValue);
                                levelEvent["rotation"] = rotationValue;
                                levelEvent["scale"] = new Vector2(xScaleValue, yScaleValue);
                                levelEvent["color"] = colorValue;
                                levelEvent["opacity"] = opacityValue;
                                levelEvent["depth"] = depthValue;
                                levelEvent["parallax"] = new Vector2(xParallaxValue, yParallaxValue);

                                editor.levelData.decorations.Add(levelEvent);
                                scrDecorationManager.instance.CreateDecoration(levelEvent, out _, -1);
                            }
                        }
                        editor.RemakePath(true, true);
                        break;
                    case Features.MagicShape:
                        if (ADOBase.lm.isOldLevel) return;

                        switch (magicShapeFeature)
                        {
                            case MagicShapeFeature.CreateMagicShape:
                                List<float> newTrack = new List<float>();
                                for (int i = 1; i < vertexCount; i++)
                                {
                                    for (int floor = affectTileRangeFrom; floor < affectTileRangeTo; floor++)
                                    {
                                        float direction = floorAngles[floor];

                                        if (direction != 999f)
                                        {
                                            direction += 360f / vertexCount * i * (useReverseAngle ? -1 : 1);
                                        }
                                        newTrack.Add(direction);
                                    }
                                }
                                scnEditor.instance.levelData.angleData.InsertRange(affectTileRangeTo, newTrack);
                                dataPanel["previewMagicShape"] = false;
                                (properties["previewMagicShape"].control as PropertyControl_Bool).value = false;
                                Main.showingFakeFloor = false;

                                int offset = (affectTileRangeTo - affectTileRangeFrom) * (vertexCount - 1);
                                OffsetFloorIDsInEvents(affectTileRangeTo, offset);

                                break;
                            case MagicShapeFeature.SynchronizeBpm:
                                float trueBpm;
                                float prevBpm = -1f;
                                for (int floor = affectTileRangeFrom; floor <= affectTileRangeTo; floor++)
                                {
                                    removeEvents(floor, LevelEventType.Twirl);
                                }
                                editor.ApplyEventsToFloors();

                                bool isCCW = listFloors[affectTileRangeFrom].isCCW;
                                for (int floor = affectTileRangeFrom; floor <= affectTileRangeTo; floor++)
                                {
                                    removeEvents(floor, LevelEventType.SetSpeed);

                                    if (listFloors[floor].midSpin) continue;

                                    float angle = correctAngle(isCCW ? correctDirection(trackDatas[floor].head - trackDatas[floor].tail) : correctDirection(trackDatas[floor].tail - trackDatas[floor].head));

                                    if (twirlStyle == TwirlStyle.Internal)
                                    {
                                        if (angle > 180)
                                        {
                                            editor.events.Add(new LevelEvent(floor, LevelEventType.Twirl));
                                            isCCW = !isCCW;
                                        }
                                        angle = angle < 180 ? angle : 360 - (angle == 360 ? 0 : angle);
                                    }
                                    else if (twirlStyle == TwirlStyle.External)
                                    {
                                        if (angle < 180)
                                        {
                                            editor.events.Add(new LevelEvent(floor, LevelEventType.Twirl));
                                            isCCW = !isCCW;
                                        }
                                        angle = angle > 180 ? angle : 360 - (angle == 360 ? 0 : angle);
                                    }
                                    trueBpm = bpmValue * angle / 180;
                                    if (floor == affectTileRangeFrom)
                                    {
                                        multiplierValue *= angle / 180;
                                    }
                                    else
                                    {
                                        multiplierValue = trueBpm / prevBpm;
                                    }
                                    LevelEvent levelEvent_setSpeed = new LevelEvent(floor, LevelEventType.SetSpeed);
                                    if (speedTypeMH == SpeedTypeMH.Bpm)
                                    {
                                        levelEvent_setSpeed["beatsPerMinute"] = trueBpm;
                                        levelEvent_setSpeed["speedType"] = SpeedType.Bpm;
                                    }
                                    else
                                    {
                                        levelEvent_setSpeed["bpmMultiplier"] = multiplierValue;
                                        levelEvent_setSpeed["speedType"] = SpeedType.Multiplier;
                                    }

                                    if (prevBpm != trueBpm)
                                    {
                                        if (showedEvent == ShowedEvent.SetSpeed)
                                        {
                                            editor.events.Insert(0, levelEvent_setSpeed);
                                        }
                                        else
                                        {
                                            editor.events.Add(levelEvent_setSpeed);
                                        }
                                    }
                                    prevBpm = trueBpm;
                                }

                                break;
                            case MagicShapeFeature.RotateMagicShape:
                                for (int floor = affectTileRangeFrom; floor <= affectTileRangeTo; floor++)
                                {
                                    if (listFloors[floor].midSpin || floor == scnEditor.instance.levelData.angleData.Count) continue;

                                    scnEditor.instance.levelData.angleData[floor] = correctDirection(scnEditor.instance.levelData.angleData[floor] + magicShapeRotateValue);
                                }
                                break;
                        }
                        scnEditor.instance.RemakePath(true, true);
                        break;

                    case Features.TrackSizeChange:
                        int range = affectTileRangeTo - affectTileRangeFrom;

                        for (int floor = affectTileRangeFrom; floor <= affectTileRangeTo; floor++)
                        {
                            float t = (range == 0 ? 0f : (float)(floor - affectTileRangeFrom) / range);
                            if (Enum.TryParse(ease.ToString(), out EaseType result))
                            {
                                t = DOTweenEaseUtils.Evaluate(t, result);
                            }

                            if (!dataPanel.disabled["positionTrackScale"])
                            {
                                removeEvents(floor, LevelEventType.PositionTrack);
                                LevelEvent levelEvent = new LevelEvent(floor, LevelEventType.PositionTrack);
                                levelEvent.disabled["scale"] = false;
                                float value = Mathf.LerpUnclamped(positionTrackScale.Item1, positionTrackScale.Item2, t);
                                levelEvent["scale"] = value;
                                editor.events.Add(levelEvent);
                            }
                            if (!dataPanel.disabled["scaleRadiusScale"])
                            {
                                removeEvents(floor, LevelEventType.ScaleRadius);
                                LevelEvent levelEvent = new LevelEvent(floor, LevelEventType.ScaleRadius);
                                float value = Mathf.LerpUnclamped(scaleRadiusScale.Item1, scaleRadiusScale.Item2, t);
                                levelEvent["scale"] = value;
                                editor.events.Add(levelEvent);
                            }
                            if (!dataPanel.disabled["scalePlanetsScale"])
                            {
                                removeEvents(floor, LevelEventType.ScalePlanets);
                                LevelEvent levelEvent = new LevelEvent(floor, LevelEventType.ScalePlanets);
                                levelEvent["duration"] = 0f;
                                levelEvent["targetPlanet"] = TargetPlanet.All;
                                float value = Mathf.LerpUnclamped(scalePlanetsScale.Item1, scalePlanetsScale.Item2, t);
                                levelEvent["scale"] = value;
                                editor.events.Add(levelEvent);
                            }
                        }
                        scnEditor.instance.RemakePath(true, true);
                        editor.DeselectFloors();
                        editor.SelectFloor(listFloors[affectTileRangeFrom]);
                        break;
                    case Features.TrackExplosionAnimation:
                        for (int floor = affectTileRangeFrom; floor <= affectTileRangeTo; floor++)
                        {
                            float trackExplosionAngleExplosion = 0f;
                            for (int i = startTile.Item1; i <= endTile.Item1; i++)
                            {
                                Tuple<int, TileRelativeTo> tile = Tuple.Create(i, TileRelativeTo.ThisTile);
                                LevelEvent levelEvent = new LevelEvent(floor, LevelEventType.MoveTrack);

                                levelEvent.disabled["positionOffset"] = dataPanel.disabled["xPosOffsetRange"] && dataPanel.disabled["yPosOffsetRange"];
                                levelEvent.disabled["rotationOffset"] = dataPanel.disabled["rotationOffsetRange"];
                                levelEvent.disabled["scale"] = dataPanel.disabled["scaleRange"];
                                levelEvent.disabled["opacity"] = dataPanel.disabled["opacity"];

                                levelEvent["startTile"] = tile;
                                levelEvent["endTile"] = tile;
                                levelEvent["duration"] = duration;
                                levelEvent["positionOffset"] = new Vector2(dataPanel.disabled["xPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(xPosOffsetRange.Item1, xPosOffsetRange.Item2), dataPanel.disabled["yPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(yPosOffsetRange.Item1, yPosOffsetRange.Item2));
                                levelEvent["rotationOffset"] = UnityEngine.Random.Range(rotationOffsetRange.Item1, rotationOffsetRange.Item2);
                                float scale = UnityEngine.Random.Range(scaleRange.Item1, scaleRange.Item2);
                                levelEvent["scale"] = new Vector2(scale, scale);
                                levelEvent["opacity"] = opacity;
                                levelEvent["angleOffset"] = trackExplosionAngleExplosion;
                                levelEvent["ease"] = ease;
                                trackExplosionAngleExplosion += angleOffset;

                                editor.events.Add(levelEvent);
                            }
                        }

                        editor.RemakePath(true, true);
                        editor.DeselectFloors();
                        editor.SelectFloor(listFloors[affectTileRangeFrom]);
                        break;
                    case Features.Lyric:
                        if (lyricGeneratedAs == LyricGeneratedAs.Decoration && levelPath.IsNullOrEmpty())
                        {
                            Popup.ShowMessage(Main.Localizations.GetValue("mh.saveTheLevelFirst"));
                            return;
                        }
                        if (lyricGeneratedAs == LyricGeneratedAs.Decoration && !lyric.IsNullOrEmpty() && lyric.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                        {
                            Popup.ShowMessage(Main.Localizations.GetValue("mh.LyricContainInvalidCharacter"));
                            return;
                        }

                        string[] lyricList;
                        float[] xPositionIntervalList;
                        float[] yPositionIntervalList;

                        if (delimiter == Delimiter.SplitBySpace)
                        {
                            lyricList = lyric.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                        }
                        else
                        {
                            lyricList = lyric.ToCharArray().Select(c => c.ToString()).ToArray();
                        }

                        xPositionIntervalList = new float[lyricList.Length];
                        yPositionIntervalList = new float[lyricList.Length];
                        if (lyricList.Length % 2 == 0) // 偶数
                        {
                            for (int i = 0; i < lyricList.Length; i++)
                            {
                                xPositionIntervalList[i] = (i - lyricList.Length / 2 + 0.5f) * positionInterval.x;
                                yPositionIntervalList[i] = (i - lyricList.Length / 2 + 0.5f) * positionInterval.y;
                            }
                        }
                        else // 奇数
                        {
                            for (int i = 0; i < lyricList.Length; i++)
                            {
                                xPositionIntervalList[i] = (i - (lyricList.Length - 1) / 2f) * positionInterval.x;
                                yPositionIntervalList[i] = (i - (lyricList.Length - 1) / 2f) * positionInterval.y;
                            }
                        }

                        int generationNum = lyricGenerationMode == LyricGenerationMode.GenerateAllAtOnce ? lyricList.Length : 1;


                        for (int i = 0; i < generationNum; i++)
                        {
                            if (string.IsNullOrWhiteSpace(lyricList[i]))
                            {
                                continue;
                            }
                            for (int floor = affectTileRangeFrom; floor <= affectTileRangeTo; floor++)
                            {
                                LevelEvent levelEventAD = lyricGeneratedAs == LyricGeneratedAs.BuiltInText ? new LevelEvent(floor, LevelEventType.AddText) : new LevelEvent(floor, LevelEventType.AddDecoration);
                                LevelEvent levelEventMD1 = new LevelEvent(floor, LevelEventType.MoveDecorations);
                                LevelEvent levelEventMD2 = new LevelEvent(floor, LevelEventType.MoveDecorations);
                                float scale = UnityEngine.Random.Range(scaleRange.Item1, scaleRange.Item2);
                                float scale2 = UnityEngine.Random.Range(lyricDisappearScaleRange.Item1, lyricDisappearScaleRange.Item2);
                                float parallax = UnityEngine.Random.Range(parallaxRange.Item1, parallaxRange.Item2);
                                float parallax2 = UnityEngine.Random.Range(lyricDisappearParallaxRange.Item1, lyricDisappearParallaxRange.Item2);

                                levelEventAD["tag"] = $"{tag} {tag}_{lyricList[i]}_{i + 1}";
                                levelEventAD["relativeTo"] = DecPlacementType.Tile;
                                levelEventAD["floor"] = floor;
                                levelEventAD["position"] = new Vector2(xPositionIntervalList[i], yPositionIntervalList[i]);
                                levelEventAD["parallax"] = dataPanel.disabled["parallaxRange"] ? new Vector2(0f, 0f) : new Vector2(parallax, parallax);
                                levelEventAD["depth"] = (int)parallax;
                                levelEventAD["scale"] = dataPanel.disabled["scaleRange"] ? new Vector2(100f, 100f) : new Vector2(scale, scale);
                                levelEventAD["color"] = color;

                                if (lyricGeneratedAs == LyricGeneratedAs.BuiltInText)
                                {
                                    levelEventAD["decText"] = lyricList[i];
                                    levelEventAD["font"] = FontName.Arial;
                                }
                                else
                                {
                                    string ttfPath = useCustomFont ? fontPath : Path.Combine(Main.ModEntry.Path, "Fonts", $"{font.ToString()}.ttf");
                                    // 检查文件是否存在
                                    if (string.IsNullOrEmpty(ttfPath) || !File.Exists(ttfPath))
                                    {
                                        Popup.ShowMessage(Main.Localizations.GetValue("mh.fileDoesNotExist"));
                                        return;
                                    }

                                    // 检查扩展名
                                    string ext = Path.GetExtension(ttfPath).ToLower();
                                    if (ext != ".ttf" && ext != ".otf")
                                    {
                                        Popup.ShowMessage(Main.Localizations.GetValue("mh.pleaseInputValidFontFile"));
                                        return;
                                    }

                                        

                                    string ttfName = useCustomFont ? Path.GetFileNameWithoutExtension(ttfPath) : font.ToString();
                                    //生成歌词图片
                                    TextToPNG.GeneratePNG(
                                        CleanFileName(lyricList[i]),
                                        Path.Combine(Path.GetDirectoryName(levelPath), $"{tag}_[{ttfName}]{CleanFileName(lyricList[i])}.png"),
                                        fontPath: ttfPath,
                                        fontSize: 80,

                                        // 描边
                                        enableStroke: useStroke,
                                        strokeSize: strokeSize,
                                        strokeColor: RgbaToColor(strokeColor),

                                        // 阴影
                                        enableShadow: useShadow,
                                        shadowOffset: shadowOffset,
                                        shadowBlurRadius: shadowSpread,
                                        shadowDensity: 0.1f + (shadowDensity / 10f) * 1.9f,
                                        shadowColor: RgbaToColor(shadowColor)

                                        //文字颜色
                                        //color: RgbaToColor(color)
                                    );

                                    levelEventAD["decorationImage"] = $"{tag}_[{ttfName}]{CleanFileName(lyricList[i])}.png";
                                }

                                levelEventMD1.disabled["positionOffset"] = dataPanel.disabled["xPosOffsetRange"] && dataPanel.disabled["yPosOffsetRange"];
                                levelEventMD1.disabled["pivotOffset"] = dataPanel.disabled["xPivotOffsetRange"] && dataPanel.disabled["yPivotOffsetRange"];
                                levelEventMD1.disabled["parallaxOffset"] = dataPanel.disabled["xParallaxOffsetRange"] && dataPanel.disabled["yParallaxOffsetRange"];
                                levelEventMD1.disabled["rotationOffset"] = dataPanel.disabled["rotationOffsetRange"];
                                levelEventMD1.disabled["scale"] = true;
                                levelEventMD1.disabled["opacity"] = true;
                                levelEventMD2.disabled["positionOffset"] = dataPanel.disabled["xPosOffsetRange"] && dataPanel.disabled["yPosOffsetRange"];
                                levelEventMD2.disabled["pivotOffset"] = dataPanel.disabled["xPivotOffsetRange"] && dataPanel.disabled["yPivotOffsetRange"];
                                levelEventMD2.disabled["parallax"] = dataPanel.disabled["parallaxRevertTo"];
                                levelEventMD2.disabled["parallaxOffset"] = dataPanel.disabled["xParallaxOffsetRange"] && dataPanel.disabled["yParallaxOffsetRange"];
                                levelEventMD2.disabled["rotationOffset"] = dataPanel.disabled["rotationOffsetRange"];
                                levelEventMD2.disabled["scale"] = dataPanel.disabled["scaleRevertTo"];
                                levelEventMD2.disabled["opacity"] = dataPanel.disabled["opacity"];
                                

                                levelEventMD1["duration"] = 0f;
                                levelEventMD1["tag"] = $"{tag}_{lyricList[i]}_{i + 1}";
                                levelEventMD1["relativeTo"] = DecPlacementType.Tile;
                                levelEventMD1["positionOffset"] = new Vector2(dataPanel.disabled["xPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(xPosOffsetRange.Item1, xPosOffsetRange.Item2), dataPanel.disabled["yPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(yPosOffsetRange.Item1, yPosOffsetRange.Item2));
                                levelEventMD1["pivotOffset"] = new Vector2(dataPanel.disabled["xPivotOffsetRange"] ? float.NaN : UnityEngine.Random.Range(xPivotOffsetRange.Item1, xPivotOffsetRange.Item2), dataPanel.disabled["yPivotOffsetRange"] ? float.NaN : UnityEngine.Random.Range(yPivotOffsetRange.Item1, yPivotOffsetRange.Item2));
                                levelEventMD1["parallaxOffset"] = new Vector2(dataPanel.disabled["xParallaxOffsetRange"] ? float.NaN : UnityEngine.Random.Range(xParallaxOffsetRange.Item1, xParallaxOffsetRange.Item2), dataPanel.disabled["yParallaxOffsetRange"] ? float.NaN : UnityEngine.Random.Range(yParallaxOffsetRange.Item1, yParallaxOffsetRange.Item2));
                                levelEventMD1["rotationOffset"] = UnityEngine.Random.Range(rotationOffsetRange.Item1, rotationOffsetRange.Item2);
                                levelEventMD1["angleOffset"] = angleOffset + (lyricGenerationMode == LyricGenerationMode.GenerateAllAtOnce ? timeInterval * i : 0f);

                                levelEventMD2["duration"] = duration;
                                levelEventMD2["tag"] = $"{tag}_{lyricList[i]}_{i + 1}";
                                levelEventMD2["relativeTo"] = DecPlacementType.Tile;
                                levelEventMD2["positionOffset"] = new Vector2(dataPanel.disabled["xPosOffsetRange"] ? float.NaN : 0, dataPanel.disabled["yPosOffsetRange"] ? float.NaN : 0);
                                levelEventMD2["pivotOffset"] = new Vector2(dataPanel.disabled["xPivotOffsetRange"] ? float.NaN : 0, dataPanel.disabled["yPivotOffsetRange"] ? float.NaN : 0);
                                levelEventMD2["parallax"] = dataPanel.disabled["parallaxRevertTo"] ? new Vector2(0f, 0f) : new Vector2(parallaxRevertTo, parallaxRevertTo);
                                levelEventMD2["parallaxOffset"] = new Vector2(dataPanel.disabled["xParallaxOffsetRange"] ? float.NaN : 0, dataPanel.disabled["yParallaxOffsetRange"] ? float.NaN : 0);
                                levelEventMD2["rotationOffset"] = 0f;
                                levelEventMD2["scale"] = new Vector2(scaleRevertTo, scaleRevertTo);
                                levelEventMD2["opacity"] = opacity;
                                levelEventMD2["angleOffset"] = angleOffset + (lyricGenerationMode == LyricGenerationMode.GenerateAllAtOnce ? timeInterval * i : 0f);
                                levelEventMD2["ease"] = ease;

                                editor.levelData.decorations.Add(levelEventAD);
                                editor.events.Add(levelEventMD1);
                                editor.events.Add(levelEventMD2);
                                scrDecorationManager.instance.CreateDecoration(levelEventAD, out _, -1);

                                if (lyricDisappearAnimation)
                                {
                                    LevelEvent levelEventMD3 = new LevelEvent(floor, LevelEventType.MoveDecorations);
                                    levelEventMD3.disabled["positionOffset"] = dataPanel.disabled["lyricDisappearXPosOffsetRange"] && dataPanel.disabled["lyricDisappearYPosOffsetRange"];
                                    levelEventMD3.disabled["pivotOffset"] = dataPanel.disabled["lyricDisappearXPivotOffsetRange"] && dataPanel.disabled["lyricDisappearYPivotOffsetRange"];
                                    levelEventMD3.disabled["parallaxOffset"] = dataPanel.disabled["lyricDisappearXParallaxOffsetRange"] && dataPanel.disabled["lyricDisappearYParallaxOffsetRange"];
                                    levelEventMD3.disabled["rotationOffset"] = dataPanel.disabled["lyricDisappearRotationOffsetRange"];
                                    levelEventMD3.disabled["scale"] = dataPanel.disabled["lyricDisappearScaleRange"];
                                    levelEventMD3.disabled["opacity"] = dataPanel.disabled["lyricDisappearOpacity"];
                                    levelEventMD3.disabled["parallax"] = dataPanel.disabled["lyricDisappearParallaxRange"];

                                    levelEventMD3["duration"] = lyricDisappearDuration;
                                    levelEventMD3["tag"] = $"{tag}_{lyricList[i]}_{i + 1}";
                                    levelEventMD3["relativeTo"] = DecPlacementType.Tile;
                                    levelEventMD3["positionOffset"] = new Vector2(dataPanel.disabled["lyricDisappearXPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(xPosOffsetRange.Item1, xPosOffsetRange.Item2), dataPanel.disabled["lyricDisappearYPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(yPosOffsetRange.Item1, yPosOffsetRange.Item2));
                                    levelEventMD3["pivotOffset"] = new Vector2(dataPanel.disabled["lyricDisappearXPivotOffsetRange"] ? float.NaN : UnityEngine.Random.Range(xPivotOffsetRange.Item1, xPivotOffsetRange.Item2), dataPanel.disabled["lyricDisappearYPivotOffsetRange"] ? float.NaN : UnityEngine.Random.Range(yPivotOffsetRange.Item1, yPivotOffsetRange.Item2));
                                    levelEventMD3["parallaxOffset"] = new Vector2(dataPanel.disabled["lyricDisappearXParallaxOffsetRange"] ? float.NaN : UnityEngine.Random.Range(xParallaxOffsetRange.Item1, xParallaxOffsetRange.Item2), dataPanel.disabled["lyricDisappearYParallaxOffsetRange"] ? float.NaN : UnityEngine.Random.Range(yParallaxOffsetRange.Item1, yParallaxOffsetRange.Item2));
                                    levelEventMD3["rotationOffset"] = UnityEngine.Random.Range(lyricDisappearRotationOffsetRange.Item1, lyricDisappearRotationOffsetRange.Item2);
                                    levelEventMD3["scale"] = dataPanel.disabled["lyricDisappearScaleRange"] ? new Vector2(100f, 100f) : new Vector2(scale2, scale2);
                                    levelEventMD3["opacity"] = dataPanel.disabled["lyricDisappearOpacity"] ? 100f : lyricDisappearOpacity;
                                    levelEventMD3["parallax"] = dataPanel.disabled["lyricDisappearParallaxRange"] ? new Vector2(0f, 0f) : new Vector2(parallax2, parallax2);
                                    levelEventMD3["angleOffset"] = angleOffset + (lyricGenerationMode == LyricGenerationMode.GenerateAllAtOnce ? timeInterval * i : 0f) + lyricDisappearAfter * 180f;
                                    levelEventMD3["ease"] = lyricDisappearEase;

                                    editor.events.Add(levelEventMD3);
                                }
                            }

                        }




                        //if (lyricGenerationMode == LyricGenerationMode.GenerateOneByOne)
                        //{
                        //    if (delimiter == Delimiter.SplitBySpace)
                        //    {
                        //        if (lyricList.Length > 1)
                        //        {
                        //            lyric = string.Join(" ", lyricList.Skip(1));
                        //        }
                        //        else
                        //        {
                        //            lyric = "";
                        //        }
                        //    }
                        //    else // 逐字切割
                        //    {
                        //        if (lyric.Length > 1)
                        //        {
                        //            lyric = lyric.Substring(1);
                        //        }
                        //        else
                        //        {
                        //            lyric = "";
                        //        }
                        //    }
                        //    (properties["lyric"].control as PropertyControl_Text).text = lyric;
                        //    dataPanel["lyric"] = lyric;
                        //}
                        editor.RemakePath(true, true);
                        break;
                    case Features.GenerateTrack:
                        float tail_GT = (trackDatas[affectTileRangeTo].head + 180) % 360;
                        bool isCCW_GT = listFloors[affectTileRangeTo].isCCW;

                        var matches = Regex.Matches(trackAngleData, @"(-?\d+(?:\.\d+)?)([Tt]?)");

                        List<float> processedTrackAngleData = new List<float>();
                        List<bool> goTwirl = new List<bool>();

                        foreach (Match match in matches)
                        {
                            // 角度
                            processedTrackAngleData.Add(float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture));
                            // 是否有 Twirl
                            goTwirl.Add(match.Groups[2].Length > 0);
                        }

                        for (int i = 0; i < processedTrackAngleData.Count; i++)
                        {
                            //if (processedTrackAngleData[i] == 999)
                            //    continue;

                            float value = processedTrackAngleData[i] % 360;

                            if (value <= 0)
                                value += 360;

                            processedTrackAngleData[i] = value;
                        }

                        List<float> newTrack_GT = new List<float>();
                        List<bool> newTrack_GT_goTwirl = new List<bool>();
                        List<int> addTwirlToTheseTrack = new List<int>();
                        isCCW_GT = goTwirl[0] ? !isCCW_GT : isCCW_GT;

                        for (int i = 0; i < generationCount; i++)
                        {
                            newTrack_GT.AddRange(processedTrackAngleData);
                            newTrack_GT_goTwirl.AddRange(goTwirl);
                        }

                        for (int i = 0; i < newTrack_GT.Count; i++)
                        {
                            newTrack_GT[i] = correctDirection(tail_GT + (isCCW_GT ? newTrack_GT[i] : -newTrack_GT[i]));
                            tail_GT = (newTrack_GT[i] + 180) % 360;
                            if(newTrack_GT_goTwirl[i])
                            {
                                addTwirlToTheseTrack.Add(affectTileRangeTo + 1 + i);
                                isCCW_GT = !isCCW_GT;
                            }
                        }
                        OffsetFloorIDsInEvents(affectTileRangeTo, newTrack_GT.Count);
                        scnEditor.instance.levelData.angleData.InsertRange(affectTileRangeTo + 1, newTrack_GT);

                        for (int i = 0; i < addTwirlToTheseTrack.Count; i++)
                        {
                            editor.events.Add(new LevelEvent(addTwirlToTheseTrack[i], LevelEventType.Twirl));
                        }

                        dataPanel["previewTrack"] = false;
                        (properties["previewTrack"].control as PropertyControl_Bool).value = false;

                        //Main.showingFakeFloor = false;

                        scnEditor.instance.RemakePath(true, true);
                        break;
                    default:
                        return;
                }
            }
        }

        private static int GetIndex(object data)
        {
            Tuple<int, TileRelativeTo> tuple = (Tuple<int, TileRelativeTo>)data;
            int length = ADOBase.lm.floorAngles.Length;
            return Mathf.Clamp(tuple.Item1 + (tuple.Item2 == TileRelativeTo.End ? length : 0), 0, length);
        }

        public static TrackData[] getTrackDatas(Dictionary<int, Dictionary<LevelEventType, List<LevelEvent>>> floorEvents = null)
        {
            AngleData[] angles = getAnglesData();
            TrackData[] result = new TrackData[angles.Length];
            List<scrFloor> listFloors = scrLevelMaker.instance.listFloors;
            float bpm = scnEditor.instance.levelData.bpm;
            float arrivalTime = 0;
            float departureTime = angles[0].angle / (3f * bpm);

            result[0] = new TrackData(angles[0].angle, angles[0].head, angles[0].tail, Vector2.zero, 0, false, 0, false, 0, 0, bpm, arrivalTime, departureTime);

            arrivalTime = departureTime;
            for (int floor = 1; floor < angles.Length; floor++)
            {
                bool isPause = false;
                float pauseDuration = 0;
                bool isHold = false;
                int holdDuration = 0;
                int holdDistance = 0;

                if (floorEvents != null)
                {
                    if (floorEvents.TryGetValue(floor, out Dictionary<LevelEventType, List<LevelEvent>> events))
                    {
                        if (events.TryGetValue(LevelEventType.Hold, out List<LevelEvent> hold_events))
                        {
                            isHold = true;
                            holdDuration = (int)hold_events[0]["duration"];
                            holdDistance = (int)hold_events[0]["distanceMultiplier"];
                        }

                        if (events.TryGetValue(LevelEventType.Pause, out List<LevelEvent> pause_events))
                        {
                            isPause = true;
                            pauseDuration = (float)pause_events[0]["duration"];
                        }

                        if (events.TryGetValue(LevelEventType.SetSpeed, out List<LevelEvent> bpm_events))
                        {
                            foreach (LevelEvent bpm_event in bpm_events)
                            {
                                switch (bpm_event["speedType"])
                                {
                                    case SpeedType.Bpm:
                                        bpm = (float)bpm_event["beatsPerMinute"];
                                        break;
                                    case SpeedType.Multiplier:
                                        bpm *= (float)bpm_event["bpmMultiplier"];
                                        break;
                                }
                            }
                        }
                    }
                }
                float duration = listFloors[floor].midSpin ? 0f : (60f / bpm) * (angles[floor].angle / 180f + pauseDuration + 2 * holdDuration);
                departureTime = arrivalTime + duration;
                result[floor] = new TrackData(angles[floor].angle, angles[floor].head, angles[floor].tail, listFloors[floor].transform.position / 1.5f, listFloors[floor].transform.rotation.eulerAngles.z, isPause, pauseDuration, isHold, holdDuration, holdDistance, bpm, arrivalTime, departureTime);
                arrivalTime = departureTime;

            }

            return result;
        }

        public static AngleData[] getAnglesData()
        {
            float[] floorAngles = scrLevelMaker.instance.floorAngles;
            List<scrFloor> listFloors = scrLevelMaker.instance.listFloors;
            AngleData[] result = new AngleData[floorAngles.Length + 1];
            float head = floorAngles[0];
            float tail = 180;
            for (int i = 0; i < result.Length - 1; i++)
            {
                if (floorAngles[i] == 999f)
                {
                    result[i] = new AngleData(999f, head, tail);
                    tail = head;
                    continue;
                }
                else
                {
                    floorAngles[i] = correctDirection(floorAngles[i]);
                }
                head = floorAngles[i];
                float angle = (listFloors[i].isCCW ? head - tail : tail - head) == 0 ? 360 : listFloors[i].isCCW ? head - tail : tail - head;
                angle = correctAngle(angle);
                result[i] = new AngleData(angle, head, tail);
                tail = (head + 180) % 360;
            }
            result[result.Length - 1] = new AngleData(180, head, tail);
            return result;
        }

        public class AngleData
        {
            public float angle;
            public float head;
            public float tail;
            public AngleData(float angle, float head, float tail)
            {
                this.angle = angle;
                this.head = head;
                this.tail = tail;
            }
        }

        public class TrackData
        {
            public float angle;
            public float head;
            public float tail;
            public Vector2 position;
            public float rotation;
            public bool isPause;
            public float pauseDuration;
            public bool isHold;
            public int holdDuration;
            public int holdDistance;
            public float bpm;
            public float arrivalTime;
            public float departureTime;
            public TrackData(float angle, float head, float tail, Vector2 position, float rotation, bool isPause, float pauseDuration, bool isHold, int holdDuration, int holdDistance, float bpm, float arrivalTime, float departureTime)
            {
                this.angle = angle;
                this.head = head;
                this.tail = tail;
                this.position = position;
                this.rotation = rotation;
                this.isPause = isPause;
                this.isHold = isHold;
                this.pauseDuration = isPause ? pauseDuration : 0;
                this.holdDuration = isHold ? holdDuration : 0;
                this.holdDistance = isHold ? holdDistance : 0;
                this.bpm = bpm;
                this.arrivalTime = arrivalTime;
                this.departureTime = departureTime;
            }
        }

        public static Vector2 GetPivotOffset(Vector2 offset, float rotation)
        {
            float rad = rotation * Mathf.Deg2Rad;

            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            float newX = cos * offset.x + sin * offset.y;
            float newY = -sin * offset.x + cos * offset.y;

            return new Vector2(newX, newY);
        }

        public static float correctDirection(float direction)
        {
            direction %= 360f;
            if (direction < 0) direction += 360f;
            return direction;
        }

        public static float correctAngle(float direction)
        {
            direction %= 360f;
            if (direction <= 0) direction += 360f;
            return direction;
        }

        public static Tuple<int, int> getAffectedRange()
        {
            LevelEvent levelEvent = Main.GetEvent((LevelEventType)Main.MappingHelper.type);
            int v1 = 0;
            int v2 = 0;
            AffectAt affect = (AffectAt)levelEvent["affectAt"];
            switch (affect)
            {
                case AffectAt.SpecificRange:
                    v1 = GetIndex(levelEvent["affectTileRangeFrom"]);
                    v2 = GetIndex(levelEvent["affectTileRangeTo"]);
                    break;
                case AffectAt.SelectedTiles:
                    if (scnEditor.instance.selectedFloors != null && scnEditor.instance.selectedFloors.Count > 0)
                    {
                        v1 = scnEditor.instance.selectedFloors[0].seqID;
                        v2 = scnEditor.instance.selectedFloors[scnEditor.instance.selectedFloors.Count - 1].seqID;
                    }
                    else
                    {
                        v1 = -1;
                        v2 = -1;
                    }
                    break;
            }

            return Tuple.Create(v1, v2);
        }

        public static float getMidDirection(float startDirection, float endDirection, bool isCCW, bool goFullCircleIfEqual)
        {
            startDirection = (startDirection % 360 + 360) % 360;
            endDirection = (endDirection % 360 + 360) % 360;

            float delta;
            if (isCCW)
            {
                delta = (endDirection - startDirection + 360) % 360;
                if (delta == 0 && goFullCircleIfEqual) delta = 360;
                return (startDirection + delta / 2) % 360;
            }
            else
            {
                delta = (startDirection - endDirection + 360) % 360;
                if (delta == 0 && goFullCircleIfEqual) delta = 360;
                return (startDirection - delta / 2 + 360) % 360;
            }
        }

        public static void ExtraVideo(MonoBehaviour runner, string videoPath, string outputFolder, ImageFormat format)
        {
            runner.StartCoroutine(new ExtraVideo().ExtractFramesCoroutine(videoPath, outputFolder, format));
        }

        public static void OffsetFloorIDsInEvents(int startFloorID, int offset)
        {
            List<LevelEvent>[] array = new List<LevelEvent>[]
            {
            scnEditor.instance.events,
            scnEditor.instance.decorations
            };
            for (int i = 0; i < array.Length; i++)
            {
                foreach (LevelEvent levelEvent in array[i])
                {
                    if (levelEvent.floor > startFloorID)
                    {
                        levelEvent.floor += offset;
                    }
                }
            }
        }

        public static System.Drawing.Color RgbaToColor(string rgbaHex)
        {
            // 输入: "FFFFFFFF" (RGBA)
            // 输出: Color (ARGB)

            if (rgbaHex.Length != 8)
                throw new ArgumentException("必须是8位十六进制");

            uint rgba = Convert.ToUInt32(rgbaHex, 16);

            // 提取 RGBA 各通道
            byte r = (byte)((rgba >> 24) & 0xFF);  // 第1-2位
            byte g = (byte)((rgba >> 16) & 0xFF);  // 第3-4位
            byte b = (byte)((rgba >> 8) & 0xFF);   // 第5-6位
            byte a = (byte)(rgba & 0xFF);          // 第7-8位

            // 转换为 ARGB 格式
            return System.Drawing.Color.FromArgb(a, r, g, b);
        }

        public static string CleanFileName(string name)
        {
            var invalid = Path.GetInvalidFileNameChars();
            return string.Concat(name.Where(c => !invalid.Contains(c)));
        }
    }

    internal class ExtraVideo
    {
        public IEnumerator ExtractFramesCoroutine(string videoPath, string outputFolder, ImageFormat format)
        {
            int frameIndex = 0;
            if (!File.Exists(videoPath)) yield break;

            // 自动创建 VideoPlayer
            VideoPlayer videoPlayer = new GameObject("TempVideoPlayer").AddComponent<VideoPlayer>();
            videoPlayer.url = videoPath;
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = false;
            videoPlayer.playbackSpeed = 0f; // 暂停，用 StepForward 逐帧

            // 确保输出文件夹存在
            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            // 视频准备完成后开始提取帧
            videoPlayer.Prepare();
            while (!videoPlayer.isPrepared)
                yield return null;

            // 创建 RenderTexture 与视频分辨率一致
            RenderTexture renderTexture = new RenderTexture((int)videoPlayer.width, (int)videoPlayer.height, 0, RenderTextureFormat.ARGB32);
            renderTexture.Create();
            videoPlayer.targetTexture = renderTexture;

            // -------------------------------
            // 提取剩余帧
            while (videoPlayer.frame < (long)videoPlayer.frameCount - 1)
            {
                videoPlayer.StepForward();
                yield return new WaitForEndOfFrame();

                string path = Path.Combine(outputFolder, $"{Path.GetFileNameWithoutExtension(videoPath)} ({frameIndex + 1}).{format.ToString().ToLower()}");

                if (!File.Exists(path))
                {
                    RenderTexture.active = renderTexture;
                    Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
                    tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                    tex.Apply();
                    RenderTexture.active = null;
                    Main.Logger.Log($"2 - {format}");

                    if (format == ImageFormat.PNG)
                    {
                        File.WriteAllBytes(path, tex.EncodeToPNG());
                    }
                    else if (format == ImageFormat.JPG)
                    {
                        File.WriteAllBytes(path, tex.EncodeToJPG());
                    }

                    UnityEngine.GameObject.Destroy(tex);
                }

                frameIndex++;
            }

            // 清理资源
            renderTexture.Release();
            UnityEngine.GameObject.Destroy(renderTexture);
            UnityEngine.GameObject.Destroy(videoPlayer);
        }
    }


    public static class ColorGradientUtil
    {
        /// <summary>
        /// 在两个颜色之间生成 n 个颜色（线性插值，sRGB + alpha）。
        /// 例：n=5，返回 5 个色：起点、3 个中间、终点。
        /// </summary>
        public static List<string> SplitGradientHex(string startHex, string endHex, int n,
                                                    bool includeEndpoints = true, bool withAlpha = true)
        {
            if (n <= 0) throw new ArgumentOutOfRangeException(nameof(n), "n 必须 > 0");

            if (!ColorUtility.TryParseHtmlString(startHex, out var c1))
                throw new ArgumentException($"Invalid Start Color：{startHex}");
            if (!ColorUtility.TryParseHtmlString(endHex, out var c2))
                throw new ArgumentException($"Invalid End Color：{endHex}");

            var list = new List<string>(n);

            if (includeEndpoints)
            {
                if (n == 1)
                {
                    list.Add(ToHex(c1, withAlpha));
                    return list;
                }

                for (int i = 0; i < n; i++)
                {
                    float t = (n == 1) ? 0f : i / (float)(n - 1); // 0..1（含端点）
                    var c = UnityEngine.Color.LerpUnclamped(c1, c2, t);
                    list.Add(ToHex(c, withAlpha));
                }
            }
            else
            {
                // 只要中间 n 个点（不含两端）
                for (int i = 1; i <= n; i++)
                {
                    float t = i / (float)(n + 1); // (0,1) 内部均分
                    var c = UnityEngine.Color.LerpUnclamped(c1, c2, t);
                    list.Add(ToHex(c, withAlpha));
                }
            }

            return list;
        }

        private static string ToHex(UnityEngine.Color c, bool withAlpha)
        {
            var r = Mathf.Clamp(Mathf.RoundToInt(c.r * 255f), 0, 255);
            var g = Mathf.Clamp(Mathf.RoundToInt(c.g * 255f), 0, 255);
            var b = Mathf.Clamp(Mathf.RoundToInt(c.b * 255f), 0, 255);
            var a = Mathf.Clamp(Mathf.RoundToInt(c.a * 255f), 0, 255);

            return withAlpha
                ? $"{r:X2}{g:X2}{b:X2}{a:X2}"
                : $"{r:X2}{g:X2}{b:X2}";
        }
    }

}
