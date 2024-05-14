using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Vagalume : MonoBehaviour
{
    private Transform child;
    private SpriteRenderer spriteRenderer;
    private SimpleFlash flashScript;

    [Header("Sprites")]
    [SerializeField] private Sprite orangeSprite;
    [SerializeField] private Sprite blueSprite;

    [Header("Flash Colors")]
    [SerializeField] private Color orangeColor;
    [SerializeField] private Color blueColor;  

    [Header("Tween")]
    [SerializeField] private float intensity;
    [SerializeField] private float duration;
    [SerializeField] private AnimationCurve curveGo;
    [SerializeField] private AnimationCurve curveBack;
    private bool movingChild = false;
    private Vector3 pos;
    private Coroutine co;


    void Start()
    {
        child = transform.GetChild(0);
        spriteRenderer = child.GetComponent<SpriteRenderer>();
        flashScript = child.GetComponent<SimpleFlash>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.GetComponent<Player_Movement>().vagalumeAtual = gameObject;
            col.GetComponent<Player_Movement>().canGancho = true; 
            ChangeSprite(true);
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.GetComponent<Player_Movement>().canGancho = false;
            ChangeSprite(false);
        }
    }

    public void ChangeSprite(bool toBlue)
    {
        if (toBlue) {
            flashScript.Flash(blueColor);
            spriteRenderer.sprite = blueSprite;
        }
        
        else {
            flashScript.Flash(orangeColor);
            spriteRenderer.sprite = orangeSprite;
        }
    }

    public IEnumerator TakeGancho()
    {
        child.GetComponent<ItemFloating>().canUpdate = false;
        yield return new WaitForSeconds(0.25f);

        if (movingChild) {
            StopCoroutine(co);
            DOTween.Kill(child.transform);
        }

        co = StartCoroutine(MoveChild(movingChild));
    }

    private IEnumerator MoveChild(bool wasMoving)
    {
        child.GetComponent<ItemFloating>().canUpdate = false;
        movingChild = true;

        if (!wasMoving) {
            pos = new Vector3(child.localPosition.x, child.localPosition.y, child.localPosition.z);
        }
        
        float newX = pos.x + Random.Range(-0.3f, 0.3f);
        float newY = pos.y - intensity;

        child.transform.DOLocalMove(new Vector3(newX, newY, pos.z), duration * 0.5f, false).SetEase(curveGo);
        yield return new WaitForSeconds(duration * 0.5f);

        child.transform.DOLocalMove(new Vector3(-newX * 0.25f, pos.y + intensity * 0.25f, pos.z), duration, false).SetEase(curveBack);
        yield return new WaitForSeconds(duration);

        child.transform.DOLocalMove(pos, duration * 0.5f, false).SetEase(curveBack);
        yield return new WaitForSeconds(duration * 0.5f);

        child.GetComponent<ItemFloating>().canUpdate = true;

        movingChild = false;
    }
}
