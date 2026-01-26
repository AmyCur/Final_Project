using UnityEngine;
using System.Collections.Generic;

namespace Audio
{
	[CreateAssetMenu(fileName = "New Level Audio", menuName = "Audio/Create/Audio")]
	public class LevelAudio : ScriptableObject
	{
		public AudioClip baseClip; 
		public AudioClip enemyClip; 
		public AudioClip bossClip; 

		public List<AudioClip> clips => new(){baseClip, enemyClip, bossClip};		
	}
}
