using ADOFAI;
using ADOFAI.Editor;
using ADOFAI.LevelEditor.Controls;
using HarmonyLib;
using MappingHelper.PropertyCollection;
using MappingHelper.Utils;
using SA.GoogleDoc;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Windows;

namespace MappingHelper
{
    internal static class Patches
    {
        [HarmonyPatch(typeof(RDString), "GetWithCheck")]
        public static class GetWithCheck
        {
            public static void Postfix(ref string __result, ref string key, ref bool exists, ref Dictionary<string, object> parameters)
            {
                if (key.Contains("enum.MappingHelper") && key.Contains("Version") && key.Contains("Culture") && key.Contains("PublicKeyToken"))
                {
                    int commaIndex = key.IndexOf(',');
                    if (commaIndex != -1)
                    {
                        string before = key.Substring(0, commaIndex);
                        string after = key.Substring(key.LastIndexOf('.') + 1);
                        key = before + "." + after;
                    }
                }

                if (Main.Localizations.Get(key, out string value, parameters))
                {
                    exists = true;
                    __result = value;
                }
            }
        }

        [HarmonyPatch]
        internal static class RDUtilsParseEnumPatch
        {
            internal static MethodBase TargetMethod()
            {
                return AccessTools.Method(typeof(RDUtils), "ParseEnum", null, null).MakeGenericMethod(new Type[]
                {
                typeof(LevelEventType)
                });
            }

            internal static bool Prefix(string str, ref LevelEventType defaultValue, ref LevelEventType __result)
            {
                if (str == "MappingHelperSettings")
                {
                    __result = (LevelEventType)801;
                    return false;
                }
                else if (str == "Features")
                {
                    __result = (LevelEventType)802;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(EditorConstants), "IsSetting")]
        private static class EditorConstantsIsSettingPatch
        {
            public static void Postfix(LevelEventType type, ref bool __result)
            {
                if ((int)type == 801)
                    __result = true;
            }
        }

        [HarmonyPatch(typeof(scnEditor), "GetSelectedFloorEvents")]
        internal static class scnEditorGetSelectedFloorEventsPatch
        {
            internal static bool Prefix(LevelEventType eventType, ref List<LevelEvent> __result)
            {
                if (scnEditor.instance.selectedFloors == null || scnEditor.instance.selectedFloors.Count == 0 || (int)eventType == 801 && __result == null)
                {
                    __result = new List<LevelEvent>();
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(InspectorTab), "SetSelected")]
        internal static class InspectorTabSetSelectedPatch
        {
            internal static void Postfix(InspectorTab __instance, bool selected)
            {
                int type = (int)__instance.levelEventType;
                if (type != 801)
                    return;
                if (selected)
                    Main.MappingHelper.onFocused();
                else
                    Main.MappingHelper.onUnFocused();
            }
        }

        [HarmonyPatch(typeof(InspectorPanel), "ShowPanel")]
        public static class InspectorPanelShowPanelPatch
        {
            internal static readonly Dictionary<LevelEventType, LevelEvent> saves = new Dictionary<LevelEventType, LevelEvent>();
            public static bool Prefix(InspectorPanel __instance, LevelEventType eventType, int eventIndex = 0)
            {
                if (eventType == (LevelEventType)Main.MappingHelper.type)
                {
                    FieldInfo showingPanelField = AccessTools.Field(typeof(InspectorPanel), "showingPanel");
                    showingPanelField.SetValue(__instance, true);
                    scnEditor.instance.SaveState(true, false);
                    scnEditor.instance.changingState++;
                    PropertiesPanel propertiesPanel = null;
                    foreach (PropertiesPanel propertiesPanel2 in __instance.panelsList)
                        if (propertiesPanel2.levelEventType == eventType)
                            (propertiesPanel = propertiesPanel2).gameObject.SetActive(true);
                        else
                            propertiesPanel2.gameObject.SetActive(false);

                    __instance.title.text = Main.Localizations.GetValue("mh.MappingHelper");
                    LevelEvent levelEvent = Main.MappingHelper.saveSetting && saves.TryGetValue((LevelEventType)Main.MappingHelper.type, out LevelEvent e) ? e : new LevelEvent(0, (LevelEventType)Main.MappingHelper.type, GCS.settingsInfo[Main.MappingHelper.name]);
                    if (Main.MappingHelper.saveSetting)
                        saves[(LevelEventType)Main.MappingHelper.type] = levelEvent;
                    if (propertiesPanel == null)
                        goto end;
                    if (levelEvent == null)
                        goto end;
                    Main.MappingHelperLevelEvent = levelEvent;
                    __instance.selectedEvent = levelEvent;
                    __instance.selectedEventType = levelEvent.eventType;
                    propertiesPanel.SetProperties(levelEvent, true);
                    levelEvent.UpdatePanel();
                    IEnumerator enumerator2 = __instance.tabs.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        RectTransform rect = (RectTransform)enumerator2.Current;
                        InspectorTab component = rect.gameObject.GetComponent<InspectorTab>();
                        component?.SetSelected(eventType == component.levelEventType);
                    }
                    end:
                    scnEditor.instance.changingState--;
                    showingPanelField.SetValue(__instance, false);

                    return false;
                }
                return true;
            }

