using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System;
using System.Text;

public class SpeechRecognition : MonoBehaviour, ISpeechToTextListener
{
    public TMP_Text resultText;
    public TMP_Text numberInLetterText;
    public GameObject[] numberPrefabs;
    public Transform numberSpawnPoint;
    public AudioClip cheeringSound;
    public AudioClip sadSound;
    public AudioSource audioSource;
    public Transform resultSpawnPoint;
    public GameObject[] resultSpawnPointPrefabs;

    private int currentNumber = 1;
    private GameObject currentNumberInstance;
    public GameObject mouse;
    private GameObject currentResultInstance;

    private readonly string[] numberInWords = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten" };

    void Start()
    {
        // Initialize SpeechToText plugin with default language
       Debug.Log("Initializing SpeechToText...");
        SpeechToText.Initialize("en-US");
        Debug.Log("Initialization completed.");

        // Check for a microphone
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone found!");
        }
        else
        {
            Debug.Log("Microphone found: " + Microphone.devices[0]);
        }

        DisplayNextNumber();
    }

    public void ToggleRecording()
    {
        if (SpeechToText.IsBusy())
        {
            // Stop the ongoing recognition session
            SpeechToText.ForceStop();
            Debug.Log("Stopped recording.");
        }
        else
        {
            // Start a new speech recognition session
            SpeechToText.Start(this, useFreeFormLanguageModel: true, preferOfflineRecognition: false);
            Debug.Log("Started recording.");
        }
    }

    // ISpeechToTextListener method implementations
    public void OnReadyForSpeech()
    {
        Debug.Log("Ready for speech.");
    }

    public void OnBeginningOfSpeech()
    {
        Debug.Log("Speech started.");
    }

    public void OnVoiceLevelChanged(float normalizedVoiceLevel)
    {
        // You can add feedback for the voice level if needed
    }

    public void OnPartialResultReceived(string spokenText)
    {
        Debug.Log("Partial result: " + spokenText);
    }

    public void OnResultReceived(string spokenText, int? errorCode)
    {
        if (errorCode == null)
        {
            Debug.Log("Final recognized text: " + spokenText);
            ProcessRecognizedText(spokenText);
        }
        else
        {
            Debug.LogError("Speech recognition error: " + errorCode);
        }
    }

    private void ProcessRecognizedText(string recognizedText)
    {
        Debug.Log("Processing recognized text: " + recognizedText);
        recognizedText = "zero";
        // Check if the recognized text matches the current number or word
        if (recognizedText.Contains(currentNumber.ToString()) || recognizedText.ToLower().Contains(numberInWords[currentNumber - 1].ToLower()))
        {
            PlayFeedback(true);
            currentNumber++;
            DisplayNextNumber();
        }
        else
        {
            PlayFeedback(false);
        }
    }

    private void PlayFeedback(bool isCorrect)
    {
        if (isCorrect)
        {
            audioSource.clip = cheeringSound;
            DisplayResult(isCorrect);

            if (mouse != null)
            {
                Animator animator = mouse.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animation/ClappingController");
                    Debug.Log("Clapping animation played.");
                }
            }
        }
        else
        {
            audioSource.clip = sadSound;
            DisplayResult(isCorrect);

            if (mouse != null)
            {
                Animator animator = mouse.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animation/SadController");
                    Debug.Log("Sad animation played.");
                }
            }
        }
        audioSource.Play();
    }

    private void DisplayNextNumber()
    {
        // Destroy the previous number instance if it exists
        if (currentNumberInstance != null)
        {
            Destroy(currentNumberInstance);
        }

        // Check if the current number has a corresponding prefab
        if (currentNumber - 1 < numberPrefabs.Length && currentNumber - 1 < numberInWords.Length)
        {
            // Instantiate the next number prefab
            currentNumberInstance = Instantiate(numberPrefabs[currentNumber - 1], numberSpawnPoint.position, Quaternion.identity);
            currentNumberInstance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            numberInLetterText.text = numberInWords[currentNumber - 1];
        }
    }

    private void DisplayResult(bool result)
    {
        // Destroy the previous result instance if it exists
        if (currentResultInstance != null)
        {
            Destroy(currentResultInstance);
        }

        // Check if the result is correct and display the corresponding prefab
        currentResultInstance = Instantiate(result ? resultSpawnPointPrefabs[0] : resultSpawnPointPrefabs[1], resultSpawnPoint.position, Quaternion.identity);
        currentResultInstance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        // Start the coroutine to destroy the result object after 3 seconds
        StartCoroutine(DestroyResultAfterDelay(currentResultInstance, 3f));
    }

    private IEnumerator DestroyResultAfterDelay(GameObject resultObject, float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        Destroy(resultObject); // Destroy the result object

        if (mouse != null)
        {
            Animator animator = mouse.GetComponent<Animator>();
            if (animator != null)
            {
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animation/MouseBreathing");
                Debug.Log("Breathing animation played.");
            }
        }
    }
}
