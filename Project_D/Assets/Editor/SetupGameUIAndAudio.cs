using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class SetupGameUIAndAudio : EditorWindow
{
    [MenuItem("Tools/Setup Final UI and Audio")]
    public static void Setup()
    {
        // 1. Clean up old Canvas objects
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name.Contains("Canvas") || go.name.Contains("Header") || go.name == "EventSystem")
            {
                DestroyImmediate(go);
            }
        }

        // 2. Create Event System
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        // 3. Setup Canvas
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // 4. Setup Score Text
        GameObject scoreTextGO = new GameObject("ScoreText");
        scoreTextGO.transform.SetParent(canvas.transform, false);
        TextMeshProUGUI scoreText = scoreTextGO.AddComponent<TextMeshProUGUI>();
        RectTransform scoreRect = scoreTextGO.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0, 1);
        scoreRect.anchorMax = new Vector2(0, 1);
        scoreRect.pivot = new Vector2(0, 1);
        scoreRect.anchoredPosition = new Vector2(20, -20);
        scoreRect.sizeDelta = new Vector2(400, 100);
        scoreText.text = "점수: 0";
        scoreText.fontSize = 48;
        scoreText.color = Color.white;
        scoreText.alignment = TextAlignmentOptions.TopLeft;

        // 5. Setup Start/Menu UI
        GameObject startPanel = new GameObject("StartMenu");
        startPanel.transform.SetParent(canvas.transform, false);
        RectTransform startRect = startPanel.AddComponent<RectTransform>();
        startRect.anchorMin = Vector2.zero;
        startRect.anchorMax = Vector2.one;
        startRect.offsetMin = Vector2.zero;
        startRect.offsetMax = Vector2.zero;

        Image startBg = startPanel.AddComponent<Image>();
        startBg.color = new Color(0, 0, 0, 0.8f);

        GameObject startTitle = new GameObject("Title");
        startTitle.transform.SetParent(startPanel.transform, false);
        TextMeshProUGUI titleText = startTitle.AddComponent<TextMeshProUGUI>();
        titleText.text = "TETRIS";
        titleText.fontSize = 100;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        RectTransform titleRect = startTitle.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 100);
        titleRect.sizeDelta = new Vector2(600, 150);

        GameObject startBtnGO = DefaultControls.CreateButton(new DefaultControls.Resources());
        startBtnGO.name = "StartButton";
        startBtnGO.transform.SetParent(startPanel.transform, false);
        RectTransform startBtnRect = startBtnGO.GetComponent<RectTransform>();
        startBtnRect.anchoredPosition = new Vector2(0, -50);
        startBtnRect.sizeDelta = new Vector2(250, 80);
        startBtnGO.GetComponentInChildren<Text>().text = "게임 시작";
        startBtnGO.GetComponentInChildren<Text>().fontSize = 28;
        Button startButton = startBtnGO.GetComponent<Button>();

        // 6. Setup GameOver UI
        GameObject gameOverPanel = new GameObject("GameOverMenu");
        gameOverPanel.transform.SetParent(canvas.transform, false);
        RectTransform goRect = gameOverPanel.AddComponent<RectTransform>();
        goRect.anchorMin = Vector2.zero;
        goRect.anchorMax = Vector2.one;
        goRect.offsetMin = Vector2.zero;
        goRect.offsetMax = Vector2.zero;

        Image goBg = gameOverPanel.AddComponent<Image>();
        goBg.color = new Color(0.5f, 0, 0, 0.8f);

        GameObject goTitle = new GameObject("GameOverText");
        goTitle.transform.SetParent(gameOverPanel.transform, false);
        TextMeshProUGUI goText = goTitle.AddComponent<TextMeshProUGUI>();
        goText.text = "GAME OVER";
        goText.fontSize = 80;
        goText.alignment = TextAlignmentOptions.Center;
        goText.color = Color.white;
        RectTransform goTitleRect = goTitle.GetComponent<RectTransform>();
        goTitleRect.anchoredPosition = new Vector2(0, 100);
        goTitleRect.sizeDelta = new Vector2(600, 150);

        GameObject restartBtnGO = DefaultControls.CreateButton(new DefaultControls.Resources());
        restartBtnGO.name = "RestartButton";
        restartBtnGO.transform.SetParent(gameOverPanel.transform, false);
        RectTransform restartBtnRect = restartBtnGO.GetComponent<RectTransform>();
        restartBtnRect.anchoredPosition = new Vector2(0, -50);
        restartBtnRect.sizeDelta = new Vector2(250, 80);
        restartBtnGO.GetComponentInChildren<Text>().text = "재시작";
        restartBtnGO.GetComponentInChildren<Text>().fontSize = 28;
        Button restartButton = restartBtnGO.GetComponent<Button>();

        // 7. Configure GameManager
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.scoreText = scoreText;
            gm.startUI = startPanel;
            gm.gameOverUI = gameOverPanel;

            // Link Buttons to GameManager.StartGame
            UnityEditor.Events.UnityEventTools.AddPersistentListener(startButton.onClick, gm.StartGame);
            UnityEditor.Events.UnityEventTools.AddPersistentListener(restartButton.onClick, gm.StartGame);

            // Load Audio Clips
            AudioClip bgmClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/bgm.wav");
            AudioClip clearClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/clear.wav");
            AudioClip overClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/gameover.wav");

            AudioSource[] gmSources = gm.GetComponents<AudioSource>();
            AudioSource bgmSource = gmSources.Length > 0 ? gmSources[0] : gm.gameObject.AddComponent<AudioSource>();
            AudioSource sfxSource = gmSources.Length > 1 ? gmSources[1] : gm.gameObject.AddComponent<AudioSource>();
            
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
            bgmSource.clip = bgmClip;
            
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;

            gm.bgmSource = bgmSource;
            gm.sfxSource = sfxSource;
            gm.lineClearSound = clearClip;
            gm.gameOverSound = overClip;

            EditorUtility.SetDirty(gm);
        }

        // 8. Configure Piece Audio
        Board board = FindObjectOfType<Board>();
        if (board != null)
        {
            Piece piece = board.GetComponent<Piece>();
            if (piece != null)
            {
                AudioClip moveClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/move.wav");
                AudioClip rotateClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/rotate.wav");
                AudioClip lockClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/lock.wav");

                AudioSource pieceAudio = piece.GetComponent<AudioSource>();
                if (pieceAudio == null)
                {
                    pieceAudio = piece.gameObject.AddComponent<AudioSource>();
                }
                pieceAudio.loop = false;
                pieceAudio.playOnAwake = false;

                piece.audioSource = pieceAudio;
                piece.moveSound = moveClip;
                piece.rotateSound = rotateClip;
                piece.lockSound = lockClip;

                EditorUtility.SetDirty(piece);
            }
        }

        EditorUtility.SetDirty(canvas);
        
        Debug.Log("Game UI and Audio setup complete!");
    }
}
