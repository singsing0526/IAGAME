using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    private float audioLength = 0;
    private bool isLoopable = false;

    public void SetAudioDetails(AudioClip clip, bool isLoopable, float vol)
    {
        this.isLoopable = isLoopable;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = vol;
        audioLength = audioSource.clip.length;

        if (isLoopable == true)
        {
            audioSource.loop = enabled;
            audioSource.Play();
        }
        else
        {
            StartCoroutine(WaitDestroy());
            audioSource.Play();
        }

    }

    private IEnumerator WaitDestroy()
    {
        yield return new WaitForSeconds(audioLength);
        Destroy(gameObject);
    }
}
