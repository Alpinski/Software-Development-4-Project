using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
	[Header("Settings")]
	[Tooltip("Decide if you want to lock cursor to the middle of screen and hide it.")]
	public bool m_bLockCursor;
	[Tooltip("How fast you move the camera with input.")]
	public float m_fMovementSensitivity = 10;
	[Tooltip("The target to follow normally the player or an object on the player.")]
	public Transform m_tTarget;
	[Tooltip("The distance of the camera from the target.")]
	public float m_fDistFromTarget = 2;
	[Tooltip("Limiter to your pitch. ")]
	public Vector2 m_fPitchMinMax = new Vector2(-40, 85);

	[Tooltip("How long smoothing takes.")]
	public float rotationSmoothTime = 8f;
	Vector3 currentRotation;

	private float m_fYaw;
	private float m_fPitch;

	[Header("Transparency")]
	[Tooltip("If you want your charaters transperency to fade or not.(Only works if you set your materials rendering mode to fade.)")]
	public bool m_bChangeTransparency = true;
	[Tooltip("Put the mesh renderer of your charater on here.")]
	public MeshRenderer m_mrTargetRenderer;

	[Header("Speeds")]
	[Tooltip("How fast the camera will zoom in to the player once you are in a tight space.")]
	public float m_fMoveSpeed = 5;
	[Tooltip("How fast the camera will return to its original position.")]
	public float m_fReturnSpeed = 9;
	[Tooltip("How far out the camera will be pushed by the wall once it collides with it.")]
	public float m_fWallPush = 0.7f;

	[Header("Distances")]
	public float m_fClosestDistanceToPlayer = 2;
	public float m_fEvenCloserDistanceToPlayer = 1;

	[Header("Mask")]
	public LayerMask m_lmCollisionMask;

	private bool m_bPitchLock = false;

	private void Start()
	{
		//locks camera to the front of the screen
		if (m_bLockCursor)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	private void LateUpdate()
	{

		CollisionCheck(m_tTarget.position - transform.forward * m_fDistFromTarget);
		WallCheck();

		if (!m_bPitchLock)
		{
			//General camera input
			m_fYaw += Input.GetAxis("Mouse X") * m_fMovementSensitivity;
			m_fPitch -= Input.GetAxis("Mouse Y") * m_fMovementSensitivity;

			m_fPitch = Mathf.Clamp(m_fPitch, m_fPitchMinMax.x, m_fPitchMinMax.y);

			//actual moving of the camera
			currentRotation = Vector3.Lerp(currentRotation, new Vector3(m_fPitch, m_fYaw), rotationSmoothTime * Time.deltaTime);
		}
		else
		{
			m_fYaw += Input.GetAxis("Mouse X") * m_fMovementSensitivity;
			m_fPitch = m_fPitchMinMax.y;

			currentRotation = Vector3.Lerp(currentRotation, new Vector3(m_fPitch, m_fYaw), rotationSmoothTime * Time.deltaTime);
		}


		transform.eulerAngles = currentRotation;

		Vector3 e = transform.eulerAngles;
		e.x = 0;

		m_tTarget.eulerAngles = e;
	}

	//checks if a wall is close to the player
	private void WallCheck()
	{
		Ray ray = new Ray(m_tTarget.position, -m_tTarget.forward);
		RaycastHit hit;

		if (Physics.SphereCast(ray, 0.2f, out hit, 0.7f, m_lmCollisionMask))
		{
			m_bPitchLock = true;
		}
		else
		{
			m_bPitchLock = false;
		}
	}

	//checks if the player has collided with the use of linecast
	private void CollisionCheck(Vector3 retPoint)
	{
		RaycastHit hit;

		//moves the camera to an appropriate position so that camera doesnt go into the obstructing object
		if (Physics.Linecast(m_tTarget.position, retPoint, out hit, m_lmCollisionMask))
		{
			Vector3 norm = hit.normal * m_fWallPush;
			Vector3 p = hit.point + norm;

			TransparencyCheck();

			if (Vector3.Distance(Vector3.Lerp(transform.position, p, m_fMoveSpeed * Time.deltaTime), m_tTarget.position) <= m_fEvenCloserDistanceToPlayer)
			{}
			else
			{
				transform.position = Vector3.Lerp(transform.position, p, m_fMoveSpeed * Time.deltaTime);
			}
			return;
		}

		FullTransparency();
		//camera goes back to it's original spot
		transform.position = Vector3.Lerp(transform.position, retPoint, m_fReturnSpeed * Time.deltaTime);
		m_bPitchLock = false;
	}

	//Checks if the character should be made transperent or not, so that the player can actually see throught the character
	private void TransparencyCheck()
	{

		if (m_bChangeTransparency)
		{
			//takes distance into account when making the charater transperent so the closer you are to the player the more transperent it is
			if (Vector3.Distance(transform.position, m_tTarget.position) <= m_fClosestDistanceToPlayer)
			{

				Color temp = m_mrTargetRenderer.sharedMaterial.color;
				temp.a = Mathf.Lerp(temp.a, 0.2f, m_fMoveSpeed * Time.deltaTime);

				m_mrTargetRenderer.sharedMaterial.color = temp;
			}
			else
			{
				if (m_mrTargetRenderer.sharedMaterial.color.a <= 0.99f)
				{

					Color temp = m_mrTargetRenderer.sharedMaterial.color;
					temp.a = Mathf.Lerp(temp.a, 1, m_fMoveSpeed * Time.deltaTime);

					m_mrTargetRenderer.sharedMaterial.color = temp;

				}
			}
		}
	}

	//makes the player transperent
	private void FullTransparency()
	{
		if (m_bChangeTransparency)
		{
			if (m_mrTargetRenderer.sharedMaterial.color.a <= 0.99f)
			{
				Color temp = m_mrTargetRenderer.sharedMaterial.color;
				temp.a = Mathf.Lerp(temp.a, 1, m_fMoveSpeed * Time.deltaTime);

				m_mrTargetRenderer.sharedMaterial.color = temp;
			}
		}
	}
}
