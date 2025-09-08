using ADOFAI;
using DG.Tweening;
using Discord;
using Newtonsoft.Json.Linq;
using OggVorbisEncoder.Setup;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.Video;
using UnityStandardAssets.ImageEffects;

namespace MappingHelper
{
    internal static class FeaturesFunction
    {
        private static bool _creating = false;
        public static void create()
        {
            if (_creating) return;
            _creating = true;

            try
            {
                using (new SaveStateScope(scnEditor.instance))
                {
                    LevelEvent dataPanel = Main.GetEvent((LevelEventType)Main.MappingHelper.type);

                    Features feature = (Features)dataPanel.data["FeaturesOption"];
                    List<scrFloor> listFloors = scrLevelMaker.instance.listFloors;
                    TrackData[] trackDatas = getTrackData();

                    scnEditor editor = scnEditor.instance;
                    if (editor == null) return;
                    
                    int affectTileRangeFrom = GetIndex(dataPanel.data["affectTileRangeFrom"]);
                    int affectTileRangeTo = GetIndex(dataPanel.data["affectTileRangeTo"]);
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
                    TrackDistribution trackDistribution = (TrackDistribution)dataPanel.data["TrackDistrubution"];
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
                            editor.ApplyEventsToFloors();
                            editor.RemakePath(true, true);
                            reselectCurrentFloorAndShowPanel(LevelEventType.MoveTrack);
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
                            editor.ApplyEventsToFloors();
                            editor.RemakePath(true, true);
                            reselectCurrentFloorAndShowPanel(LevelEventType.MoveTrack);
                            break;

                        case Features.MultipleTracks:
                            bool isCentralized = (trackDistribution == TrackDistribution.Centralized);
                            float prevHead = (trackDatas[affectTileRangeFrom].tail + 180) % 360;
                            float pivotTrackOffset = isCentralized ? (float)dataPanel.data["trackRotation"] : 0;

                            Dictionary<int, List<LevelEvent>> pt_dict = new Dictionary<int, List<LevelEvent>>();
                            Dictionary<int, LevelEvent> hold_dict = new Dictionary<int, LevelEvent>();
                            Dictionary<int, LevelEvent> pause_dict = new Dictionary<int, LevelEvent>();

                            foreach (var e in editor.events)
                            {
                                if (e.floor <= affectTileRangeFrom || e.floor >= affectTileRangeTo)
                                    continue;

                                switch (e.eventType)
                                {
                                    case LevelEventType.PositionTrack:
                                        if (!pt_dict.TryGetValue(e.floor, out var list))
                                        {
                                            list = new List<LevelEvent>();
                                            pt_dict[e.floor] = list;
                                        }
                                        list.Add(e);
                                        break;

                                    case LevelEventType.Hold:
                                        hold_dict[e.floor] = e;
                                        break;

                                    case LevelEventType.Pause:
                                        pause_dict[e.floor] = e;
                                        break;
                                }
                            }

                            if ((bool)dataPanel.data["usePlanet"] && trackFeatures==TrackFeatures.CreateTrack)
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

                                levelEvent_RedPlanet["relativeTo"] = DecPlacementType.Tile;
                                levelEvent_RedPlanet["objectType"] = ObjectDecorationType.Planet;
                                levelEvent_RedPlanet["planetColorType"] = PlanetDecorationColorType.Custom;
                                levelEvent_RedPlanet["planetColor"] = "ff0000ff";
                                levelEvent_RedPlanet["planetTailColor"] = "ff000000";
                                levelEvent_RedPlanet["position"] = new Vector2(Mathf.Cos(pivotTrackOffset * Mathf.Deg2Rad), Mathf.Sin(pivotTrackOffset * Mathf.Deg2Rad));
                                levelEvent_RedPlanet["pivotOffset"] = new Vector2(-Mathf.Cos(pivotTrackOffset * Mathf.Deg2Rad), -Mathf.Sin(pivotTrackOffset * Mathf.Deg2Rad));
                                levelEvent_RedPlanet["tag"] = tag.IsNullOrEmpty() ? $"_RedPlanet" : $"{tag} {tag}_RedPlanet";
                                levelEvent_RedPlanet["depth"] = (bool)dataPanel.data["useIncreasingDepth"] ? (int)dataPanel.data["initialDepth"] - 1 : -2;

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

                                        float trackRotation;
                                        LevelEvent levelEvent = new LevelEvent(isCentralized ? affectTileRangeFrom : floor, LevelEventType.AddObject);
                                        levelEvent["relativeTo"] = DecPlacementType.Tile;
                                        levelEvent["position"] = Vector2.zero;
                                        if (trackDatas[floor].angle == 999f)
                                        {
                                            levelEvent["trackType"] = FloorDecorationType.Midspin;
                                        }
                                        else
                                        {
                                            levelEvent["trackAngle"] = trackDatas[floor].angle;
                                        }

                                        if (listFloors[floor].isCCW)
                                        {
                                            trackRotation = (prevHead + 180) % 360 - (180 - trackDatas[floor].angle) % 360;
                                            levelEvent["rotation"] = trackRotation;
                                        }
                                        else
                                        {
                                            trackRotation = prevHead;
                                            levelEvent["rotation"] = trackRotation;
                                        }

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
                                            LevelEvent levelEvent_MovePlanetRed1 = new LevelEvent(floor, LevelEventType.MoveDecorations);
                                            LevelEvent levelEvent_MovePlanetRed2 = new LevelEvent(floor, LevelEventType.MoveDecorations);
                                            LevelEvent levelEvent_MovePlanetBlue1 = new LevelEvent(floor, LevelEventType.MoveDecorations);
                                            LevelEvent levelEvent_MovePlanetBlue2 = new LevelEvent(floor, LevelEventType.MoveDecorations);

                                            levelEvent_MovePlanetRed1.disabled["positionOffset"] = false;
                                            levelEvent_MovePlanetRed1.disabled["rotationOffset"] = false;
                                            levelEvent_MovePlanetRed2.disabled["positionOffset"] = false;
                                            levelEvent_MovePlanetRed2.disabled["rotationOffset"] = false;

                                            levelEvent_MovePlanetBlue1.disabled["positionOffset"] = false;
                                            levelEvent_MovePlanetBlue1.disabled["rotationOffset"] = false;
                                            levelEvent_MovePlanetBlue2.disabled["positionOffset"] = false;
                                            levelEvent_MovePlanetBlue2.disabled["rotationOffset"] = false;

                                            levelEvent_MovePlanetRed1["duration"] = 0f;
                                            levelEvent_MovePlanetRed2["duration"] = trackDatas[floor].angle / 180f;
                                            levelEvent_MovePlanetBlue1["duration"] = 0f;
                                            levelEvent_MovePlanetBlue2["duration"] = trackDatas[floor].angle / 180f;

                                            levelEvent_MovePlanetRed1["tag"] = $"{tag}_RedPlanet";
                                            levelEvent_MovePlanetRed2["tag"] = $"{tag}_RedPlanet";
                                            levelEvent_MovePlanetBlue1["tag"] = $"{tag}_BluePlanet";
                                            levelEvent_MovePlanetBlue2["tag"] = $"{tag}_BluePlanet";

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
                                                    levelEvent_MovePlanetRed2["positionOffset"] = GetPivotOffset(new Vector2(nextFloorPos.x - 2, nextFloorPos.y), -pivotTrackOffset);
                                                    levelEvent_MovePlanetBlue2["positionOffset"] = GetPivotOffset(nextFloorPos, -pivotTrackOffset);
                                                    mambo = true;
                                                }

                                                if (trackDatas[floor].angle != 999)
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
                                                    levelEvent_MovePlanetRed2["positionOffset"] = GetPivotOffset(new Vector2(nextFloorPos.x - 1, nextFloorPos.y), -pivotTrackOffset);
                                                    levelEvent_MovePlanetBlue2["positionOffset"] = GetPivotOffset(new Vector2(nextFloorPos.x - 1, nextFloorPos.y), -pivotTrackOffset);
                                                    mambo = true;
                                                }

                                                if (trackDatas[floor].angle != 999)
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
                                                levelEvent["trackRedSwirl"] = trackDatas[floor].angle < 180;
                                                levelEvent["trackIconFlipped"] = listFloors[floor].isCCW;
                                                levelEvent["trackIconAngle"] = listFloors[floor].isCCW ? ((trackDatas[floor].head == 0 ? 360f : trackDatas[floor].head) + trackDatas[floor].tail) / 2f - (90f + trackRotation) + (trackDatas[floor].angle == 360f ? 180f : 0f) : (trackDatas[floor].head + trackDatas[floor].tail) / 2f - (90f + trackRotation) + (trackDatas[floor].angle == 360f ? 180f : 0f);
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
                                        editor.levelData.decorations.Add(levelEvent);
                                        scrDecorationManager.instance.CreateDecoration(levelEvent, out _, -1);
                                        break;
                                    case TrackFeatures.CreateAnimation:

                                        if (!tag.IsNullOrEmpty())
                                        {
                                            LevelEvent levelEvent_MD1 = new LevelEvent(floor, LevelEventType.MoveDecorations);
                                            LevelEvent levelEvent_MD2 = new LevelEvent(floor, LevelEventType.MoveDecorations);

                                            levelEvent_MD1.disabled["positionOffset"] = dataPanel.disabled["xPosOffsetRange"] && dataPanel.disabled["yPosOffsetRange"];
                                            levelEvent_MD1.disabled["rotationOffset"] = dataPanel.disabled["rotationOffsetRange"];
                                            levelEvent_MD1.disabled["scale"] = dataPanel.disabled["scaleRange"];
                                            levelEvent_MD1.disabled["opacity"] = dataPanel.disabled["opacity"];

                                            levelEvent_MD2.disabled["positionOffset"] = dataPanel.disabled["xPosOffsetRange"] && dataPanel.disabled["yPosOffsetRange"];
                                            levelEvent_MD2.disabled["rotationOffset"] = dataPanel.disabled["rotationOffsetRange"];
                                            levelEvent_MD2.disabled["scale"] = dataPanel.disabled["scaleRevertTo"];
                                            levelEvent_MD2.disabled["opacity"] = dataPanel.disabled["opacity"];

                                            switch (trackAnimation)
                                            {
                                                case TrackAnimation.DisappearAnimation:
                                                    if (i > 1)
                                                    {
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
                                                        levelEvent_MD2.data["angleOffset"] = angleOffset;
                                                        levelEvent_MD2.data["ease"] = ease;
                                                        editor.events.Add(levelEvent_MD2);
                                                        reselectCurrentFloorAndShowPanel(LevelEventType.MoveDecorations);
                                                    }
                                                    break;
                                                case TrackAnimation.AppearAnimation:
                                                    if (floor <= affectTileRangeTo - (startTile.Item1 >= 0 ? startTile.Item1 : 0))
                                                    {
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
                                                        levelEvent_MD1.data["angleOffset"] = angleOffset;

                                                        levelEvent_MD2.data["tag"] = $"{tag}{i + startTile.Item1}";
                                                        levelEvent_MD2.data["duration"] = duration;
                                                        levelEvent_MD2.data["positionOffset"] = new Vector2(dataPanel.disabled["xPosOffsetRange"] ? float.NaN : 0f, dataPanel.disabled["yPosOffsetRange"] ? float.NaN : 0f);
                                                        levelEvent_MD2.data["rotationOffset"] = 0f;
                                                        levelEvent_MD2.data["parallax"] = new Vector2(parallaxRevertTo, parallaxRevertTo);
                                                        levelEvent_MD2.data["scale"] = new Vector2(scaleRevertTo, scaleRevertTo);
                                                        levelEvent_MD2.data["opacity"] = opacity;
                                                        levelEvent_MD2.data["angleOffset"] = angleOffset;

                                                        editor.events.Add(levelEvent_MD1);
                                                        editor.events.Add(levelEvent_MD2);

                                                        reselectCurrentFloorAndShowPanel(LevelEventType.MoveDecorations);
                                                    }
                                                    break;
                                            }
                                        }

                                        break;
                                }

                            }
                            editor.ApplyEventsToFloors();
                            editor.RemakePath(true, true);

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

