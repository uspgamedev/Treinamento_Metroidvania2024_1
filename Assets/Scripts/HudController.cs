using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class HudController : MonoBehaviour
{
    private enum InOut
    {
        IN,
        OUT
    }

    private const float NEXT_BUTTON_POSITION = 400;
    private bool isTweening = false;
    private GameObject[] menuButtons;
    private Image blackFade;
    private GameObject[] pauseTiles = new GameObject[2];
    private GameObject pauseText;
    [HideInInspector] public bool isOnPauseMenu = false;

    //Usado ao inicializar o menu e toda vez que o abrimos ou fechamos ele para garantir que os elementos
    //Não permaneçam ativos enquanto não estão sendo usados.

    private void setButtonStatus(bool status){
        pauseText.SetActive(status);
        foreach (var button in menuButtons)
        {
            button.SetActive(status);
        }
    }
    void Awake(){ //PauseMenu
        blackFade = GameObject.Find("BlackFade").GetComponent<Image>();
        pauseText = GameObject.Find("Pause_(text)");
        menuButtons = GameObject.FindGameObjectsWithTag("MenuButton");
        pauseTiles[0] = GameObject.Find("LadoATiles");
        pauseTiles[1] = GameObject.Find("LadoBTiles");

        setButtonStatus(false);     
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape) && !isTweening){ //Só pra garantir que o maluco não vai ficar apertando esc igual um doido durante o tween.
            if (isOnPauseMenu){
                ContinueGame();
            }else{
                callPauseMenu();
            }
        }

        // if (isOnPauseMenu){
        // }
    }



    void callPauseMenu()
    {
        isOnPauseMenu = true;
        setButtonStatus(true);
        Time.timeScale = 0;

        
        System.Array.Sort(menuButtons, (button1, button2) => button2.transform.position.y.CompareTo(button1.transform.position.y));

        doBlackFadeTween(0.4f, 0.5f);
        tweenButtons(0.1f, InOut.IN);
        tweenTiles(0.5f, InOut.IN);
    }

    private void tweenTiles(float tweenTime, InOut direction){
        isTweening=true;
        int direction_multiplier = 1;
        if (direction == InOut.OUT)
        {
            direction_multiplier = -1;
        }

        foreach (var tile in pauseTiles)
        {
            if (tile != null && tile.activeInHierarchy){
                tile.transform.DOMoveY(tile.transform.position.y + 14 * direction_multiplier,tweenTime)
                .SetLoops(1, LoopType.Restart)
                .SetEase(Ease.InOutSine)
                .OnComplete(()=> isTweening=false)
                .SetUpdate(true);
            }
        }
    }

    private void doBlackFadeTween(float nextAlpha, float tweenTime)
    {
        // Fade the black fade image
        blackFade.DOFade(nextAlpha, tweenTime).SetEase(Ease.InOutSine).SetUpdate(true);
    }

    private void ReturnToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        if(!isTweening) return;
        doBlackFadeTween(1f, 0.35f);
        tweenButtons(0.1f, InOut.OUT);        
        tweenTiles(0.5f, InOut.OUT);
        StartCoroutine(resetObjects(0.4f));
    }

    private IEnumerator callToReturn(float timeBeforeReturn){
        yield return new WaitForSeconds(timeBeforeReturn);
        setButtonStatus(false);
        ReturnToMenu();
    }

    private void tweenButtons(float init_delay, InOut direction)
    {
        int direction_multiplier = 1;
        if (direction == InOut.OUT)
        {
            direction_multiplier = -direction_multiplier;
        }
        float delay = init_delay;
        pauseText.transform.DOMoveY(pauseText.transform.position.y + NEXT_BUTTON_POSITION * direction_multiplier, 0.5f)
            .SetDelay(init_delay/2)
            .SetUpdate(true)
            .SetEase(Ease.InOutSine);
        foreach (var button in menuButtons)
        {
            button.transform.DOMoveY(button.transform.position.y + NEXT_BUTTON_POSITION * direction_multiplier, 0.5f)
                .SetDelay(delay)
                .SetLoops(1, LoopType.Restart)
                .SetEase(Ease.InOutSine)
                .SetUpdate(true);
            delay += 0.1f;
        }
    }
    
    private IEnumerator resetObjects(float timeBeforeInactivation){
        yield return new WaitForSeconds(timeBeforeInactivation);
        isOnPauseMenu = false;
        setButtonStatus(false);
    }

    public void ContinueGame()
    {
        if(!isTweening) return;
        Time.timeScale = 1;
        doBlackFadeTween(0.0f, 0.35f);
        tweenButtons(0.1f, InOut.OUT);
        tweenTiles(0.55f, InOut.OUT);
        StartCoroutine(resetObjects(0.6f));   
    }
}
