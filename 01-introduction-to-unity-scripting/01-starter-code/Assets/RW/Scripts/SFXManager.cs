/// 
/// Author: Lucas Storm
/// June 2024
/// Bugs: None known at this time.
/// 
/// This script manages all the sound effects that play throughout the game.

using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    public Transform Camera;
    public AudioClip ShootSFX;
    public AudioClip SheepHitSFX;
    public AudioClip SheepDropSFX;

    private AudioSource audioSource;

    private void Awake()
    {
        Instance = this;

        // Get or add an AudioSource component to the GameObject
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Play the shoot sound effect
    public void PlayShootSFX()
    {
        PlaySFX(ShootSFX);
    }

    // Play the sheep hit sound effect
    public void PlaySheepHitSFX()
    {
        PlaySFX(SheepHitSFX);
    }

    // Play the sheep drop sound effect
    public void PlaySheepDropSFX()
    {
        PlaySFX(SheepDropSFX);
    }

    // Play the specified audio clip
    private void PlaySFX(AudioClip clip)
    {
        // Check if the clip and audio source are valid before playing the clip
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            // Log a warning if the audio clip or AudioSource is missing
            Debug.LogWarning("Audio clip or AudioSource is missing.");
        }
    }
}
