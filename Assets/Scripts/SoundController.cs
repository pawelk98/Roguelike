using System.Collections;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance { get; private set; }

    public AudioSource backgroundMusic;
    public AudioClip[] footsteps;
    public AudioClip[] meleeAttack;
    public AudioClip bowAttack;
    public AudioClip[] magicAttack;
    public AudioClip doors;
    public AudioClip coins;
    public AudioClip coinReward;
    public AudioClip itemChange;
    public AudioClip dodge;
    public AudioClip[] breakVase;
    public AudioClip takeDamage;

    IEnumerator footstepsCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    void Start()
    {
        backgroundMusic.Play();
    }

    public void PlaySound(AudioClip clip, Vector3 pos, float vol)
    {
        AudioSource.PlayClipAtPoint(clip, pos, vol);
    }

    public void PlayRandomSound(AudioClip[] clips, Vector3 pos, float vol)
    {
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        AudioSource.PlayClipAtPoint(clip, pos, vol);
    }

    public void StartFootsteps(float delay)
    {
        footstepsCoroutine = FootstepsCoroutine(delay);
        StartCoroutine(footstepsCoroutine);
    }

    public void StopFootsteps()
    {
        StopCoroutine(footstepsCoroutine);
    }

    IEnumerator FootstepsCoroutine(float delay)
    {
        while(true)
        {
            AudioSource.PlayClipAtPoint(footsteps[1], transform.position, 0.1f);
            yield return new WaitForSeconds(delay);
        }
    }
}
