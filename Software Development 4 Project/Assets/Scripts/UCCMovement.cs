using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UCCMovement : MonoBehaviour
{
	[Header("Speed")]
	[Tooltip("Normal movement speed.")]
	public float m_fWalkSpeed;
	[Tooltip("Speed you move at while sprinting")]
	public float m_fRunSpeed;
	[Tooltip("How quickly you build up to set speed")]
	public float m_fRunBuildUpSpeed;

	//final movement speed
	private float m_fMovementSpeed;
	[Header("Slopes")]
	[Tooltip("How much you want to push into the slope.")]
	public float m_fSlopeForce;
	[Tooltip("Range to detect the slope you are on.")]
	public float m_fSlopeForceRayLength;

	private CharacterController m_ccCharController;
	[Header("Jump")]
	[Tooltip("Smoothes out the jump curve.")]
	public AnimationCurve m_acJumpFallOff;
	[Tooltip("How high the jump will be.")]
	public float m_fJumpMultiplier;

	//Bool to check if charcter is jumping
	private bool m_bIsJumping;

	[Header("Camera")]
	[Tooltip("The camera following the player.")]
	public Transform m_tCam;
	private void Awake()
	{
		m_ccCharController = GetComponent<CharacterController>();
	}

	private void Update()
	{
		PlayerMovement();
		Turn();
	}

	private void PlayerMovement()
	{
		//gets player inputs
		float horizInput = Input.GetAxis("Horizontal");
		float vertInput = Input.GetAxis("Vertical");

		//Calculation to move the character
		Vector3 forwardMovement = transform.forward * vertInput;
		Vector3 rightMovement = transform.right * horizInput;
		//The moving of the charcter
		m_ccCharController.SimpleMove(Vector3.ClampMagnitude(forwardMovement + rightMovement, 1.0f) * m_fMovementSpeed);

		if ((vertInput != 0 || horizInput != 0) && OnSlope())
			m_ccCharController.Move(Vector3.down * m_ccCharController.height / 2 * m_fSlopeForce * Time.deltaTime);


		SetMovementSpeed();
		JumpInput();
	}

	//Sets how fast the player is moving normal speed or sprint speed.
	private void SetMovementSpeed()
	{
		if (Input.GetButton("Sprint"))
			m_fMovementSpeed = Mathf.Lerp(m_fMovementSpeed, m_fRunSpeed, Time.deltaTime * m_fRunBuildUpSpeed);
		else
			m_fMovementSpeed = Mathf.Lerp(m_fMovementSpeed, m_fWalkSpeed, Time.deltaTime * m_fRunBuildUpSpeed);
	}

	//Raycast to check if the player is on a slope.
	private bool OnSlope()
	{
		if (m_bIsJumping)
			return false;

		RaycastHit hit;

		if (Physics.Raycast(transform.position, Vector3.down, out hit, m_ccCharController.height / 2 * m_fSlopeForceRayLength))
			if (hit.normal != Vector3.up)
			{
				print("OnSlope");
				return true;
			}

		return false;
	}

	//Checks if the jump button has been pressed and starts the coroutine for the jump.
	private void JumpInput()
	{
		if (Input.GetButtonDown("Jump") && !m_bIsJumping)
		{
			m_bIsJumping = true;
			StartCoroutine(JumpEvent());
		}
	}

	// Where the actual jump happens. 
	private IEnumerator JumpEvent()
	{
		m_ccCharController.slopeLimit = 90.0f;
		float timeInAir = 0.0f;
		do
		{
			//Smoothes out the jump by using an animation curve.
			float jumpForce = m_acJumpFallOff.Evaluate(timeInAir);
			//Moves charcter in an arc, this is what makes the jump happen
			m_ccCharController.Move(Vector3.up * jumpForce * m_fJumpMultiplier * Time.deltaTime);
			timeInAir += Time.deltaTime;
			yield return null;
			//stops the jump if player hits a ceiling.
		} while (!m_ccCharController.isGrounded && m_ccCharController.collisionFlags != CollisionFlags.Above);

		m_ccCharController.slopeLimit = 45.0f;
		m_bIsJumping = false;
	}

	//Turns the player in relation to where the camera is facing.
	private void Turn()
	{
		this.transform.rotation = Quaternion.Euler(0, m_tCam.eulerAngles.y, 0);
	}
}
