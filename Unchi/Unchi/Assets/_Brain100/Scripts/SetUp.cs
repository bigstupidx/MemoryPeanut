using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetUp : MonoBehaviour
{

    public CheckHit checkHit;

    [SerializeField]
    private GameObject pics;

    int i = 0;
    int tempNumberOfWinning = 0;
    public int numberOfPics;
    public int numberOfPreviousPics;

    private bool canInstantiate;

    void Start()
    {
        gameObject.GetComponent<GridLayoutGroup>().spacing = new Vector2(GameManager.Instance.space, GameManager.Instance.space);

        numberOfPics = 9;
        numberOfPreviousPics = 9;

        gameObject.GetComponent<GridLayoutGroup>().constraintCount = 3;
        gameObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2((GameManager.Instance.gridEdgeSize - 2 * GameManager.Instance.space) / 3, (GameManager.Instance.gridEdgeSize - 2 * GameManager.Instance.space) / 3);

        for (int j = 0; j < 9; j++)
        {
            GameObject temp;
            temp = Instantiate(pics, transform.position, transform.rotation);
            Image img = temp.GetComponent<Image>();
            img.sprite = GameManager.Instance.defaultSprite;
            img.color = GameManager.Instance.defaultSpriteColor;
            temp.transform.SetParent(gameObject.transform);
            temp.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        }

        GameObject[] tempObject = GameObject.FindGameObjectsWithTag("FocusGameObject");

        for (int j = 0; j < 9; j++)
        {
            tempObject[j].GetComponent<BoxCollider2D>().size = new Vector2((GameManager.Instance.gridEdgeSize - 2 * GameManager.Instance.space) / 3, (GameManager.Instance.gridEdgeSize - 2 * GameManager.Instance.space) / 3);
        }

    }

    void Update()
    {
        int a = 0;
        if (tempNumberOfWinning == checkHit.numberOfWinning)
        {
            for (i = 2; i < 50; i++)
            {
                a = a + i;
                if (tempNumberOfWinning == a)
                {
                    canInstantiate = true;
                    numberOfPics = (i + 2) * (i + 2);
                    numberOfPreviousPics = (i + 1) * (i + 1);
                    break;
                }
                if (tempNumberOfWinning < a)
                {
                    numberOfPics = (i + 1) * (i + 1);
                    numberOfPreviousPics = (i + 1) * (i + 1);
                    break;
                }

            }
            tempNumberOfWinning++;
        }

        if (canInstantiate)
        {
            Invoke("InstantiaObject", GameManager.Instance.instantiateObjectAfter);
            canInstantiate = false;
        }

    }

    void InstantiaObject()
    {
        gameObject.GetComponent<GridLayoutGroup>().constraintCount = i + 2;
        gameObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2((GameManager.Instance.gridEdgeSize - (i + 1) * GameManager.Instance.space) / (i + 2), (GameManager.Instance.gridEdgeSize - (i + 1) * GameManager.Instance.space) / (i + 2));
        gameObject.GetComponent<GridLayoutGroup>().spacing = new Vector2(GameManager.Instance.space, GameManager.Instance.space);

        for (int j = 0; j < ((i + 2) * (i + 2) - (i + 1) * (i + 1)); j++)
        {
            GameObject temp;
            temp = Instantiate(pics, transform.position, transform.rotation);
            temp.transform.SetParent(gameObject.transform);
            temp.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        }

        GameObject[] tempObject = GameObject.FindGameObjectsWithTag("FocusGameObject");
        for (int j = 0; j < (i + 2) * (i + 2); j++)
        {
            tempObject[j].GetComponent<BoxCollider2D>().size = new Vector2((GameManager.Instance.gridEdgeSize - (i + 1) * GameManager.Instance.space) / (i + 2), (GameManager.Instance.gridEdgeSize - (i + 1) * GameManager.Instance.space) / (i + 2));
        }
    }
}