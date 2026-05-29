using ADOFAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MappingHelper.Utils
{
    public static class LevelEventUtils
    {
        public static void UpdatePanel(this LevelEvent e)
        {
            scnEditor.instance.settingsPanel.panelsList.Find(panel => panel.levelEventType == e.eventType).SetProperties(e);
        }
    }
}
