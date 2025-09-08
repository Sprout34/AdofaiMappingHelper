using ADOFAI;

namespace MappingHelper
{
    public abstract class CustomTabBehaviour : ADOBase
    {
        public PropertiesPanel properties;

        public abstract void OnFocused();
        public abstract void OnUnFocused();
    }
}
