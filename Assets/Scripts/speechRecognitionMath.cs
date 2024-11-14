using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;


public class SpeechRecognitionMath : MonoBehaviour
{
    public TMP_Text equationText;
    public GameObject[] answerObjects; // Array of game objects (fruits/vegetables) to represent answers
    public GameObject[] secondNumberObjects;
    public GameObject[] symbolsObjects;
    public GameObject[] numberObjects;
    public Transform resultSpawnPoint;
    public Transform firstNumberSpawnPoint;
    public Transform correctanswerSpawnPoint;
    public Transform mathSymbol;
    public Transform equalSymbol;
    public Transform secondNumber;
    public GameObject proposition1;
    public GameObject proposition2;
    public GameObject proposition3;
    public AudioClip cheeringSound;
    public AudioClip sadSound;
    public AudioSource audioSource;
    public GameObject mouse;

    private string deviceName;
    private AudioClip recording;
    private bool isRecording = false;
    private int correctAnswer;
    private GameObject currentAnswerInstance;
    private int fruit;
    private Vector3 smallScale = new Vector3(0.2f, 0.2f, 0.2f);

    void Start()
    {
        Debug.Log("Initializing SpeechToText...");
        SpeechToText.Initialize("en-US");
        Debug.Log("Initialization completed.");
        if (Microphone.devices.Length > 0)
        {
            deviceName = Microphone.devices[0];
        }
        else
        {
            Debug.LogError("No microphone found!");
        }
        GenerateNewEquation();
        

}

public void ToggleRecording()
    {
        if (isRecording)
        {
            StopRecording();
            Debug.Log("stop recording");
        }
        else
        {
            StartRecording();
            Debug.Log("start recording");
        }
    }

    private void StartRecording()
    {
        recording = Microphone.Start(deviceName, true, 10, 44100);
        isRecording = true;
    }

    private void StopRecording()
    {
        Microphone.End(deviceName);
        isRecording = false;
        string filePath = Path.Combine(Application.persistentDataPath, "Recording.wav");

        // Call the SavWav.Save method to save the recording
        SavWav.Save(filePath, recording);

        Debug.Log("Recording saved at: " + filePath);
        StartCoroutine(ProcessRecording());
    }

    private IEnumerator ProcessRecording()
    {
        yield return new WaitForSeconds(1); // Simulate processing delay

        // Call the speech-to-text API with the recording data
        yield return StartCoroutine(SendToSpeechAPI());
    }

    private IEnumerator SendToSpeechAPI()
    {
        // Placeholder: Replace with actual API request and response processing
        string recognizedText = "1"; // Simulated recognized text for testing

        int recognizedNumber;
        if (int.TryParse(recognizedText, out recognizedNumber))
        {
            if (recognizedNumber == correctAnswer)
            {
                PlayFeedback(true);
                GenerateNewEquation();
            }
            else
            {
                PlayFeedback(false);
            }
        }
        else
        {
           Debug.Log("Please say a number!");
        }

        yield return null;
    }