                                    for (int floor = affectTileRangeFrom; floor <= affectTileRangeTo; floor++)
                                    {
                                        float curAngleOffset = 0f;

                                        int start = selectFrame ? imageStart : 1;
                                        int end = selectFrame ? imageEnd : images.Length;
                                        start = Mathf.Clamp(start, 1, images.Length);
                                        end = Mathf.Clamp(end, 1, images.Length);
                                        for (int i = start; i <= end; i++)
                                        {
                                            LevelEvent levelEvent_MD = new LevelEvent(floor, LevelEventType.MoveDecorations);
                                            levelEvent_MD.disabled["decorationImage"] = false;
                                            levelEvent_MD.disabled["positionOffset"] = true;

                                            levelEvent_MD.data["duration"] = 0f;
                                            levelEvent_MD.data["tag"] = $"{tag}";
                                            levelEvent_MD.data["decorationImage"] = Path.Combine(imageNameWithoutExtension, images[i-1]);
                                            levelEvent_MD.data["angleOffset"] = curAngleOffset;
                                            levelEvent_MD.data["eventTag"] = eventTag;
                                            curAngleOffset += angleOffset;

                                            editor.events.Add(levelEvent_MD);
                                        }
                                    }

                                    editor.ApplyEventsToFloors();
                                    editor.RemakePath(true, true);
                                    reselectCurrentFloorAndShowPanel(LevelEventType.MoveDecorations);
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
                                    ExtraVideo(runner, videoPath, imageOutputrPath);
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
                                    levelEvent.data["position"] = new Vector2(xPositionValue,yPositionValue);
                                    levelEvent.data["pivotOffset"] = new Vector2(xPivotValue, yPivotValue);
                                    levelEvent.data["rotation"] = rotationValue;
                                    levelEvent.data["scale"] = new Vector2(xScaleValue,yScaleValue);
                                    levelEvent.data["color"] = colorValue;
                                    levelEvent.data["opacity"] = opacityValue;
                                    levelEvent.data["depth"] = depthValue;
                                    levelEvent.data["parallax"] = new Vector2(xParallaxValue,yParallaxValue);