            public static void Postfix(InspectorPanel __instance, LevelEventType eventType, int eventIndex = 0)
            {
                if (eventType == (LevelEventType)Main.MappingHelper.type)
                {
                    Main.activeChilden();
                }
            }   
        }

        [HarmonyPatch(typeof(scnEditor), "Awake")]
        internal static class scnEditorAwakePatch
        {
            internal static void Prefix()
            {
                if (GCS.levelEventsInfo == null)
                    return;
                if (GCS.levelEventIcons == null)
                {
                    GCS.levelEventIcons = new Dictionary<LevelEventType, Sprite>();
                    foreach (object obj in Enum.GetValues(typeof(LevelEventType)))
                    {
                        Sprite sprite = Resources.Load<Sprite>("LevelEditor/LevelEvents/" + obj.ToString());
                        if (sprite != null)
                        {
                            GCS.levelEventIcons.Add((LevelEventType)obj, sprite);
                        }
                    }
                }

                LevelEventInfo levelEventInfo = new LevelEventInfo
                {
                    categories = new List<LevelEventCategory>(),
                    executionTime = LevelEventExecutionTime.Special,
                    name = Main.MappingHelper.name,
                    propertiesInfo = new Dictionary<string, ADOFAI.PropertyInfo>(),
                    type = (LevelEventType)Main.MappingHelper.type
                };
                if (Main.MappingHelper.properties != null)
                    foreach (var dictionary in Main.MappingHelper.properties)
                    {
                        ADOFAI.PropertyInfo propertyInfo = new ADOFAI.PropertyInfo(dictionary, levelEventInfo);
                        if (dictionary.TryGetValue("type", out object obj) && obj is string str && str == "Export" && dictionary.TryGetValue("default", out obj) && obj is UnityAction action)
                            propertyInfo.value_default = action;
                        propertyInfo.order = 0;
                        levelEventInfo.propertiesInfo.Add(propertyInfo.name, propertyInfo);
                    }

                GCS.levelEventTypeString[(LevelEventType)Main.MappingHelper.type] = Main.MappingHelper.name;
                GCS.levelEventIcons[(LevelEventType)Main.MappingHelper.type] = Main.MappingHelper.icon;
                GCS.settingsInfo[Main.MappingHelper.name] = levelEventInfo;

                InspectorPanel settingsPanel = scnEditor.instance?.settingsPanel;
                if (settingsPanel == null)
                    return;
                GameObject gameObject = UnityEngine.Object.Instantiate(RDConstants.data.prefab_propertiesPanel);
                gameObject.name = Main.MappingHelper.name;
                PropertiesPanel component = gameObject.GetComponent<PropertiesPanel>();
                component.levelEventType = (LevelEventType)Main.MappingHelper.type;
                component.gameObject.SetActive(false);
                GameObject gameObject2 = UnityEngine.Object.Instantiate(RDConstants.data.prefab_tab);
                InspectorTab component2 = gameObject2.GetComponent<InspectorTab>();
                component.Init(settingsPanel, GCS.settingsInfo[Main.MappingHelper.name]);
                component2.Init((LevelEventType)Main.MappingHelper.type, settingsPanel);
                component2.SetSelected(false);

                if (Main.MappingHelper.index == -1)
                {
                    component2.GetComponent<RectTransform>().AnchorPosY(8f - 68f * settingsPanel.tabs.childCount);
                    gameObject2.transform.SetParent(settingsPanel.tabs, false);
                }
                else
                {
                    if (settingsPanel.tabs.childCount <= Main.MappingHelper.index)
                        Main.MappingHelper.index = settingsPanel.tabs.childCount - 1;
                    List<InspectorTab> tabs = new List<InspectorTab>();
                    for (int i = Main.MappingHelper.index; i < settingsPanel.tabs.childCount; i++)
                    {
                        InspectorTab tab2 = settingsPanel.tabs.GetChild(i).GetComponent<InspectorTab>();
                        if (tab2 == null || tab2.levelEventType == (LevelEventType)Main.MappingHelper.type)
                            continue;
                        tabs.Add(tab2);
                    }
                    tabs.ForEach(tab2 => tab2.transform.SetParent(null, false));
                    component2.GetComponent<RectTransform>().AnchorPosY(8f - 68f * Main.MappingHelper.index);
                    gameObject2.transform.SetParent(settingsPanel.tabs, false);
                    foreach (InspectorTab tab2 in tabs)
                    {
                        tab2.GetComponent<RectTransform>().AnchorPosY(8f - 68f * ++Main.MappingHelper.index);
                        tab2.transform.SetParent(settingsPanel.tabs, false);
                    }
                }
            }
        }



        [HarmonyPatch(typeof(scnEditor), "Start")]
        internal static class PopupPatch
        {
            public static void Postfix()
            {
                Popup.popup = UnityEngine.Object.Instantiate(scnEditor.instance.okPopupContainer, scnEditor.instance.popupWindow.transform);
                foreach (var comp in Popup.popup.GetComponentsInChildren<scrTextChanger>())
                {
                    UnityEngine.Object.Destroy(comp);
                }
            }
        }


