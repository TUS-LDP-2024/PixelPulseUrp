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
            director.Play();
        }
    }

    private void OnCutsceneFinished(PlayableDirector obj)
    {
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.ResumeAfterCutscene();
        }

        Destroy(gameObject);
    }
}
