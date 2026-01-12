using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;


namespace Cur.UI{
	public class Notification{
		public GameObject obj;
		public TMP_Text text; 
		public Image img;

		public IEnumerator DecayNotification(float? decayTime=null){
			decayTime??=NotificationManager.defaultDecayTime;
			yield return new WaitForSeconds((float)decayTime);
			GameObject.Destroy(obj);
			NotificationManager.notificationStack.Remove(this);
		}

		public Notification(GameObject obj){
			this.obj=obj;
			this.text = this.obj.transform.GetChild(0).GetComponent<TMP_Text>();
			this.img = this.obj.GetComponent<Image>();
		}
		
		
	}

	public static class NotificationManager{
		public static GameObject notifObj => Resources.Load<GameObject>("Prefabs/UI/Notification");
		public static float? defaultDecayTime=2f;
		public static List<Notification> notificationStack= new();
		public static MonoBehaviour mb;

		public static void AddNotification(string content){
			// Make notification
			GameObject notif = GameObject.Instantiate(notifObj);
			// Set its parent
			notif.transform.parent=GameObject.Find("Notifications").transform;
			// Make new notification obhject
			Notification newNotif=new Notification(notif);
			// Set Text
			newNotif.text.text = content;
			// Add it to the stack
			notificationStack.Add(newNotif);
			// Unity hates this one simple trick
			newNotif.img.rectTransform.anchoredPosition = new(150,-(210*notificationStack.Count-1));
			// This code hecking sucks!!!!
			mb.StartCoroutine(newNotif.DecayNotification());
		}
		
	}
}
