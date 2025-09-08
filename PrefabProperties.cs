using ADOFAI;
using DG.Tweening;
using MappingHelper.PropertyCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MappingHelper
{
    public static class PrefabProperties
    {
        public static List<PropertyCollection.Property> Properties { get; } =
            new List<PropertyCollection.Property>
            {
                new PropertyCollection.Property_Tile(
                    name: "affectTileRangeFrom",
                    value_default: (0, TileRelativeTo.Start),
                    hideButtons: new List<int>{PropertyCollection.Property_Tile.THIS_TILE },
                    key: "mh.affectTileRangeFrom"
                ),
                new PropertyCollection.Property_Tile(
                    name: "affectTileRangeTo",
                    value_default: (0, TileRelativeTo.Start),
                    hideButtons: new List<int>{PropertyCollection.Property_Tile.THIS_TILE },
                    key: "mh.affectTileRangeTo"
                ),
                new PropertyCollection.Property_Enum<Features>(
                    name: "FeaturesOption",
                    value_default: Features.TrackDisappearAnimation,
                    key: "mh.features.option"
                ),
                new PropertyCollection.Property_Tile(
                    name: "startTile",
                    value_default: (-1, TileRelativeTo.ThisTile),
                    hideButtons: new List<int>{PropertyCollection.Property_Tile.START,PropertyCollection.Property_Tile.END },
                    key: "mh.startTile"
                ),
                new PropertyCollection.Property_Tile(
                    name: "endTile",
                    value_default: (-1, TileRelativeTo.ThisTile),
                    hideButtons: new List<int>{PropertyCollection.Property_Tile.START,PropertyCollection.Property_Tile.END },
                    key: "mh.endTile"
                ),
                new PropertyCollection.Property_InputField(
                    name: "duration",
                    type: PropertyCollection.Property_InputField.InputType.Float,
                    value_default: 1,
                    min: 0,
                    unit: "beats",
                    key: "editor.duration"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "xPosOffsetRange",
                    value_default: new UnityEngine.Vector2(0, 0),
                    canBeDisabled: true,
                    startEnabled: true,
                    key: "mh.xPosOffsetRange"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "yPosOffsetRange",
                    value_default: new UnityEngine.Vector2(0, 0),
                    canBeDisabled: true,
                    startEnabled: true,
                    key: "mh.yPosOffsetRange"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "rotationOffsetRange",
                    value_default: new UnityEngine.Vector2(0, 0),
                    canBeDisabled: true,
                    startEnabled: true,
                    key: "mh.rotationOffsetRange"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "parallaxRange",
                    value_default: new UnityEngine.Vector2(0, 0),
                    canBeDisabled: true,
                    startEnabled: false,
                    key: "mh.parallaxRange"
                ),
                new PropertyCollection.Property_InputField(
                    name: "parallaxRevertTo",
                    type: PropertyCollection.Property_InputField.InputType.Float,
                    value_default: 0,
                    canBeDisabled: true,
                    startEnabled: false,
                    key: "mh.parallaxRevertTo"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "scaleRange",
                    value_default: new UnityEngine.Vector2(100, 100),
                    canBeDisabled: true,
                    startEnabled: true,
                    key: "mh.scaleRange"
                ),
                new PropertyCollection.Property_InputField(
                    name: "opacity",
                    type: PropertyCollection.Property_InputField.InputType.Float,
                    value_default: 0,
                    unit: "%",
                    canBeDisabled: true,
                    startEnabled: true,
                    key: "editor.opacity"
                ),
                new PropertyCollection.Property_InputField(
                    name: "angleOffset",
                    type: PropertyCollection.Property_InputField.InputType.Float,
                    value_default: 0,
                    unit: "°",
                    key: "editor.angleOffset"
                ),
                new PropertyCollection.Property_Ease(
                    name: "ease",
                    key: "editor.ease"
                ),
                new PropertyCollection.Property_Button(
                    name: "createButton", 
                    action: FeaturesFunction.create, 
                    key: "mh.create"),
                new PropertyCollection.Property_InputField(
                    name: "scaleRevertTo",
                    type: PropertyCollection.Property_InputField.InputType.Float,
                    value_default: 100,
                    canBeDisabled: true,
                    startEnabled: true,
                    key: "mh.scaleRevertTo"
                ),
                new PropertyCollection.Property_Enum<TrackDistribution>(
                    name: "TrackDistrubution",
                    value_default: TrackDistribution.Distributed,
                    key: "mh.trackDistribution"
                ),
                new PropertyCollection.Property_InputField(
                    name: "trackRotation",
                    type: PropertyCollection.Property_InputField.InputType.Float,
                    value_default: 0,
                    unit: "°",
                    enableIf: new Dictionary<string, string>() { { "TrackDistrubution", "Centralized" } },
                    key: "mh.rotation"
                ),
                new PropertyCollection.Property_Bool(
                    name: "useIncreasingDepth",
                    key: "mh.useIncreasingDepth"
                ),
                new PropertyCollection.Property_InputField(
                    name: "initialDepth",
                    type: PropertyCollection.Property_InputField.InputType.Int,
                    value_default: 1,
                    enableIf: new Dictionary<string, string>() { { "useIncreasingDepth", "Enabled" } },
                    key: "mh.initialDepth"
                ),
                new PropertyCollection.Property_InputField(
                    name: "increasingValue",
                    type: PropertyCollection.Property_InputField.InputType.Int,
                    value_default: 1,
                    enableIf: new Dictionary<string, string>() { { "useIncreasingDepth", "Enabled" } },
                    key: "mh.increasingValue"
                ),
                new PropertyCollection.Property_Enum<TrackAnimation>(
                    name: "TrackAnimation",
                    value_default: TrackAnimation.DisappearAnimation,
                    key: "mh.TrackAnimation"
                ),
                new PropertyCollection.Property_InputField(
                    name: "tag",
                    type: PropertyCollection.Property_InputField.InputType.String,
                    value_default: "",
                    key: "mh.tag"
                ),
                new PropertyCollection.Property_InputField(
                    name: "eventTag",
                    type: PropertyCollection.Property_InputField.InputType.String,
                    value_default: "",
                    key: "mh.eventTag"
                ),
                new PropertyCollection.Property_Enum<TrackFeatures>(
                    name: "TrackFeatures",
                    value_default: TrackFeatures.CreateTrack,
                    key: "mh.TrackFeatures"
                ),
                new PropertyCollection.Property_Enum<FileType>(
                    name: "FileType",
                    value_default: FileType.Image,
                    key: "mh.FileType"
                ),
                new PropertyCollection.Property_File(
                    name: "selectDirectory",
                    fileType: "Directory",
                    key: "mh.selectDirectory"
                ),
                new PropertyCollection.Property_File(
                    name: "selectImage",
                    fileType: "Image",
                    key: "mh.selectImage"
                ),
                new PropertyCollection.Property_File(
                    name: "selectVideo",
                    fileType: "Video",
                    key: "mh.selectVideo"
                ),
                new PropertyCollection.Property_InputField(
                    name: "imageStart",
                    type: PropertyCollection.Property_InputField.InputType.Int,
                    value_default: 1,
                    min: 1,
                    enableIf: new Dictionary<string, string>() { { "selectFrame", "Enabled" } },
                    key: "mh.imageStart"
                ),
                new PropertyCollection.Property_InputField(
                    name: "imageEnd",
                    type: PropertyCollection.Property_InputField.InputType.Int,
                    value_default: 10,
                    min: 1,
                    enableIf: new Dictionary<string, string>() { { "selectFrame", "Enabled" } },
                    key: "mh.imageEnd"
                ),
                new PropertyCollection.Property_Bool(
                    name: "selectFrame",
                    key: "mh.selectFrame"
                ),
                new PropertyCollection.Property_InputField(
                    name: "positionStartValue",
                    value_default: new UnityEngine.Vector2(0f, 0f),
                    type: PropertyCollection.Property_InputField.InputType.Vector2,
                    key: "mh.positionStartValue"
                ),
                new PropertyCollection.Property_InputField(
                    name: "positionEndValue",
                    value_default: new UnityEngine.Vector2(0f, 0f),
                    type: PropertyCollection.Property_InputField.InputType.Vector2,
                    key: "mh.positionEndValue"
                ),
                new PropertyCollection.Property_InputField(
                    name: "pivotStartValue",
                    value_default: new UnityEngine.Vector2(0f, 0f),
                    type: PropertyCollection.Property_InputField.InputType.Vector2,
                    key: "mh.pivotStartValue"
                ),
                new PropertyCollection.Property_InputField(
                    name: "pivotEndValue",
                    value_default: new UnityEngine.Vector2(0f, 0f),
                    type: PropertyCollection.Property_InputField.InputType.Vector2,
                    key: "mh.pivotEndValue"
                ),
                new PropertyCollection.Property_InputField(
                    name: "rotationStartValue",
                    value_default: 0f,
                    type: PropertyCollection.Property_InputField.InputType.Float,
                    key: "mh.rotationStartValue"
                ),
                new PropertyCollection.Property_InputField(
                    name: "rotationEndValue",
                    value_default: 0f,
                    type: PropertyCollection.Property_InputField.InputType.Float,
                    key: "mh.rotationEndValue"
                ),
                new PropertyCollection.Property_InputField(
                    name: "scaleStartValue",
                    value_default: new UnityEngine.Vector2(100f, 100f),
                    type: PropertyCollection.Property_InputField.InputType.Vector2,
                    key: "mh.scaleStartValue"
                ),
                new PropertyCollection.Property_InputField(
                    name: "scaleEndValue",
                    value_default: new UnityEngine.Vector2(100f, 100f),
                    type: PropertyCollection.Property_InputField.InputType.Vector2,
                    key: "mh.scaleEndValue"
                ),
                new PropertyCollection.Property_Color(
                    name: "colorStartValue",
                    key: "mh.colorStartValue"
                ),
                new PropertyCollection.Property_Color(
                    name: "colorEndValue",
                    key: "mh.colorEndValue"
                ),
                new PropertyCollection.Property_InputField(
                    name: "opacityStartValue",
                    type: PropertyCollection.Property_InputField.InputType.Float,
                    value_default: 100,
                    unit: "%",
                    key: "mh.opacityStartValue"
                ),
                new PropertyCollection.Property_InputField(
                    name: "opacityEndValue",
                    type: PropertyCollection.Property_InputField.InputType.Float,
                    value_default: 100,
                    unit: "%",
                    key: "mh.opacityEndValue"
                ),
                new PropertyCollection.Property_InputField(
                    name: "depthStartValue",
                    type: PropertyCollection.Property_InputField.InputType.Int,
                    value_default: 1,
                    key: "mh.depthStartValue"
                ),
                new PropertyCollection.Property_InputField(
                    name: "depthEndValue",
                    type: PropertyCollection.Property_InputField.InputType.Int,
                    value_default: 1,
                    key: "mh.depthEndValue"
                ),
                new PropertyCollection.Property_InputField(
                    name: "parallaxStartValue",
                    value_default: new UnityEngine.Vector2(0f, 0f),
                    type: PropertyCollection.Property_InputField.InputType.Vector2,
                    key: "mh.parallaxStartValue"
                ),
                new PropertyCollection.Property_InputField(
                    name: "parallaxEndValue",
                    value_default: new UnityEngine.Vector2(0f, 0f),
                    type: PropertyCollection.Property_InputField.InputType.Vector2,
                    key: "mh.parallaxEndValue"
                ),
                new PropertyCollection.Property_InputField(
                    name: "decoCount",
                    type: PropertyCollection.Property_InputField.InputType.Int,
                    min: 1,
                    value_default: 10,
                    key: "mh.decoCount"
                ),
                new PropertyCollection.Property_Bool(
                    name: "usePlanet",
                    key: "mh.usePlanet"
                ),
            };




        public static List<string> TrackDisappearAnimationToActive { get;set; } = new List<string>()
            {
                "affectTileRangeFrom",
                "affectTileRangeTo",
                "FeaturesOption",
                "startTile",
                "endTile",
                "duration",
                "xPosOffsetRange",
                "yPosOffsetRange",
                "rotationOffsetRange",
                "scaleRange",
                "opacity",
                "angleOffset",
                "ease",
                "createButton",
            };

        public static List<string> TrackAppearAnimationToActive { get; set; } = new List<string>()
            {
                "affectTileRangeFrom",
                "affectTileRangeTo",
                "FeaturesOption",
                "startTile",
                "endTile",
                "duration",
                "xPosOffsetRange",
                "yPosOffsetRange",
                "rotationOffsetRange",
                "scaleRange",
                "scaleRevertTo",
                "opacity",
                "angleOffset",
                "ease",
                "createButton",
            };

        public static List<string> MultipleTrackToActive { get; set; } = new List<string>()
            {
                "affectTileRangeFrom",
                "affectTileRangeTo",
                "FeaturesOption",
                "tag",
                "TrackFeatures",
                "TrackDistrubution",
                "trackRotation",
                "useIncreasingDepth",
                "initialDepth",
                "increasingValue",
                "usePlanet",
                "createButton",
            };

        public static List<string> MultipleTrackDisappearToActive { get; set; } = new List<string>()
            {
                "affectTileRangeFrom",
                "affectTileRangeTo",
                "FeaturesOption",
                "tag",
                "TrackFeatures",
                "TrackAnimation",
                "startTile",
                "duration",
                "xPosOffsetRange",
                "yPosOffsetRange",
                "rotationOffsetRange",
                "scaleRange",
                "parallaxRange",
                "opacity",
                "angleOffset",
                "ease",
                "createButton",
            };

        public static List<string> MultipleTrackAppearToActive { get; set; } = new List<string>()
            {
                "affectTileRangeFrom",
                "affectTileRangeTo",
                "FeaturesOption",
                "tag",
                "TrackFeatures",
                "TrackAnimation",
                "startTile",
                "duration",
                "xPosOffsetRange",
                "yPosOffsetRange",
                "rotationOffsetRange",
                "scaleRange",
                "scaleRevertTo",
                "parallaxRange",
                "parallaxRevertTo",
                "opacity",
                "angleOffset",
                "ease",
                "createButton",
            };


        public static List<string> DynamicDecorationSelectImageToActive { get; set; } = new List<string>()
            {
                "affectTileRangeFrom",
                "affectTileRangeTo",
                "FeaturesOption",
                "FileType",
                "selectDirectory",
                "tag",
                "selectFrame",
                "imageStart",
                "imageEnd",
                "eventTag",
                "angleOffset",
                "createButton",
            };

        public static List<string> DynamicDecorationSelectVideoToActive { get; set; } = new List<string>()
            {
                "affectTileRangeFrom",
                "affectTileRangeTo",
                "FeaturesOption",
                "FileType",
                "selectVideo",
                "createButton",
            };

        public static List<string> Decoration3DToActive { get; set; } = new List<string>()
            {
                "affectTileRangeFrom",
                "affectTileRangeTo",
                "FeaturesOption",
                "selectImage",
                "tag",
                "decoCount",
                "positionStartValue",
                "positionEndValue",
                "pivotStartValue",
                "pivotEndValue",
                "rotationStartValue",
                "rotationEndValue",
                "scaleStartValue",
                "scaleEndValue",
                "colorStartValue",
                "colorEndValue",
                "opacityStartValue",
                "opacityEndValue",
                "depthStartValue",
                "depthEndValue",
                "parallaxStartValue",
                "parallaxEndValue",
                "createButton",
            };
    }
}