        [HarmonyPatch(typeof(PropertiesPanel), "RenderControl")]
        class Patch_RenderControl
        {
            public static bool Prefix(PropertiesPanel __instance, string propertyKey, ADOFAI.PropertyInfo propertyInfo)
            {
                if(propertyInfo.type != PropertyType.Export) return true;

                LevelEventInfo levelEventInfo = propertyInfo.levelEventInfo;
                GameObject original = ADOBase.gc.prefab_controlExport;
                List<string> list = new List<string>();
                List<string> list2 = new List<string>();
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(ADOBase.gc.prefab_property);
                gameObject.transform.SetParent(__instance.content, false);
                ADOFAI.Property property = gameObject.GetComponent<ADOFAI.Property>();
                property.gameObject.name = propertyKey;
                property.key = propertyKey;
                property.info = propertyInfo;
                GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(original);
                gameObject2.GetComponent<RectTransform>().SetParent(property.controlContainer, false);
                property.control = gameObject2.GetComponent<PropertyControl>();
                property.control.propertyInfo = propertyInfo;
                property.control.propertiesPanel = __instance;
                property.control.propertyTransform = property.GetComponent<RectTransform>();
                property.control.Setup(true);
                string key2 = "editor." + property.key + ".help";
                bool flag3;
                string helpString = RDString.GetWithCheck(key2, out flag3, null, LangSection.Translations);
                if (flag3)
                {
                    Button helpButton = property.helpButton;
                    helpButton.transform.parent.gameObject.SetActive(true);
                    string buttonText = RDString.GetWithCheck("editor." + property.key + ".help.buttonText", out flag3, null, LangSection.Translations);
                    string buttonURL = RDString.GetWithCheck("editor." + property.key + ".help.buttonURL", out flag3, null, LangSection.Translations);
                    helpButton.onClick.AddListener(delegate ()
                    {
                        ADOBase.editor.ShowPropertyHelp(true, helpButton.transform, helpString, buttonText, buttonURL);
                    });
                }
                property.control.Setup(true);
                if (property.info.hasRandomValue && levelEventInfo != null)
                {
                    string randValueKey = property.info.randValueKey;
                    property.control.randomControl.propertyInfo = levelEventInfo.propertiesInfo[randValueKey];
                    property.control.randomControl.propertiesPanel = __instance;
                    property.control.randomControl.Setup(true);
                    Button randomButton = property.randomButton;
                    randomButton.gameObject.SetActive(true);
                    randomButton.onClick.AddListener(delegate ()
                    {
                        string randModeKey = property.info.randModeKey;
                        int num = ((int)__instance.inspectorPanel.selectedEvent[randModeKey] + 1) % 3;
                        __instance.inspectorPanel.selectedEvent[randModeKey] = (RandomMode)num;
                        property.control.SetRandomLayout();
                    });
                }
                property.enabledButton.onClick.AddListener(delegate ()
                {
                    bool isFake = __instance.inspectorPanel.selectedEvent.isFake;
                    using (new SaveStateScope(ADOBase.editor, false, true, false))
                    {
                        bool flag5;
                        bool flag4 = __instance.inspectorPanel.selectedEvent.disabled.TryGetValue(propertyKey, out flag5) && !flag5;
                        __instance.inspectorPanel.selectedEvent.disabled[propertyKey] = flag4;
                        property.offText.SetActive(flag4);
                        property.enabledCheckmark.SetActive(!flag4);
                        property.control.gameObject.SetActive(!flag4);
                        property.control.OnValueChange();
                        MethodInfo method = AccessTools.Method(typeof(PropertiesPanel), "UpdateEnabledButton");
                        method.Invoke(__instance, new object[] { property, flag4 });
                        if (isFake)
                        {
                            property.enabledCheckmark.transform.parent.gameObject.SetActive(false);
                            property.enabledButton.gameObject.SetActive(false);
                            __instance.inspectorPanel.selectedEvent.ApplyPropertiesToRealEvents();
                        }
                        property.control.ApplyTileChanges();
                    }
                });
                if (property.info.canBeDisabled)
                {
                    Button component4 = property.enabledButton.GetComponent<Button>();
                    ColorBlock colors = component4.colors;
                    colors.selectedColor = InspectorPanel.selectionColor;
                    component4.colors = colors;
                    PropertiesPanel.PropertySelectable item = new PropertiesPanel.PropertySelectable(component4, property.control, property, true);
                    __instance.propertySelectables.Add(item);
                }
                if (property.control.selectables != null)
                {
                    foreach (Selectable sel in property.control.selectables)
                    {
                        PropertiesPanel.PropertySelectable item2 = new PropertiesPanel.PropertySelectable(sel, property.control, property, false);
                        __instance.propertySelectables.Add(item2);
                    }
                }
                __instance.properties.Add(propertyInfo.name, property);

                return false;
            }
        }

        [HarmonyPatch]
        internal static class PropertyControl_ExportSetupPatch
        {
            internal static MethodBase TargetMethod()
            {
                return AccessTools.Method(typeof(ADOFAI.LevelEditor.Controls.PropertyControl), "Setup");
            }
            internal static void Postfix(object __instance)
            {
                ADOFAI.PropertyInfo info = __instance.Get<ADOFAI.PropertyInfo>("propertyInfo");
                if (!(info.value_default is UnityAction action))
                    return;

                Button button = __instance.Get<Button>("exportButton");
                
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(action);
                button.name = info.name;

                object text = __instance.Get("buttonText");
                if (info.customLocalizationKey == null)
                {
                    string str = "editor." + info.levelEventInfo.name + "." + info.name;
                    text.Set("text", RDString.GetWithCheck(str, out bool flag, null));
                    if (!flag)
                        text.Set("text", RDString.GetWithCheck("editor." + info.name, out _, null));
                }
                else
                    text.Set("text", (info.customLocalizationKey == "") ? "" : RDString.Get(info.customLocalizationKey, null));
            }
        }

