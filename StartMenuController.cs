using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class StartMenuController : MonoBehaviour
{

    // settings
    [Header("UI Panels")]
    public GameObject titlesPanel;
    public GameObject controlsPanel;

    [Header("Scene Settings")]
    public string gameSceneName = "feett";

    [Header("Font")]
    public Font customFont; // Gotham font in Inspector


    [Header("Audio")]
    public AudioClip clickSound; // assign in inspector
    private AudioSource audioSource; // sound

    private Canvas loadingCanvas;
    private GameObject loadingPanel;
    private Slider loadingBar;
    private Text loadingText;

    void Start()
    {
        ShowTitlesPanel();

        // Set up audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }


    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            // plays sound
            audioSource.PlayOneShot(clickSound);
        }
    }


    // loads game
    public void OnPlayButton()
    {
        PlayClickSound();
        StartCoroutine(LoadGameScene());
    }

    // show controls
    public void OnInstructionsButton()
    {
        PlayClickSound();
        ShowControlsPanel();
    }

    // show title
    public void OnControlsBackButton()
    {
        PlayClickSound();
        ShowTitlesPanel();
    }

    private void ShowTitlesPanel()
    {
        if (titlesPanel != null) titlesPanel.SetActive(true);
        if (controlsPanel != null) controlsPanel.SetActive(false);
    }

    private void ShowControlsPanel()
    {
        if (titlesPanel != null) titlesPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(true);
    }

    private IEnumerator LoadGameScene()
    {
        // Create loading screen
        CreateLoadingScreen();

        // Small delay so loading screen shows up
        yield return new WaitForSeconds(0.1f);

        // Start loading
        AsyncOperation operation = SceneManager.LoadSceneAsync(gameSceneName);
        operation.allowSceneActivation = false;

        // Update progress
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (loadingBar != null)
                loadingBar.value = progress;

            if (loadingText != null)
                loadingText.text = $"Loading... {Mathf.RoundToInt(progress * 100)}%";

            // activate the scene when done
            if (operation.progress >= 0.9f)
            {
                if (loadingText != null)
                    loadingText.text = "Ready!";

                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    // creates the loading bar + text
    private void CreateLoadingScreen()
    {
        // Create canvas
        GameObject canvasObj = new GameObject("LoadingCanvas");
        loadingCanvas = canvasObj.AddComponent<Canvas>();
        loadingCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        loadingCanvas.sortingOrder = 9999;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        // Black background panel
        loadingPanel = new GameObject("Panel");
        loadingPanel.transform.SetParent(canvasObj.transform, false);

        Image panelImg = loadingPanel.AddComponent<Image>();
        panelImg.color = Color.black;

        RectTransform panelRect = loadingPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;

        // Loading text
        GameObject textObj = new GameObject("LoadingText");
        textObj.transform.SetParent(loadingPanel.transform, false);

        loadingText = textObj.AddComponent<Text>();
        loadingText.text = "Loading... 0%";
        loadingText.font = customFont != null ? customFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        loadingText.fontSize = 40;
        loadingText.alignment = TextAnchor.MiddleCenter;
        loadingText.color = Color.white;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0.6f);
        textRect.anchorMax = new Vector2(1, 0.7f);
        textRect.sizeDelta = Vector2.zero;

        // Create slider using Unity's built-in prefab structure
        GameObject sliderObj = CreateSlider();
        sliderObj.transform.SetParent(loadingPanel.transform, false);

        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.25f, 0.45f);
        sliderRect.anchorMax = new Vector2(0.75f, 0.5f);
        sliderRect.sizeDelta = Vector2.zero;

        loadingBar = sliderObj.GetComponent<Slider>();
    }

    private GameObject CreateSlider()
    {
        // Main slider object
        GameObject slider = new GameObject("LoadingSlider");
        Slider sliderComponent = slider.AddComponent<Slider>();

        RectTransform sliderRect = slider.GetComponent<RectTransform>();
        sliderRect.sizeDelta = new Vector2(160, 20);

        // Background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(slider.transform, false);

        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f);

        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Fill area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(slider.transform, false);

        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.sizeDelta = new Vector2(-10, -10);

        // Fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);

        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0, 0.8f, 0);

        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;

        // Configure slider
        sliderComponent.fillRect = fillRect;
        sliderComponent.targetGraphic = fillImage;
        sliderComponent.minValue = 0;
        sliderComponent.maxValue = 1;
        sliderComponent.value = 0;
        sliderComponent.interactable = false;

        return slider;
    }
}