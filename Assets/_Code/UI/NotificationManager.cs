using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;


namespace Cur.UI{
	public class Notification{
		public GameObject obj;
		public TMP_Text text;

		public IEnumerator DecayNotification(float? decayTime=null){
			decayTime??=NotificationManager.defaultDecayTime;
			yield return new WaitForSeconds((float)decayTime);
			GameObject.Destroy(obj);
			NotificationManager.notificationStack.Remove(this);
		}

		public Notification(GameObject obj){
			this.obj=obj;
			this.text = this.obj.transform.GetChild(0).GetComponent<TMP_Text>();
		}
		
		
	}

	public static class NotificationManager{
		public static GameObject notifObj => Resources.Load<GameObject>("Prefabs/UI/Notification");
		public static float? defaultDecayTime=2f;
		public static List<Notification> notificationStack;
		public static MonoBehaviour mb;

		public static Notification AddNotification(string content){
			Notification newNotif=new Notification(GameObject.Instantiate(notifObj));
			newNotif.text.text = content;
			RectTransform rt = newNotif.obj.GetComponent<RectTransform>();
			rt.anchoredPosition = new(-738,348-(250*notificationStack.Count-1));
			mb.StartCoroutine(newNotif.DecayNotification());
			notificationStack.Add(newNotif);
			return newNotif;
		}
		
	}
}
