using UnityEngine;
using System.Collections;

public class LightFlicker : MonoBehaviour
{
	public ParticleSystem partilceSystem;
	public float time = 0.05f;
	
	private float timer;
	
	void Start ()
	{
		timer = time;
	}

    private void Update()
    {
        if(partilceSystem.isPlaying)
			StartCoroutine("Flicker");
	}

    IEnumerator Flicker()
	{
		while(true)
		{
			GetComponent<Light>().enabled = !GetComponent<Light>().enabled;
			
			do
			{
				timer -= Time.deltaTime;
				yield return null;
			}
			while(timer > 0);
			timer = time;
		}
	}
}
