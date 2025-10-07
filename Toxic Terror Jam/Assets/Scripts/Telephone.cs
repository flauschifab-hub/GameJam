using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

[System.Serializable]
public class PhoneResponseSet
{
    [TextArea(2, 5)] public string playerLeftAnswer;
    [TextArea(2, 5)] public string playerRightAnswer;
    [TextArea(2, 5)] public string leftReaction;
    [TextArea(2, 5)] public string rightReaction;

    public int leftRelationshipChange;
    public int rightRelationshipChange;
}

[System.Serializable]
public class PhoneCharacter
{
    public string characterName;
    [TextArea(2, 5)] public string[] possibleResponses;
    public PhoneResponseSet[] responseSets;

    [HideInInspector] public int currentResponseIndex = 0;
    [HideInInspector] public int relationship = 0;
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

    public GameObject leftAnswerPanel;
    public GameObject rightAnswerPanel;
    public TMP_Text leftOptionText;
    public TMP_Text rightOptionText;

    public string targetTag = "Telephone";

    public AudioClip ringingClip;
    public AudioSource audioSource;

    private bool isRinging = false;

    [Header("Characer")]
    public PhoneCharacter[] allCharacters;
    public int maxUniqueCharactersPerRun = 5;
    private List<PhoneCharacter> activeCharacters = new List<PhoneCharacter>();
    private PhoneCharacter currentCaller;

    [Header("WriteEffectSettings")]
    public float typingSpeed = 0.1f;
    private Coroutine typingCoroutine;

    [Header("Select Anim Settings")]
    public float selectedScale = 1.2f;
    public float deselectedScale = 1.0f;
    public float scaleDuration = 0.25f;
    public float fadeDuration = 0.4f;
    public float confirmHoldTime = 1.0f;
    public Image confirmFillImage;

    private bool inCall = false;
    private bool choosingOption = false;

    private int currentSelection = -1;
    private Vector3 initialLeftScale;
    private Vector3 initialRightScale;
    private Coroutine scaleCoroutine;
    private Coroutine confirmCoroutine;
    private Coroutine fadeCoroutine;

    private CanvasGroup leftGroup;
    private CanvasGroup rightGroup;
    // Start is called before the first frame update
    void Start()
    {
        if (DialogPanel != null)
            DialogPanel.SetActive(false);
        leftAnswerPanel.SetActive(false);
        rightAnswerPanel.SetActive(false);

        //Cache
        leftGroup = leftAnswerPanel.GetComponent<CanvasGroup>();
        rightGroup = rightAnswerPanel.GetComponent<CanvasGroup>();

        if (leftGroup == null) leftGroup = leftAnswerPanel.AddComponent<CanvasGroup>();
        if (rightGroup == null) rightGroup = rightAnswerPanel.AddComponent<CanvasGroup>();

        leftGroup.alpha = 0;
        rightGroup.alpha = 0;


        audioSource.playOnAwake = false;
        audioSource.clip = ringingClip;
        audioSource.loop = true;

        initialLeftScale = leftAnswerPanel.transform.localScale;
        initialRightScale = rightAnswerPanel.transform.localScale;

        if (confirmFillImage != null)
        {
            confirmFillImage.fillAmount = 0;
            confirmFillImage.gameObject.SetActive(false);
        }

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
        int count = Mathf.Min(maxUniqueCharactersPerRun, allCharacters.Length);
        for (int i = 0; i < count; i++)
        {
            int index = UnityEngine.Random.Range(0, pool.Count);
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
                Transform parent = hit.collider.transform.parent;
                if (hit.collider.CompareTag(targetTag) || parent != null && parent.CompareTag(targetTag))
                {
                    Debug.Log("PlayerAnsweredPhone");
                    AnswerPhone();
                }
            }
        }

