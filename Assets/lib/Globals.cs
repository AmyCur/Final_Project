// This is where I store all of the awesome constants that are used over a load of scripts and other important things

using UnityEngine;
using Magical;

namespace Globals
{
    public static class glob {
        public const string playerTag = "Player";
        public const string swordsmanTag = "swordsman";
        public const string enemyTag = "Enemy";
        public const string holderTag = "Holder";
        public const string projTag = "Projectile";
        public const string imgTag = "damage_image";
        public const string deathScreenTag = "death_menu";
        public const string languageTextTag = "language_text";
        public const string WeaponIconTag = "WeaponIcon";
        public const string playerChildTag = "PlayerChild";
		public const string projectileTag = "Projectile";

        // This contains the max range for AI's when raycasting
        public const float maxAiCheckRange = 10000f;

        public static readonly LayerMask enemyMask = 64;

        // public static bool isEntity(string tag) {
            // return tag == enemyTag || tag == playerTag;
        // }
		//
	
		public static void Update(){
			if(magic.key.down(KeyCode.C)){
				Debug.ClearDeveloperConsole();
			}
		}
    }




}