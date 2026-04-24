using UnityEngine;
using System.Collections.Generic;

namespace UI.Menus{

public enum MenuType : byte{
    overlay = 0x00000000,
    main = 0x00000001,
    
}

[System.Serializable]
public class Menu{
    public GameObject menu;
    public MenuType type;

    public void Open(){
       menu.SetActive(true);
    }   

    public void Close(){
       menu.SetActive(false);
    }
}


public static class ManageMenus{
    public static Menu currentOpenMenu;
}
}