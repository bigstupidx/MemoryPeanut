using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;
using SgLib;

#if EASY_MOBILE
using EasyMobile;
#endif

public class UIManager : MonoBehaviour
{
    [Header("Object References")]
    public GameObject header;
    public GameObject title;
    public Text score;
    public Text bestScore;
    public GameObject tapToStart;
    public GameObject menuButtons;
    public GameObject settingsUI;
    public GameObject soundOnBtn;
    public GameObject soundOffBtn;
    public GameObject musicOnBtn;
    public GameObject musicOffBtn;
    public GameObject scoreCirlces;
    public GameObject gamePlay;
    public GameObject mouseCheck;
    public Text scoreTextWhenGameOver;
    public Text gameOverText;
    public Text bestScoreText;
    public GameObject bestText;
    public CheckHit checkHit;
    public Text countText;

    public Image scoreCircleImage;
    public Image scoreCircleImageMaxValue;
    public Image scoreBestCircleImage;

    [Header("Premium Features Buttons")]
    public GameObject leaderboardBtn;
    public GameObject achievementBtn;
    public GameObject shareBtn;
    public GameObject settingBtn;
    public GameObject reStartBtn;
    public GameObject removeAdsBtn;
    public GameObject restorePurchaseBtn;

    [Header("Sharing-Specific")]
    public GameObject shareUI;
    public Image sharedImage;

    Animator scoreAnimator;
    Animator dailyRewardAnimator;
    bool isWatchAdsForCoinBtnActive;

    void OnEnable()
    {
        GameManager.GameStateChanged += GameManager_GameStateChanged;
        ScoreManager.ScoreUpdated += OnScoreUpdated;
    }

    void OnDisable()
    {
        GameManager.GameStateChanged -= GameManager_GameStateChanged;
        ScoreManager.ScoreUpdated -= OnScoreUpdated;
    }

    // Use this for initialization
    void Start()
    {
        scoreCircleImage.fillAmount = 0;
        scoreCircleImageMaxValue.fillAmount = 1;
        scoreAnimator = score.GetComponent<Animator>();
        Reset();
        ShowStartUI();
    }

    // Update is called once per frame
    void Update()
    {
        score.text = ScoreManager.Instance.Score.ToString();
        scoreTextWhenGameOver.text = ScoreManager.Instance.Score.ToString();
        bestScoreText.text = ScoreManager.Instance.HighScore.ToString();
        bestScore.text = ScoreManager.Instance.HighScore.ToString();
        countText.text = (checkHit.numberOfHit - checkHit.count).ToString();

        if (settingsUI.activeSelf)
        {
            UpdateMuteButtons();
            UpdateMusicButtons();
        }
    }

    void GameManager_GameStateChanged(GameState newState, GameState oldState)
    {
        if (newState == GameState.Playing)
        {              
            ShowGameUI();
        }
        else if (newState == GameState.PreGameOver)
        {
            // Before game over, i.e. game potentially will be recovered
        }
        else if (newState == GameState.GameOver)
        {
            Invoke("ShowGameOverUI", 1f);
        }
    }

    void OnScoreUpdated(int newScore)
    {
        scoreAnimator.Play("NewScore");
    }