    private void PlayFeedback(bool isCorrect)
    {
        if (isCorrect)
        {
            audioSource.clip = cheeringSound;
            //resultText.text = "Correct!";
            // Instantiate placeholder objects for the missing number (correctAnswer) - visually represent the "blank" space
            for (int i = 0; i < correctAnswer; i++)
            {
                GameObject obj = Instantiate(answerObjects[fruit], mathSymbol.position + new Vector3((i + 1), 0, 0), Quaternion.identity);
                obj.transform.localScale = smallScale;
            }
            if (mouse != null)
            {
                Animator animator = mouse.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animation/ClappingController");
                }
            }
        }
        else
        {
            audioSource.clip = sadSound;
            //resultText.text = "Try again!";
            if (mouse != null)
            {
                Animator animator = mouse.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animation/SadController");
                }
            }
        }
        audioSource.Play();
        StartCoroutine(PlayBreathingAnimationAfterDelay(3f));
    }
    private IEnumerator PlayBreathingAnimationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay

        // After the delay, play mouse breathing animation
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

    private void GenerateNewEquation()
    {
        // Destroy the previous answer instance if it exists
        if (currentAnswerInstance != null)
        {
            Destroy(currentAnswerInstance);
        }

        fruit = Random.Range(0, answerObjects.Length);

        // Generate random equation
        int firstNumber = Random.Range(1, 10); // Randomize first number
        correctAnswer = Random.Range(0, 10 - firstNumber); // Calculate the correct answer
        int result = firstNumber + correctAnswer; // Final result to display

        int wrongAnswer1, wrongAnswer2;
        do
        {
            wrongAnswer1 = Random.Range(0, 10);
        } while (wrongAnswer1 == correctAnswer); 

        do
        {
            wrongAnswer2 = Random.Range(0, 10);
        } while (wrongAnswer2 == correctAnswer || wrongAnswer2 == wrongAnswer1);

        List<int> answers = new List<int> { correctAnswer, wrongAnswer1, wrongAnswer2 };
        List<GameObject> answerBoxes = new List<GameObject> { proposition1, proposition2, proposition3 };

        ClearChildren(proposition1.transform);
        ClearChildren(proposition2.transform);
        ClearChildren(proposition3.transform);

        for (int i = 0; i < answers.Count; i++)
        {
            int randomIndex = Random.Range(0, answerBoxes.Count);
            GameObject answerBox = answerBoxes[randomIndex];

            // Display the answer in the selected box
            DisplayNumberInBox(answers[i], answerBox.transform);

            // Remove the assigned box to avoid reuse
            answerBoxes.RemoveAt(randomIndex);
        }

        // Clear current answer objects from the screen
        ClearChildren(resultSpawnPoint);
        ClearChildren(firstNumberSpawnPoint);
        ClearChildren(secondNumber);
        ClearChildren(mathSymbol);
        ClearChildren(equalSymbol);


        // Set smaller spacing between the main elements
        float spacing = 1.0f; // Adjust this value to control distance between elements
        float screenCenterX = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0, mathSymbol.position.z)).x;

        // Position for the first number (first set of objects)
        Vector3 firstNumberPosition = new Vector3(screenCenterX - spacing * 2, mathSymbol.position.y, mathSymbol.position.z);
        DisplayInGrid(firstNumber, answerObjects[fruit], firstNumberPosition, smallScale);

        // Position for the "+" symbol
        Vector3 plusSymbolPosition = new Vector3(screenCenterX - spacing, mathSymbol.position.y, mathSymbol.position.z);
        GameObject plusSymbolObj = Instantiate(symbolsObjects[0], plusSymbolPosition, Quaternion.identity);
        plusSymbolObj.transform.localScale = smallScale;

        // Position for the "?" (missing number placeholder)
        Vector3 questionMarkPosition = new Vector3(screenCenterX, mathSymbol.position.y, mathSymbol.position.z);
        GameObject questionMarkObj = Instantiate(secondNumberObjects[0], questionMarkPosition, Quaternion.identity); // Placeholder object for "?"
        questionMarkObj.transform.localScale = smallScale;

        // Position for the "=" symbol
        Vector3 equalSymbolPosition = new Vector3(screenCenterX + spacing, mathSymbol.position.y, mathSymbol.position.z);
        GameObject equalSymbolObj = Instantiate(symbolsObjects[1], equalSymbolPosition, Quaternion.identity);
        equalSymbolObj.transform.localScale = smallScale;

        // Position for the result (second set of objects)
        Vector3 resultPosition = new Vector3(screenCenterX + spacing * 2, mathSymbol.position.y, mathSymbol.position.z);
        DisplayInGrid(result, answerObjects[fruit], resultPosition, smallScale);

        // Display equation with objects instead of numbers
        equationText.text = "choose the correct number!";
    }

    // Helper method to clear children
    private void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject != null)
            {
                Destroy(child.gameObject);
            }

        }
    }

    private void DisplayInGrid(int count, GameObject objectPrefab, Vector3 startPosition, Vector3 scale)
    {
        int maxPerRow = 2; // Maximum number of objects per row
        float spacing = 0.5f; // Adjust the spacing between objects
        int row = 0;

        for (int i = 0; i < count; i++)
        {
            int column = i % maxPerRow;
            if (i > 0 && column == 0)
            {
                row++; // Move to the next row after maxPerRow elements
            }

            Vector3 position = startPosition + new Vector3(column * spacing, -row * spacing, 0);
            GameObject obj = Instantiate(objectPrefab, position, Quaternion.identity);
            obj.transform.localScale = scale;
        }
    }

    private void DisplayNumberInBox(int number, Transform boxTransform)
    {
       
        GameObject numberObject = Instantiate(numberObjects[number], boxTransform);

        numberObject.transform.localPosition = Vector3.zero; 

        numberObject.transform.localPosition += new Vector3(0, 0, -0.1f);

        numberObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
       
        numberObject.transform.localRotation = Quaternion.identity; 
    }


}
