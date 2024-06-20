/// 
/// Author: Lucas Storm
/// June 2024
/// Bugs: None known at this time.
/// 
/// This script manages the music that plays throughout the game.

using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip soundClip; // Public sound clip
    private AudioSource audioSource;

    private static MusicManager instance;

    void Awake()
    {
        // Ensure that only one instance of the SoundManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Don't destroy this GameObject on scene change
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Get or add an AudioSource component to the GameObject
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.volume = 0.1f;

        // Play the sound clip
        PlaySound();
    }

    public void PlaySound()
    {
        if (soundClip != null)
        {
            audioSource.clip = soundClip;
            audioSource.Play();
        }
    }
}
