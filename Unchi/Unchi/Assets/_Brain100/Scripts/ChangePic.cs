using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePic : MonoBehaviour
{
    private Image pic;

    public bool canChange = false;
    public bool isHit = false;

    void Awake()
    {
        pic = gameObject.GetComponent<Image>();
        StartCoroutine(SetDefaultSprite(false));
    }

    void Update()
    {
        if (canChange)
        {
            pic.sprite = GameManager.Instance.openedSprite;
            pic.color = GameManager.Instance.openedSpriteColor;
            StartCoroutine(CRScaleAnim());
            StartCoroutine(SetDefaultSprite(true, GameManager.Instance.changePicAfter));
            canChange = false;
        }
    }

    IEnumerator SetDefaultSprite(bool scaleEffect, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        pic.sprite = GameManager.Instance.defaultSprite;
        pic.color = GameManager.Instance.defaultSpriteColor;

        if (scaleEffect)
            StartCoroutine(CRScaleAnim());

        yield return null;
    }

    IEnumerator CRScaleAnim()
    {
        float time = 0.1f;
        float t = 0;

        while (t < time)
        {
            t += Time.deltaTime;
            float factor = t / time;
            transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.5f, 0.5f, 0.5f), factor);
            yield return null;
        }

        time = 0.1f;
        t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            float factor = t / time;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, factor);
            yield return null;
        }
    }
}
