using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Magical
{
    public enum tags
    {
        dev
    }

    public static class keys
    {

        //* Movement
        public static KeyCode[] left = { KeyCode.LeftArrow, KeyCode.A };
        public static KeyCode[] right = { KeyCode.RightArrow, KeyCode.D };
        public static KeyCode[] up = { KeyCode.UpArrow, KeyCode.W };
        public static KeyCode[] down = { KeyCode.DownArrow, KeyCode.S };

        public static KeyCode[] jump = { KeyCode.Space };
        public static KeyCode[] slide = { KeyCode.LeftControl };
        public static KeyCode[] slam = { KeyCode.LeftControl };
        public static KeyCode[] dash = { KeyCode.LeftShift };

        public static KeyCode[] noclip = { KeyCode.V };

        //* Combat
        public static KeyCode[] attack = { KeyCode.Mouse0 };
        public static KeyCode[] hook = { KeyCode.R };

        public static KeyCode[] killAllKey = { KeyCode.LeftBracket };

        public static KeyCode[] goToSpawnKey = { KeyCode.F6 };

        public static KeyCode[] pause = { /*KeyCode.Escape,*/ KeyCode.Tab };
        // public static KeyCode[] respawnKey = {KeyCode.}


        public static Dictionary<KeyCode[], List<tags>> tagDict = new(){
          {killAllKey, new(){tags.dev}}
        };
    }

    public static class magic
    {
        public static class key
        {
            public static bool isDev(KeyCode[] key)
            {
                List<tags> tgs = keys.tagDict[key];
                if (tgs.Count != 0)
                {
                    foreach (tags t in tgs)
                    {
                        if (t == tags.dev)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            public static bool down(KeyCode[] key)
            {
                if (key.Length > 0)
                {
                    foreach (KeyCode k in key)
                    {
                        if (Input.GetKeyDown(k))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            public static bool up(KeyCode[] key)
            {
                if (key.Length > 0)
                {
                    foreach (KeyCode k in key)
                    {
                        if (Input.GetKeyUp(k))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            public static bool gk(KeyCode[] key)
            {
                if (key.Length > 0)
                {
                    foreach (KeyCode k in key)
                    {
                        if (Input.GetKey(k))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            public static KeyCode? PressedKey()
            {
                foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKey(kcode)) { return kcode; }
                }

                // Dont think anyones missing this key, and it should never be returned (Unless this is called while no key is pressed)
                return KeyCode.DoubleQuote;
            }
        }


    }
}