        if (!choosingOption) return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            SetSelection(-1);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            SetSelection(1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (confirmCoroutine == null)
                confirmCoroutine = StartCoroutine(ConfirmingChoice());
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            if (confirmCoroutine != null)
            {
                StopCoroutine(confirmCoroutine);
                confirmCoroutine = null;
                ResetConfirmUI();
            }
        }

    }

    void SetSelection(int newSelection)
    {
        if (newSelection == currentSelection) return;
        currentSelection = newSelection;

        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);
        

        scaleCoroutine = StartCoroutine(AnimateSelectionScales());
    }

    IEnumerator AnimateSelectionScales()
    {
        Vector3 leftStart = leftAnswerPanel.transform.localScale;
        Vector3 rightStart = rightAnswerPanel.transform.localScale;

        Vector3 leftTarget = (currentSelection == -1) ? initialLeftScale * selectedScale : initialLeftScale;
        Vector3 rightTarget = (currentSelection == 1) ? initialRightScale * selectedScale : initialRightScale;

        float elapsedTime = 0f;
        while (elapsedTime < scaleDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / scaleDuration);

            leftAnswerPanel.transform.localScale = Vector3.Lerp(leftStart, leftTarget, t);
            rightAnswerPanel.transform.localScale = Vector3.Lerp(rightStart, rightTarget, t);

            yield return null;
        }
    }

    IEnumerator ConfirmingChoice()
    {
        if (confirmFillImage != null)
        {
            confirmFillImage.gameObject.SetActive(true);
        }

        float timer = 0f;
        while (timer < confirmHoldTime)
        {
            timer += Time.deltaTime;

            if (!Input.GetKey(KeyCode.Space))
            {
                ResetConfirmUI();
                confirmCoroutine = null;
                yield break;
            }
            if (confirmFillImage != null)
                confirmFillImage.fillAmount = timer / confirmHoldTime;
            yield return null;
        }
        choosingOption = false;
        ChooseOption(currentSelection);
        confirmCoroutine = null;
    }

    void ResetConfirmUI()
    {
        if (confirmFillImage != null)
        {
            confirmFillImage.fillAmount = 0;
            confirmFillImage.gameObject.SetActive(false);
        }
    }

    IEnumerator RingingLoop()
    {
        while (true)
        {
            float delay = UnityEngine.Random.Range(minRingDelay, maxRingDelay);
            yield return new WaitForSeconds(delay);

            StartRinging();

            float duration = UnityEngine.Random.Range(minRingDuration, maxRingDuration);
            yield return new WaitForSeconds(duration);

            StopRinging();
        }
    }


    void StartRinging()
    {
        if (isRinging || inCall) return;
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
        if (inCall) return;
        inCall = true;
        StopRinging();
        if (DialogPanel != null)
            DialogPanel.SetActive(true);

        if (DialogText != null && activeCharacters.Count > 0)
        {
           currentCaller = activeCharacters[UnityEngine.Random.Range(0, activeCharacters.Count)];

            string response = currentCaller.possibleResponses[currentCaller.currentResponseIndex];
            string finalLine = $"{currentCaller.characterName}: {response}";

            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeText(finalLine, () => ShowAnswerOptions()));

        }
    }

    void ShowAnswerOptions()
    {
        int index = currentCaller.currentResponseIndex;
        var set = currentCaller.responseSets[index];

        leftOptionText.text = set.playerLeftAnswer;
        rightOptionText.text = set.playerRightAnswer;

        leftAnswerPanel.SetActive(true);
        rightAnswerPanel.SetActive(true);
        choosingOption = true;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadePanels(1f));

        SetSelection(-1);
    }

    IEnumerator FadePanels(float targetAlpha)
    {
        float startAlphaLeft = leftGroup.alpha;
        float startAlphaRight = rightGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            leftGroup.alpha = Mathf.Lerp(startAlphaLeft, targetAlpha, t);
            rightGroup.alpha = Mathf.Lerp(startAlphaRight, targetAlpha, t);
            yield return null;
        }

        leftGroup.alpha = targetAlpha;
        rightGroup.alpha = targetAlpha;

        if (targetAlpha == 0f)
        {
            leftAnswerPanel.SetActive(false);
            rightAnswerPanel.SetActive(false);
        }
    }

    void ChooseOption(int direction)
    {
        choosingOption = false;
        if (confirmCoroutine != null)
        {
            StopCoroutine(confirmCoroutine);
            confirmCoroutine = null;
        }
        ResetConfirmUI();

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadePanels(0f));

        int index = currentCaller.currentResponseIndex;
        var set = currentCaller.responseSets[index];

        string reaction = "";
        int change = 0;

        if (direction == -1)
        {
            reaction = set.leftReaction;
            change = set.leftRelationshipChange;
            currentCaller.relationship += change;
        }
        else
        {
            reaction = set.rightReaction;
            change = set.rightRelationshipChange;
            currentCaller.relationship += change;
        }

        string sign = change >= 0 ? "+" : "";
        //Debug Log beneath me buggy
        Debug.Log($"Relationship with {currentCaller.characterName}: {sign}{change} (Total = {currentCaller.relationship})");
        currentCaller.currentResponseIndex =
        (currentCaller.currentResponseIndex + 1) % currentCaller.possibleResponses.Length;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText($"{currentCaller.characterName}: {reaction}", () =>
        {
            StartCoroutine(EndCallAfterDelay(2f));
        }
        ));
    }

    IEnumerator EndCallAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndCall();
    }

    public void EndCall()
    {
        inCall = false;
        if (DialogPanel != null)
            DialogPanel.SetActive(false);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        DialogText.text = "";
        Debug.Log("Call ended");
    }

    IEnumerator TypeText(string textToType, System.Action onComplete = null)
    {
        DialogText.text = "";
        foreach (char letter in textToType.ToCharArray())
        {
            DialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        onComplete?.Invoke();
    }
}
