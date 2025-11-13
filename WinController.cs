using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class WinController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject textBackground;
    public GameObject restartButton;

    [Header("Buttons")]
    public Button returnBackgroundButton;

    [Header("Audio")]
    public AudioClip clickSound;
    public AudioSource audioSource;

    [Header("Scene Build Indices")]
    public int startMenuSceneIndex = 0; 
    public int feettSceneIndex = 1; 

    void Start()
    {
        Debug.Log("WinController Start() called");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Setup audio source
        if (audioSource == null)
        {
            Debug.Log("Creating new AudioSource");
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // Verify button assignment
        if (returnBackgroundButton != null)
        {
            returnBackgroundButton.onClick.AddListener(OnReturnBackgroundClicked);
        }
    }

    private void OnReturnBackgroundClicked()
    {

        // Play sound
        if (clickSound != null && audioSource != null)
        {
            Debug.Log("Playing click sound");
            audioSource.PlayOneShot(clickSound);
        }

        // Determine target scene INDEX
        int targetSceneIndex;
        float randomValue = Random.value;
        Debug.Log("Random value: " + randomValue);

        if (randomValue < 0.01f)
        {
            targetSceneIndex = feettSceneIndex;
        }
        else
        {
            targetSceneIndex = startMenuSceneIndex;
        }

        // Calculate delay
        float delay = 0.1f;
        if (clickSound != null)
            delay = clickSound.length;
        StartCoroutine(LoadSceneAfterDelay(targetSceneIndex, delay));
    }

    private IEnumerator LoadSceneAfterDelay(int sceneIndex, float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(sceneIndex);
    }
}