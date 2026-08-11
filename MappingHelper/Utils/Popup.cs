using System;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;


namespace MappingHelper.Utils
{
    internal static class Popup
    {
        public static GameObject popup;

        public static void ShowMessage(string message)
        {
            if (popup == null)
                return;
            popup.transform.SetParent(null, false);
            popup.SetActive(true);
            scnEditor.instance.ShowPopup(true, (scnEditor.PopupType)233, false);
            popup.transform.SetParent(scnEditor.instance.popupWindow.transform, false);
            popup.transform.Find("popupText").GetComponent<TMP_Text>().text = message;
            Button button = popup.transform.Find("buttonOk").GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => scnEditor.instance.ShowPopup(false, (scnEditor.PopupType)233, false));
        }
    }

    internal static class Popup_Confirm
    {
        public static GameObject popup_confirm;

        public static void ShowMessage(string message,Action onConfirm = null, Action onCancel = null)
        {
            if (popup_confirm == null)
                return;
            popup_confirm.transform.SetParent(null, false);
            popup_confirm.SetActive(true);
            scnEditor.instance.ShowPopup(true, (scnEditor.PopupType)234, false);
            popup_confirm.transform.SetParent(scnEditor.instance.popupWindow.transform, false);
            popup_confirm.transform.Find("popupText").GetComponent<TMP_Text>().text = message;

            Button confirmButton = popup_confirm.transform.Find("buttonConfirm").GetComponent<Button>();
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() =>
            {
                onConfirm?.Invoke();
                scnEditor.instance.ShowPopup(false, (scnEditor.PopupType)234, false);
            });
            Button cancelButton = popup_confirm.transform.Find("buttonCancel").GetComponent<Button>();
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() =>
            {
                onCancel?.Invoke();
                scnEditor.instance.ShowPopup(false, (scnEditor.PopupType)234, false);
            });


            //GameObject test = UnityEngine.Object.Instantiate(scnEditor.instance.missingFilesPopupContainer, scnEditor.instance.popupWindow.transform);
            //GameObject test = popup_confirm;
            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine($"=== {test.name} 的直接子对象 ===");

            //foreach (Transform child in test.transform)
            //{
            //    Component[] components = child.GetComponents<Component>();
            //    string componentList = "";
            //    foreach (var comp in components)
            //    {
            //        componentList += comp.GetType().Name + " ";
            //    }
            //    sb.AppendLine($"- {child.name} (Components: {componentList})");
            //}

            //Main.Logger.Log(sb.ToString());
        }
    }

    internal static class Popup_Confirm_Scroll
    {
        public static GameObject popup_confirm_scroll;
        public static void ShowMessage(string message, string scroll_text = null, Action onConfirm = null, Action onCancel = null)
        {
            if (popup_confirm_scroll == null)
                return;

            popup_confirm_scroll.transform.SetParent(null, false);
            popup_confirm_scroll.SetActive(true);
            scnEditor.instance.ShowPopup(true, (scnEditor.PopupType)234, false);
            popup_confirm_scroll.transform.SetParent(scnEditor.instance.popupWindow.transform, false);
            popup_confirm_scroll.transform.Find("title").GetComponent<Text>().text = message;
            popup_confirm_scroll.transform.Find("files/viewport/files").GetComponent<TMP_Text>().text = scroll_text;

            Button confirmButton = popup_confirm_scroll.transform.Find("buttonConfirm").GetComponent<Button>();
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() =>
            {
                onConfirm?.Invoke();
                scnEditor.instance.ShowPopup(false, (scnEditor.PopupType)235, false);
            });
            Button cancelButton = popup_confirm_scroll.transform.Find("buttonCancel").GetComponent<Button>();
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() =>
            {
                onCancel?.Invoke();
                scnEditor.instance.ShowPopup(false, (scnEditor.PopupType)235, false);
            });
        }

    }
}
