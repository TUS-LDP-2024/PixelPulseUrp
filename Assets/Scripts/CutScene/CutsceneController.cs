using UnityEngine;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    private PlayableDirector director;

    private void Start()
    {
        director = GetComponent<PlayableDirector>();
        if (director != null)
        {
            director.stopped += OnCutsceneFinished;
            director.Play(); // Just in case you want it to autoplay when spawned
        }
        else
        {
            Debug.LogWarning("CutsceneController: No PlayableDirector found on cutscene object.");
        }
    }

    private void OnCutsceneFinished(PlayableDirector obj)
    {
        // Resume the game
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.ResumeAfterCutscene();
        }

        // Clean up the cutscene object
        Destroy(gameObject);
    }
}
