using System;
using System.Collections;
using System.IO;
using MultiResTester.Data;
using MultiResTester.Utility;
using UnityEngine;

namespace MultiResTester.Scripts.Runtime
{
    public class MultiResTesterRuntime : MonoBehaviour
    {
        private Action m_OnFinished;
        private MultiResScreenConfig m_ScreenConfig;

        public void Initialize(MultiResScreenConfig screenConfig, Action onFinished)
        {
            m_ScreenConfig = screenConfig;
            m_OnFinished = onFinished;
        }

        public void TryStartRecording()
        {
            StartCoroutine(CoroutineTryStartRecording());
        }

        private IEnumerator CoroutineTryStartRecording()
        {
            Debug.Log("MultiRes Testing is Started!");

            var screenshotPath = Path.Combine(Application.dataPath, "..", "MultiRes Tester",
                DateTime.Now.ToString("yy_d_M_hh_mm_ss"));
            Debug.Log($"Screenshot Path: {screenshotPath}");

            if (Directory.Exists(screenshotPath) == false)
                Directory.CreateDirectory(screenshotPath);

            foreach (var config in m_ScreenConfig.configs)
            {
                yield return CoroutineRecordSingleScreen(config, m_ScreenConfig.overlays, screenshotPath);
            }

            Debug.Log("All Done!");

            m_OnFinished?.Invoke();
            Destroy(gameObject);
            yield break;
        }

        private IEnumerator CoroutineRecordSingleScreen(SingleScreenConfig singleScreenConfig, SingleOverlayConfig[] overlays,
            string screenshotPath)
        {
            var profileName = "MultiRes Tester";
            var gameSizeView = new GameViewSizeHelper.GameViewSize()
            {
                baseText = profileName,
                height = singleScreenConfig.screenHeight,
                width = singleScreenConfig.screenWidth,
                type = GameViewSizeHelper.GameViewSizeType.FixedResolution
            };

            ChangeGameViewSize(gameSizeView);

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            var screenResult = TakeScreenshot();

            yield return new WaitForEndOfFrame();

            yield return DrawOverlays(screenResult, overlays, singleScreenConfig, (t) =>
            {
                screenResult = t;
                SaveScreenshot(screenResult, singleScreenConfig, screenshotPath);
            });

            yield return new WaitForSeconds(0.1f);

            ResetGameViewSize(gameSizeView);

            Debug.Log(
                $"Screen Recorded: {singleScreenConfig.screenName} {singleScreenConfig.screenWidth}x{singleScreenConfig.screenHeight}");
        }

        private void ChangeGameViewSize(GameViewSizeHelper.GameViewSize gameSizeView)
        {
            GameViewSizeHelper.AddCustomSize(GameViewSizeHelper.GetCurrentGroupType(), gameSizeView);
            GameViewSizeHelper.ChangeGameViewSize(GameViewSizeHelper.GetCurrentGroupType(), gameSizeView);
        }

        private Texture2D TakeScreenshot()
        {
            return ScreenCapture.CaptureScreenshotAsTexture();
        }

        private IEnumerator DrawOverlays(Texture2D inputTexture, SingleOverlayConfig[] overlays,
            SingleScreenConfig screenConfig, Action<Texture2D> onFinished)
        {
            foreach (var overlay in overlays)
            {
                if(overlay.isEnabled == false)
                    continue;
                
                var posX = 0;
                var posY = 0;
                var overlayWidth = 0;
                var overlayHeight = 0;

                if (overlay.widthSizeUnit == SingleOverlayConfig.SizeUnit.Percent)
                {
                    overlayWidth = (int)((float)overlay.overlayWidth / 100 * screenConfig.screenWidth);
                }
                else
                {
                    overlayWidth = overlay.overlayWidth;
                }


                if (overlay.heightSizeUnit == SingleOverlayConfig.SizeUnit.Percent)
                {
                    overlayHeight = (int)((float)overlay.overlayHeight / 100 * screenConfig.screenHeight);
                }
                else
                {
                    overlayHeight = overlay.overlayHeight;
                }

                switch (overlay.overlayPosition)
                {
                    case SingleOverlayConfig.OverlayPosition.TopLeft:
                    case SingleOverlayConfig.OverlayPosition.TopMiddle:
                    case SingleOverlayConfig.OverlayPosition.TopRight:
                        posY = screenConfig.screenHeight - overlayHeight;
                        break;
                    case SingleOverlayConfig.OverlayPosition.CenterLeft:
                    case SingleOverlayConfig.OverlayPosition.CenterMiddle:
                    case SingleOverlayConfig.OverlayPosition.CenterRight:
                        posY = (screenConfig.screenHeight - overlayHeight) / 2;
                        break;
                    case SingleOverlayConfig.OverlayPosition.BottomLeft:
                    case SingleOverlayConfig.OverlayPosition.BottomMiddle:
                    case SingleOverlayConfig.OverlayPosition.BottomRight:
                        posY = 0;
                        break;
                }

                switch (overlay.overlayPosition)
                {
                    case SingleOverlayConfig.OverlayPosition.TopLeft:
                    case SingleOverlayConfig.OverlayPosition.CenterLeft:
                    case SingleOverlayConfig.OverlayPosition.BottomLeft:
                        posX = 0;
                        break;
                    case SingleOverlayConfig.OverlayPosition.TopMiddle:
                    case SingleOverlayConfig.OverlayPosition.CenterMiddle:
                    case SingleOverlayConfig.OverlayPosition.BottomMiddle:
                        posX = (screenConfig.screenWidth - overlayWidth) / 2;
                        break;
                    case SingleOverlayConfig.OverlayPosition.TopRight:
                    case SingleOverlayConfig.OverlayPosition.CenterRight:
                    case SingleOverlayConfig.OverlayPosition.BottomRight:
                        posX = screenConfig.screenWidth - overlayWidth;
                        break;
                }

                TextureHelper.DrawRedRectangle(inputTexture, posX, posY, overlayWidth, overlayHeight,overlay.overlayColor);
            }

            yield return new WaitForEndOfFrame();
            onFinished?.Invoke(inputTexture);
        }

        private void SaveScreenshot(Texture2D texture, SingleScreenConfig screenConfig, string path)
        {
            var filename = Path.Combine(path, screenConfig.screenName + ".jpg");
            File.WriteAllBytes(filename, texture.EncodeToJPG(100));
        }

        private void ResetGameViewSize(GameViewSizeHelper.GameViewSize gameSizeView)
        {
            GameViewSizeHelper.RemoveCustomSize(GameViewSizeHelper.GetCurrentGroupType(), gameSizeView);
        }
    }
}