using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [HideInInspector] public int currentHealth;
    [HideInInspector] public int maxHealth;
    [SerializeField] private GameObject healthUI;
    [SerializeField] private float healthSize = 1f;
    private Image[] hpSprites;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    private Image blackFade;
    float min = 0f;
    float max = 1f;
    float t = 0f;
    float alpha;
    private bool toFade = false;
    private float fadeDur = 1f;

    private Player_Movement moveScript;
    private SupportScript support;
    private Vector3 currentLastPos;
    private Vector3 currentLastDir;
    private Rigidbody2D rb;

    [SerializeField] private float immortalTime;
    [SerializeField] private float knockbackForce;
    private float dir;
    [HideInInspector] public bool onKnockback = false;
    private bool damageable = true;
    private SimpleFlash flashScript;
    private Animator anim;
    private float maxBlinkTime = 0.5f;
    private float blinkTime;
    private GameObject pauseMenu;
    private GameObject gameOverMenu;

    private bool onHazard = false;
    
    private void Start()
    {
        support = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>();
        maxHealth = support.maxHealth;
        currentHealth = maxHealth;

        hpSprites = new Image[maxHealth];
        for (int i = 0; i < maxHealth; i++)
        {
            InstantiateHealth(i);
        }

        blinkTime = maxBlinkTime;
        pauseMenu = GameObject.Find("PauseMenu");
        gameOverMenu = GameObject.Find("GameOverMenu");

        blackFade = GameObject.FindGameObjectWithTag("BlackFade").GetComponent<Image>();
        moveScript = GetComponent<Player_Movement>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        flashScript = GetComponent<SimpleFlash>();

        moveScript.canMove2 = true;

    }
    

    private void OnCollisionEnter2D(Collision2D collision) {
            if (LayerMask.LayerToName(collision.gameObject.layer) == "Enemies" && damageable && !GetComponent<PlayerCombat>().isParrying && !onHazard) {

                TomarDano(collision.gameObject);
            }
            if (LayerMask.LayerToName(collision.gameObject.layer) == "Disparo" && damageable && !onHazard) {
                if ((collision.transform.position.x > transform.position.x && transform.localScale.x < 0) || (collision.transform.position.x < transform.position.x && transform.localScale.x > 0) || !GetComponent<PlayerCombat>().isParrying) {
                    TomarDano(collision.gameObject);
                }
            }
            if (LayerMask.LayerToName(collision.gameObject.layer) == "Fogo" && damageable && !onHazard) {
                TomarDano(collision.gameObject);
                Destroy(collision.gameObject);
            }
        
    }

    private void OnTriggerStay2D(Collider2D other){
        if (other.gameObject.tag=="Raio" && damageable){
                damageable=false;
                TomarDano(other.gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.tag == "Hazard" && !onHazard){
                hpSprites[currentHealth-1].GetComponent<Animator>().SetTrigger("DamageTaken");
                currentHealth--;

                StartCoroutine(HazardDamage());
            }
    }

    public void TomarDano(GameObject enemy) {
        Physics2D.IgnoreLayerCollision(gameObject.layer, enemy.layer, true);
        hpSprites[currentHealth-1].GetComponent<Animator>().SetTrigger("DamageTaken");
        currentHealth--;

        StartCoroutine(DamageKnockback(enemy));
    }

    private IEnumerator HazardDamage()
    {
        onHazard = true;

        currentLastPos = moveScript.lastPos;
        currentLastDir = new Vector3(moveScript.lastDir, 1f, 1f);

        moveScript.canMove2 = false;
        toFade = true;

        yield return new WaitForSeconds(fadeDur);

        rb.velocity = Vector2.zero;
        transform.position = currentLastPos;
        transform.localScale = currentLastDir;

        yield return new WaitForSeconds(0.5f * fadeDur);

        moveScript.canMove2 = true;

        yield return new WaitForSeconds(0.5f * fadeDur);
        
        toFade = false;
        onHazard = false;
    }

    void Update()
    {
        // Debug.Log(currentHealth);

        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }

        // Debug.Log(hpSprites.Length);
        

        for (int i = 0; i < hpSprites.Length; i++)
        {
            if (i < currentHealth) {
                hpSprites[i].sprite = fullHeart;
            }
            else {
                hpSprites[i].sprite = emptyHeart;
            }

            // if (i < maxHealth) {
            //     hpSprites[i].enabled = true;
            // }
            // else {
            //     hpSprites[i].enabled = false;
            // }
        }

        if (toFade && currentHealth >= 1) {
            Fade(fadeDur, blackFade, 0f, alpha);
        }
        else {
            if ((blackFade.color.a > 0f || t > 0f) && !pauseMenu.GetComponent<HudController>().isOnPauseMenu && !gameOverMenu.GetComponent<GameOverMenu>().isGameOver) {
                blackFade.color = new Color (0f, 0f, 0f, 0f);
                alpha = 0f;
                t = 0f;
                min = 0f;
                max = 1f;
            }
        }

        if (onKnockback) {
            rb.velocity = new Vector2(dir * knockbackForce, rb.velocity.y);
        }
    }

    public void Fade(float fadeDuration, Image image, float rgb, float a)
    {
        a = Mathf.Lerp(min, max, t);

        t += Time.deltaTime / fadeDuration;

        image.color = new Color(rgb, rgb, rgb, a);

        if (t >= 1f) {
            float temp = max;
            max = min;
            min = temp;
            t = 0f;
        }
    }

    private IEnumerator DamageKnockback(GameObject enemy)
    {
        anim.SetBool("Damaged", true);

        if (enemy.transform.position.x > transform.position.x) {
            dir = -1;
        }
        else {
            dir = 1;
        }

        moveScript.canMove2 = false;
        onKnockback = true;

        rb.velocity = new Vector2(dir * knockbackForce, knockbackForce * 1.5f);

        yield return new WaitForSeconds(0.4f);

        anim.SetBool("Damaged", false);

        moveScript.canMove2 = true;
        onKnockback = false;
        damageable = false;
        StartCoroutine(Blink());

        yield return new WaitForSeconds(immortalTime);
        if (enemy != null){
            Physics2D.IgnoreLayerCollision(gameObject.layer, enemy.layer, false);
        }

        damageable = true;
    }

    private IEnumerator Blink()
    {
        blinkTime = maxBlinkTime;

        while (!damageable)
        {
            flashScript.Flash(new Color(1f, 1f, 1f, 0f));

            yield return new WaitForSeconds(blinkTime);

            blinkTime -= 0.05f;

            if (blinkTime < 0.25f) {
                blinkTime = 0.25f;
            }
        }
    }

    public void HealthUp()
    {
        foreach (Image sprite in hpSprites) {
            Destroy(sprite.gameObject);
        }
        
        hpSprites = new Image[maxHealth + 1];
        
        for (int i = 0; i < maxHealth + 1; i++) {
            InstantiateHealth(i);
        }

        maxHealth++;
        currentHealth = maxHealth;
    }

    public void HealthRestore(int amount)
    {
        if (currentHealth < maxHealth) {
            currentHealth += amount;
            hpSprites[currentHealth-1].GetComponent<Animator>().SetTrigger("HealthRecovered");
        }
        flashScript.Flash(Color.green);
    }

    private void InstantiateHealth(int index)
    {
        GameObject t = Instantiate(healthUI, new Vector3(0, 0, 0), Quaternion.identity ,GameObject.Find("CanvasHP").transform);
        hpSprites[index] = t.GetComponent<Image>();
        t.GetComponent<RectTransform>().anchoredPosition = new Vector2(20f * index * healthSize * 1.5f + 16f, -16f);
        t.GetComponent<RectTransform>().sizeDelta = new Vector2(30f * healthSize, 30f * healthSize);
    }
}