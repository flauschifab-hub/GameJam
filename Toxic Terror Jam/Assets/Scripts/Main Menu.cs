using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip musicClip;         // Deine Musikdatei (z. B. MP3 oder WAV)
    public float volume = 0.5f;         // Lautstärke (0.0 - 1.0)
    public bool loop = true;            // Soll die Musik endlos wiederholt werden?

    private static MainMenuMusic instance; // Für Singleton-Logik
    private AudioSource audioSource;

    void Awake()
    {
        // Prüfe, ob es schon eine Instanz gibt → keine doppelte Musik
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Bleibt beim Szenenwechsel bestehen

        // AudioSource automatisch hinzufügen
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.playOnAwake = false;

        // Musik starten
        if (musicClip != null)
            audioSource.Play();
        else
            Debug.LogWarning("[MainMenuMusic] Kein Musikclip zugewiesen!");
    }
}
