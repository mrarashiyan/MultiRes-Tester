using MultiResTester.Data;
using MultiResTester.Scripts.Runtime;
using UnityEditor;
using UnityEngine;

namespace MultiResTester.Scripts.Editor
{
#if UNITY_EDITOR
    public class MultiResRecorderWindow : EditorWindow
    {
        private MultiResScreenConfig screenConfig;
        private bool m_IsProcessStarted = false;

        [MenuItem("Window/Multi-Res Recorder")]
        public static void ShowWindow()
        {
            var window = GetWindow<MultiResRecorderWindow>("Multi-Res Recorder");
            window.minSize = new Vector2(400, 100);
            window.maxSize = new Vector2(401, 101);
        }

        private void OnGUI()
        {
            GUILayout.Label("Multi-Resolution Recorder", EditorStyles.boldLabel);

            screenConfig = (MultiResScreenConfig)EditorGUILayout.ObjectField("Screen Config", screenConfig,
                typeof(MultiResScreenConfig), false);

            GUILayout.FlexibleSpace();

            GUIStyle recordButtonStyle = new GUIStyle(GUI.skin.button);
            recordButtonStyle.fontSize = 20;
            recordButtonStyle.fixedHeight = 40;

            if (m_IsProcessStarted == false)
            {
                if (GUILayout.Button("🔴 Record", recordButtonStyle))
                {
                    StartRecording();
                }
            }
            else
            {
                GUILayout.Button("⌛ Testing In Progress...", recordButtonStyle);
            }
        }

        private void StartRecording()
        {
            if (screenConfig == null)
            {
                Debug.LogWarning("Please assign a MultiResScreenConfig before recording.");
                return;
            }

            if (Application.isPlaying == false)
            {
                EditorUtility.DisplayDialog("Error", "You have to enter to Play Mode", "Ok");
                return;
            }

            m_IsProcessStarted = true;

            var prefab = Resources.Load<MultiResTesterRuntime>(nameof(MultiResTesterRuntime));
            if(prefab == null)
                Debug.LogWarning("MultiRes Tester Prefab is null");

            var runtimeObj = Instantiate(prefab);
            runtimeObj.Initialize(screenConfig, () =>
            {
                m_IsProcessStarted = false;
            });
            runtimeObj.TryStartRecording();
        }

        
    }
#endif
}