/*
* Original C++ code from "Game AI By Example" translated to C# 
* for use with Unity Game Engine by Sam Arutyunyan
*/
using UnityEngine;
using System.Collections;
//base class from which all moving agents are derived

public class MovingEntity : MonoBehaviour 
{
    protected Transform myTransform;
    protected Vector3 velocity = Vector3.zero;
    public float mass = 1;//the higehr the mass, the harder it is to steer
    public float maxSpeed = 10;//the max speed at which this entity can travel
    public float maxForce;//max force this entity produces to power itself
    public float maxTurnRate;//radians per second that this entity can rotate
    public float neighborRadius = 10;//how close they have to be to be considered in the same group: replaced with OnTriggerEnter
    public float nonPenRadius = 2;//to enforce nonPenetration Constraint

    #region Getters and Setters
    public Vector3 Velocity{ get { return velocity; } set { velocity = value; } }
    public float Mass { get { return mass; } }
    public float MaxSpeed { get { return maxSpeed; } set { maxSpeed = value; } }
    public float MaxForce { get { return MaxForce; } set { MaxForce = value; } }

    public bool IsSpeedMaxedOut { get { return maxSpeed*maxSpeed > velocity.sqrMagnitude; }  }

    public float Speed { get { return velocity.magnitude; } }
    public float SpeedSq { get { return velocity.sqrMagnitude; } }

    public float MaxTurnRate { get { return maxTurnRate; } set { maxTurnRate = value; } }
	
	#endregion

    //returns true when the heading faces the target
    bool RotateHeadingToFacePosition(Vector3 target)
    {
        Vector3 toTarget = target - myTransform.position;

        Vector3 newDirection = Vector3.RotateTowards(myTransform.rotation.eulerAngles, toTarget, (float)(Time.deltaTime * maxTurnRate), 0.0f);
        myTransform.rotation = Quaternion.LookRotation(newDirection);

        // toTarget.Normalize();

        //return true if we are facing target
        if (Vector3.Angle(target, myTransform.position) == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }  
}