        [HarmonyPatch(typeof(Button), "OnSubmit")]
        class Patch_ButtonOnSubmit
        {
            public static bool Prefix(Button __instance, BaseEventData eventData)
            {
                if (__instance.name.Contains("createButton"))
                    return false;
                return true;
            }
        }

        [HarmonyPatch(typeof(ADOFAI.Property), "info", MethodType.Setter)]
        internal static class Propertyset_infoPatch
        {
            internal static void Postfix(ADOFAI.Property __instance)
            {
                if (__instance.info.type == PropertyType.Export)
                    __instance.label.text = "";
            }
        }

        [HarmonyPatch]
        public static class SetupPatch
        {
            internal static MethodBase TargetMethod()
            {
                return AccessTools.Method(typeof(ADOFAI.LevelEditor.Controls.PropertyControl_Tile), "Setup");
            }
            public static void Postfix(object __instance)
            {
                if (__instance.Get<ADOFAI.PropertyInfo>("propertyInfo").dict.TryGetValue("hideButtons", out object listObj) && listObj is IEnumerable list)
                {
                    foreach (var value in list)
                        if (value is int i)
                        {
                            if ((i & Property_Tile.THIS_TILE) != 0)
                                __instance.Get<MonoBehaviour>("buttonThisTile")?.gameObject.SetActive(false);
                            if ((i & Property_Tile.START) != 0)
                                __instance.Get<MonoBehaviour>("buttonFirstTile")?.gameObject.SetActive(false);
                            if ((i & Property_Tile.END) != 0)
                                __instance.Get<MonoBehaviour>("buttonLastTile")?.gameObject.SetActive(false);
                        }
                }
            }
        }

        [HarmonyPatch(typeof(PropertiesPanel), "UpdateEnabledButton")]
        internal static class EnabledButtonPatch
        {
            internal static void Postfix(ADOFAI.Property property, bool disabled)
            {
                if (!disabled && property.helpButton.gameObject.activeInHierarchy)
                    property.enabledButton.GetComponent<RectTransform>().offsetMax = new Vector2(-30, 0);
            }
        }

        [HarmonyPatch]
        internal static class ValueChangePatch1
        {
            internal static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.Method(typeof(ADOFAI.LevelEditor.Controls.PropertyControl_Bool), "SetValue");
                yield return AccessTools.Method(typeof(ADOFAI.LevelEditor.Controls.PropertyControl_Color), "OnEndEdit") ?? AccessTools.Method(typeof(ADOFAI.LevelEditor.Controls.PropertyControl_Color), "OnChange");
                yield return AccessTools.Method(typeof(ADOFAI.LevelEditor.Controls.PropertyControl_File), "ProcessFile");
                yield return AccessTools.Method(typeof(ADOFAI.LevelEditor.Controls.PropertyControl_Rating), "SetInt");
                yield return AccessTools.Method(typeof(ADOFAI.LevelEditor.Controls.PropertyControl_Toggle), "SelectVar");
                yield return AccessTools.Method(typeof(ADOFAI.LevelEditor.Controls.PropertyControl_Vector2), "SetVectorVals");
                yield return AccessTools.Method(typeof(ADOFAI.LevelEditor.Controls.PropertyControl_FloatPair), "Save");
            }
            internal static void Prefix(object __instance, PropertiesPanel ___propertiesPanel, ADOFAI.PropertyInfo ___propertyInfo, ref object __state)
            {
                if (__instance.GetType().Equals(typeof(ADOFAI.LevelEditor.Controls.PropertyControl_Toggle)) && __instance.Get<bool>("settingText"))
                    return;
                ___propertiesPanel.inspectorPanel.selectedEvent.data.TryGetValue(___propertyInfo.name, out __state);
            }

            internal static void Postfix(object __instance, PropertiesPanel ___propertiesPanel, ADOFAI.PropertyInfo ___propertyInfo, object __state)
            {
                if (__instance.GetType().Equals(typeof(ADOFAI.LevelEditor.Controls.PropertyControl_Toggle)) && __instance.Get<bool>("settingText"))
                    return;
                LevelEvent e = ___propertiesPanel.inspectorPanel.selectedEvent;
                object newVar = e[___propertyInfo.name];
                if(Main.MappingHelper.type==(int)___propertyInfo.levelEventInfo.type && Main.MappingHelper.onChange != null && !Main.MappingHelper.onChange.Invoke(e, ___propertyInfo.name, __state, newVar))
                {
                    e[___propertyInfo.name] = __state;
                    e.UpdatePanel();
                }
            }
        }

        [HarmonyPatch]
        internal static class ValueChangePatch2
        {
            internal static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.Method(typeof(ADOFAI.LevelEditor.Controls.PropertyControl_LongText), "Setup");
                yield return AccessTools.Method(typeof(ADOFAI.LevelEditor.Controls.PropertyControl_Text), "Setup");
            }

