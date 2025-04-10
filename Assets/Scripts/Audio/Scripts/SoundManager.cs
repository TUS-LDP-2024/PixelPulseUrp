using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Source Prefab")]
    public AudioSource audioSourcePrefab; // A prefab with an AudioSource component set for 3D sound

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    
    /// Plays a one-shot sound at a given position.
    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1.0f)
    {
        AudioSource audioSource = Instantiate(audioSourcePrefab, position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1.0f; // Fully 3D
        audioSource.Play();
        Destroy(audioSource.gameObject, clip.length);
    }

    /// Starts a looping sound attached to a parent (for example, a moving zombie).
    /// Returns the AudioSource so it can be stopped later.
    public AudioSource StartLoopingSound(AudioClip clip, Transform parentTransform, float volume = 1.0f)
    {
        AudioSource audioSource = Instantiate(audioSourcePrefab, parentTransform.position, Quaternion.identity, parentTransform);
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1.0f;
        audioSource.Play();
        return audioSource;
    }

    /// Stops a looping sound and cleans up the audio source.
    public void StopLoopingSound(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            Destroy(audioSource.gameObject);
        }
    }
}
