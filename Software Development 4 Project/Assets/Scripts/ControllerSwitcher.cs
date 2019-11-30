using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSwitcher : MonoBehaviour
{
	RCCMovement m_rbMovement;
	UCCMovement m_ccMovement;

	CharacterController m_ccController;
	Rigidbody m_rbRigidbody;


	Renderer m_mMaterialSwitch;
	// Start is called before the first frame update
	void Start()
    {
		//gets the components that are attached to the Object.
		m_ccController = GetComponent<CharacterController>();
		m_rbRigidbody = GetComponent<Rigidbody>();
		m_rbMovement = GetComponent<RCCMovement>();
		m_ccMovement = GetComponent<UCCMovement>();
		m_mMaterialSwitch = GetComponent<Renderer>();
		m_rbRigidbody.isKinematic = false;
		m_ccController.enabled = false;
		m_ccMovement.enabled = false;
	}

    // Update is called once per frame
    void Update()
    {
		//switches between the two types of controllers
        if(Input.GetKeyDown("f5"))
		{
			m_mMaterialSwitch.material.SetColor("_Color", Color.blue);
			m_rbRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			m_rbRigidbody.isKinematic = true;
			m_ccController.enabled = true;
			m_ccMovement.enabled = true;
			m_rbMovement.enabled = false;
		}
		if (Input.GetKeyDown("f6"))
		{
			m_mMaterialSwitch.material.SetColor("_Color", Color.red);
			m_rbRigidbody.isKinematic = false;
			m_rbRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			m_ccController.enabled = false;
			m_ccMovement.enabled = false;
			m_rbMovement.enabled = true;
		}
	}
}
