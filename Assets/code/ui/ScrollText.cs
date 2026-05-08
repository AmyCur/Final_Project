using UnityEngine;

public class ScrollText : MonoBehaviour
{
    // Update is called once per frame
    void Update(){
        if (Mathf.FloorToInt(transform.position.y) == -100)
        {
            transform.position=new Vector3(transform.position.x,transform.position.y+167,transform.position.z);
        }
        
        transform.position=new Vector3(transform.position.x,transform.position.y-1,transform.position.z);
    }
}
