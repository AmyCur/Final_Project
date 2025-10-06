using UnityEngine;

namespace GameDebug
{
    public static class Combat
    {
        public static bool drawLightningArcRadius = false;
        public static bool drawVortexRadius = true;
        public static bool drawSeekRay = false;
        public static bool drawHuntRay = false;
        public static bool showThoughts = true;
    }

    public static class Player
    {
        public static bool drawJumpCollider = false;
    }

    

    // Im not smart enough :(
    public static class JSON
    {
        // DO NOT USE FILE EXTENSIONS
        const string filePath = "JSON/Settings";

        static TextAsset LoadFile() => Resources.Load<TextAsset>(filePath);



        public static void ReadSettings()
        {
            TextAsset file = LoadFile();


            // DebugBool[] objs = JsonUtility.FromJson<DebugBool[]>(file.text);
            // Bools bools = JsonUtility.FromJson<Bools>(file.text);

            // foreach (B b in bools.booleans)
            // {
            //     Debug.Log(b._bool);
            // }



        }

        public static void WriteSettings()
        {

        }
    }
}

// class SettingsManager
// {
//     // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
//     // static void Start() => GameDebug.JSON.ReadSettings();
// }