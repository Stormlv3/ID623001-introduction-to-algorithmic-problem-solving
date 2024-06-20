/// 
/// Lucas Storm
/// June 2024
/// Bugs: None known at this time.
/// 
/// This script handles the growl sound effect for the monster.

using System.Collections;
using UnityEngine;

public class Growl : MonoBehaviour
{
    public float minDelayBetweenGrowl = 2.0f;
    public float maxDelayBetweenGrowl = 6.0f; 
    private AudioSource audioSource;

    void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();

        // Start the coroutine to loop the audio
        StartCoroutine(LoopAudio());
    }

    IEnumerator LoopAudio()
    {
        while (true)
        {
            // Play the audio
            audioSource.Play();

            // Wait for the audio to finish playing and a random delay between min and max delay
            float delay = Random.Range(minDelayBetweenGrowl, maxDelayBetweenGrowl);
            yield return new WaitForSeconds(audioSource.clip.length + delay);
        }
    }
}
