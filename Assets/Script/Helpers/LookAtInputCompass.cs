using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGAE.CharacterController;

public class LookAtInputCompass : MonoBehaviour
{
    [Header("Get Axis Vector")]
	public float multiplier;

	public GameObject relativeTo;

	public Vector3 storeVector;

	public float storeMagnitude;
    [Header("Do Smooth Look At Direction")]
    public GameObject gameObject;

    public Vector3 targetDirection;

    public float minMagnitude;

    public Vector3 upVector;

    public bool keepVertical;

    [Tooltip("How quickly to rotate.")]
    public float speed;

    GameObject previousGo; // track game object so we can re-initialize when it changes.
    Quaternion lastRotation;
    Quaternion desiredRotation;

    private ThirdPersonController cc;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponentInParent<ThirdPersonController>();
        gameObject = GameObject.Find("LookAtInputDirection");
        if (relativeTo == null)
            relativeTo = FindObjectOfType<ThirdPersonCamera>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        targetDirection = storeVector;
        GetAxisVector();
        DoSmoothLookAtDirection();
    }

    void GetAxisVector()
    {
        var forward = new Vector3();
        var right = new Vector3();

        if (relativeTo == null)
        {
            forward = Vector3.forward;
            right = Vector3.right;
        }
        else
        {
            var transform = relativeTo.transform;

            forward = transform.TransformDirection(Vector3.forward);
            forward.y = 0;
            forward = forward.normalized;
            right = new Vector3(forward.z, 0, -forward.x);

            // Right vector relative to the object
            // Always orthogonal to the forward vector
        }

        // get individual axis
        // leaving an axis blank or set to None sets it to 0

        var h = (cc.rpgaeIM.PlayerControls.Movement.ReadValue<Vector2>().x == 0 ? 0f : cc.rpgaeIM.PlayerControls.Movement.ReadValue<Vector2>().x);
        var v = (cc.rpgaeIM.PlayerControls.Movement.ReadValue<Vector2>().y == 0 ? 0f : cc.rpgaeIM.PlayerControls.Movement.ReadValue<Vector2>().y);

        // calculate resulting direction vector
        var direction = h * right + v * forward;
        direction *= multiplier;

        storeVector = direction;

        if (storeMagnitude == 0)
        {
            storeMagnitude = direction.magnitude;
        }
    }

    void DoSmoothLookAtDirection()
    {
        var go = gameObject;
        if (go == null)
        {
            return;
        }

        // re-initialize if game object has changed

        if (previousGo != go)
        {
            lastRotation = go.transform.rotation;
            desiredRotation = lastRotation;
            previousGo = go;
        }

        // desired direction

        var diff = targetDirection;
        if (keepVertical)
        {
            diff.y = 0;
        }

        if (diff.sqrMagnitude > minMagnitude)
            desiredRotation = Quaternion.LookRotation(diff, Vector3.up);

        lastRotation = Quaternion.Slerp(lastRotation, desiredRotation, speed * Time.deltaTime);
        go.transform.rotation = lastRotation;
    }
}
