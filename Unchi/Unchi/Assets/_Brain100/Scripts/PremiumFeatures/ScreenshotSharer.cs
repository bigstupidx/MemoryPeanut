using UnityEngine;
using System.Collections;

#if EASY_MOBILE
using EasyMobile;
#endif

namespace SgLib
{
    public class ScreenshotSharer : MonoBehaviour
    {
        [Header("Sharing Config")]
        [Tooltip("Any instances of [score] will be replaced by the actual score achieved in the last game, [AppName] will be replaced by the app name declared in AppInfo")]
        [TextArea(3, 3)]
        public string shareMessage = "Awesome! I've just scored [score] in [AppName]! [#AppName]";
        public string screenshotFilename = "screenshot.png";

        #if EASY_MOBILE
        public static ScreenshotSharer Instance { get; private set; }

        Texture2D capturedScreenshot;

        void OnEnable()
        {
            CheckHit.LevelFailed += OnLevelFailed;
        }

        void OnDisable()
        {
            CheckHit.LevelFailed -= OnLevelFailed;
        }

        void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        void OnLevelFailed()
        {
            if (PremiumFeaturesManager.Instance.enablePremiumFeatures)
            {
                StartCoroutine(CRCaptureScreenshot());
            }
        }

        IEnumerator CRCaptureScreenshot()
        {
            // Wait for right timing to take screenshot
            yield return new WaitForEndOfFrame();

            capturedScreenshot = MobileNativeShare.CaptureScreenshot();
        }

        public Texture2D GetScreenshotTexture()
        {
            return capturedScreenshot;
        }

        public void ShareScreenshot()
        {
            if (capturedScreenshot == null)
            {
                Debug.Log("ShareScreenshot: FAIL. No captured screenshot.");
                return;
            } 

            string msg = shareMessage;
            msg = msg.Replace("[score]", ScoreManager.Instance.Score.ToString());
            msg = msg.Replace("[AppName]", AppInfo.Instance.APP_NAME);
            msg = msg.Replace("[#AppName]", "#" + AppInfo.Instance.APP_NAME.Replace(" ", ""));
            MobileNativeShare.ShareTexture2D(capturedScreenshot, screenshotFilename, msg);
        }

        #endif
    }
}
