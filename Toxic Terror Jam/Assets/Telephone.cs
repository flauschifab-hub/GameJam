using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public string targetTag = "Telephone";

    public AudioClip ringingClip;
    public AudioSource audioSource;

    private bool isRinging = false;
    private Coroutine ringRoutine;
    // Start is called before the first frame update
    void Start()
    {
        if (DialogPanel != null)
            DialogPanel.SetActive(false);

        audioSource.playOnAwake = false;
        audioSource.clip = ringingClip;
        audioSource.loop = true;
        StartCoroutine(RingingLoop());
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
                    if (DialogPanel != null) DialogPanel.SetActive(true);

                    StopRinging();
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
}
