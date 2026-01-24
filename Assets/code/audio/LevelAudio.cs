using UnityEngine;


namespace FileManagement.Audio
{
	[CreateAssetMenu(fileName = "New Level Audio", menuName = "Audio/Create/Audio")]
	public class LevelAudio : ScriptableObject
	{
		public AudioClip baseClip; 
		public AudioClip enemyClip; 
		public AudioClip bossClip; 
	}
}
