using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace MappingHelper.Utils
{
    internal static class Popup
    {
        public static GameObject popup;

        public static void ShowMessage(string message, bool skipAnim = false)
        {
            if (popup == null)
                return;
            popup.transform.SetParent(null, false);
            popup.SetActive(true);
            scnEditor.instance.ShowPopup(true, (scnEditor.PopupType)233, skipAnim);
            popup.transform.SetParent(scnEditor.instance.popupWindow.transform, false);
            popup.transform.Find("popupText").GetComponent<TMP_Text>().text = message;
            Button button = popup.transform.Find("buttonOk").GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => scnEditor.instance.ShowPopup(false, (scnEditor.PopupType)233, skipAnim));
        }
    }
}
