using UnityEngine;
using Magical;
using System;


public class PauseController : MonoBehaviour
{

    bool paused;

    GameObject pauseMenu;

    // Update is called once per frame
    void Update()
    {
        if (magic.key.down(keys.pause))
        {
            paused = !paused;
            PlayerPrefs.SetInt(nameof(paused), Convert.ToInt32(paused));
            pauseMenu.SetActive(paused);
        }
    }
}