    void Reset()
    {
        scoreCircleImage.gameObject.SetActive(false);
        scoreBestCircleImage.gameObject.SetActive(false);
        bestScoreText.gameObject.SetActive(false);
        header.SetActive(false);
        title.SetActive(false);
        score.gameObject.SetActive(false);
        tapToStart.SetActive(false);
        menuButtons.SetActive(false);
        scoreTextWhenGameOver.gameObject.SetActive(false);
        countText.gameObject.SetActive(false);
        scoreCirlces.SetActive(false);
        reStartBtn.SetActive(false);

        // Enable or disable premium stuff
        bool enablePremium = IsPremiumFeaturesEnabled();
        leaderboardBtn.SetActive(enablePremium);
        achievementBtn.SetActive(enablePremium);
        shareBtn.SetActive(enablePremium);
        removeAdsBtn.SetActive(enablePremium);
        restorePurchaseBtn.SetActive(enablePremium);

        // Hidden by default
        settingsUI.SetActive(false);
        shareUI.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        bestText.SetActive(false);
    }

    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }

    public void EndGame()
    {
        GameManager.Instance.GameOver();
    }

    public void RestartGame()
    {
        GameManager.Instance.RestartGame(0.2f);
    }

    public void ShowStartUI()
    {
        settingsUI.SetActive(false);
        gamePlay.SetActive(false);
        mouseCheck.SetActive(false);
        header.SetActive(true);
        title.SetActive(true);
        tapToStart.SetActive(true);
        menuButtons.SetActive(true);
        shareBtn.SetActive(false);
        reStartBtn.SetActive(false);
        scoreTextWhenGameOver.gameObject.SetActive(false);
    }

    public void ShowGameUI()
    {
        header.SetActive(true);
        gamePlay.SetActive(true);
        mouseCheck.SetActive(true);
        title.SetActive(false);
        score.gameObject.SetActive(true);


        if (GameManager.Instance.showRemainedTilesCount)
            countText.gameObject.SetActive(true);
        
        tapToStart.SetActive(false);
        menuButtons.SetActive(false);
        scoreTextWhenGameOver.gameObject.SetActive(false);
        reStartBtn.SetActive(false);
        menuButtons.SetActive(false);
    }

    public void ShowGameOverUI()
    {
        StartCoroutine(CRShowGameOverUI());
    }

    IEnumerator CRShowGameOverUI()
    {
        float runTime = 1f;
        header.SetActive(false);
        gamePlay.SetActive(false);
        mouseCheck.SetActive(false);
        
        title.SetActive(false);
        score.gameObject.SetActive(false);
        scoreTextWhenGameOver.gameObject.SetActive(true);
        tapToStart.SetActive(false);
        settingsUI.SetActive(false);
        shareUI.SetActive(false);
        countText.gameObject.SetActive(false);
        scoreCirlces.SetActive(true);

        float pastTime = 0;
        scoreCircleImage.gameObject.SetActive(true);
        scoreCircleImageMaxValue.gameObject.SetActive(true);
        scoreBestCircleImage.gameObject.SetActive(true);
       
        float temp = (float)ScoreManager.Instance.Score / GameManager.Instance.maxScore;
        float temp2 = (float)ScoreManager.Instance.HighScore / GameManager.Instance.maxScore;
        scoreBestCircleImage.fillAmount = temp2;

        while (pastTime < runTime)
        {
            pastTime += Time.deltaTime;
            float t = pastTime / runTime;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);
            scoreCircleImage.fillAmount = temp * t;
            scoreTextWhenGameOver.text = (Mathf.RoundToInt(t * ScoreManager.Instance.Score)).ToString();

            yield return null;
        }
            
        if (ScoreManager.Instance.HasNewHighScore)
        {
            gameOverText.text = "NEW BEST!";
            SoundManager.Instance.PlaySound(SoundManager.Instance.newBest);
        }
        else
        {
            gameOverText.text = "GAME OVER";
        }

        gameOverText.gameObject.SetActive(true);
        gameOverText.gameObject.GetComponent<Animator>().Play("NewHighScoreAnim");

        bestText.SetActive(true);
        bestScoreText.gameObject.SetActive(true);
        reStartBtn.SetActive(true);
        menuButtons.SetActive(true);
        shareBtn.SetActive(IsPremiumFeaturesEnabled());
    }

    public void ShowSettingsUI()
    {
        settingsUI.SetActive(true);
    }

    public void HideSettingsUI()
    {
        settingsUI.SetActive(false);
    }

    public void ShowLeaderboardUI()
    {
        #if EASY_MOBILE
        if (GameServiceManager.IsInitialized())
        {
            GameServiceManager.ShowLeaderboardUI();
        }
        else
        {
        #if UNITY_IOS
            MobileNativeUI.Alert("Service Unavailable", "The user is not logged in to Game Center.");
        #elif UNITY_ANDROID
            GameServiceManager.Init();
        #endif
        }
        #else
        Debug.Log("This feature requires EasyMobile plugin.");
        #endif
    }

    public void ShowAchievementsUI()
    {
        #if EASY_MOBILE
        if (GameServiceManager.IsInitialized())
        {
            GameServiceManager.ShowAchievementsUI();
        }
        else
        {
        #if UNITY_IOS
        MobileNativeUI.Alert("Service Unavailable", "The user is not logged in to Game Center.");
        #elif UNITY_ANDROID
            GameServiceManager.Init();
        #endif
        }
        #else
        Debug.Log("This feature requires EasyMobile plugin.");
        #endif
    }

    public void PurchaseRemoveAds()
    {
        #if EASY_MOBILE
        InAppPurchaser.Instance.Purchase(InAppPurchaser.Instance.removeAds);
        #else
        Debug.Log("This feature requires EasyMobile plugin.");
        #endif
    }

    public void RestorePurchase()
    {
        #if EASY_MOBILE
        InAppPurchaser.Instance.RestorePurchase();
        #else
        Debug.Log("This feature requires EasyMobile plugin.");
        #endif
    }

    public void ShowShareUI()
    {
        #if EASY_MOBILE
        Texture2D texture = ScreenshotSharer.Instance.GetScreenshotTexture();

        if (texture != null)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            Transform imgTf = sharedImage.transform;
            Image imgComp = imgTf.GetComponent<Image>();
            float scaleFactor = imgTf.GetComponent<RectTransform>().rect.width / sprite.rect.width;
            imgComp.sprite = sprite;
            imgComp.SetNativeSize();
            imgTf.localScale = imgTf.localScale * scaleFactor;

            shareUI.SetActive(true);
        }
        #else
        Debug.Log("This feature requires EasyMobile plugin.");
        #endif
    }

    public void HideShareUI()
    {
        shareUI.SetActive(false);
    }

    public void ShareScreenshot()
    {
        #if EASY_MOBILE
        shareUI.SetActive(false);
        ScreenshotSharer.Instance.ShareScreenshot();
        #endif
    }

    public void ShowCharacterSelectionScene()
    {
        SceneManager.LoadScene("CharacterSelection");
    }

    public void ToggleSound()
    {
        SoundManager.Instance.ToggleMute();
    }

    public void ToggleMusic()
    {
        SoundManager.Instance.ToggleMusic();
    }

    public void RateApp()
    {
        Utilities.RateApp();
    }

    public void OpenTwitterPage()
    {
        Utilities.OpenTwitterPage();
    }

    public void OpenFacebookPage()
    {
        Utilities.OpenFacebookPage();
    }

    public void ButtonClickSound()
    {
        Utilities.ButtonClickSound();
    }

    void UpdateMuteButtons()
    {
        if (SoundManager.Instance.IsMuted())
        {
            soundOnBtn.gameObject.SetActive(false);
            soundOffBtn.gameObject.SetActive(true);
        }
        else
        {
            soundOnBtn.gameObject.SetActive(true);
            soundOffBtn.gameObject.SetActive(false);
        }
    }

    void UpdateMusicButtons()
    {
        if (SoundManager.Instance.IsMusicOff())
        {
            musicOffBtn.gameObject.SetActive(true);
            musicOnBtn.gameObject.SetActive(false);
        }
        else
        {
            musicOffBtn.gameObject.SetActive(false);
            musicOnBtn.gameObject.SetActive(true);
        }
    }

    bool IsPremiumFeaturesEnabled()
    {
        return PremiumFeaturesManager.Instance != null && PremiumFeaturesManager.Instance.enablePremiumFeatures;
    }
}
