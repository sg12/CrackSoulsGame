using UnityEngine;
using System.Collections;

public class Destroy : MonoBehaviour {

	public float destoryDelay;
	public FunctionType functionType;

	private Light lights;

    void Start()
    {
		lights = GetComponentInChildren<Light>();
    }

    private void Update()
	{
		switch (functionType)
		{
			case FunctionType.Destroy:
				Destroy(gameObject, destoryDelay);
				break;
			case FunctionType.Deactivate:
				StartCoroutine(Deactivate());
				break;
			case FunctionType.StopParticle:
				StartCoroutine(Deactivate());
				break;
		}
	}

	public IEnumerator Deactivate()
	{
		yield return new WaitForSeconds(destoryDelay);

		switch (functionType)
		{
			case FunctionType.Destroy:
				Destroy(gameObject, destoryDelay);
				break;
			case FunctionType.Deactivate:
				gameObject.SetActive(false);
				break;
			case FunctionType.StopParticle:
				GetComponent<ParticleSystem>().Stop();
				if (lights != null)
					lights.enabled = false;
				Destroy(gameObject, 2);
				break;
		}
	}

	public enum FunctionType
	{
		Destroy,
		Deactivate,
		StopParticle
	}
}
