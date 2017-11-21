using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SgLib;

public class CheckHit : MonoBehaviour
{

    public static event System.Action LevelFailed;
    public static event System.Action LevelCompleted;

    public LayerMask layer;
    public int count;
    public bool isReset = false;
    public int numberOfWinning = 0;
    public int numberOfHit;

    bool isFail = false;
    PickRandom pickRandom;

    void Awake()
    {
        count = 0;
    }

    void OnEnable()
    {
        GameManager.GameStateChanged += OnGameStateChanged;
    }

    void OnDisable()
    {
        GameManager.GameStateChanged -= OnGameStateChanged;
    }

    void OnGameStateChanged(GameState newState, GameState oldState)
    {
        if (newState == GameState.Playing)
        {

        }      
    }

    public void Die()
    {
        gameObject.SetActive(false);
     
        LevelFailed();
    }

    void Start()
    {
        pickRandom = GetComponent<PickRandom>();
    }

    void Update()
    {
        CheckHowManyHit();
        ScreenMouseRay();
    }

    void CheckHowManyHit()
    {
        int i;
        for (i = 0; i <= 100; i++)
        {
            if (numberOfWinning >= (2 * i + i * (i - 1) / 2) && numberOfWinning < (2 * (i + 1) + (i + 1) * i / 2))
            {
                break;
            }
        }

        numberOfHit = numberOfWinning - (2 * i + i * (i - 1) / 2) + 3 + i;

    }

    void DeactiveParticle()
    {
        GameManager.Instance.checkParticle.gameObject.SetActive(false);
    }

    IEnumerator ScaleChangeOnClick(GameObject plane, bool isTrue)
    {
        float time = 0.1f;
        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            float factor = t / time;
            plane.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.5f, 0.5f, 0.5f), factor);
            yield return null;
        }
        time = 0.1f;
        t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            float factor = t / time;
            plane.transform.localScale = Vector3.Lerp(plane.transform.localScale, Vector3.one, factor);
            yield return null;
        }
        plane.transform.localScale = Vector3.one;

        if (isTrue)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.correct);
            ScoreManager.Instance.AddScore(1);
        }
        else
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.wrong);
            plane.GetComponent<Image>().color = GameManager.Instance.wrongSelectionColor;
            yield return new WaitForSeconds(0.1f);
            GameObject[] trueList = GameObject.FindGameObjectsWithTag("FocusGameObject");
            foreach (GameObject tr in trueList)
            {
                if (tr.GetComponent<ChangePic>().enabled == true)
                {
                    StartCoroutine(ShowAllTrue(tr));
                }
            }
                
            Invoke("Die", 0.8f);          
        }
    }

    IEnumerator ShowAllTrue(GameObject tr)
    {
        
        float time = 0.1f;
        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            float factor = t / time;
            tr.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.5f, 0.5f, 0.5f), factor);
            yield return null;
        }
        time = 0.1f;
        t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            float factor = t / time;
            tr.transform.localScale = Vector3.Lerp(tr.transform.localScale, Vector3.one, factor);
            yield return null;
        }
        Image img = tr.GetComponent<Image>();
        img.sprite = GameManager.Instance.openedSprite;
        img.color = GameManager.Instance.openedSpriteColor;

        tr.GetComponent<ChangePic>().isHit = true;
       
    }

    public void ScreenMouseRay()
    {
		
        if (count < numberOfHit)
        {
            
            if (Input.GetMouseButtonDown(0) && !isFail)
            {
                var mousePos = Input.mousePosition;
                Vector2 posBuff = Camera.main.ScreenToWorldPoint(mousePos);
                RaycastHit2D hit = Physics2D.Linecast(posBuff, posBuff + new Vector2(0f, 0f), layer);
                if (hit.collider)
                {
                    if (hit.collider.tag == "FocusGameObject" && pickRandom.canClick == true)
                    {
                       
                        if (hit.collider.gameObject.GetComponent<ChangePic>().enabled == true && hit.collider.gameObject.GetComponent<ChangePic>().isHit == false)
                        {
                            GameManager.Instance.checkParticle.gameObject.SetActive(false);
                            StartCoroutine(ScaleChangeOnClick(hit.collider.gameObject, true));
                            hit.collider.gameObject.GetComponent<ChangePic>().isHit = true;
                            count++;
                            Image img = hit.collider.gameObject.GetComponent<Image>();
                            img.sprite = GameManager.Instance.openedSprite;
                            img.color = GameManager.Instance.openedSpriteColor;
                            GameManager.Instance.checkParticle.gameObject.SetActive(true);
                            Transform particle = GameManager.Instance.checkParticle.gameObject.transform;
                            Vector2 pos = Camera.main.ScreenToWorldPoint(mousePos);
                            particle.position = pos;
                        }
                        else if (hit.collider.gameObject.GetComponent<ChangePic>().enabled == false)
                        {
                            GameObject.FindGameObjectWithTag("GameTiles").GetComponent<Animator>().enabled = true;
                            GameObject.FindGameObjectWithTag("GameTiles").GetComponent<Animator>().Play("Shaking");
                            isFail = true;
                            StartCoroutine(ScaleChangeOnClick(hit.collider.gameObject, false));           
                        }

                    }

                }
            }

        }
        else
        {
            count = 0;
            numberOfWinning++;
            pickRandom.canClick = false;
            Invoke("ResetLevel", GameManager.Instance.resetLevel);	

            if (GameManager.Instance.showRemainedTilesCount)
                pickRandom.uiManager.countText.gameObject.SetActive(false);
        }
    }

    void ResetLevel()
    {    
        pickRandom.ResetBack();
        LevelCompleted();
        SoundManager.Instance.PlaySound(SoundManager.Instance.levelComplete);
    }
}


