using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DeathScreenController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject textBackground;
    public GameObject restartButton;

    [Header("Scene Settings")]
    public string gameSceneName = "feett";


    [Header("Audio")]
    public AudioClip clickSound; // assign in inspector
    private AudioSource audioSource; // sound

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        Button btn = restartButton.GetComponentInChildren<Button>();
        if (btn != null)
            btn.onClick.AddListener(OnRespawnButtonClicked);
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            // plays sound
            audioSource.PlayOneShot(clickSound);
        }
    }

    private void OnRespawnButtonClicked()
    {
        PlayClickSound();
        StartCoroutine(LoadGameScene());
    }

    private IEnumerator LoadGameScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(gameSceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
