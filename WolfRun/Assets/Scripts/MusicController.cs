using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public Vector2 waitRange = new Vector2(10.0f, 20.0f);
    public AudioSource windSource;
    public AudioSource crowSource;
    
	void Start()
    {
        windSource.Play();
	}

	void Update()
    {
        StartCoroutine(PlayOccasionalCrows());
	}

    IEnumerator PlayOccasionalCrows()
    {
        float randomWaitTime = Random.Range(waitRange.x, waitRange.y);
        yield return new WaitForSeconds(randomWaitTime);

        crowSource.Play();
    }
}
