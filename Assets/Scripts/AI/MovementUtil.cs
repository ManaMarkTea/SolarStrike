using System;
using UnityEngine;

/// <summary>
/// Movement utilities!
/// </summary>
public class MovementUtil
{
	/// <summary>
	/// the object that we will move:
	/// <value>The control object.</value>
	public GameObject ControlObject { get; set; }
	public float MaxSpeed { get; set; }

	public MovementUtil ()
	{
		
	}
	
	public void ApplyMaxSpeed() {
		var vel = ControlObject.rigidbody.velocity;

		float speed = vel.magnitude;
		if ( speed < 0.01f) 
		{
			vel = Vector3.zero; 
		}
		else if ( speed > MaxSpeed )
		{
			vel /= speed;
			speed = MaxSpeed;
			vel *= speed;
		}

		ControlObject.rigidbody.velocity = vel;
	}
	
	public void Update() {
		ApplyMaxSpeed();
	}

	/// <summary>
	/// Moves the object in the given direction
	/// </summary>
	/// <param name="dir">Dir.</param>
	/// <param name="movementPerSecond">Movement per second.</param>
	public void ThrustersDir( Vector3 dir, float movementPerSecond)
	{		
		var vel = ControlObject.rigidbody.velocity;
		vel += dir.normalized * movementPerSecond * Time.deltaTime;
		ControlObject.rigidbody.velocity = vel;
		ApplyMaxSpeed();
	}
		
	/// <summary>
	/// Strafes the object to the side:
	/// </summary>
	/// <param name="dir">Dir.</param>
	/// <param name="movementPerSecond">Movement per second.</param>
	/// <param name="leftwards">If set to <c>true</c> leftwards.</param>
	public void ThrustersSide( Vector3 dir , float movementPerSecond, bool leftwards)
	{
		var sideways = Vector3.Cross( dir.normalized, Vector3.up) * movementPerSecond * Time.deltaTime;
		var vel = ControlObject.rigidbody.velocity;
		vel += leftwards ? sideways : -sideways;
		ControlObject.rigidbody.velocity = vel;		
		ApplyMaxSpeed();
	}

	/// <summary>
	///	Slows the object down.
	/// </summary>
	/// <param name="decelerationPerSecond">Deceleration per second.</param>
	public void SlowDown( float decelerationPerSecond)
	{
		var vel = ControlObject.rigidbody.velocity;
		float speed = vel.magnitude;		
		if ( speed < 0.01f) 
		{
			vel = Vector3.zero; 
		}
		else 
		{ 					
			vel /= speed;
			speed -= decelerationPerSecond * Time.deltaTime;
			vel *= speed;
		}
		ControlObject.rigidbody.velocity = vel;
	}

	/// <summary>
	/// Stablizes the rotation of the object using angular velocity.
	/// </summary>
	/// <param name="toTarget">To target.</param>
	public void StablizeToward( Vector3 toTarget)
	{
		float stability = 0.6f;
    	float speed = 10.0f;
		
		Vector3 predictedUp = Quaternion.AngleAxis(
            ControlObject.rigidbody.angularVelocity.magnitude * Mathf.Rad2Deg * stability / speed,
            ControlObject.rigidbody.angularVelocity
        ) * ControlObject.transform.up;
 
        Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
        ControlObject.rigidbody.AddTorque(torqueVector * speed * speed);
		

        Vector3 torqueVector2 = Vector3.Cross(ControlObject.transform.forward, toTarget.normalized);
		var theta = Mathf.Asin(torqueVector2.magnitude);
		speed *= Mathf.Abs(theta) + 0.2f;
        
		
		ControlObject.rigidbody.angularVelocity = torqueVector2 * speed;		
		ControlObject.rigidbody.maxAngularVelocity = torqueVector2.magnitude * speed;
		
	}
	
}


