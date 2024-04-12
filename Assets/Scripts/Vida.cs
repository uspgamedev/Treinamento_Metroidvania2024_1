using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    private int currentHealth;
    [SerializeField] private int maxHealth;
    [SerializeField] private GameObject healthUI;
    private Image[] hpSprites;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    private void Start()
    {
        currentHealth = maxHealth;

        hpSprites = new Image[maxHealth];
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject t = Instantiate(healthUI, new Vector3(0, 0, 0), Quaternion.identity ,GameObject.Find("Canvas").transform);
            hpSprites[i] = t.GetComponent<Image>();
            t.GetComponent<RectTransform>().anchoredPosition = new Vector2(65 * i, 0);
        }
    }
    

    void OnCollisionEnter2D(Collision2D collision){
            if (collision.gameObject.tag == "Inimigo"){
                hpSprites[currentHealth-1].GetComponent<Animator>().SetTrigger("DamageTaken");
                currentHealth--;
            }
    }

    void Update()
    {
        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
        

        for (int i = 0; i < hpSprites.Length; i++)
        {
            if (i < currentHealth) {
                hpSprites[i].sprite = fullHeart;
            }
            else {
                hpSprites[i].sprite = emptyHeart;
            }

            if (i < maxHealth) {
                hpSprites[i].enabled = true;
            }
            else {
                hpSprites[i].enabled = false;
            }
        }
        
    }

}
