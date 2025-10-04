using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[System.Serializable]
public class PhoneCharacter
{
    public string characterName;
    [TextArea(2, 5)]
    public string[] possibleResponses;
}

public class Telephone : MonoBehaviour
{
    [Header("Settings")]
    public float minRingDelay = 10f;
    public float maxRingDelay = 15f;
    public float minRingDuration = 5f;
    public float maxRingDuration = 8f;
    public float RaycastRange = 5f;

    [Header("Refrences")]
    public Camera MainCamera;
    public GameObject DialogPanel;

    public TMP_Text DialogText;
    public string targetTag = "Telephone";

    public AudioClip ringingClip;
    public AudioSource audioSource;

    private bool isRinging = false;
    private Coroutine ringRoutine;

    [Header("Characer")]
    public PhoneCharacter[] allCharacters;
    public int maxUniqueCharacters = 5;
    private List<PhoneCharacter> activeCharacters = new List<PhoneCharacter>();

    [Header("WriteEffectSettings")]
    public float typingSpeed = 0.1f;
    private Coroutine typingCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        if (DialogPanel != null)
            DialogPanel.SetActive(false);

        audioSource.playOnAwake = false;
        audioSource.clip = ringingClip;
        audioSource.loop = true;
        SetupCharacters();
        StartCoroutine(RingingLoop());
    }

    void SetupCharacters()
    {
        activeCharacters.Clear();

        if (allCharacters == null || allCharacters.Length == 0)
        {
            Debug.LogError("NoCharacters");
            return;
        }
        List<PhoneCharacter> pool = new List<PhoneCharacter>(allCharacters);
        for (int i = 0; i < Mathf.Min(maxUniqueCharacters, pool.Count); i++)
        {
            int index = Random.Range(0, pool.Count);
            activeCharacters.Add(pool[index]);
            pool.RemoveAt(index);
        }

        Debug.Log("Caller pool created with" + activeCharacters.Count + "characters");
    }

    // Update is called once per frame
    void Update()
    {
        if (isRinging && Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(MainCamera.transform.position, MainCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, RaycastRange))
            {
                if (hit.collider.CompareTag(targetTag) || hit.collider.transform.parent.CompareTag(targetTag))
                {
                    Debug.Log("PlayerAnsweredPhone");
                    AnswerPhone();
                }
            }
        }
    }

    IEnumerator RingingLoop()
    {
        while (true)
        {
            float delay = Random.Range(minRingDelay, maxRingDelay);
            yield return new WaitForSeconds(delay);

            StartRinging();

            float duration = Random.Range(minRingDuration, maxRingDuration);
            yield return new WaitForSeconds(duration);

            StopRinging();
        }
    }


    void StartRinging()
    {
        if (isRinging) return;
        isRinging = true;
        Debug.Log("Ringing");

        if (audioSource != null && ringingClip != null)
            audioSource.Play();
    }

    void StopRinging()
    {
        if (!isRinging) return;
        isRinging = false;
        Debug.Log("Stoppe ringing");

        if (audioSource != null && ringingClip != null)
            audioSource.Stop();
    }

    void AnswerPhone()
    {
        StopRinging();
        if (DialogPanel != null)
            DialogPanel.SetActive(true);

        if (DialogText != null && activeCharacters.Count > 0)
        {
            PhoneCharacter caller = activeCharacters[Random.Range(0, activeCharacters.Count)];

            string response = caller.possibleResponses[Random.Range(0, caller.possibleResponses.Length)];
            string finalLine = $"{caller.characterName}: {response}";

            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeText(finalLine));

        }
    }

    IEnumerator TypeText(string textToType)
    {
        DialogText.text = "";
        foreach (char letter in textToType.ToCharArray())
        {
            DialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
