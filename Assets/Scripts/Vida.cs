using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    private int currentHealth;
    [SerializeField] private int maxHealth;
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
    private Vector3 currentLastPos;
    private Vector3 currentLastDir;
    private Rigidbody2D rb;

    [SerializeField] private float immortalTime;
    [SerializeField] private float knockbackForce;
    private bool damageable = true;
    private SimpleFlash flashScript;
    private Animator anim;
    private float maxBlinkTime = 0.5f;
    private float blinkTime;

    private void Start()
    {
        currentHealth = maxHealth;
        blinkTime = maxBlinkTime;

        hpSprites = new Image[maxHealth];
        for (int i = 0; i < maxHealth; i++)
        {
            // GameObject t = Instantiate(healthUI, new Vector3(0, 0, 0), Quaternion.identity ,GameObject.Find("CanvasHP").transform);
            // hpSprites[i] = t.GetComponent<Image>();
            // t.GetComponent<RectTransform>().anchoredPosition = new Vector2(65 * i * healthSize * 1.5f, 0);
            // t.GetComponent<RectTransform>().sizeDelta = new Vector2(100f * healthSize, 100f * healthSize);

            InstantiateHealth(i);
        }

        blackFade = GameObject.FindGameObjectWithTag("BlackFade").GetComponent<Image>();
        moveScript = GetComponent<Player_Movement>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        flashScript = GetComponent<SimpleFlash>();

        moveScript.canMove2 = true;
    }
    

    private void OnCollisionEnter2D(Collision2D collision) {
            if (collision.gameObject.tag == "Inimigo" && damageable){
                Physics2D.IgnoreLayerCollision(gameObject.layer, collision.gameObject.layer, true);
                hpSprites[currentHealth-1].GetComponent<Animator>().SetTrigger("DamageTaken");
                currentHealth--;

                StartCoroutine(DamageKnockback(collision.gameObject));
            }
    }

    private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.tag == "Hazard"){
                hpSprites[currentHealth-1].GetComponent<Animator>().SetTrigger("DamageTaken");
                currentHealth--;

                StartCoroutine(HazardDamage());
            }
    }

    private IEnumerator HazardDamage()
    {
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

        if (toFade) {
            Fade(fadeDur, blackFade, 0f, alpha);
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

        float dir;

        if (enemy.transform.position.x > transform.position.x) {
            dir = -1;
        }
        else {
            dir = 1;
        }

        moveScript.canMove2 = false;

        rb.velocity = new Vector2(dir * knockbackForce, knockbackForce);

        yield return new WaitForSeconds(0.4f);

        anim.SetBool("Damaged", false);

        moveScript.canMove2 = true;
        damageable = false;
        StartCoroutine(Blink());

        yield return new WaitForSeconds(immortalTime);
        
        Physics2D.IgnoreLayerCollision(gameObject.layer, enemy.layer, false);

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
        hpSprites = new Image[maxHealth + 1];
        
        for (int i = 0; i < maxHealth + 1; i++) {
            InstantiateHealth(i);
        }

        maxHealth++;
    }

    private void InstantiateHealth(int index)
    {
        GameObject t = Instantiate(healthUI, new Vector3(0, 0, 0), Quaternion.identity ,GameObject.Find("CanvasHP").transform);
        hpSprites[index] = t.GetComponent<Image>();
        t.GetComponent<RectTransform>().anchoredPosition = new Vector2(65 * index * healthSize * 1.5f, 0);
        t.GetComponent<RectTransform>().sizeDelta = new Vector2(100f * healthSize, 100f * healthSize);
    }
}
