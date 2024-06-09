using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [HideInInspector] public int currentHealth;
    public int maxHealth;
    [SerializeField] private GameObject healthUI;
    [SerializeField] private float healthSize = 1f;
    private List<Image> hpSprites = new List<Image>();
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    private Image blackFade;
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
    public bool damageable = true;
    private SimpleFlash flashScript;
    private AudioManager audioPlayer;
    private Animator anim;
    private float maxBlinkTime = 0.5f;
    private float blinkTime;
    private GameObject pauseMenu;
    private GameObject gameOverMenu;
    private bool onHazard = false;

    private LayerMask enemyLayer;
    [SerializeField] private LayerMask layerImmortality;
    private LayerMask playerLayer;
    
    private void Start()
    {
        playerLayer = gameObject.layer;
        support = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>();
        maxHealth = support.maxHealth;
        currentHealth = maxHealth;

        InitializeHealthUI();
        InitializeReferences();

        blinkTime = maxBlinkTime;
        pauseMenu = GameObject.Find("PauseMenu");
        gameOverMenu = GameObject.Find("GameOverMenu");
        Fade(fadeDur, 0f);
    }

    private void InitializeHealthUI()
    {
        for (int i = 0; i < maxHealth; i++)
        {
            Image heart = Instantiate(healthUI, Vector3.zero, Quaternion.identity, GameObject.Find("CanvasHP").transform).GetComponent<Image>();
            hpSprites.Add(heart);
            heart.rectTransform.anchoredPosition = new Vector2(20f * i * healthSize * 1.5f + 16f, -16f);
            heart.rectTransform.sizeDelta = new Vector2(30f * healthSize, 30f * healthSize);
        }
    }

    private void InitializeReferences()
    {
        blackFade = GameObject.FindGameObjectWithTag("BlackFade").GetComponent<Image>();
        moveScript = GetComponent<Player_Movement>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        flashScript = GetComponent<SimpleFlash>();
        audioPlayer = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>().GetAudioManagerInstance();
        moveScript.CanMove2 = true;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
            enemyLayer = collision.gameObject.layer;
            if (LayerMask.LayerToName(enemyLayer) == "Enemies" && damageable && !GetComponent<PlayerCombat>().isParrying && !onHazard) {
                damageable = false;
                TomarDano(collision.gameObject);
            }
            if (LayerMask.LayerToName(enemyLayer) == "Disparo" && damageable && !onHazard) {
                if ((collision.transform.position.x > transform.position.x && transform.localScale.x < 0) || (collision.transform.position.x < transform.position.x && transform.localScale.x > 0) || !GetComponent<PlayerCombat>().isParrying) {
                damageable = false;
                TomarDano(collision.gameObject);
                }
            }
            if (LayerMask.LayerToName(enemyLayer) == "Fogo" && damageable && !onHazard) {
                Debug.Log("fogo");
                damageable = false;
                TomarDano(collision.gameObject);
                Destroy(collision.gameObject);
            }
        
    }

    private void OnTriggerStay2D(Collider2D other){
        if (other.gameObject.tag=="Raio" && damageable){
                Debug.Log("raio");
                damageable=false;
                TomarDano(other.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Hazard" && !onHazard)
        {
            if (currentHealth > 0)
            {
                hpSprites[currentHealth - 1].GetComponent<Animator>().SetTrigger("DamageTaken");
                currentHealth--;
                UpdateHealthUI();

                StartCoroutine(HazardDamage());
            }
        }
    }

    public void TomarDano(GameObject enemy) {
        gameObject.layer = LayerMask.NameToLayer("Immortality");
        if (currentHealth > 0) {
            hpSprites[currentHealth-1].GetComponent<Animator>().SetTrigger("DamageTaken");
            currentHealth--;
            audioPlayer.Play("PlayerDamaged");
            UpdateHealthUI();
            
            StartCoroutine(DamageKnockback(enemy));
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

        moveScript.CanMove2 = false;
        onKnockback = true;

        rb.velocity = new Vector2(dir * knockbackForce, knockbackForce * 1.5f);

        yield return PushBackPlayer(0.4f);

        anim.SetBool("Damaged", false);

        moveScript.CanMove2 = true;
        onKnockback = false;
        damageable = false;
        StartCoroutine(Blink());

        yield return new WaitForSeconds(immortalTime);
        /*if (enemy != null){
            Physics2D.IgnoreLayerCollision(gameObject.layer, enemy.layer, false);
        }*/

        damageable = true;
        gameObject.layer = playerLayer;
    }

    private IEnumerator HazardDamage()
    {
        onHazard = true;

        currentLastPos = moveScript.LastPos; 
        //Note for future self:

        //Essa parte em si parece sus, mas ela deve ser resolvida lá no playerMovement
        //Acredito que não tem mais nada a mexer por aqui.
        currentLastDir = new Vector3(moveScript.LastDir, 1f, 1f);

        moveScript.CanMove2 = false;
        Fade(fadeDur, 1f);

        yield return new WaitForSeconds(fadeDur);

        rb.velocity = Vector2.zero;
        transform.position = currentLastPos;
        transform.localScale = currentLastDir;
        moveScript.CanMove2 = true; 
        if (currentHealth>0) Fade(fadeDur, 0f);
        onHazard = false;
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
        hpSprites.Clear();

        maxHealth++;
        currentHealth = maxHealth;
        InitializeHealthUI();
    }

    public void HealthRestore(int amount)
    {
        if (currentHealth < maxHealth) {
            currentHealth += amount;
            for (int i = currentHealth - amount; i < currentHealth; i++) {
                hpSprites[i].GetComponent<Animator>().SetTrigger("HealthRecovered");
            }
        }
        UpdateHealthUI();
        flashScript.Flash(Color.green);
    }

    private IEnumerator PushBackPlayer(float pushBackTime){
        float elapsedTime = 0f;
        while (elapsedTime < pushBackTime)
        {
            rb.velocity = new Vector2(dir * knockbackForce, rb.velocity.y);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    private void UpdateHealthUI()
    {
        for (int i = 0; i < hpSprites.Count; i++)
        {
            hpSprites[i].sprite = i < currentHealth ? fullHeart : emptyHeart;
        }
    }
    private void Fade(float fadeDuration, float toAlpha)
    {
        blackFade.DOFade(toAlpha, fadeDuration).SetEase(Ease.Linear);
    }
}