            internal static void Postfix(object __instance, PropertiesPanel ___propertiesPanel, ADOFAI.PropertyInfo ___propertyInfo)
            {
                if (!(Main.MappingHelper.type == (int)___propertyInfo.levelEventInfo.type) || Main.MappingHelper.onChange == null)
                    return;
                TMP_InputField inputField = __instance.Get<TMP_InputField>("inputField");
                object obj = typeof(UnityEventBase).Get("m_Calls", inputField.onEndEdit);
                object runtime = obj.Get("m_RuntimeCalls");
                object var = null;
                string strVar = null;
                UnityAction<string> prefix = v => { var = ___propertiesPanel.inspectorPanel.selectedEvent[___propertyInfo.name]; strVar = v; };
                UnityAction<string> postfix = v =>
                {
                    LevelEvent e = ___propertiesPanel.inspectorPanel.selectedEvent;
                    object newVar = e[___propertyInfo.name];
                    if (!Main.MappingHelper.onChange.Invoke(e, ___propertyInfo.name, var, newVar))
                    {
                        e[___propertyInfo.name] = var;
                        e.UpdatePanel();
                    }
                };
                object prefixObj = inputField.onEndEdit.Method("GetDelegate", new object[] { prefix }, new Type[] { typeof(UnityAction<string>) });
                object postfixObj = inputField.onEndEdit.Method("GetDelegate", new object[] { postfix }, new Type[] { typeof(UnityAction<string>) });
                runtime.Method("Insert", new object[] { 0, prefixObj }, new Type[] { typeof(int), Reflections.GetType("UnityEngine.Events.BaseInvokableCall") });
                runtime.Method("Add", new object[] { postfixObj }, new Type[] { Reflections.GetType("UnityEngine.Events.BaseInvokableCall") });
                obj.Set("m_NeedsUpdate", true);
            }
        }

        // Property_List相关函数
        [HarmonyPatch]
        internal static class PropertyInfoConstructor
        {
            internal static MethodBase TargetMethod()
            {
                return AccessTools.Constructor(typeof(ADOFAI.PropertyInfo), new Type[] { typeof(Dictionary<string, object>), typeof(LevelEventInfo) });
            }

            internal static void Prefix(Dictionary<string, object> dict, out (bool, string) __state)
            {
                string text = dict["type"] as string;
                if (text.StartsWith("Enum:") && text.Length > 5 && typeof(Property_List.Dummy).Equals(Type.GetType(text.Substring(5))))
                {
                    __state = (true, dict["default"] as string);
                    dict.Remove("default");
                }
                else
                    __state = (false, null);
            }

            internal static void Postfix(ADOFAI.PropertyInfo __instance, (bool, string) __state)
            {
                if (__state.Item1)
                    __instance.value_default = __state.Item2;
            }
        }

        // Property_List相关函数
        [HarmonyPatch(typeof(TweakableDropdownItem), "localizedValue", MethodType.Getter)]
        internal static class TweakableDropdownItemgetlocalizedValuePatch
        {
            internal static bool Prefix(TweakableDropdownItem __instance, ref string __result)
            {
                if (__instance.localizeValue && __instance.dropdown.transform.parent?.GetComponent(typeof(ADOFAI.LevelEditor.Controls.PropertyControl_Toggle))?.Get<ADOFAI.PropertyInfo>("propertyInfo").enumType is Type t && t.Equals(typeof(Property_List.Dummy)))
                {
                    __result = RDStringEx.GetOrOrigin(__instance.value);
                    return false;
                }
                return true;
            }
        }

        // Property_List相关函数
        [HarmonyPatch]
        internal static class PropertyControl_ToggleEnumSetupPatch
        {
            internal static MethodBase TargetMethod()
            {
                return AccessTools.Method(typeof(ADOFAI.LevelEditor.Controls.PropertyControl_Toggle), "EnumSetup");
            }

