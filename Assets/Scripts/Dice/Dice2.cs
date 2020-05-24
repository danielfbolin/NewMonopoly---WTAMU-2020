using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Dice2 : MonoBehaviourPun {


	public Rigidbody rb;

	Vector3 initPosition;
    Vector3 fallPosition;
    Quaternion initRotation;

	public int diceValue;

	public DiceSide[] diceSides;

    public bool hasLanded;
	public bool thrown;
    private float timeDelay = 0.0f;


    void Awake() {
		
	}


	void Start()
	{
		rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        initPosition = new Vector3(-7.0f, 0.23f, -6.0f);
        fallPosition = new Vector3(-5.5f, 3.0f, -4.0f);
        initRotation = rb.rotation;
        hasLanded = false;
        //Debug.Log("Dice2 in spawn position");
    }


    void Update()
    {
        if (timeDelay > 1.0f)
        {
            if (Mathf.Abs(rb.velocity.x) < .0005 && Mathf.Abs(rb.velocity.y) < .0005 && Mathf.Abs(rb.velocity.z) < 0.0005)
                hasLanded = true;
        }
        else if (thrown)
        {
            timeDelay += Time.deltaTime;
        }
    }


    void FixedUpdate()
    {

    }


    public void RollDice()
	{
        Quaternion rotation = Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f));
        int torque_x = Random.Range(0, 500);
        int torque_y = Random.Range(0, 500);
        int torque_z = Random.Range(0, 500);
        base.photonView.RPC("RPC_RollDice", RpcTarget.All, fallPosition, rotation, torque_x, torque_y, torque_z);
        //Debug.Log("Dice2 thrown");					
	}


    [PunRPC]
    private void RPC_RollDice(Vector3 position, Quaternion rotation, int x, int y, int z)
    {
        transform.position = position;
        transform.rotation = rotation;
        rb.AddTorque(x, y, z);
        thrown = true;
    }


	public void reset()
	{
        base.photonView.RPC("RPC_reset", RpcTarget.All);
	}


    [PunRPC]
    private void RPC_reset()
    {
        thrown = false;
        hasLanded = false;
        timeDelay = 0.0f;
        rb.velocity = new Vector3(0f, 0f, 0f);
        //Debug.Log("Dice 2 reset");
    }

	
	public void SideValueCheck()
	{
		diceValue = 0;
		foreach (DiceSide side in diceSides)
		{
			if (side.OnGround())
			{
				diceValue = side.sideValue;
                //Debug.Log("Dice 2 value: " + diceValue);
			}
		}
	}
}
