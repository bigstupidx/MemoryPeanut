using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PickRandom : MonoBehaviour
{
    public bool canClick = false;

    int[] z = new int[100];

    private GameObject[] pics;
    public GameObject groupObject;
    public SetUp setUp;

    RectTransform rt;

    private Vector3 leaveStartPoint;
    private Vector3 leaveEndPoint;
    private Vector3 comeBackStartPoint;
    private Vector3 comeBackEndPoint;

    public UIManager uiManager;

    private void OnEnable()
    {
        CheckHit.LevelCompleted += OnLevelCompleted;
    }

    private void OnDisable()
    {
        CheckHit.LevelCompleted -= OnLevelCompleted;
    }

    void Start()
    {
        leaveStartPoint = groupObject.GetComponent<RectTransform>().localPosition;
        comeBackEndPoint = groupObject.GetComponent<RectTransform>().localPosition;
        leaveEndPoint = new Vector3(1000, groupObject.GetComponent<RectTransform>().localPosition.y, 0);
        comeBackStartPoint = new Vector3(-1368, groupObject.GetComponent<RectTransform>().localPosition.y, 0);

        rt = groupObject.GetComponent<RectTransform>();

        Invoke("RandomAppear", GameManager.Instance.appearAfter);
    }

    void Update()
    {
        pics = GameObject.FindGameObjectsWithTag("FocusGameObject");
    }

    IEnumerator CRMoveGridRight(float moveTime)
    {
        float startTime = Time.time;

        while (Time.time - startTime < moveTime)
        {
            float f = (Time.time - startTime) / moveTime;
            rt.localPosition = Vector3.Lerp(leaveStartPoint, leaveEndPoint, f);
            yield return null;
        }

        rt.localPosition = leaveEndPoint;

        startTime = Time.time;
        while (Time.time - startTime < moveTime)
        {
            float f = (Time.time - startTime) / moveTime;
            rt.localPosition = Vector3.Lerp(comeBackStartPoint, comeBackEndPoint, f);
            yield return null;
        }

        rt.localPosition = comeBackEndPoint;

        Invoke("RandomAppear", GameManager.Instance.appearAfter);

        if (GameManager.Instance.showRemainedTilesCount)
            uiManager.countText.gameObject.SetActive(true);
    }

    public void OnLevelCompleted()
    {
        StartCoroutine(CRMoveGridRight(GameManager.Instance.disappearTime / 2));
    }

    public void ResetBack()
    {
       
        for (int p = 0; p < setUp.numberOfPreviousPics; p++)
        {
            pics[p].GetComponent<ChangePic>().enabled = false;
            Image img = pics[p].GetComponent<Image>();
            img.sprite = GameManager.Instance.defaultSprite;
            img.color = GameManager.Instance.defaultSpriteColor;
            pics[p].GetComponent<ChangePic>().canChange = false;
            pics[p].GetComponent<ChangePic>().isHit = false;
        }
       
    }

    void RandomAppear()
    {
        
        z[0] = Random.Range(0, setUp.numberOfPics);

        for (int j = 1; j < gameObject.GetComponent<CheckHit>().numberOfHit; j++)
        {
            for (int k = 0; k < 100; k++)
            {
                int t = -1;
                z[j] = Random.Range(0, setUp.numberOfPics);
                for (int l = 0; l < j; l++)
                {
                    t++;
                    if (z[l] == z[j])
                        break;
                }
                if (z[t] != z[j])
                    break;

            }

        }

        for (int k = 0; k < setUp.numberOfPics; k++)
        {
            for (int i = 0; i <= gameObject.GetComponent<CheckHit>().numberOfHit; i++)
            {
                if (i == gameObject.GetComponent<CheckHit>().numberOfHit)
                {
                    pics[k].GetComponent<ChangePic>().enabled = false;
                    pics[k].GetComponent<ChangePic>().canChange = false;
                    break;
                }
                else if (k == z[i])
                {
                    pics[k].GetComponent<ChangePic>().enabled = true;
                    pics[k].GetComponent<ChangePic>().canChange = true;
                    break;
                }
            }
        }

        Invoke("SetCanClickTrue", GameManager.Instance.changePicAfter + 0.1f);
    }

    void SetCanClickTrue()
    {
        canClick = true;
    }

}