            internal static void Prefix(object __instance, ref string enumTypeString, ref List<string> enumVals)
            {
                if (enumTypeString != null)
                {
                    Type type = typeof(ADOBase).Assembly.GetType(enumTypeString);
                    if (type != null || (type = Type.GetType(enumTypeString))?.Assembly == typeof(ADOBase).Assembly)
                        enumTypeString = type.FullName;
                }
                ADOFAI.PropertyInfo info = __instance.Get<ADOFAI.PropertyInfo>("propertyInfo");
                if (typeof(Property_List.Dummy).Equals(info.enumType))
                    enumVals = info.unit.Split(';').ToList();
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                List<CodeInstruction> codes = new List<CodeInstruction>();
                Label? targetLabel1 = null;
                for (int i = 0; i < instructions.Count(); i++)
                {
                    CodeInstruction code = instructions.ElementAt(i);
                    if (code.opcode == OpCodes.Ldarg_3)
                    {
                        CodeInstruction nextCode = instructions.ElementAt(i + 1);
                        if (nextCode.opcode == OpCodes.Brtrue_S)
                            targetLabel1 = (Label)nextCode.operand;
                    }
                    if (targetLabel1.HasValue && code.labels.Contains(targetLabel1.Value))
                    {
                        Label label1 = generator.DefineLabel();
                        Label label2 = (Label)instructions.ElementAt(i - 1).operand;
                        codes.Add(new CodeInstruction(OpCodes.Ldarg_0).WithLabels(code.ExtractLabels()));
                        codes.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ADOFAI.LevelEditor.Controls.PropertyControl), "propertyInfo")));
                        codes.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ADOFAI.PropertyInfo), "enumType")));
                        codes.Add(new CodeInstruction(OpCodes.Ldtoken, typeof(Property_List.Dummy)));
                        codes.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Type), "GetTypeFromHandle")));
                        codes.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Type), "Equals", new Type[] { typeof(Type) })));
                        codes.Add(new CodeInstruction(OpCodes.Brfalse, label1));
                        codes.Add(new CodeInstruction(instructions.ElementAt(i - 3)));
                        codes.Add(new CodeInstruction(instructions.ElementAt(i - 2)));
                        codes.Add(new CodeInstruction(OpCodes.Ldnull));
                        codes.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RDStringEx), "GetOrOrigin")));
                        codes.Add(new CodeInstruction(OpCodes.Br, label2));
                        codes.Add(code.WithLabels(label1));

                        continue;
                    }
                    codes.Add(code);
                }
                return codes;
            }
        }

        // Property_List相关函数
        //[HarmonyPatch(typeof(PropertyControl_Toggle), "SelectVar")]
        internal static class PropertyControl_ToggleSelectVarPatch
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                List<CodeInstruction> codes = new List<CodeInstruction>();
                for (int i = 0; i < instructions.Count(); i++)
                {
                    CodeInstruction code = instructions.ElementAt(i);
                    if (code.opcode == OpCodes.Call && (code.operand is MethodInfo method) && method.Name == "Parse")
                    {
                        CodeInstruction nextCode = instructions.ElementAt(i + 1);
                        if (nextCode.opcode != OpCodes.Unbox_Any)
                        {
                            codes.RemoveAt(codes.Count() - 2);
                            codes.RemoveAt(codes.Count() - 1);
                            Label label1 = generator.DefineLabel();
                            Label label2 = generator.DefineLabel();
                            codes.Add(new CodeInstruction(OpCodes.Ldarg_0));
                            codes.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ADOFAI.LevelEditor.Controls.PropertyControl), "propertyInfo")));
                            codes.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ADOFAI.PropertyInfo), "enumType")));
                            codes.Add(new CodeInstruction(OpCodes.Ldtoken, typeof(Property_List.Dummy)));
                            codes.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Type), "GetTypeFromHandle")));
                            codes.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Type), "Equals", new Type[] { typeof(Type) })));
                            codes.Add(new CodeInstruction(OpCodes.Brfalse, label1));
                            codes.Add(new CodeInstruction(OpCodes.Ldarg_1));
                            codes.Add(new CodeInstruction(OpCodes.Br, label2));
                            codes.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ADOFAI.LevelEditor.Controls.PropertyControl), "propertyInfo")).WithLabels(label1));
                            codes.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ADOFAI.PropertyInfo), "enumType")));
                            codes.Add(new CodeInstruction(OpCodes.Ldarg_1));
                            codes.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Enum), "Parse", new Type[] { typeof(Type), typeof(string) })));
                            codes.Add(nextCode.WithLabels(label2));
                            i++;
                            continue;
                        }
                    }
                    codes.Add(code);
                }
                return codes;
            }
        }

        [HarmonyPatch]
        class PropertyInfoCtorPatch
        {

            internal static MethodBase TargetMethod()
            {
                return AccessTools.Constructor(typeof(ADOFAI.PropertyInfo), new Type[] { typeof(Dictionary<string, object>), typeof(LevelEventInfo) });
            }

            internal static void Postfix(ADOFAI.PropertyInfo __instance, Dictionary<string, object> dict, LevelEventInfo levelEventInfo)
            {
                if (dict.TryGetValue("fileType", out object fileTypeObj))
                {
                    string fileTypeStr = fileTypeObj as string;
                    if (fileTypeStr != null && fileTypeStr.Equals("Directory"))
                    {
                        __instance.fileType = (ADOFAI.FileType)FileTypeExtension.Directory;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(ADOFAI.LevelEditor.Controls.PropertyControl_File), "BrowseFile")]
        internal static class BrowseFilePatch
        {
            public static bool Prefix(PropertyControl_File __instance)
            {
                bool levelSaved = (bool)AccessTools.Method(typeof(PropertyControl_File), "CheckIfLevelIsSaved").Invoke(__instance, null);
                if (!levelSaved)
                {
                    return true; 
                }

                ADOFAI.FileType fileType = __instance.propertyInfo.fileType;
                if (fileType == (ADOFAI.FileType)FileTypeExtension.Directory)
                {
                    scnGame instance = scnGame.instance;
                    if (string.IsNullOrEmpty(instance.levelPath))
                    {
                        return false;
                    }
                    string[] paths = StandaloneFileBrowser.OpenFolderPanel("", Persistence.GetLastUsedFolder(), false);
                    if (paths.Length == 0 || string.IsNullOrEmpty(paths[0]))
                    {
                        return false;
                    }

                    string path = Uri.UnescapeDataString(paths[0]);
                    string directoryName = Path.GetFileNameWithoutExtension(path);
                    string levelDirectory = Path.GetDirectoryName(instance.levelPath);
                    string targetDir = Path.Combine(levelDirectory, directoryName);
                    string sourceDir = path;
                    if (!Directory.Exists(targetDir))
                    {
                        Directory.CreateDirectory(targetDir);

                        if (!Directory.Exists(sourceDir))
                        {
                            Main.Logger.Log("Source Directory is Not Exist");
                            return false;
                        }

                        string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

                        foreach (string file in Directory.GetFiles(sourceDir))
                        {
                            string extension = Path.GetExtension(file)?.ToLower();
                            if (Array.Exists(allowedExtensions, ext => ext == extension))
                            {
                                string fileName = Path.GetFileName(file);
                                string destFile = Path.Combine(targetDir, fileName);
                                File.Copy(file, destFile, true);
                            }
                        }
                    }

                    FieldInfo filenameField = AccessTools.Field(typeof(PropertyControl_File), "filename");
                    LevelEvent selectedEvent2 = __instance.propertiesPanel.inspectorPanel.selectedEvent;
                    filenameField.SetValue(__instance, path);
                    selectedEvent2[__instance.propertyInfo.name] = path;
                    __instance.inputField.text = path;
                    var ToggleOthersEnabledMethod = AccessTools.Method(typeof(PropertyControl_File).BaseType, "ToggleOthersEnabled");
                    ToggleOthersEnabledMethod.Invoke(__instance, null);

                    var processMethod = AccessTools.Method(typeof(PropertyControl_File), "ProcessFile");
                    processMethod.Invoke(__instance, new object[] { "IM FREE", (ADOFAI.FileType)FileTypeExtension.Directory });

                    return false;
                }

                return true;

            }
        }

        public static class FakeFloor
        {
            public static List<scrFloor> fakeFloors = new List<scrFloor>();
            public static bool playing = false;
            public static bool enabled = false;
            public static bool nowPleaseMoveTheFakeFloor = false;
            public static int GetIndex(object data)
            {
                Tuple<int, TileRelativeTo> tuple = (Tuple<int, TileRelativeTo>)data;
                int length = ADOBase.lm.floorAngles.Length;
                return Mathf.Clamp(tuple.Item1 + (tuple.Item2 == TileRelativeTo.End ? length : 0), 0, length);
            }

            public static Vector3 getNextPosition(float head)
            {
                return new Vector3(Mathf.Cos(head * Mathf.Deg2Rad), Mathf.Sin(head * Mathf.Deg2Rad), 0) * 1.5f;
            }

            public static float AdofaiRadToDeg(double Rad)
            {
                return -(float)(Rad * Mathf.Rad2Deg - 90);
            }
            public static double AdofaiDegToRad(double Deg)
            {
                return (90.0 - Deg) * Mathf.Deg2Rad;
            }

            [HarmonyPatch(typeof(scrLevelMaker), "MakeLevel")]
            public static class CreateFakeFloor
            {
                public static void Postfix()
                {
                    foreach (var go in fakeFloors)
                    {
                        UnityEngine.Object.DestroyImmediate(go.gameObject);
                    }
                    fakeFloors.Clear();

                    if (ADOBase.lm.isOldLevel) return;

                    if (scnEditor.instance != null && !playing && enabled)
                    {
                        LevelEvent levelEvent = Main.GetEvent((LevelEventType)Main.MappingHelper.type);
                        if (levelEvent == null)
                            return;
                        
                        bool previewMagicShape = (bool)levelEvent.data["previewMagicShape"];
                        if (!previewMagicShape)
                            return;

                        scnEditor.instance.ApplyEventsToFloors();

                        Tuple<int, int> affectedRange = FeaturesFunction.getAffectedRange();
                        int affectTileRangeFrom = affectedRange.Item1;
                        int affectTileRangeTo = affectedRange.Item2;
                        if (affectTileRangeFrom == -1 || affectTileRangeTo == -1) return;

                        int vertexCount = (int)levelEvent.data["vertexCount"];
                        bool useReverseAngle = (bool)levelEvent.data["useReverseAngle"];

                        scrFloor lastAffectedFloor = scrLevelMaker.instance.listFloors[affectTileRangeTo];

                        int order = 100 + scrLevelMaker.instance.listFloors.Count + (affectTileRangeTo - affectTileRangeFrom + 1) * (vertexCount - 1);
                        for (int i = 0; i <= affectTileRangeTo; i++)
                        {
                            scrLevelMaker.instance.listFloors[i].SetSortingOrder(order * 5);
                            order--;
                        }

                        Vector3 thisPosition = lastAffectedFloor.transform.position;
                        scrFloor prev = lastAffectedFloor;
                        FeaturesFunction.AngleData[] angles = FeaturesFunction.getAnglesData();

                        for (int i = 1; i < vertexCount; i++)
                        {
                            for (int j = affectTileRangeFrom; j < affectTileRangeTo; j++)
                            {
                                float direction = angles[j].head + (360f / vertexCount * i * (useReverseAngle ? -1 : 1));
                                thisPosition += getNextPosition(direction);
                                prev.exitangle = AdofaiDegToRad(direction);
                                GameObject obj = UnityEngine.Object.Instantiate(ADOBase.lm.meshFloor, thisPosition, Quaternion.identity);
                                obj.name = $"FakeFloor_{i}.{j}";
                                scrFloor floor = obj.GetComponent<scrFloor>();
                                floor.entryangle = AdofaiDegToRad((direction + 180) % 360);
                                floor.floorRenderer.color = new Color(1, 1, 1, 0.5f);
                                floor.editorNumText.letterText.gameObject.SetActive(false);
                                floor.SetSortingOrder(order * 5);
                                order--;
                                prev.midSpin = angles[j].angle == 999;
                                prev.nextfloor = floor;
                                fakeFloors.Add(floor);
                                
                                prev.UpdateAngle();
                                prev = floor;
                            }
                        }

                        for (int i = affectTileRangeTo + 1; i < angles.Length; i++)
                        {
                            scrLevelMaker.instance.listFloors[i].SetSortingOrder(order * 5);
                            order--;
                        }

                        if (scrLevelMaker.instance.listFloors.Count > affectTileRangeTo + 1)
                        {
                            float direction = angles[affectTileRangeTo + 1].tail;

                            prev.exitangle = AdofaiDegToRad((direction + 180) % 360);
                            prev.UpdateAngle();
                        }
                        else if (scrLevelMaker.instance.listFloors.Count == affectTileRangeTo + 1 && scnEditor.instance.selectedFloors.Count > 1)
                        {
                            float head = angles[affectTileRangeTo].head + (360f / vertexCount) * (vertexCount - 1);

                            prev.exitangle = AdofaiDegToRad(head);
                            prev.entryangle = AdofaiDegToRad((head + 180) % 360);
                            prev.UpdateAngle();
                        }

                        nowPleaseMoveTheFakeFloor = true;
                    }
                }
            }

            [HarmonyPatch(typeof(scnGame), "ApplyEventsToFloors", typeof(List<scrFloor>), typeof(LevelData), typeof(scrLevelMaker), typeof(List<LevelEvent>))]
            public static class MoveTile
            {
                public static void Postfix()
                {
                    if (nowPleaseMoveTheFakeFloor)
                    {
                        nowPleaseMoveTheFakeFloor = false;
                        if (scnEditor.instance != null && !playing && enabled && !ADOBase.lm.isOldLevel && fakeFloors.Count > 0)
                        {
                            LevelEvent levelEvent = Main.GetEvent((LevelEventType)Main.MappingHelper.type);
                            if (levelEvent == null)
                                return;

                            Tuple<int, int> affectedRange = FeaturesFunction.getAffectedRange();
                            int affectTileRangeFrom = affectedRange.Item1;
                            int affectTileRangeTo = affectedRange.Item2;
                            if (affectTileRangeFrom == -1 || affectTileRangeTo == -1) return;

                            if (scrLevelMaker.instance.listFloors.Count > affectTileRangeTo + 1)
                            {
                                Vector3 vector = fakeFloors[fakeFloors.Count - 1].transform.position - scrLevelMaker.instance.listFloors[affectTileRangeTo].transform.position;
                                for (int i = affectTileRangeTo + 1; i < scrLevelMaker.instance.listFloors.Count; i++)
                                {
                                    scrLevelMaker.instance.listFloors[i].transform.position += vector;
                                }
                            }
                        }
                    }
                }
            }

            [HarmonyPatch(typeof(scnEditor), "Play")]
            public static class PlayPatch
            {
                public static void Prefix()
                {
                    playing = true;
                }
            }

            [HarmonyPatch(typeof(scnEditor), "SwitchToEditMode")]
            public static class SwitchToEditModePatch
            {
                public static void Prefix()
                {
                    playing = false;
                }
            }

            [HarmonyPatch(typeof(scnEditor), "Awake")]
            public static class AwakePatch
            {
                public static void Prefix()
                {
                    playing = false;
                    enabled = false;
                }
            }
        }



        [HarmonyPatch(typeof(PropertiesPanel), "SetProperties")]
        public static class ActiveChildPatch
        {
            public static void Postfix(LevelEvent levelEvent, bool checkIfEnabled = true)
            {
                if (levelEvent.eventType == (LevelEventType)Main.MappingHelper.type)
                {
                    Main.activeChilden();
                }
            }
        }

        [HarmonyPatch(typeof(ADOFAI.Editor.Components.MinMaxControl), "OnChange")]
        public static class ActiveChildPatch2
        {
            public static void Postfix()
            {
                Main.activeChilden();
            }
        }

        [HarmonyPatch(typeof(scnEditor), "UpdateSelectedFloor")]
        public static class UpdateFakeFloorOfSelectedTile
        {
            public static void Postfix()
            {
                if (FakeFloor.playing) return;

                LevelEvent levelEvent = Main.GetEvent((LevelEventType)Main.MappingHelper.type);
                if (levelEvent == null)
                    return;
                Features feature = (Features)levelEvent.data["FeaturesOption"];
                MagicShapeFeature magicShapeFeature = (MagicShapeFeature)levelEvent.data["magicShapeFeature"];
                bool previewMagicShape = (bool)levelEvent.data["previewMagicShape"];
                AffectAt affectAt = (AffectAt)levelEvent.data["affectAt"];

                if (Main.NowIsOnFocused && feature == Features.MagicShape && magicShapeFeature == MagicShapeFeature.CreateMagicShape && previewMagicShape && Main.showingFakeFloor && affectAt == AffectAt.SelectedTiles)
                {
                    if (Main.lastSelectedFloorsCount != scnEditor.instance.selectedFloors.Count)
                    {
                        if (scnEditor.instance.selectedFloors.Count > 1 || Main.lastSelectedFloorsCount > 1)
                        {
                            scnEditor.instance.RemakePath();
                        }
                        Main.lastSelectedFloorsCount = scnEditor.instance.selectedFloors.Count;
                    }
                }
            }
        }
    }
}
