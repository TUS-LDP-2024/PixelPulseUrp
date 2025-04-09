using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Source Prefab")]
    public AudioSource audioSourcePrefab; // Prefab with an AudioSource component set up for 3D sound

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

  
    /// Play a sound clip at a given position with a specified volume.
   
    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1.0f)
    {
        AudioSource audioSource = Instantiate(audioSourcePrefab, position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1.0f; // Make sure the sound is fully 3D
        audioSource.Play();
        // Destroy the audio source after the clip finishes playing
        Destroy(audioSource.gameObject, clip.length);
    }

    public AudioSource StartLoopingSound(AudioClip clip, Transform parentTransform, float volume = 1.0f)
    {
        // Instantiate as child so it follows the zombie if it moves
        AudioSource audioSource = Instantiate(audioSourcePrefab, parentTransform.position, Quaternion.identity, parentTransform);
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1.0f; // Ensure full 3D behavior
        audioSource.Play();
        return audioSource;
    }

    public void StopLoopingSound(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            Destroy(audioSource.gameObject);
        }
    }

}