                                    editor.levelData.decorations.Add(levelEvent);
                                    scrDecorationManager.instance.CreateDecoration(levelEvent, out _, -1);
                                }
                            }
                            editor.ApplyEventsToFloors();
                            editor.RemakePath(true, true);
                            break;

                        default:
                            return;
                    }
                }
            }
            finally
            {
                _creating = false;
            }
        }

        private static int GetIndex(object data)
        {
            Tuple<int, TileRelativeTo> tuple = (Tuple<int, TileRelativeTo>)data;
            int length = ADOBase.lm.floorAngles.Length;
            return Mathf.Clamp(tuple.Item1 + (tuple.Item2 == TileRelativeTo.End ? length : 0), 0, length);
        }

        private static void reselectCurrentFloorAndShowPanel(LevelEventType eventType)
        {
            scnEditor editor = scnEditor.instance;
            List<scrFloor> selectedFloors = editor.selectedFloors;

            if (selectedFloors.Count > 0)
            {
                List<scrFloor> listFloors = scrLevelMaker.instance.listFloors;
                var datas = Main.GetEvent((LevelEventType)Main.MappingHelper.type).data;
                scrFloor selectFloor = listFloors[GetIndex(datas["affectTileRangeFrom"])];
                editor.levelEventsPanel.ShowTabsForFloor(selectFloor.seqID);
                editor.levelEventsPanel.selectedEventType = eventType;
                editor.levelEventsPanel.ShowPanel(eventType, 0);
                editor.SelectFloor(selectFloor, true);
                editor.ShowEventIndicators(selectFloor);
            }
        }

        public static TrackData[] getTrackData()
        {
            AngleData[] angles = getAnglesData();
            TrackData[] result = new TrackData[angles.Length];
            List<scrFloor> listFloors = scrLevelMaker.instance.listFloors;

            Vector2 trackPositionThis = Vector2.zero;
            Vector2 trackPositionLast = Vector2.zero;

            float trackRotationThis = 0f;
            float trackRotationLast = 0f;

            result[0] = new TrackData(angles[0].angle, angles[0].head, angles[0].tail, trackPositionThis, trackRotationThis, false, 0, false, 0, 0);

            Dictionary<int, List<LevelEvent>> pt_dict = new Dictionary<int, List<LevelEvent>>();
            Dictionary<int, LevelEvent> hold_dict = new Dictionary<int, LevelEvent>();
            Dictionary<int, LevelEvent> pause_dict = new Dictionary<int, LevelEvent>();

            foreach (var e in scnEditor.instance.events)
            {
                if (e.floor <= 0 || e.floor > angles.Length)
                    continue;

                switch (e.eventType)
                {
                    case LevelEventType.PositionTrack:
                        if (!pt_dict.TryGetValue(e.floor, out var list))
                        {
                            list = new List<LevelEvent>();
                            pt_dict[e.floor] = list;
                        }
                        list.Add(e);
                        break;

                    case LevelEventType.Hold:
                        hold_dict[e.floor] = e;
                        break;

                    case LevelEventType.Pause:
                        pause_dict[e.floor] = e;
                        break;
                }
            }

            for (int floor = 1; floor < angles.Length; floor++)
            {
                float radiusScale = listFloors[floor].radiusScale;
                bool isPause = false;
                float pauseDuration = 0;
                bool isHold = false;
                int holdDuration = 0;
                int holdDistance = 0;

                float x = Mathf.Cos(angles[floor - 1].head * Mathf.Deg2Rad) * radiusScale;
                float y = Mathf.Sin(angles[floor - 1].head * Mathf.Deg2Rad) * radiusScale;

                trackPositionThis = trackPositionLast + new Vector2(x, y);
                trackRotationThis = trackRotationLast;

                if (pt_dict.TryGetValue(floor, out List<LevelEvent> pt_list))
                {
                    foreach (LevelEvent pt in pt_list)
                    {
                        if (!pt.disabled["positionOffset"])
                        {
                            Vector2 position = (Vector2)pt.data["positionOffset"];

                            if ((bool)pt.data["justThisTile"])
                            {
                                trackPositionThis += position;
                            }
                            else
                            {
                                trackPositionThis += position;
                                trackPositionLast += position;
                            }
                        }

                        if (!pt.disabled["rotation"])
                        {
                            float rotation = (float)pt.data["rotation"];

                            if ((bool)pt.data["justThisTile"])
                            {
                                trackRotationThis = rotation;
                            }
                            else
                            {
                                trackRotationThis = rotation;
                                trackRotationLast = rotation;
                            }
                        }
                    }
                }

                if (hold_dict.TryGetValue(floor - 1 < 0 ? 0 : floor - 1, out LevelEvent hold))
                {
                    int hold_duration = (int)hold.data["duration"];
                    int hold_distanceMultiplier = (int)hold.data["distanceMultiplier"];
                    float rate = (hold_duration * 2 + 1) * hold_distanceMultiplier / 100f;

                    trackPositionThis.x += Mathf.Cos(angles[floor - 1].head * Mathf.Deg2Rad) * rate;
                    trackPositionThis.y += Mathf.Sin(angles[floor - 1].head * Mathf.Deg2Rad) * rate;

                    trackPositionLast.x += Mathf.Cos(angles[floor - 1].head * Mathf.Deg2Rad) * rate;
                    trackPositionLast.y += Mathf.Sin(angles[floor - 1].head * Mathf.Deg2Rad) * rate;
                }

                
                if(hold_dict.TryGetValue(floor, out LevelEvent holdData))
                {
                    isHold = true;
                    holdDuration = (int)holdData.data["duration"];
                    holdDistance = (int)holdData.data["distanceMultiplier"];
                }

                if (pause_dict.TryGetValue(floor, out LevelEvent pauseData))
                {
                    isPause = true;
                    pauseDuration = (float)pauseData.data["duration"];
                }

                result[floor] = new TrackData(angles[floor].angle, angles[floor].head, angles[floor].tail, trackPositionThis, trackRotationThis, isPause, pauseDuration, isHold, holdDuration, holdDistance);
                trackPositionLast += new Vector2(x, y);
            }

            return result;
        }

        private static AngleData[] getAnglesData()
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
                    result[i] = new AngleData(999f, tail, head);
                    tail = head;
                    continue;
                }
                else
                {
                    while(floorAngles[i] >360) floorAngles[i] -= 360;
                    while(floorAngles[i] <0) floorAngles[i] += 360;
                }
                head = floorAngles[i];
                float angle = (listFloors[i].isCCW ? head - tail : tail - head) == 0 ? 360 : listFloors[i].isCCW ? head - tail : tail - head;
                if (angle < 0)
                    angle += 360;
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
            public TrackData(float angle, float head, float tail, Vector2 position, float rotation, bool isPause, float pauseDuration, bool isHold, int holdDuration, int holdDistance)
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

        public static void ExtraVideo(MonoBehaviour runner, string videoPath, string outputFolder)
        {
            runner.StartCoroutine(new ExtraVideo().ExtractFramesCoroutine(videoPath, outputFolder));
        }
    }

    internal class ExtraVideo
    {
        public IEnumerator ExtractFramesCoroutine(string videoPath, string outputFolder)
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

                string path = Path.Combine(outputFolder, $"{Path.GetFileNameWithoutExtension(videoPath)} ({frameIndex + 1}).png");

                if (!File.Exists(path))
                {
                    RenderTexture.active = renderTexture;
                    Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
                    tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                    tex.Apply();
                    RenderTexture.active = null;

                    File.WriteAllBytes(path, tex.EncodeToPNG());

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
                throw new ArgumentException($"无效起始色：{startHex}");
            if (!ColorUtility.TryParseHtmlString(endHex, out var c2))
                throw new ArgumentException($"无效结束色：{endHex}");

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
