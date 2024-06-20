/// 
/// Lucas Storm
/// June 2024
/// Bugs: Main music restarts on each scene loading -
///       - this should have been fixed by not destroying the music on scene load.
/// 
/// This script handles the sound effects played throughout the game.

using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    public Transform Camera;

    public AudioClip ShootSFX;
    public AudioClip TowerPlacedSFX;
    public AudioClip EnemyDeathSFX;
    public AudioClip LifeLostSFX;

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
    // SFXManager.Instance.PlayShootSFX();
    public void PlayShootSFX()
    {
        PlaySFX(ShootSFX);
    }

    // Play the tower placed sound effect
    public void PlayTowerPlaced()
    {
        PlaySFX(TowerPlacedSFX);
    }

    // Play the enemy death sound effect
    public void PlayEnemyDeathSFX()
    {
        PlaySFX(EnemyDeathSFX);
    }

    // Play the life lost sound effect
    public void PlayLifeLostSFX()
    {
        PlaySFX(LifeLostSFX);
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
