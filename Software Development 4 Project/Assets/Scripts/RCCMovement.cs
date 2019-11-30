using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RCCMovement : MonoBehaviour
{
	[Header("Speed")]
	[Tooltip("How fast your charater will accelerate.")]
	public float m_fSpeed = 1000f;
	[Tooltip("How fast the charater will accelerate during sprinting.")]
	public float m_fSprintSpeed = 1000f;
	[Tooltip("You can monitor how fast your character is moving through here.")]
	public float m_fVelocity;
	[Tooltip("Max speed achievable for your character.")]
	public float m_fMaxSpeed = 3f;

	[Header("Jump")]
	//jump vars
	[Tooltip("Gravity modifier for when your jump feels too floaty when coming down.")]
	public float m_fGravity = 19.614f;
	[Tooltip("How high you jump")]
	public float m_fJumpPower = 500f;



	[Header("Camera")]
	[Tooltip("The camera following the player.")]
	public Transform m_tCam;

	//the radius of the sphere for checking if player is grounded
	private readonly float m_fRadius = 1.1f;

	//speed limiter vars for the player
	private Vector3 m_fCurrSpeed;
	private float m_fCurrSpeedMag;

	//general vars
	private Rigidbody m_rbRigidbody;
	private Vector3 m_v3Inputs = Vector3.zero;

	//variables for sprint function returning it back to it's original speed
	private float m_fTempMaxSpeed;
	private float m_fTempMovementSpeed;
	// Start is called before the first frame update
	void Awake()
	{
		m_fTempMaxSpeed = m_fMaxSpeed;
		m_fTempMovementSpeed = m_fSpeed;

		if (GetComponent<Rigidbody>())
		{
			m_rbRigidbody = GetComponent<Rigidbody>();
		}
		else
		{
			Debug.LogError("Thecharacter needs a rigidbody");
		}
	}
	public bool falling = false;
	// Update is called once per frame
	void Update()
	{
		falling = GroundCheck();
		Inputs();
		Jump();
		m_fVelocity = m_rbRigidbody.velocity.magnitude;
	}

	private void FixedUpdate()
	{
		Turn();
		Move();
	}

	//checks for ground through a sphere cast at the bottom of the charater accurate for edges
	bool GroundCheck()
	{
		RaycastHit hit;
		return Physics.SphereCast(gameObject.transform.position - new Vector3(0f, -0.6f, 0f), m_fRadius, Vector3.down, out hit, 1f);
	}

	//manageing the inputs
	private void Inputs()
	{
		m_v3Inputs = Vector3.zero;
		m_v3Inputs.x = Input.GetAxis("Horizontal");
		m_v3Inputs.z = Input.GetAxis("Vertical");
	}

	//movement is applied here with the use of add relative force
	private void Move()
	{
		m_fCurrSpeed.Set(m_rbRigidbody.velocity.x, 0, m_rbRigidbody.velocity.z);

		m_fCurrSpeedMag = Vector3.Magnitude(m_fCurrSpeed);
		//sprint is done here because it relates to the basic movement of the character and some tinkering needs to be done with the speed limiter so that the player can actually go faster.
		if (Input.GetButton("Sprint"))
		{
			m_fMaxSpeed *= 2f;
			m_fSpeed = m_fSprintSpeed;
		}
		else
		{
			m_fMaxSpeed = m_fTempMaxSpeed;
			m_fSpeed = m_fTempMovementSpeed;
		}
		//the speed is also limited with in here by applying a counter force when a certain speed is reached so as to nt infinitely accelerate.
		if (m_fCurrSpeedMag > m_fMaxSpeed * m_fMaxSpeed == false && GroundCheck())
		{
			m_rbRigidbody.AddRelativeForce(Vector3.zero + m_v3Inputs * m_fSpeed * Time.fixedDeltaTime, ForceMode.Acceleration);
		}
	}

	//The jump function which adds an istant change of velocity to the character to make it jump
	//Double jump is also here allows you to jump once more after you have already jumped.
	private void Jump()
	{
		if (Input.GetButtonDown("Jump") && GroundCheck() == true)
		{
			m_rbRigidbody.AddForce(Vector3.up * m_fJumpPower * Time.fixedDeltaTime, ForceMode.VelocityChange);
		}
		
		//Manipulates gravity so that when you are falling down you dont just float around but actually fall down.
		if (m_rbRigidbody.velocity.y < 0 && GroundCheck() == false)
		{
			m_rbRigidbody.AddForce(Vector3.down * m_fGravity * Time.fixedDeltaTime, ForceMode.Acceleration);
		}
	}

	//Turns the player in relation to the rotation of the camera
	private void Turn()
	{
		this.transform.rotation = Quaternion.Euler(0, m_tCam.eulerAngles.y, 0);
	}
}
