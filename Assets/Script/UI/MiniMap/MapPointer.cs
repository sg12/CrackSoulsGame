using System.Collections;
using System.Collections.Generic;
using RPGAE.CharacterController;
using UnityEngine;

public class MapPointer : MonoBehaviour
{
    public string PlayerTag = "Player";
    public MeshRenderer m_Render;
    public bool isAudioActive;
    public AudioSource audioClip;
    private ThirdPersonController cc;
    public bool audioPlayed;
    
    private void OnEnable()
    {
        cc = FindObjectOfType<ThirdPersonController>();
    }

    void Update()
    {
        if (!audioPlayed && isAudioActive)
        {
            if (audioClip != null)
            {
                audioClip.Play();
            }
            audioPlayed = true;
           
        }
    }

    public void SetColor(Color c)
    {
        GetComponent<MiniMapItem>().IconColor = c;
        c.a = 0.25f;
        m_Render.material.SetColor("_BaseColor", c);
    }

    void OnTriggerEnter(Collider c)
    {
        if(c.tag == PlayerTag)
        {
           Destroy(gameObject);
        }
    }
}