using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour
{
    private const float NEXT_BUTTON_POSITION = 360;
    private GameObject[] menuButtons;    
    private SpriteLooping spriteLooping; 
    //O motivo de impedir o click durante a transição está relacionado
    //A uma animação de transição especialmente para o Tutorial e New Game
    //Mas isso é coisa que faço depois.
    void Start()
    {
        spriteLooping = FindObjectOfType<SpriteLooping>();
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
    public void StartGame()
    {
        if (spriteLooping != null)
        {
            SceneManager.LoadScene("Mapa");
        }
    }

    public void StartTutorial()
    {
        if (spriteLooping != null)
        {
            SceneManager.LoadScene("Mapa");
        }
    }

    public void QuitGame()
    {
        if (spriteLooping != null)
        {
            Application.Quit();
        }
    }
}
