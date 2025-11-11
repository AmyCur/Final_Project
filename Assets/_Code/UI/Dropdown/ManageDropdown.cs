using UnityEngine;
using UnityEngine.UI;
using GameDebug;
using Magical;

public class ManageDropdown : MonoBehaviour
{
    public void EnableThoughts(bool status) => GameDebug.Combat.showThoughts = status;
    [SerializeField] GameObject dropdown;


    bool canDropdown = true;
    bool down = true;
    bool shouldDropdown => canDropdown && magic.key.down(keys.terminal);

    void Update()
    {
        if (shouldDropdown)
        {
            down = !down;
            dropdown.SetActive(down);
        }
    }
}
