using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class MainMenuButton : MonoBehaviour
{
    private GameObject[] menuButtons;    
    private SpriteLooping spriteLooping; 
    private Light2D backLight;
    //O motivo de impedir o click durante a transição está relacionado
    //A uma animação de transição especialmente para o Tutorial e New Game
    //Mas isso é coisa que faço depois.
    void Start()
    {
        backLight = GameObject.Find("BackLight").GetComponent<Light2D>();
        spriteLooping = FindObjectOfType<SpriteLooping>();
        menuButtons = GameObject.FindGameObjectsWithTag("MenuButton");
        System.Array.Sort(menuButtons, (button1, button2) => button2.transform.position.x.CompareTo(button1.transform.position.x));
        float delay = 0f;
        foreach (var button in menuButtons)
        {
            button.transform.DOMoveX(-5f, 0.5f) //Se tudo funciona a partir da ancora, resta então se colocar o coiso proximo ao zero, ele vai ficar travado na posição da ancora. 
                .SetDelay(delay)
                .SetLoops(1, LoopType.Restart)
                .SetEase(Ease.InOutSine);
            delay += 0.35f; 
        }
        DOTween.To(() => backLight.intensity, (x) => backLight.intensity = x, 2f, 2.5f);
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
