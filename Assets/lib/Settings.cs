using UnityEngine;
using Globals;
using TMPro;

namespace Cur.Settings
{
    [System.Serializable]
    public enum languages
    {
        english,
        spanish,
        french,
        german
    }

    public static class Settings
    {
            public static class Development
        {
            public static bool devMode = true;
            public static bool displayThoughts = true;
        }
    }
}
