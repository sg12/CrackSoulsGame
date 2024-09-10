using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoMessageTrigger : MonoBehaviour
{
    [TextArea(3, 10)]
    public string message;
    private InfoMessage infoMessage;

    // Start is called before the first frame update
    void Start()
    {
        infoMessage = FindObjectOfType<InfoMessage>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            infoMessage.info.text = message;
        }
    }
}
