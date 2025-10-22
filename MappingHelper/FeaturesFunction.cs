using ADOFAI;
using ADOFAI.LevelEditor.Controls;
using DG.Tweening;
using Discord;
using HarmonyLib;
using MappingHelper.Utils;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Linq;
using OggVorbisEncoder.Setup;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.Video;
using UnityEngine.Windows;
using UnityStandardAssets.ImageEffects;

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

                Features feature = (Features)dataPanel.data["FeaturesOption"];
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

                Tuple<int, TileRelativeTo> startTile = (Tuple<int, TileRelativeTo>)dataPanel.data["startTile"];
                Tuple<int, TileRelativeTo> endTile = (Tuple<int, TileRelativeTo>)dataPanel.data["endTile"];
                float duration = (float)dataPanel.data["duration"];
                Tuple<float, float> xPosOffsetRange = (Tuple<float, float>)dataPanel.data["xPosOffsetRange"];
                Tuple<float, float> yPosOffsetRange = (Tuple<float, float>)dataPanel.data["yPosOffsetRange"];
                Tuple<float, float> rotationOffsetRange = (Tuple<float, float>)dataPanel.data["rotationOffsetRange"];
                Tuple<float, float> scaleRange = (Tuple<float, float>)dataPanel.data["scaleRange"];
                float scaleRevertTo = (float)dataPanel.data["scaleRevertTo"];
                Tuple<float, float> parallaxRange = (Tuple<float, float>)dataPanel.data["parallaxRange"];
                float parallaxRevertTo = (float)dataPanel.data["parallaxRevertTo"];
                float opacity = (float)dataPanel.data["opacity"];
                float angleOffset = (float)dataPanel.data["angleOffset"];
                Ease ease = (Ease)dataPanel.data["ease"];
                string tag = (string)dataPanel.data["tag"];
                TrackDistribution trackDistribution = (TrackDistribution)dataPanel.data["TrackDistribution"];
                TrackAnimation trackAnimation = (TrackAnimation)dataPanel.data["TrackAnimation"];
                TrackFeatures trackFeatures = (TrackFeatures)dataPanel.data["TrackFeatures"];
                FileType fileType = (FileType)dataPanel.data["FileType"];
                string levelPath = scnEditor.instance.customLevel.levelPath;
                string directoryPath = (string)dataPanel.data["selectDirectory"];
                string imagePath = (string)dataPanel.data["selectImage"];
                string videoPath = (string)dataPanel.data["selectVideo"];
                int imageStart = (int)dataPanel.data["imageStart"];
                int imageEnd = (int)dataPanel.data["imageEnd"];
                string eventTag = (string)dataPanel.data["eventTag"];
                bool selectFrame = (bool)dataPanel.data["selectFrame"];
                Vector2 positionStartValue = (Vector2)dataPanel.data["positionStartValue"];
                Vector2 positionEndValue = (Vector2)dataPanel.data["positionEndValue"];
                Vector2 pivotStartValue = (Vector2)dataPanel.data["pivotStartValue"];
                Vector2 pivotEndValue = (Vector2)dataPanel.data["pivotEndValue"];
                float rotationStartValue = (float)dataPanel.data["rotationStartValue"];
                float rotationEndValue = (float)dataPanel.data["rotationEndValue"];
                Vector2 scaleStartValue = (Vector2)dataPanel.data["scaleStartValue"];
                Vector2 scaleEndValue = (Vector2)dataPanel.data["scaleEndValue"];
                Vector2 parallaxStartValue = (Vector2)dataPanel.data["parallaxStartValue"];
                Vector2 parallaxEndValue = (Vector2)dataPanel.data["parallaxEndValue"];
                float opacityStartValue = (float)dataPanel.data["opacityStartValue"];
                float opacityEndValue = (float)dataPanel.data["opacityEndValue"];
                int depthStartValue = (int)dataPanel.data["depthStartValue"];
                int depthEndValue = (int)dataPanel.data["depthEndValue"];
                string colorStartValue = "#" + (string)dataPanel.data["colorStartValue"];
                string colorEndValue = "#" + (string)dataPanel.data["colorEndValue"];
                int decoCount = (int)dataPanel.data["decoCount"];
                List<string> colorList = ColorGradientUtil.SplitGradientHex(colorStartValue, colorEndValue, decoCount, true, true);
                bool useReverseAngle = (bool)dataPanel.data["useReverseAngle"];
                int vertexCount = (int)dataPanel.data["vertexCount"];
                MagicShapeFeature magicShapeFeature = (MagicShapeFeature)dataPanel.data["magicShapeFeature"];
                TwirlStyle twirlStyle = (TwirlStyle)dataPanel.data["twirlStyle"];
                float bpmValue = (float)dataPanel.data["bpmValue"];
                float magicShapeRotateValue = (float)dataPanel.data["magicShapeRotateValue"];
                ShowedEvent showedEvent = (ShowedEvent)dataPanel.data["showedEvent"];
                Tuple<float, float> positionTrackScale = (Tuple<float, float>)dataPanel.data["positionTrackScale"];
                Tuple<float, float> scaleRadiusScale = (Tuple<float, float>)dataPanel.data["scaleRadiusScale"];
                Tuple<float, float> scalePlanetsScale = (Tuple<float, float>)dataPanel.data["scalePlanetsScale"];
                float initialAngleOffset = (float)dataPanel.data["initialAngleOffset"];
                ImageFormat imageFormat = (ImageFormat)dataPanel.data["imageFormat"]; 
                TrackDistribution planetAnimationDistribution = (TrackDistribution)dataPanel.data["planetAnimationDistribution"];
                Tuple<float, float> parallaxChange = (Tuple<float, float>)dataPanel.data["parallaxChange"];
                TrackDistribution eventDistribution = (TrackDistribution)dataPanel.data["eventDistribution"];

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

                            levelEvent.data["startTile"] = startTile;
                            levelEvent.data["endTile"] = endTile;
                            levelEvent.data["duration"] = duration;
                            levelEvent.data["positionOffset"] = new Vector2(dataPanel.disabled["xPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(xPosOffsetRange.Item1, xPosOffsetRange.Item2), dataPanel.disabled["yPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(yPosOffsetRange.Item1, yPosOffsetRange.Item2));
                            levelEvent.data["rotationOffset"] = UnityEngine.Random.Range(rotationOffsetRange.Item1, rotationOffsetRange.Item2);
                            float scale = UnityEngine.Random.Range(scaleRange.Item1, scaleRange.Item2);
                            levelEvent.data["scale"] = new Vector2(scale, scale);
                            levelEvent.data["opacity"] = opacity;
                            levelEvent.data["angleOffset"] = angleOffset;
                            levelEvent.data["ease"] = ease;
                            editor.events.Add(levelEvent);
                        }

                        editor.RemakePath(true, true);
                        editor.DeselectFloors();
                        editor.SelectFloor(listFloors[affectTileRangeFrom]);
                        break;

                    case Features.TrackAppearAnimation:
                        LevelEvent initialization_levelEvent = new LevelEvent(affectTileRangeFrom, LevelEventType.MoveTrack);
                        initialization_levelEvent.disabled["opacity"] = false;

                        initialization_levelEvent.data["startTile"] = new Tuple<int, TileRelativeTo>(affectTileRangeFrom + startTile.Item1, TileRelativeTo.Start);
                        initialization_levelEvent.data["endTile"] = new Tuple<int, TileRelativeTo>(affectTileRangeTo, TileRelativeTo.Start);
                        initialization_levelEvent.data["duration"] = 0f;
                        initialization_levelEvent.data["opacity"] = 0f;
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

                            levelEvent1.data["startTile"] = startTile;
                            levelEvent1.data["endTile"] = endTile;
                            levelEvent1.data["duration"] = 0f;
                            levelEvent1.data["positionOffset"] = new Vector2(dataPanel.disabled["xPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(xPosOffsetRange.Item1, xPosOffsetRange.Item2), dataPanel.disabled["yPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(yPosOffsetRange.Item1, yPosOffsetRange.Item2));
                            levelEvent1.data["rotationOffset"] = UnityEngine.Random.Range(rotationOffsetRange.Item1, rotationOffsetRange.Item2);
                            float scale = UnityEngine.Random.Range(scaleRange.Item1, scaleRange.Item2);
                            levelEvent1.data["scale"] = new Vector2(scale, scale);

                            levelEvent2.data["startTile"] = startTile;
                            levelEvent2.data["endTile"] = endTile;
                            levelEvent2.data["duration"] = duration;
                            levelEvent2.data["positionOffset"] = new Vector2(dataPanel.disabled["xPosOffsetRange"] ? float.NaN : 0f, dataPanel.disabled["yPosOffsetRange"] ? float.NaN : 0f);
                            levelEvent2.data["rotationOffset"] = 0f;
                            levelEvent2.data["scale"] = new Vector2(scaleRevertTo, scaleRevertTo);
                            levelEvent2.data["opacity"] = opacity;
                            levelEvent2.data["angleOffset"] = angleOffset;
                            levelEvent2.data["ease"] = ease;
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
                        float pivotTrackOffset = isCentralized ? (float)dataPanel.data["trackRotation"] : 0;
                        float startBpm = trackDatas[affectTileRangeFrom].bpm;
                        int trackCount = affectTileRangeTo - affectTileRangeFrom + 1;

                        if ((bool)dataPanel.data["usePlanet"] && trackFeatures == TrackFeatures.CreateTrack)
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
                            levelEvent_BluePlanet["depth"] = (bool)dataPanel.data["useIncreasingDepth"] ? (int)dataPanel.data["initialDepth"] - 1 : -2;
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
                            levelEvent_RedPlanet["depth"] = (bool)dataPanel.data["useIncreasingDepth"] ? (int)dataPanel.data["initialDepth"] - 1 : -2;
                            levelEvent_RedPlanet["scale"] = new Vector2(100 - parallaxChange.Item1, 100 - parallaxChange.Item1);
                            levelEvent_RedPlanet["parallax"] = new Vector2(parallaxChange.Item1, parallaxChange.Item1);

                            editor.levelData.decorations.Add(levelEvent_BluePlanet);
                            scrDecorationManager.instance.CreateDecoration(levelEvent_BluePlanet, out _, -1);
                            editor.levelData.decorations.Add(levelEvent_RedPlanet);
                            scrDecorationManager.instance.CreateDecoration(levelEvent_RedPlanet, out _, -1);
                        }

                        bool moveRedPlanet = false;
                        int trackDepth = (int)dataPanel.data["initialDepth"];
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


                                    if ((bool)dataPanel.data["useIncreasingDepth"])
                                    {
                                        levelEvent["depth"] = trackDepth;
                                        trackDepth += (int)dataPanel.data["increasingValue"];
                                    }


                                    if ((bool)dataPanel.data["usePlanet"])
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

                                        if(planetAnimationDistribution == TrackDistribution.Distributed)
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
                                    if (isCentralized) levelEvent["rotation"] = (float)levelEvent["rotation"] + (float)dataPanel.data["trackRotation"];
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
                                        if (eventDistribution ==TrackDistribution.Distributed)
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
                                                        levelEvent_MD2.data["angleOffset"] = angleOffset;
                                                    }
                                                    else
                                                    {
                                                        float time = trackDatas[floor - 1].departureTime - trackDatas[affectTileRangeFrom].arrivalTime;
                                                        float angleOffsetValue = time / (60f / trackDatas[affectTileRangeFrom].bpm) * 180;
                                                        levelEvent_MD2.data["angleOffset"] = angleOffset + angleOffsetValue;
                                                    }

                                                    levelEvent_MD2.disabled["positionOffset"] = dataPanel.disabled["xPosOffsetRange"] && dataPanel.disabled["yPosOffsetRange"];
                                                    levelEvent_MD2.disabled["rotationOffset"] = dataPanel.disabled["rotationOffsetRange"];
                                                    levelEvent_MD2.disabled["scale"] = dataPanel.disabled["scaleRange"];
                                                    levelEvent_MD2.disabled["opacity"] = dataPanel.disabled["opacity"];
                                                    levelEvent_MD2.disabled["parallax"] = dataPanel.disabled["parallaxRange"];

                                                    levelEvent_MD2.data["tag"] = $"{tag}{i + startTile.Item1}";
                                                    levelEvent_MD2.data["duration"] = duration;
                                                    levelEvent_MD2.data["positionOffset"] = new Vector2(dataPanel.disabled["xPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(xPosOffsetRange.Item1, xPosOffsetRange.Item2), dataPanel.disabled["yPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(yPosOffsetRange.Item1, yPosOffsetRange.Item2));
                                                    levelEvent_MD2.data["rotationOffset"] = UnityEngine.Random.Range(rotationOffsetRange.Item1, rotationOffsetRange.Item2);
                                                    float parallax = UnityEngine.Random.Range(parallaxRange.Item1, parallaxRange.Item2);
                                                    levelEvent_MD2.data["parallax"] = new Vector2(parallax, parallax);
                                                    float scale = UnityEngine.Random.Range(scaleRange.Item1, scaleRange.Item2);
                                                    levelEvent_MD2.data["scale"] = new Vector2(scale, scale);
                                                    levelEvent_MD2.data["opacity"] = opacity;
                                                    levelEvent_MD2.data["ease"] = ease;
                                                    editor.events.Add(levelEvent_MD2);
                                                }
                                                break;
                                            case TrackAnimation.AppearAnimation:
                                                if (floor <= affectTileRangeTo - (startTile.Item1 >= 0 ? startTile.Item1 : 0))
                                                {
                                                    if (eventDistribution == TrackDistribution.Distributed)
                                                    {
                                                        levelEvent_MD1.data["angleOffset"] = angleOffset;
                                                        levelEvent_MD2.data["angleOffset"] = angleOffset;
                                                    }
                                                    else
                                                    {
                                                        float time = trackDatas[floor].arrivalTime - trackDatas[affectTileRangeFrom].arrivalTime;
                                                        float angleOffsetValue = time / (60f / trackDatas[affectTileRangeFrom].bpm) * 180;
                                                        levelEvent_MD1.data["angleOffset"] = angleOffset + angleOffsetValue;
                                                        levelEvent_MD2.data["angleOffset"] = angleOffset + angleOffsetValue;
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

                                                    levelEvent_MD1.data["tag"] = $"{tag}{i + startTile.Item1}";
                                                    levelEvent_MD1.data["duration"] = 0f;
                                                    levelEvent_MD1.data["positionOffset"] = new Vector2(dataPanel.disabled["xPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(xPosOffsetRange.Item1, xPosOffsetRange.Item2), dataPanel.disabled["yPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(yPosOffsetRange.Item1, yPosOffsetRange.Item2));
                                                    levelEvent_MD1.data["rotationOffset"] = UnityEngine.Random.Range(rotationOffsetRange.Item1, rotationOffsetRange.Item2);
                                                    float parallax = UnityEngine.Random.Range(parallaxRange.Item1, parallaxRange.Item2);
                                                    levelEvent_MD1.data["parallax"] = new Vector2(parallax, parallax);
                                                    float scale = UnityEngine.Random.Range(scaleRange.Item1, scaleRange.Item2);
                                                    levelEvent_MD1.data["scale"] = new Vector2(scale, scale);
                                                    levelEvent_MD1.data["opacity"] = 0f;

                                                    levelEvent_MD2.data["tag"] = $"{tag}{i + startTile.Item1}";
                                                    levelEvent_MD2.data["duration"] = duration;
                                                    levelEvent_MD2.data["positionOffset"] = new Vector2(dataPanel.disabled["xPosOffsetRange"] ? float.NaN : 0f, dataPanel.disabled["yPosOffsetRange"] ? float.NaN : 0f);
                                                    levelEvent_MD2.data["rotationOffset"] = 0f;
                                                    levelEvent_MD2.data["parallax"] = new Vector2(parallaxRevertTo, parallaxRevertTo);
                                                    levelEvent_MD2.data["scale"] = new Vector2(scaleRevertTo, scaleRevertTo);
                                                    levelEvent_MD2.data["opacity"] = opacity;
                                                    levelEvent_MD2.data["ease"] = ease;

                                                    editor.events.Add(levelEvent_MD1);
                                                    editor.events.Add(levelEvent_MD2);
                                                }
                                                break;
                                        }
                                        
                                    }
                                    else
                                    {
                                        Popup.ShowMessage(Main.Localizations.GetValue("mh.enterTheTagFirst"));
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
                        if (levelPath.IsNullOrEmpty()) return;
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
                                levelEvent.data["decorationImage"] = Path.Combine(imageNameWithoutExtension, images[0]);
                                levelEvent.data["tag"] = $"{tag}";
                                levelEvent.data["relativeTo"] = DecPlacementType.Tile;
                                levelEvent.data["depth"] = 1;

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

                                            levelEvent_MD.data["duration"] = 0f;
                                            levelEvent_MD.data["tag"] = $"{tag}";
                                            levelEvent_MD.data["decorationImage"] = Path.Combine(imageNameWithoutExtension, images[i - 1]);
                                            levelEvent_MD.data["angleOffset"] = curAngleOffset;
                                            levelEvent_MD.data["eventTag"] = eventTag;
                                            curAngleOffset += angleOffset;

                                            editor.events.Add(levelEvent_MD);
                                        }
                                    }
                                    else if(imageStart > imageEnd)
                                    {
                                        for (int i = start; i >= end; i--)
                                        {
                                            LevelEvent levelEvent_MD = new LevelEvent(floor, LevelEventType.MoveDecorations);
                                            levelEvent_MD.disabled["decorationImage"] = false;
                                            levelEvent_MD.disabled["positionOffset"] = true;

                                            levelEvent_MD.data["duration"] = 0f;
                                            levelEvent_MD.data["tag"] = $"{tag}";
                                            levelEvent_MD.data["decorationImage"] = Path.Combine(imageNameWithoutExtension, images[i - 1]);
                                            levelEvent_MD.data["angleOffset"] = curAngleOffset;
                                            levelEvent_MD.data["eventTag"] = eventTag;
                                            curAngleOffset += angleOffset;

                                            editor.events.Add(levelEvent_MD);
                                        }
                                    }
                                }

                                editor.RemakePath(true, true);
                                editor.DeselectFloors();
                                editor.SelectFloor(listFloors[affectTileRangeFrom]);
                            }
                            else
                            {
                                videoPath = Path.Combine(levelFolderPath, (string)dataPanel.data["selectVideo"]);
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

                                levelEvent.data["decorationImage"] = imagePath;
                                levelEvent.data["tag"] = tag.IsNullOrEmpty() ? string.Empty : $"{tag} {tag}{i + 1}";
                                levelEvent.data["relativeTo"] = DecPlacementType.Tile;
                                levelEvent.data["position"] = new Vector2(xPositionValue, yPositionValue);
                                levelEvent.data["pivotOffset"] = new Vector2(xPivotValue, yPivotValue);
                                levelEvent.data["rotation"] = rotationValue;
                                levelEvent.data["scale"] = new Vector2(xScaleValue, yScaleValue);
                                levelEvent.data["color"] = colorValue;
                                levelEvent.data["opacity"] = opacityValue;
                                levelEvent.data["depth"] = depthValue;
                                levelEvent.data["parallax"] = new Vector2(xParallaxValue, yParallaxValue);

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

                                        if (direction != 999)
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
                                    LevelEvent levelEvent_setSpeed = new LevelEvent(floor, LevelEventType.SetSpeed);
                                    levelEvent_setSpeed.data["beatsPerMinute"] = trueBpm;
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
                            for(int i = startTile.Item1; i <= endTile.Item1; i++)
                            {
                                Tuple<int, TileRelativeTo> tile = Tuple.Create(i, TileRelativeTo.ThisTile);
                                LevelEvent levelEvent = new LevelEvent(floor, LevelEventType.MoveTrack);

                                levelEvent.disabled["positionOffset"] = dataPanel.disabled["xPosOffsetRange"] && dataPanel.disabled["yPosOffsetRange"];
                                levelEvent.disabled["rotationOffset"] = dataPanel.disabled["rotationOffsetRange"];
                                levelEvent.disabled["scale"] = dataPanel.disabled["scaleRange"];
                                levelEvent.disabled["opacity"] = dataPanel.disabled["opacity"];

                                levelEvent.data["startTile"] = tile;
                                levelEvent.data["endTile"] = tile;
                                levelEvent.data["duration"] = duration;
                                levelEvent.data["positionOffset"] = new Vector2(dataPanel.disabled["xPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(xPosOffsetRange.Item1, xPosOffsetRange.Item2), dataPanel.disabled["yPosOffsetRange"] ? float.NaN : UnityEngine.Random.Range(yPosOffsetRange.Item1, yPosOffsetRange.Item2));
                                levelEvent.data["rotationOffset"] = UnityEngine.Random.Range(rotationOffsetRange.Item1, rotationOffsetRange.Item2);
                                float scale = UnityEngine.Random.Range(scaleRange.Item1, scaleRange.Item2);
                                levelEvent.data["scale"] = new Vector2(scale, scale);
                                levelEvent.data["opacity"] = opacity;
                                levelEvent.data["angleOffset"] = trackExplosionAngleExplosion;
                                levelEvent.data["ease"] = ease;
                                trackExplosionAngleExplosion += angleOffset;

                                editor.events.Add(levelEvent);
                            }
                        }

                        editor.RemakePath(true, true);
                        editor.DeselectFloors();
                        editor.SelectFloor(listFloors[affectTileRangeFrom]);
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
                            holdDuration = (int)hold_events[0].data["duration"];
                            holdDistance = (int)hold_events[0].data["distanceMultiplier"];
                        }

                        if (events.TryGetValue(LevelEventType.Pause, out List<LevelEvent> pause_events))
                        {
                            isPause = true;
                            pauseDuration = (float)pause_events[0].data["duration"];
                        }

                        if (events.TryGetValue(LevelEventType.SetSpeed, out List<LevelEvent> bpm_events))
                        {
                            foreach(LevelEvent bpm_event in bpm_events)
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
            int v1=0;
            int v2=0;
            AffectAt affect = (AffectAt)levelEvent.data["affectAt"];
            switch (affect)
            {
                case AffectAt.SpecificRange:
                    v1 = GetIndex(levelEvent.data["affectTileRangeFrom"]);
                    v2 = GetIndex(levelEvent.data["affectTileRangeTo"]);
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

        public static void ExtraVideo(MonoBehaviour runner, string videoPath, string outputFolder,ImageFormat format)
        {
            runner.StartCoroutine(new ExtraVideo().ExtractFramesCoroutine(videoPath, outputFolder,format));
        }
    }

    internal class ExtraVideo
    {
        public IEnumerator ExtractFramesCoroutine(string videoPath, string outputFolder,ImageFormat format)
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
                    var c = Color.LerpUnclamped(c1, c2, t);
                    list.Add(ToHex(c, withAlpha));
                }
            }
            else
            {
                // 只要中间 n 个点（不含两端）
                for (int i = 1; i <= n; i++)
                {
                    float t = i / (float)(n + 1); // (0,1) 内部均分
                    var c = Color.LerpUnclamped(c1, c2, t);
                    list.Add(ToHex(c, withAlpha));
                }
            }

            return list;
        }

        private static string ToHex(Color c, bool withAlpha)
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
