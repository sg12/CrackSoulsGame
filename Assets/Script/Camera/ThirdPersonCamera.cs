using System.Collections;
using System.Collections.Generic;
using RPGAE.CharacterController;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
	[Header("CAMERA SETTINGS")]
	public Transform followTarget;
	public CameraOffSet cameraOffSets;
	public float targetFOV;                                   
	[Range(-90, 90)]
	public float maxVerticalAngle = 60f;
	[Range(-90, 90)]
	public float minVerticalAngle = -90f;

	[Header("LOCK ON")]
	[SerializeField] private float m_lockOnLossTime = 1;
	[SerializeField] private float m_lockOnDistance = 15f;
	[SerializeField] private LayerMask m_LockOnLayers;
	[SerializeField] private LayerMask lockOnObstacles;

	[Header("CAMERA SHAKE")]
	[SerializeField] private float cameraShakePower;
	[SerializeField] private float cameraShakeduration;
	[SerializeField] private float slowDownAmount;
	[SerializeField] private bool shakeCam;

	[Header("REFERENCES")]
	public CanvasGroup[] fadeUIOnCinematicCamera;

	#region Private 

	[HideInInspector] public float angleH = 0;
	[HideInInspector] public float angleV = 0;
	private float m_lockOnLossTimeCurrent;
	[HideInInspector] public Camera cam;
	private Vector3 smoothPivotOffset;                            
	private Vector3 smoothCamOffset;                               
	private Vector3 targetPivotOffset;                       
	private Vector3 targetCamOffset;                                  
	private float defaultFOV;                                        
	private float targetMaxVerticalAngle;                          

	private Quaternion camYRotation;
	private Quaternion aimRotation;

	private Vector3 m_pivotOffset;
	private Vector3 m_camOffset;

	private Vector3 boundCenter;
	public List<Transform> targetsInCameraBound;
	public CameraState cameraState = CameraState.Orbit;

	private bool m_lockedOn;
	private TargetableInterface m_lockedOnTarget;
	public bool lockedOn { get => m_lockedOn; }
	public TargetableInterface lockedOnTarget { get => m_lockedOnTarget; }

	[HideInInspector] public Transform lastCrossHairPosition;

	private float defaultCameraShakeDuration;
	[HideInInspector] public bool cameraLocked;

	[HideInInspector] public Transform newCamPos;
	[HideInInspector] public Transform newCamLookAt;
	public float resetCameraStateTimer;
	public bool cameraInProcess;

	private ThirdPersonController cc;
	private InventoryManager inventoryM;
	private SystemManager systemM;
	private HUDManager hudM;

	[System.Serializable]
	public class CameraOffSet
	{
		public Vector3 pivotOffSet;
		public Vector3 camOffSet;
		public Vector3 lockTargetPivotOffSet;
		public Vector3 lockTargetOffSet;
		public Vector3 aimPivotOffSet;
		public Vector3 aimOffSet;
		public float smooth = 10f;
	}

	public enum CameraState
	{
		LookAt, Tween, Orbit
	};

	#endregion

    void Awake()
	{
		// Set Camera
		cam = GetComponent<Camera>();

		// Add Player transform into camera bound
		targetsInCameraBound.Clear();
		AddTarget(followTarget);

		defaultCameraShakeDuration = cameraShakeduration;

		// Set camera default position.
		cam.transform.position = boundCenter + Quaternion.identity * m_pivotOffset + Quaternion.identity * m_camOffset;
		cam.transform.rotation = Quaternion.identity;

		// Set up references and default values.
		smoothPivotOffset = m_pivotOffset;
		smoothCamOffset = m_camOffset;

		defaultFOV = cam.GetComponent<Camera>().fieldOfView;

		if (followTarget != null)
			angleH = followTarget.eulerAngles.y;

		cameraState = CameraState.Orbit;

		cc = FindObjectOfType<ThirdPersonController>();
		inventoryM = FindObjectOfType<InventoryManager>();
		systemM = FindObjectOfType<SystemManager>();
		hudM = FindObjectOfType<HUDManager>();

		ResetTargetOffsets();
		ResetFOV();
		ResetMaxVerticalAngle();
	}

	void Update()
	{
		ShakeCamera();
		CameraTween();
		CameraLookAt();
		UpdateCameraOffSet();
	}


	public void CameraMovement(float _angleH, float _angleV)
    {
		if (cameraState == CameraState.Tween || cameraState == CameraState.LookAt)
			return;

		bool conditions = !inventoryM.isPauseMenuOn && systemM.loadingScreenFUI.canvasGroup.alpha == 0 && 
	    systemM.blackScreenFUI.canvasGroup.alpha == 0;

		if (!cameraLocked && conditions)
		{
			if(cc.controllerType == ThirdPersonInput.ControllerType.MOUSEKEYBOARD)
            {
				angleH += _angleH * (systemM.cameraSensitivity) * (systemM.invertCameraH ? -1 : 1) * Time.deltaTime;
				angleV += _angleV * (systemM.cameraSensitivity) * (systemM.invertCameraV ? -1 : 1) * Time.deltaTime;
			}
			else
			{
				angleH += _angleH * (systemM.cameraSensitivity * 10) * (systemM.invertCameraH ? -1 : 1) * Time.deltaTime;
				angleV += _angleV * (systemM.cameraSensitivity * 10) * (systemM.invertCameraV ? -1 : 1) * Time.deltaTime;
			}
		}

		if (m_lockedOn && m_lockedOnTarget.targetTransform)
		{
			Vector3 targetForward = m_lockedOnTarget.targetTransform.position - cam.transform.position;
			targetForward.y = 0.0f;
			targetForward.Normalize();

			Vector3 aimRotationToTaget = Vector3.ProjectOnPlane(targetForward.normalized, Vector3.up);
			Quaternion lookDirection = Quaternion.LookRotation(targetForward.normalized, Vector3.up);

			angleH = lookDirection.eulerAngles.y;
			angleV = lookDirection.eulerAngles.x;

			camYRotation = Quaternion.Euler(aimRotationToTaget.normalized);
			aimRotation = Quaternion.LookRotation(aimRotationToTaget.normalized);
			cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, aimRotation, cameraOffSets.smooth * Time.deltaTime);
		}
		else
		{
			angleV = Mathf.Clamp(angleV, minVerticalAngle, targetMaxVerticalAngle);

			camYRotation = Quaternion.Euler(0, angleH, 0);
			aimRotation = Quaternion.Euler(-angleV, angleH, 0);
			cam.transform.rotation = aimRotation;
		}

		// Set FOV.
		cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(cam.GetComponent<Camera>().fieldOfView, targetFOV, Time.deltaTime);

		// Test for collision with the environment based on current camera position.
		Vector3 baseTempPosition = boundCenter + camYRotation * targetPivotOffset;
		Vector3 noCollisionOffset = targetCamOffset;
		while (noCollisionOffset.magnitude >= 0.2f)
		{
			if (DoubleViewingPosCheck(baseTempPosition + aimRotation * noCollisionOffset))
				break;
			noCollisionOffset -= noCollisionOffset.normalized * 0.2f;
		}

		// Encapsulate targets within the camera bound
		Bounds bounds = new Bounds(targetsInCameraBound[0].transform.position, Vector3.zero);
		for (int i = 0; i < targetsInCameraBound.Count; i++)
		{
			bounds.Encapsulate(targetsInCameraBound[i].transform.position);
		}
		boundCenter = bounds.center;

		if (m_lockedOn && lockedOnTarget != null)
		{
			bool valid = m_lockedOnTarget.Targetable &&
				InDistance(m_lockedOnTarget) &&
				InScreen(m_lockedOnTarget) &&
				NotBlocked(m_lockedOnTarget);
			if (valid)
				m_lockOnLossTimeCurrent = 0;
			else
				m_lockOnLossTimeCurrent = Mathf.Clamp(m_lockOnLossTimeCurrent + Time.deltaTime, 0, m_lockOnLossTime);
			if (m_lockOnLossTimeCurrent == m_lockOnLossTime)
            {
				m_lockedOn = false;
				cc.isAiming = false;
				cc.isStrafing = false;
			}
		}

		// Repostition the camera.
		smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, targetPivotOffset, cc.slowDown ? cameraOffSets.smooth * Time.deltaTime : 40 * Time.deltaTime);
		smoothCamOffset = Vector3.Lerp(smoothCamOffset, noCollisionOffset, cc.slowDown ? cameraOffSets.smooth * Time.deltaTime : 40 * Time.deltaTime);

		cam.transform.position = boundCenter + camYRotation * smoothPivotOffset + aimRotation * smoothCamOffset;
	}

	void UpdateCameraOffSet()
	{
		targetPivotOffset = m_pivotOffset;
		targetCamOffset = m_camOffset;

		if(m_lockedOn && m_lockedOnTarget.targetTransform)
        {
			m_pivotOffset = cameraOffSets.lockTargetPivotOffSet;
			m_camOffset = cameraOffSets.lockTargetOffSet;
		}
		else
        {
            if (cc.isAiming)
            {
				m_pivotOffset = cameraOffSets.aimPivotOffSet;
				m_camOffset = cameraOffSets.aimOffSet;
			}
			else
            {
				m_pivotOffset = cameraOffSets.pivotOffSet;
				m_camOffset = cameraOffSets.camOffSet;
			}
		}
	}

	// Add a target within the camera's bounds.
	public void AddTarget(Transform target)
	{
		targetsInCameraBound.Add(target);
	}

	// Remove addtional targets from the bounds except for player.
	public void RemoveTargets()
	{
		for (int i = targetsInCameraBound.Count - 1; i > 0; i--)
		{
			targetsInCameraBound.Remove(targetsInCameraBound[i]);
		}
	}

	public void SetCinematicCamera(CameraState CameraState, Transform NewCamPos, Transform NewCamLookAt, float CameraStayTime)
	{
        if (!cameraInProcess)
        {
			cameraState = CameraState;
			newCamPos = NewCamPos;
			newCamLookAt = NewCamLookAt;
			resetCameraStateTimer = CameraStayTime;

			cameraInProcess = true;
        }
	}

	void CameraLookAt()
	{
		if (cameraState == CameraState.Tween || newCamPos == null || newCamLookAt == null)
			return;

		if ((transform.position - newCamPos.position).magnitude < 0.1f)
		{
			if (resetCameraStateTimer > 0)
				StartCoroutine(ResetCameraState(resetCameraStateTimer));

			return;
		}

		FadeUIOnCinematicCamera(0);

		cam.transform.position = newCamPos.position;
		Vector3 relativePos = newCamLookAt.position - transform.position;
		Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
		transform.rotation = rotation;
	}

	void CameraTween()
	{
		if (cameraState == CameraState.LookAt || newCamPos == null || newCamLookAt == null)
			return;

		if ((transform.position - newCamPos.position).magnitude < 0.1f)
        {
			if (resetCameraStateTimer > 0)
				StartCoroutine(ResetCameraState(resetCameraStateTimer));

			return;
        }

		FadeUIOnCinematicCamera(0);

		cam.transform.position = Vector3.Lerp(transform.position, newCamPos.position, 1.3f * Time.deltaTime);
		Quaternion rotation = Quaternion.LookRotation(newCamLookAt.position - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2);
	}

	IEnumerator ResetCameraState(float delay)
    {
		//inventoryM.FreezeWorld(false);
		yield return new WaitForSeconds(delay);
		FadeUIOnCinematicCamera(1);
		inventoryM.FreezeWorld(false);
		cameraState = CameraState.Orbit;
		cameraInProcess = false;
		resetCameraStateTimer = 0;
		newCamPos = null;
		newCamLookAt = null;
	}

	public void CameraShake(float _power, float _duration, float _slowDownAmount)
	{
		cameraShakePower = _power;
		cameraShakeduration = _duration;
		slowDownAmount = _slowDownAmount;
		shakeCam = true;
	}

	void ShakeCamera()
	{
		if (shakeCam)
		{
			Vector3 shakePos = boundCenter + camYRotation * smoothPivotOffset + aimRotation * smoothCamOffset;
			if (cameraShakeduration > 0)
			{
				cam.transform.localPosition = shakePos + Random.insideUnitSphere * cameraShakePower;
				cameraShakeduration -= Time.deltaTime * slowDownAmount;
			}
			else
			{
				shakeCam = false;
				cameraShakeduration = defaultCameraShakeDuration;
				cam.transform.localPosition = shakePos;
			}
		}
	}

	public void FadeUIOnCinematicCamera(int num)
	{
		for (int i = 0; i < fadeUIOnCinematicCamera.Length; i++)
		{
			if (fadeUIOnCinematicCamera[i] != null)
            {
				fadeUIOnCinematicCamera[i].alpha = num;
			}
		}
	}

    #region LockOn------------------------------------------------------------------------------------------------------------------------

    public void ToggleLockOn(bool toggle)
    {
		if (toggle == m_lockedOn)
			return;

		m_lockedOn = !m_lockedOn;

		if (m_lockedOn)
        {
			List<TargetableInterface> targetables = new List<TargetableInterface>();
			Collider[] m_colliders = Physics.OverlapSphere(transform.position, m_lockOnDistance, m_LockOnLayers);
			foreach(Collider m_collder in m_colliders)
            {
				TargetableInterface targetable = m_collder.GetComponent<TargetableInterface>();
                if (targetable != null)
                {
                    if (targetable.Targetable)
                    {
						if (InScreen(targetable))
                        {
							if (NotBlocked(targetable))
                            {
								targetables.Add(targetable);
							}
                        }
                    }
                }
            }

			float hypotenuse;
			float smallesthypotenuse = Mathf.Infinity;
			TargetableInterface closestTargetable = null;
			foreach(TargetableInterface targetable in targetables)
            {
				hypotenuse = CalculateHypotenuse(targetable.targetTransform.position);
				if(smallesthypotenuse > hypotenuse)
                {
					closestTargetable = targetable;
					smallesthypotenuse = hypotenuse;
				}
            }

			m_lockedOnTarget = closestTargetable;
			m_lockedOn = closestTargetable != null;
		}
    }

    bool InDistance(TargetableInterface targetable)
    {
		float distance = Vector3.Distance(cc.transform.position, targetable.targetTransform.position);
		return distance <= m_lockOnDistance;
    }

	bool InScreen(TargetableInterface targetable)
    {
		Vector3 viewportPosition = cam.WorldToViewportPoint(targetable.targetTransform.position);

		if (!(viewportPosition.x > 0) || !(viewportPosition.x < 1)) { return false; }
		if (!(viewportPosition.y > 0) || !(viewportPosition.y < 1)) { return false; }
		if (!(viewportPosition.z > 0)) { return false; }

		return true;
	}

	bool NotBlocked (TargetableInterface targetable)
    {
		Vector3 origin = cam.transform.position;
		Vector3 direction = targetable.targetTransform.position - origin;

		float radius = 0.15f;
		float distance = direction.magnitude;

		if (Physics.SphereCast(origin, radius, direction, out RaycastHit hit, distance, lockOnObstacles))
		{
			return false;
		}
		return true;
	}

	float CalculateHypotenuse(Vector3 position)
    {
		float screenCenterX = cam.pixelWidth / 2;
		float screenCenterY = cam.pixelHeight / 2;

		Vector3 screenPosition = cam.WorldToScreenPoint(position);
		float xDelta = screenCenterX - screenPosition.x;
		float yDelta = screenCenterY - screenPosition.y;
		float hypotenuse = Mathf.Sqrt(Mathf.Pow(xDelta, 2) + Mathf.Pow(yDelta, 2));

		return hypotenuse;
    }

    #endregion

    // Set camera offsets to custom values.
    public void SetTargetOffsets(Vector3 newPivotOffset, Vector3 newCamOffset)
	{
		targetPivotOffset = newPivotOffset;
		targetCamOffset = newCamOffset;
	}

	// Reset camera offsets to default values.
	public void ResetTargetOffsets()
	{
		targetPivotOffset = m_pivotOffset;
		targetCamOffset = m_camOffset;
	}

	// Reset the camera vertical offset.
	public void ResetYCamOffset()
	{
		targetCamOffset.y = m_camOffset.y;
	}

	// Set camera vertical offset.
	public void SetYCamOffset(float y)
	{
		targetCamOffset.y = y;
	}

	// Set camera horizontal offset.
	public void SetXCamOffset(float x)
	{
		targetCamOffset.x = x;
	}

	// Set custom Field of View.
	public void SetFOV(float customFOV)
	{
		this.targetFOV = customFOV;
	}

	// Reset Field of View to default value.
	public void ResetFOV()
	{
		this.targetFOV = defaultFOV;
	}

	// Set max vertical camera rotation angle.
	public void SetMaxVerticalAngle(float angle)
	{
		this.targetMaxVerticalAngle = angle;
	}

	// Reset max vertical camera rotation angle to default value.
	public void ResetMaxVerticalAngle()
	{
		this.targetMaxVerticalAngle = maxVerticalAngle;
	}

	// Double check for collisions: concave objects doesn't detect hit from outside, so cast in both directions.
	bool DoubleViewingPosCheck(Vector3 checkPos)
	{
		return ViewingPosCheck(checkPos) && ReverseViewingPosCheck(checkPos);
	}

	// Check for collision from camera to player.
	bool ViewingPosCheck(Vector3 checkPos)
	{
		// Cast target and direction.
		Vector3 target = followTarget.position + m_pivotOffset;
		Vector3 direction = target - checkPos;
		// If a raycast from the check position to the player hits something...
		if (Physics.SphereCast(checkPos, 0.2f, direction, out RaycastHit hit, direction.magnitude))
		{
			bool ignoreThis = hit.transform.GetComponent<Breakable>() || hit.transform.GetComponent<AIController>() ||
		    hit.transform.GetComponentInChildren<Collider>().isTrigger;
			// ... if it is not the player...
			if (hit.transform != followTarget && !ignoreThis)
			{
				// This position isn't appropriate.
				return false;
			}
		}
		// If we haven't hit anything or we've hit the player, this is an appropriate position.
		return true;
	}

	// Check for collision from player to camera.
	bool ReverseViewingPosCheck(Vector3 checkPos)
	{
		// Cast origin and direction.
		Vector3 origin = followTarget.position + m_pivotOffset;
		Vector3 direction = checkPos - origin;
		if (Physics.SphereCast(origin, 0.2f, direction, out RaycastHit hit, direction.magnitude))
		{
			bool ignoreThis = hit.transform.GetComponent<Breakable>() || hit.transform.GetComponent<AIController>() ||
			hit.transform.GetComponentInChildren<Collider>().isTrigger;
			if (hit.transform != followTarget && hit.transform != transform && !ignoreThis)
			{
				return false;
			}
		}
		return true;
	}

	// Get camera magnitude.
	public float GetCurrentPivotMagnitude(Vector3 finalPivotOffset)
	{
		return Mathf.Abs((finalPivotOffset - smoothPivotOffset).magnitude);
	}
}
