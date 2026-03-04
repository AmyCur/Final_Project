using UnityEngine;

namespace UI.Menus{
    public class OpenMenu : MonoBehaviour
    {
        [SerializeField] Menu menu;

        public void HandleOpenMenu(){
            if(ManageMenus.currentOpenMenu!=null) ManageMenus.currentOpenMenu.Close();
            if (ManageMenus.currentOpenMenu != menu){
                ManageMenus.currentOpenMenu=menu;
                ManageMenus.currentOpenMenu.Open();
            }
        
        }
    }
}
