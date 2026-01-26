using UnityEngine;
using MathsAndSome;

namespace Audio{
	
	public enum DangerLevel{
		no_enemy=0,
		enemy=1,
		boss=2
	}

	public static class ChangeAudioOnIntensity{

		static DangerLevel currentDangerLevel=DangerLevel.no_enemy;

		static bool EnemiesAlive(){
			return Entity.Entity.GetAllEnemies().Count > 0;
		}

		// TODO: Implement boss alive
		static bool BossAlive() => false;
		
		public static DangerLevel GetDangerLevel(){
			if (BossAlive()) return DangerLevel.boss;
			else if (EnemiesAlive()) return DangerLevel.enemy;
			else return DangerLevel.no_enemy;
		}

		public static void FadeSongBasedOnDangerLevel(){
			if(GetDangerLevel() != currentDangerLevel){
				currentDangerLevel = GetDangerLevel();
				Debug.Log($"Changing danger level! {currentDangerLevel}");
				LevelAudio levelAudio = MusicManager.GetCurrentLevelAudio();

				AudioClip targetClip = currentDangerLevel switch{
					DangerLevel.no_enemy => levelAudio.baseClip,
					DangerLevel.enemy => levelAudio.enemyClip,
					DangerLevel.boss => levelAudio.bossClip
				};

				
				mas.player.Player.StartCoroutine(MusicManager.CrossFadeAudio(targetClip));
			}
		}
	}
}