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
                    disableIf: new Dictionary<string, string>() { { "affectAt", "SelectedTiles" } },
                    key: "mh.affectTileRangeFrom"
                ),
                new PropertyCollection.Property_Tile(
                    name: "affectTileRangeTo",
                    value_default: (0, TileRelativeTo.Start),
                    hideButtons: new List<int>{PropertyCollection.Property_Tile.THIS_TILE },
                    disableIf: new Dictionary<string, string>() { { "affectAt", "SelectedTiles" } },
                    key: "mh.affectTileRangeTo"
                ),
                new PropertyCollection.Property_Enum<Features>(
                    name: "FeaturesOption",
                    value_default: Features.TrackDisappearAnimation,
                    key: "mh.featuresOption"
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
                    isRange: false,
                    key: "mh.xPosOffsetRange"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "yPosOffsetRange",
                    value_default: new UnityEngine.Vector2(0, 0),
                    canBeDisabled: true,
                    startEnabled: true,
                    isRange: false,
                    key: "mh.yPosOffsetRange"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "xParallaxOffsetRange",
                    value_default: new UnityEngine.Vector2(0, 0),
                    canBeDisabled: true,
                    startEnabled: true,
                    isRange: false,
                    key: "mh.xParallaxOffsetRange"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "yParallaxOffsetRange",
                    value_default: new UnityEngine.Vector2(0, 0),
                    canBeDisabled: true,
                    startEnabled: true,
                    isRange: false,
                    key: "mh.yParallaxOffsetRange"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "rotationOffsetRange",
                    value_default: new UnityEngine.Vector2(0, 0),
                    canBeDisabled: true,
                    startEnabled: true,
                    isRange: false,
                    key: "mh.rotationOffsetRange"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "parallaxRange",
                    value_default: new UnityEngine.Vector2(0, 0),
                    canBeDisabled: true,
                    startEnabled: false,
                    isRange: false,
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
                    isRange: false,
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
                    key: "mh.create"
                ),
                new PropertyCollection.Property_InputField(
                    name: "scaleRevertTo",
                    type: PropertyCollection.Property_InputField.InputType.Float,
                    value_default: 100,
                    canBeDisabled: true,
                    startEnabled: true,
                    key: "mh.scaleRevertTo"
                ),
                new PropertyCollection.Property_Enum<TrackDistribution>(
                    name: "TrackDistribution",
                    value_default: TrackDistribution.Distributed,
                    key: "mh.trackDistribution"
                ),
                new PropertyCollection.Property_InputField(
                    name: "trackRotation",
                    type: PropertyCollection.Property_InputField.InputType.Float,
                    value_default: 0,
                    unit: "°",
                    enableIf: new Dictionary<string, string>() { { "TrackDistribution", "Centralized" } },
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
                    usesAlpha: true,
                    key: "mh.colorStartValue"
                ),
                new PropertyCollection.Property_Color(
                    name: "colorEndValue",
                    usesAlpha: true,
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
                new PropertyCollection.Property_InputField(
                    name: "vertexCount",
                    type: PropertyCollection.Property_InputField.InputType.Int,
                    min: 2,
                    value_default: 4,
                    key: "mh.vertexCount"
                ),
                new PropertyCollection.Property_Bool(
                    name: "useReverseAngle",
                    key: "mh.useReverseAngle"
                ),
                new PropertyCollection.Property_Enum<MagicShapeFeature>(
                    name: "magicShapeFeature",
                    value_default: MagicShapeFeature.CreateMagicShape,
                    key: "mh.magicShapeFeature"
                ),
                new PropertyCollection.Property_Enum<TwirlStyle>(
                    name: "twirlStyle",
                    value_default: TwirlStyle.None,
                    key: "mh.twirlStyle"
                ),
                new PropertyCollection.Property_InputField(
                    name: "bpmValue",
                    type: PropertyCollection.Property_InputField.InputType.Float,
                    min: 0.001,
                    max: 10000.0,
                    unit: "bpm",
                    value_default: 100,
                    enableIf: new Dictionary<string, string>() { { "speedTypeMH", "Bpm" } },
                    key: "mh.bpmValue"
                ),
                new PropertyCollection.Property_InputField(
                    name: "multiplierValue",
                    type: PropertyCollection.Property_InputField.InputType.Float,
                    min: 1.0E-7,
                    max: 128.0,
                    unit: "X",
                    value_default: 1,
                    enableIf: new Dictionary<string, string>() { { "speedTypeMH", "Multiplier" } },
                    key: "mh.multiplierValue"
                ),
                new PropertyCollection.Property_InputField(
                    name: "magicShapeRotateValue",
                    type: PropertyCollection.Property_InputField.InputType.Float,
                    value_default: 0,
                    key: "mh.magicShapeRotateValue"
                ),
                new PropertyCollection.Property_Enum<ShowedEvent>(
                    name: "showedEvent",
                    value_default: ShowedEvent.SetSpeed,
                    key: "mh.showedEvent"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "posTrackScale",
                    value_default: new UnityEngine.Vector2(0, 0),
                    canBeDisabled: true,
                    startEnabled: true,
                    isRange: false,
                    key: "mh.xPosOffsetRange"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "positionTrackScale",
                    value_default: new UnityEngine.Vector2(100, 100),
                    canBeDisabled: true,
                    startEnabled: true,
                    isRange: false,
                    min: 0,
                    key: "mh.positionTrackScale"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "scaleRadiusScale",
                    value_default: new UnityEngine.Vector2(100, 100),
                    canBeDisabled: true,
                    startEnabled: true,
                    isRange: false,
                    min: 0,
                    key: "mh.scaleRadiusScale"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "scalePlanetsScale",
                    value_default: new UnityEngine.Vector2(100, 100),
                    canBeDisabled: true,
                    startEnabled: true,
                    isRange: false,
                    min: 0,
                    key: "mh.scalePlanetsScale"
                ),
                new PropertyCollection.Property_Bool(
                    name: "previewMagicShape",
                    key: "mh.previewMagicShape"
                ),
                new PropertyCollection.Property_Enum<AffectAt>(
                    name: "affectAt",
                    value_default: AffectAt.SelectedTiles,
                    key: "mh.affectAt"
                ),
                new PropertyCollection.Property_InputField(
                    name: "initialAngleOffset",
                    type: PropertyCollection.Property_InputField.InputType.Float,
                    value_default: 0,
                    key: "mh.initialAngleOffset"
                ),
                new PropertyCollection.Property_Enum<ImageFormat>(
                    name: "imageFormat",
                    value_default: ImageFormat.PNG,
                    key: "mh.imageFormat"
                ),
                new PropertyCollection.Property_Enum<TrackDistribution>(
                    name: "planetAnimationDistribution",
                    value_default: TrackDistribution.Distributed,
                    enableIf: new Dictionary<string, string>() { { "usePlanet", "Enabled" } },
                    key: "mh.planetAnimationDistribution"
                ),
                new PropertyCollection.Property_Bool(
                    name: "changeParallax",
                    key: "mh.changeParallax"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "parallaxChange",
                    value_default: new UnityEngine.Vector2(0, 0),
                    min: -100,
                    max: 100,
                    isRange: false,
                    enableIf: new Dictionary<string, string>() { { "changeParallax", "Enabled" } },
                    key: "mh.parallaxChange"
                ),
                new PropertyCollection.Property_Bool(
                    name: "affectPlanet",
                    enableIf: new Dictionary<string, string>() { { "changeParallax", "Enabled" } },
                    key: "mh.affectPlanet"
                ),
                new PropertyCollection.Property_Enum<TrackDistribution>(
                    name: "eventDistribution",
                    value_default: TrackDistribution.Distributed,
                    key: "mh.eventDistribution"
                ),
                new PropertyCollection.Property_Enum<SpeedTypeMH>(
                    name: "speedTypeMH",
                    value_default: SpeedTypeMH.Bpm,
                    key: "mh.speedTypeMH"
                ),
                new PropertyCollection.Property_Bool(
                    name: "durationMatchBpm",
                    key: "mh.durationMatchBpm"
                ),
                new PropertyCollection.Property_InputField(
                    name: "lyric",
                    type: PropertyCollection.Property_InputField.InputType.String,
                    value_default: "",
                    key: "mh.lyric"
                ),
                new PropertyCollection.Property_Enum<Delimiter>(
                    name: "delimiter",
                    value_default: Delimiter.CharacterByCharacter,
                    key: "mh.delimiter"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "xPivotOffsetRange",
                    value_default: new UnityEngine.Vector2(0, 0),
                    canBeDisabled: true,
                    startEnabled: true,
                    isRange: false,
                    key: "mh.xPivotOffsetRange"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "yPivotOffsetRange",
                    value_default: new UnityEngine.Vector2(0, 0),
                    canBeDisabled: true,
                    startEnabled: true,
                    isRange: false,
                    key: "mh.yPivotOffsetRange"
                ),
                new PropertyCollection.Property_Bool(
                    name: "lyricDisappearAnimation",
                    key: "mh.lyricDisappearAnimation"
                ),
                new PropertyCollection.Property_InputField(
                    name: "lyricDisappearAfter",
                    type: PropertyCollection.Property_InputField.InputType.Float,
                    value_default: 1,
                    min: 0,
                    unit: "beats",
                    key: "editor.lyricDisappearAfter"
                ),
                new PropertyCollection.Property_InputField(
                    name: "lyricDisappearDuration",
                    type: PropertyCollection.Property_InputField.InputType.Float,
                    value_default: 1,
                    min: 0,
                    unit: "beats",
                    key: "editor.lyricDisappearDuration"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "lyricDisappearXPosOffsetRange",
                    value_default: new UnityEngine.Vector2(0, 0),
                    canBeDisabled: true,
                    startEnabled: true,
                    isRange: false,
                    key: "mh.lyricDisappearXPosOffsetRange"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "lyricDisappearYPosOffsetRange",
                    value_default: new UnityEngine.Vector2(0, 0),
                    canBeDisabled: true,
                    startEnabled: true,
                    isRange: false,
                    key: "mh.lyricDisappearYPosOffsetRange"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "lyricDisappearXPivotOffsetRange",
                    value_default: new UnityEngine.Vector2(0, 0),
                    canBeDisabled: true,
                    startEnabled: true,
                    isRange: false,
                    key: "mh.lyricDisappearXPivotOffsetRange"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "lyricDisappearYPivotOffsetRange",
                    value_default: new UnityEngine.Vector2(0, 0),
                    canBeDisabled: true,
                    startEnabled: true,
                    isRange: false,
                    key: "mh.lyricDisappearYPivotOffsetRange"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "lyricDisappearRotationOffsetRange",
                    value_default: new UnityEngine.Vector2(0, 0),
                    canBeDisabled: true,
                    startEnabled: true,
                    isRange: false,
                    key: "mh.lyricDisappearRotationOffsetRange"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "lyricDisappearScaleRange",
                    value_default: new UnityEngine.Vector2(100, 100),
                    canBeDisabled: true,
                    startEnabled: true,
                    isRange: false,
                    key: "mh.lyricDisappearScaleRange"
                ),
                new PropertyCollection.Property_InputField(
                    name: "lyricDisappearOpacity",
                    type: PropertyCollection.Property_InputField.InputType.Float,
                    value_default: 0,
                    unit: "%",
                    canBeDisabled: true,
                    startEnabled: true,
                    key: "editor.opacity"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "lyricDisappearParallaxRange",
                    value_default: new UnityEngine.Vector2(0f, 0f),
                    canBeDisabled: true,
                    startEnabled: false,
                    isRange: false,
                    key: "mh.lyricDisappearParallaxRange"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "lyricDisappearXParallaxOffsetRange",
                    value_default: new UnityEngine.Vector2(0, 0),
                    canBeDisabled: true,
                    startEnabled: true,
                    isRange: false,
                    key: "mh.lyricDisappearXParallaxOffsetRange"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "lyricDisappearYParallaxOffsetRange",
                    value_default: new UnityEngine.Vector2(0, 0),
                    canBeDisabled: true,
                    startEnabled: true,
                    isRange: false,
                    key: "mh.lyricDisappearYParallaxOffsetRange"
                ),
                new PropertyCollection.Property_Enum<LyricGenerationMode>(
                    name: "lyricGenerationMode",
                    value_default: LyricGenerationMode.GenerateAllAtOnce,
                    key: "mh.lyricGenerationMode"
                ),
                new PropertyCollection.Property_Ease(
                    name: "lyricDisappearEase",
                    key: "editor.ease"
                ),
                new PropertyCollection.Property_InputField(
                    name: "positionInterval",
                    value_default: new UnityEngine.Vector2(1f, 0f),
                    type: PropertyCollection.Property_InputField.InputType.Vector2,
                    key: "mh.positionInterval"
                ),
                new PropertyCollection.Property_InputField(
                    name: "timeInterval",
                    type: PropertyCollection.Property_InputField.InputType.Float,
                    value_default: 45,
                    min: 0,
                    unit: "°",
                    enableIf: new Dictionary<string, string>() { { "lyricGenerationMode", "GenerateAllAtOnce" } },
                    key: "mh.timeInterval"
                ),
                new PropertyCollection.Property_Enum<LyricGeneratedAs>(
                    name: "lyricGeneratedAs",
                    value_default: LyricGeneratedAs.BuiltInText ,
                    key: "mh.lyricGeneratedAs"
                ),
                new PropertyCollection.Property_Bool(
                    name: "useCustomFont",
                    value_default: false,
                    key: "mh.useCustomFont"
                ),
                new PropertyCollection.Property_File(
                    name: "selectFont",
                    fileType: "TTF",
                    enableIf: new Dictionary<string, string>() { { "useCustomFont", "Enabled" } },
                    key: "mh.selectFont"
                ),
                new PropertyCollection.Property_Enum<Font>(
                    name: "font",
                    value_default: Font.Arial,
                    enableIf: new Dictionary<string, string>() { { "useCustomFont", "Disable" } },
                    key: "mh.font"
                ),
                new PropertyCollection.Property_Bool(
                    name: "useStroke",
                    key: "mh.useStroke"
                ),
                new PropertyCollection.Property_InputField(
                    name: "strokeSize",
                    type: PropertyCollection.Property_InputField.InputType.Int,
                    min: 0,
                    max: 100,
                    slider: true,
                    value_default: 1,
                    key: "mh.strokeSize"
                ),
                new PropertyCollection.Property_Color(
                    name: "strokeColor",
                    usesAlpha: true,
                    value_default: UnityEngine.Color.black,
                    key: "mh.strokeColor"
                ),
                new PropertyCollection.Property_Bool(
                    name: "useShadow",
                    key: "mh.useShadow"
                ),
                new PropertyCollection.Property_FloatPair(
                    name: "shadowOffset",
                    value_default: new UnityEngine.Vector2(0, 0),
                    isRange: false,
                    key: "mh.shadowOffset"
                ),
                new PropertyCollection.Property_InputField(
                    name: "shadowSpread",
                    type: PropertyCollection.Property_InputField.InputType.Int,
                    min: 0,
                    max: 100,
                    slider: true,
                    value_default: 1,
                    key: "mh.shadowSpread"
                ),
                new PropertyCollection.Property_InputField(
                    name: "shadowDensity",
                    type: PropertyCollection.Property_InputField.InputType.Float,
                    min: 0f,
                    max: 10f,
                    slider: true,
                    value_default: 5f,
                    key: "mh.shadowDensity"
                ),
                new PropertyCollection.Property_Color(
                    name: "shadowColor",
                    usesAlpha: true,
                    value_default: UnityEngine.Color.black,
                    key: "mh.shadowColor"
                ),
                new PropertyCollection.Property_Bool(
                    name: "useBlur",
                    key: "mh.useBlur"
                ),
                new PropertyCollection.Property_InputField(
                    name: "blurSize",
                    type: PropertyCollection.Property_InputField.InputType.Int,
                    min: 0,
                    max: 100,
                    slider: true,
                    value_default: 1,
                    key: "mh.blurSize"
                ),
                new PropertyCollection.Property_Color(
                    name: "blurColor",
                    usesAlpha: true,
                    value_default: UnityEngine.Color.white,
                    key: "mh.blurColor"
                ),
                new PropertyCollection.Property_Color(
                    name: "color",
                    usesAlpha: true,
                    value_default: UnityEngine.Color.white,
                    key: "mh.color"
                ),
                new PropertyCollection.Property_InputField(
                    name: "trackAngleData",
                    type: PropertyCollection.Property_InputField.InputType.String,
                    value_default: "",
                    key: "mh.trackAngleData"
                ),
                new PropertyCollection.Property_Bool(
                    name: "previewTrack",
                    key: "mh.previewTrack"
                ),
                new PropertyCollection.Property_InputField(
                    name: "generationCount",
                    type: PropertyCollection.Property_InputField.InputType.Int,
                    min: 1,
                    value_default: 4,
                    key: "mh.generationCount"
                ),
            };


        public static List<string> TrackDisappearAnimationToActive { get;set; } = new List<string>()
            {
                "affectTileRangeFrom",
                "affectTileRangeTo",
                "affectAt",
                "FeaturesOption",
                "startTile",
                "endTile",
                "duration",
                "durationMatchBpm",
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
                "affectAt",
                "FeaturesOption",
                "startTile",
                "endTile",
                "duration",
                "durationMatchBpm",
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
                "affectAt",
                "FeaturesOption",
                "tag",
                "TrackFeatures",
                "TrackDistribution",
                "trackRotation",
                "useIncreasingDepth",
                "initialDepth",
                "increasingValue",
                "usePlanet",
                "planetAnimationDistribution",
                "changeParallax",
                "parallaxChange",
                "affectPlanet",
                "createButton",
            };

        public static List<string> MultipleTrackDisappearToActive { get; set; } = new List<string>()
            {
                "affectTileRangeFrom",
                "affectTileRangeTo",
                "affectAt",
                "FeaturesOption",
                "tag",
                "TrackFeatures",
                "TrackAnimation",
                "eventDistribution",
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
                "affectAt",
                "FeaturesOption",
                "tag",
                "TrackFeatures",
                "TrackAnimation",
                "eventDistribution",
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
                "affectAt",
                "FeaturesOption",
                "FileType",
                "selectDirectory",
                "tag",
                "selectFrame",
                "imageStart",
                "imageEnd",
                "eventTag",
                "initialAngleOffset",
                "angleOffset",
                "createButton",
            };

        public static List<string> DynamicDecorationSelectVideoToActive { get; set; } = new List<string>()
            {
                "affectTileRangeFrom",
                "affectTileRangeTo",
                "affectAt",
                "FeaturesOption",
                "FileType",
                "selectVideo",
                "imageFormat",
                "createButton",
            };

        public static List<string> Decoration3DToActive { get; set; } = new List<string>()
            {
                "affectTileRangeFrom",
                "affectTileRangeTo",
                "affectAt",
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

        public static List<string> MagicShapeCreateToActive { get; set; } = new List<string>()
            {
                "affectTileRangeFrom",
                "affectTileRangeTo",
                "affectAt",
                "FeaturesOption",
                "magicShapeFeature",
                "previewMagicShape",
                "vertexCount",
                "useReverseAngle",
                "createButton",
            };

        public static List<string> MagicShapeBpmToActive { get; set; } = new List<string>()
            {
                "affectTileRangeFrom",
                "affectTileRangeTo",
                "affectAt",
                "FeaturesOption",
                "magicShapeFeature",
                "speedTypeMH",
                "bpmValue",
                "multiplierValue",
                "twirlStyle",
                "showedEvent",
                "createButton",
            };

        public static List<string> MagicShapeRotateToActive { get; set; } = new List<string>()
            {
                "affectTileRangeFrom",
                "affectTileRangeTo",
                "affectAt",
                "FeaturesOption",
                "magicShapeFeature",
                "magicShapeRotateValue",
                "createButton",
            };

        public static List<string> TrackSizeChangeToActive { get; set; } = new List<string>()
            {
                "affectTileRangeFrom",
                "affectTileRangeTo",
                "affectAt",
                "FeaturesOption",
                "positionTrackScale",
                "scaleRadiusScale",
                "scalePlanetsScale",
                "ease",
                "createButton",
            };

        public static List<string> TrackExplosionAnimationToActive { get; set; } = new List<string>()
            {
                "affectTileRangeFrom",
                "affectTileRangeTo",
                "affectAt",
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

        public static List<string> LyricToActive { get; set; } = new List<string>()
            {
                "affectTileRangeFrom",
                "affectTileRangeTo",
                "affectAt",
                "FeaturesOption",
                "lyric",
                "lyricGeneratedAs",
                "positionInterval",
                "delimiter",
                //"lyricGenerationMode",
                "timeInterval",
                "duration",
                "tag",
                "xPosOffsetRange",
                "yPosOffsetRange",
                "xPivotOffsetRange",
                "yPivotOffsetRange",
                "rotationOffsetRange",
                "scaleRange",
                "scaleRevertTo",
                "color",
                "opacity",
                "parallaxRange",
                "parallaxRevertTo",
                "xParallaxOffsetRange",
                "yParallaxOffsetRange",
                "angleOffset",
                "ease",
                "lyricDisappearAnimation",
                "createButton",
            };

        public static List<string> LyricToActiveWithDisapperAnimation { get; set; } = new List<string>()
            {
                "affectTileRangeFrom",
                "affectTileRangeTo",
                "affectAt",
                "FeaturesOption",
                "lyric",
                "lyricGeneratedAs",
                "positionInterval",
                "delimiter",
                //"lyricGenerationMode",
                "timeInterval",
                "duration",
                "tag",
                "xPosOffsetRange",
                "yPosOffsetRange",
                "xPivotOffsetRange",
                "yPivotOffsetRange",
                "rotationOffsetRange",
                "scaleRange",
                "scaleRevertTo",
                "color",
                "opacity",
                "parallaxRange",
                "parallaxRevertTo",
                "xParallaxOffsetRange",
                "yParallaxOffsetRange",
                "angleOffset",
                "ease",
                "lyricDisappearAnimation",
                "lyricDisappearAfter",
                "lyricDisappearDuration",
                "lyricDisappearXPosOffsetRange",
                "lyricDisappearYPosOffsetRange",
                "lyricDisappearXPivotOffsetRange",
                "lyricDisappearYPivotOffsetRange",
                "lyricDisappearRotationOffsetRange",
                "lyricDisappearScaleRange",
                "lyricDisappearOpacity",
                "lyricDisappearParallaxRange",
                "lyricDisappearXParallaxOffsetRange",
                "lyricDisappearYParallaxOffsetRange",
                "lyricDisappearEase",
                "createButton",
            };



        public static List<string> LyricToActive_Decoration { get; set; } = new List<string>()
            {
                "affectTileRangeFrom",
                "affectTileRangeTo",
                "affectAt",
                "FeaturesOption",
                "lyric",
                "lyricGeneratedAs",
                "useCustomFont",
                "selectFont",
                "font",
                "useStroke",
                "strokeSize",
                "strokeColor",
                "useShadow",
                "shadowOffset",
                "shadowSpread",
                "shadowDensity",
                "shadowColor",
                "positionInterval",
                "delimiter",
                "timeInterval",
                "duration",
                "tag",
                "xPosOffsetRange",
                "yPosOffsetRange",
                "xPivotOffsetRange",
                "yPivotOffsetRange",
                "rotationOffsetRange",
                "scaleRange",
                "scaleRevertTo",
                "color",
                "opacity",
                "parallaxRange",
                "parallaxRevertTo",
                "xParallaxOffsetRange",
                "yParallaxOffsetRange",
                "angleOffset",
                "ease",
                "lyricDisappearAnimation",
                "createButton",
            };

        public static List<string> LyricToActiveWithDisapperAnimation_Decoration { get; set; } = new List<string>()
            {
                "affectTileRangeFrom",
                "affectTileRangeTo",
                "affectAt",
                "FeaturesOption",
                "lyric",
                "lyricGeneratedAs",
                "useCustomFont",
                "selectFont",
                "font",
                "useStroke",
                "strokeSize",
                "strokeColor",
                "useShadow",
                "shadowOffset",
                "shadowSpread",
                "shadowDensity",
                "shadowColor",
                "positionInterval",
                "delimiter",
                "timeInterval",
                "duration",
                "tag",
                "xPosOffsetRange",
                "yPosOffsetRange",
                "xPivotOffsetRange",
                "yPivotOffsetRange",
                "rotationOffsetRange",
                "scaleRange",
                "scaleRevertTo",
                "color",
                "opacity",
                "parallaxRange",
                "parallaxRevertTo",
                "xParallaxOffsetRange",
                "yParallaxOffsetRange",
                "angleOffset",
                "ease",
                "lyricDisappearAnimation",
                "lyricDisappearAfter",
                "lyricDisappearDuration",
                "lyricDisappearXPosOffsetRange",
                "lyricDisappearYPosOffsetRange",
                "lyricDisappearXPivotOffsetRange",
                "lyricDisappearYPivotOffsetRange",
                "lyricDisappearRotationOffsetRange",
                "lyricDisappearScaleRange",
                "lyricDisappearOpacity",
                "lyricDisappearParallaxRange",
                "lyricDisappearXParallaxOffsetRange",
                "lyricDisappearYParallaxOffsetRange",
                "lyricDisappearEase",
                "createButton",
            };

        public static List<string> GenerateTrackToActive { get; set; } = new List<string>()
            {
                "affectTileRangeFrom",
                "affectTileRangeTo",
                "affectAt",
                "FeaturesOption",
                "trackAngleData",
                "previewTrack",
                "generationCount",
                "createButton",
            };
    }
}
