using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    public bool fadeOutOnStart = false;
    public bool stopOnStart = false;
    public bool playNewTrack = false;
    public AudioClip newTrack;
    public float fadeDuration = 1.5f;

    void Start()
    {
        if (MusicManager.Instance == null)
            return;
        
        if (fadeOutOnStart)
        {
            MusicManager.Instance.FadeOutMusic(fadeDuration);
        } else if (stopOnStart)
        {
            MusicManager.Instance.StopMusic();
        } else if (playNewTrack && newTrack != null)
        {
            MusicManager.Instance.FadeToNewTrack(newTrack, fadeDuration);
        }
    }
}