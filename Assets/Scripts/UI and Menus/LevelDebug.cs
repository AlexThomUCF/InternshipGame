using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class LevelDebug : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current != null && Keyboard.current.f10Key.wasPressedThisFrame)
        {
            SceneManager.LoadScene("Assets/Scenes/Levels/park_lvl.unity");
            Debug.Log("Pressed");
        }
        else if (Keyboard.current != null && Keyboard.current.f11Key.wasPressedThisFrame)
        {
            SceneManager.LoadScene("Assets/Scenes/Levels/arcade_lvl.unity");
        }
        else if (Keyboard.current != null && Keyboard.current.f12Key.wasPressedThisFrame)
        {
            SceneManager.LoadScene("Assets/Scenes/Levels/castle_lvl.unity");
        }
    }
}
