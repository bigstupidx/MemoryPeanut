using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using SgLib;

public enum GameState
{
    Prepare,
    Playing,
    Paused,
    PreGameOver,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static event System.Action<GameState, GameState> GameStateChanged = delegate { };

    private static bool isRestart;

    public GameState GameState
    {
        get
        {
            return _gameState;
        }
        private set
        {
            if (value != _gameState)
            {
                GameState oldState = _gameState;
                _gameState = value;

                GameStateChanged(_gameState, oldState);
            }
        }
    }

    private GameState _gameState = GameState.Prepare;

    public static int GameCount
    { 
        get { return _gameCount; } 
        private set { _gameCount = value; } 
    }

    private static int _gameCount = 0;

    [Header("Set the target frame rate for this game")]
    [Tooltip("Use 60 for games requiring smooth quick motion, set -1 to use platform default frame rate")]
    public int targetFrameRate = 30;

    // List of public variable for gameplay tweaking
    [Header("Gameplay Config")]
    public float gridEdgeSize = 420f;
    public float space = 10f;
    public float instantiateObjectAfter = 1f;
    public float disappearTime = 0.8f;
    public float appearAfter = 0.8f;
    public float changePicAfter = 0.8f;
    public float resetLevel = 0.4f;
    public int maxScore = 100;
    public Color wrongSelectionColor = Color.red;
    [Tooltip("Whether to show the number of unopened tiles")]
    public bool showRemainedTilesCount = true;

    [Header("Game Sprites Config")]
    public Sprite defaultSprite;
    public Color defaultSpriteColor = Color.white;
    public Sprite openedSprite;
    public Color openedSpriteColor = Color.white;

    // List of public variables referencing other objects
    [Header("Object References")]
    public CheckHit checkHit;
    public ParticleSystem checkParticle;

    void OnEnable()
    {
        CheckHit.LevelFailed += PlayerController_PlayerDied;
    }

    void OnDisable()
    {
        CheckHit.LevelFailed -= PlayerController_PlayerDied;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(Instance.gameObject);
            Instance = this;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Use this for initialization
    void Start()
    {
        // Initial setup
        Application.targetFrameRate = targetFrameRate;
        ScoreManager.Instance.Reset();

        PrepareGame();
    }

    // Listens to the event when player dies and call GameOver
    void PlayerController_PlayerDied()
    {
        GameOver();
    }

    // Make initial setup and preparations before the game can be played
    public void PrepareGame()
    {
        GameState = GameState.Prepare;

        if (isRestart)
        {
            StartGame();
        }
    }

    // A new game official starts
    public void StartGame()
    {
        GameState = GameState.Playing;
        if (SoundManager.Instance.background != null)
        {
            SoundManager.Instance.PlayMusic(SoundManager.Instance.background);
        }
    }

    // Called when the player died
    public void GameOver()
    {
        if (SoundManager.Instance.background != null)
        {
            SoundManager.Instance.StopMusic();
        }

//        SoundManager.Instance.PlaySound(SoundManager.Instance.gameOver);
        GameState = GameState.GameOver;
        GameCount++;

        // Add other game over actions here if necessary
    }

    // Start a new game
    public void RestartGame(float delay = 0)
    {
        StartCoroutine(CRRestartGame(delay));
    }

    IEnumerator CRRestartGame(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        isRestart = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

        
}
