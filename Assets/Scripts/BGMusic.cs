using UnityEngine;

public class BGMusic : MonoBehaviour
{
    public AudioSource bgMusic;
    public AudioClip musicClip;
    public bool loopMusic = true;

    void Start()
    {
        if (bgMusic == null)
        {
            bgMusic = gameObject.AddComponent<AudioSource>(); // Add AudioSource if not assigned
        }

        bgMusic.clip = musicClip;
        bgMusic.loop = loopMusic;
        bgMusic.playOnAwake = false;

        PlayFromRandomTime();
    }

    private void PlayFromRandomTime()
    {
        if (musicClip != null)
        {
            float randomStartTime = Random.Range(0, musicClip.length); // Get random time
            bgMusic.time = randomStartTime; // Set start time
            bgMusic.Play();
        }
        else
        {
            Debug.LogWarning("No music clip assigned!");
        }
    }
}
