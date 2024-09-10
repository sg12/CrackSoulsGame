using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTimer : MonoBehaviour
{
    public float explodeTimer;
    public GameObject particleSystem;
    public RandomAudioPlayer explosionAudio;
    private ThirdPersonCamera tpC;

    // Start is called before the first frame update
    void Start()
    {
        tpC = FindObjectOfType<ThirdPersonCamera>(); 
    }

    // Update is called once per frame
    void Update()
    {
        explodeTimer -= Time.deltaTime;
        if(explodeTimer < 1)
        {
            explosionAudio.transform.SetParent(null, true);
            if (explosionAudio)
                explosionAudio.PlayRandomClip();

            Destroy(explosionAudio, explosionAudio.clip == null ? 0.0f : explosionAudio.clip.length + 0.5f);

            tpC.CameraShake(0.8f, 0.3f, 0.5f);

            GameObject explosion = Instantiate(particleSystem, transform.position, transform.rotation) as GameObject;
            explosion.GetComponentInChildren<HitBox>().hurtID = 2;
            explosion.GetComponentInChildren<HitBox>().isAttacking = true;
            explosion.GetComponentInChildren<HitBox>().isProjectile = true;
            explosion.GetComponentInChildren<HitBox>().BeginAttack("Medium Blood Hit");

            Destroy(gameObject);
        }
    }
}
