using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenuButton : MonoBehaviour
{
    private const float NEXT_BUTTON_POSITION=280;
    private GameObject[] menuButtons;    
    void Start()
    {
        menuButtons = GameObject.FindGameObjectsWithTag("MenuButton");
        System.Array.Sort(menuButtons, (button1, button2) => button2.transform.position.x.CompareTo(button1.transform.position.x));
        float delay = 0f;
        foreach (var button in menuButtons)
        {
            button.transform.DOMoveX(button.transform.position.x + NEXT_BUTTON_POSITION, 0.5f)
                .SetDelay(delay)
                .SetLoops(1, LoopType.Restart)
                .SetEase(Ease.InOutSine);
            delay += 0.35f; 
        }
    }
    public void StartGame(){
        SceneManager.LoadScene("TobiasWilsonDOIS");
    }
    public void QuitGame(){
        Application.Quit();
    }
}
