/*
* Original C++ code from "Game AI By Example" translated to C# 
* for use with Unity Game Engine by Sam Arutyunyan
*/
using UnityEngine;
using System.Collections;

//all steering behaviors return a vector describing the force required to apply to its current velocity in order to reach a desired position
//the force vector, when applied to our velocity will nudge us in the direction that we want to go. (has to nudge in deltaTime increments)

public class SteeringBehaviors 
{
    public Vehicle myVehicle;//the vehicle that calls this function. set in PlayerBase.Start()
    
    //waypoint stuff
    
    float waypointAccuracy = 1f;//how close  we get to a waypoint before moving to next one

    //Wander() Variables:
    public Vector3 wanderTarget;//the random target placed about the circle. extends from the center to a point on the permitier. always has length of radius
    public float wanderRadius = 1.2f;//radius of the constraining circle
    public float wanderDistance = 20;//distance to the circle from the agent
    float wanderJitter = 40;//maximum amount of random displacement that can be added each second

    //obstacle avoidance:
    float detectionBoxLength = 0f;
    float minDetectionBoxLength = 5f;//we always look at least this far, and then we increase by velocity

    //Seek returns a force that directs the agent towards a target
    public Vector3 Seek(Vector3 targetPos)
    {
        Vector3 desiredVelocity = Vector3.Normalize((targetPos - myVehicle.transform.position)) * myVehicle.MaxSpeed;//go towards our target at our max speed
        return (desiredVelocity - myVehicle.Velocity);
    }

    //returns a force vector that directs the agent away from target
    public Vector3 Flee(Vector3 targetPos)
    {
        Vector3 desiredVelocity = Vector3.Normalize((myVehicle.transform.position - targetPos)) * myVehicle.MaxSpeed;//go towards our target at our max speed

        return (desiredVelocity - myVehicle.Velocity);
    }

    //Arrive decelerates into the darget position gracefully
    public Vector3 Arrive(Vector3 targetPos, float deceleration)//deceleration is how fast we want to slow in. a bigger number takes longer to arrive
    {
        Vector3 toTarget = targetPos - myVehicle.transform.position;
        toTarget = new Vector3(toTarget.x, 0, toTarget.z);
        //distance to target
        float dist = toTarget.magnitude;

        if (dist > .2)
        {
            //speed required to reach target given the desired deceleration
            float speed = dist / deceleration;
            if (speed > myVehicle.MaxSpeed) speed = myVehicle.MaxSpeed;



            Vector3 desiredVelocity = toTarget * speed / dist;
            return (desiredVelocity - myVehicle.Velocity);
        }
        else
            return Vector3.zero;
    }

    //pursue: page 95(118) Game Ai by example
    public Vector3 Pursue(MovingSphere evader)//have to change what we pass for different evader types
    {
        //this class can be upgraded by making sure anytime the 2 entities are almost paralel whether facing eachotehr or not, or behind eachother
        //or not, they will just directly seek

        //create a vector from us to the evader
        Vector3 toEvader = evader.transform.position - myVehicle.transform.position;//to check that they are in front of us in space

        float relativeHeading = Vector3.Dot(myVehicle.transform.forward, evader.transform.forward);//to check if htey are heading towards us
        // float relativeHeading = Vector3.Dot(toEvader.normalized, evader.transform.forward);

        //if ((relativeHeading < -.8 && relativeHeading >= -1) || (relativeHeading > .8 && relativeHeading <= 1))
        if ((Vector3.Dot(toEvader, myVehicle.transform.forward) > 0) && relativeHeading < -.91)//within 20 degrees 
        {
            //currently this version of direct pursuit does not work because the pursuer rotates with the evader
            //my solution causes a twitch... >_>  this is only a problem if the evader runs directly towards pursuer... >_> 
            //since they are heading towards us and right in front of us, we will head directly at them
            //Debug.Log("Direct PUrsuit!");
            return Seek(evader.transform.position);
        }

        //its not considered ahead of us, so lets figure where it will be

        //look ahead time just decides how much into the future we should calculate our future target
        //I've set my lookAhead to be based on the magnitude. so if both are traveling at the same speed, I look ahead by the distance
        //between them. if the evader is faster, I look ahead farther, if the pursuer is faster, I look ahead less.         
        float lookAheadTime = (toEvader.magnitude * evader.moveSpeed) / ((myVehicle.MaxSpeed));//evader.moveSpeed would be replaced by an actual changing velocity

        // Debug.Log(myVehicle.MaxSpeed + "," + evader.moveVector.magnitude);
        // Debug.Log(lookAheadTime);
        // GameObject trailSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //trailSphere.transform.position = evader.transform.position + evader.moveVector * lookAheadTime;
        // trailSphere.collider.enabled = false;
        return Seek(evader.transform.position + evader.moveVector * lookAheadTime);
    }

    public Vector3 Evade(MovingSphere pursuer)//could also pass a vehicle and just have one vehicle pursuing
    {
        //create a vector from us to the evader
        Vector3 toPursuer = pursuer.transform.position - myVehicle.transform.position;//to check that they are in front of us in space

        float lookAheadTime = (toPursuer.magnitude) / ((myVehicle.MaxSpeed + pursuer.moveVector.magnitude));

        return Flee(pursuer.transform.position + pursuer.moveVector * lookAheadTime);
    }

    //uses a circle in front of the vehicle to place a random target
    public Vector3 Wander()
    {
        float random1 = Random.Range(-1.0f, 1.0f);
        float random2 = Random.Range(-1.0f, 1.0f);
        //wanderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * wanderJitter * Time.deltaTime, 0, Random.Range(-1.0f, 1.0f) * wanderJitter * Time.deltaTime);
        wanderTarget += new Vector3(random1 * wanderJitter * Time.deltaTime, 0, random2 * wanderJitter * Time.deltaTime);

        //reproject the new vector onto the unit circle
        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        //move the target forward  in front of our agent(it was first calculated on top of the agent)
        Vector3 targetLocal = new Vector3(wanderTarget.x, 0, wanderTarget.z + wanderDistance);
        
        targetLocal = myVehicle.transform.TransformDirection(targetLocal);//TransformDirection applies whatever rotation the vehicle has to the targetLocal
        //Debug.DrawRay(myVehicle.transform.position, targetLocal, Color.white);
        return targetLocal;
        
    }

    //uses a detection box extending from the vehicle by an amount of it's current speed.
    //the width of the box is the radius of the bounding sphere of the vehicle. 
    //::a unity approach would be to have a collider box extend out and check any collisions for their collider box
    //if it collided, it passes a contact point, otherwise it passes 0 just to do regular updates of the box
    public void ObstacleAvoidance()//takes teh contact point where it hit and steers away from the normal
    {
        //length of detection box
        detectionBoxLength = minDetectionBoxLength + (myVehicle.Velocity.magnitude / myVehicle.MaxSpeed) * minDetectionBoxLength;
        //assign to the box collider
        BoxCollider myCollider = myVehicle.GetComponent<BoxCollider>();

        myCollider.size = new Vector3(myCollider.size.x, myCollider.size.y, detectionBoxLength);
        myCollider.center = new Vector3(myCollider.center.x, myCollider.center.y, detectionBoxLength * .5f);

    }

    //this one is called in OnCollisionEnter. one above is just called to adjust the box
    public Vector3 ObstacleAvoidance(GameObject contact)//for now this avoids spheres, so we pass the position of the sphere
    {
        /*My implementation;---------------
         * Cast 2 paralel lines(rays) of a distance set by detectionBoxLength. the vehicle must have an attached box collider defining its boundaries.
         * the local x value of htese rays will be calculated from the box collider's size.
         * why doesnt my solution work you ask? because I need a rectangle, it can't just be a ray. need to check all points on the face
         * the solution is simple if we constantly keep record of all obstacles and their sizes... >_>
         */

        //the obstacle's position local to vehicle
        Vector3 localPos = contact.transform.position - myVehicle.transform.position;



        //we'll need to adjust our force by our distance to contact because the closer we are the faster we should react
        float multiplier = 1f + (detectionBoxLength - localPos.x) / detectionBoxLength;
        //     steeringForce *= multiplier;//some multiplyer related to magnitude


        //the lateral force(moving away from obstacle) is calculated by taking the obstacle's local x and subtracting it from it's radius
        // i use one of the scale values for radius.
        //>apply a break force (move back in z):( radius - local.x) * some break weight
        Vector3 steeringForce = new Vector3((contact.transform.localScale.x - localPos.x) * multiplier, 0, (contact.transform.localScale.x - localPos.z) * .2f);
        Debug.Log("derp");
        return steeringForce + myVehicle.transform.position;//returning in world space
    }

    //basically cast a ray in front of the vehicle, check how far our velocity intersects a wall and then apply a magnitude of
    //that force in the direction of the wall's normal. 
    //an improved version might be to have the side whiskers stop at the width of the vehicle so it can fly closer to a wall
    //it works for all obstacles with a collider attached
    //:note: I should cast rays from the tip of the nose not the center. (since its velocity based) 
    //I believe sphere colliders must be used to detect collisions (cant collide 2 boxes)
    public Vector3 WallAvoidance()
    {
        //velocity.magnitude could be updated with currentSpeed if we ever have a variable as such

        Vector3 steeringForce = Vector3.zero;
        //3 feelers. the forward feeler will be scaled by our velocity. the 2 side feelers will be 45 degrees away from forward
        // and have a distance equal to the z value of the box collider lenth (basically the length of the vehicle) 

        //1) we want to assign closestWall to the closest point that the 3 feelers hit

        //create 3 feelers (rays that extend from the vehicle) (see page 105(128) Game AI)
        //origin, direction, hit info, distance
        RaycastHit hit;
       // Vector3 lastPoint;//this holds the last point that was hit by a ray. so that we can check against it with our next ray and determine
        //if we are setting closestWall to the next ray
        //if (Physics.Raycast(myVehicle.transform.position, myVehicle.transform.forward, out hit, myVehicle.Velocity.magnitude))//< change distance to velocity.magnitude
        //when velocity is 0 we shouldn't be calling this function.. (maybe take out the .1 at that point) 
        if (Physics.Raycast(myVehicle.transform.position, myVehicle.transform.forward, out hit, myVehicle.Velocity.magnitude + .1f))//+.1 is to make sure we dont cast a ray of 0.
        {//the overshoot in my implementation is theoretical since the ray technically stops when it hits a point

            //check penetration depth and apply to steeringForce
            Vector3 distToHit = hit.point - myVehicle.transform.position;
            float overShoot = (myVehicle.Velocity - distToHit).magnitude;//how far back we push
           // Debug.Log("overshoot: " + overShoot);
            //GameObject closestWall = hit.collider.gameObject;//the closest wall detected by the feelers
            //lastPoint = hit.point;
            //Debug.Log("hit object: " + hit.collider.gameObject.name);
           // Debug.Log("hit...");

            steeringForce += hit.normal * overShoot;
          //  Debug.Log("applying force from wall: " + hit.normal * overShoot);
        }

        //check right side: if we hit on the right, we want to apply a force that moves us leftward. reaches out at a 45 degree. use box collider size for vehicle's length
        if (Physics.Raycast(myVehicle.transform.position, myVehicle.transform.forward + myVehicle.transform.right, out hit, myVehicle.GetComponent<BoxCollider>().size.z *.7f))
        { 
            //calculate penetration and apply force to the left
            float overShoot = (myVehicle.transform.position - hit.point).magnitude;
            overShoot = myVehicle.GetComponent<BoxCollider>().size.z - overShoot;
            steeringForce += myVehicle.transform.right * -overShoot;
            
          //  Debug.Log("applying force to left: " + myVehicle.transform.right * -overShoot * 50);
        }

        //check left wisker
        if (Physics.Raycast(myVehicle.transform.position, myVehicle.transform.forward + -myVehicle.transform.right, out hit, myVehicle.GetComponent<BoxCollider>().size.z * .7f))
        {
            //calculate penetration and apply force to the right
            float overShoot = (myVehicle.transform.position - hit.point).magnitude;
            overShoot = myVehicle.GetComponent<BoxCollider>().size.z - overShoot;
            steeringForce += myVehicle.transform.right * overShoot;
            
        //    Debug.Log("applying force to right: " + myVehicle.transform.right * overShoot * 50);
        }
      //  Debug.Log("total force:" + steeringForce);

       //  GameObject trailSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
      // trailSphere.transform.position = hit.point;
       // trailSphere.collider.enabled = false;
        Debug.DrawRay(myVehicle.transform.position, myVehicle.Velocity, Color.blue);
        Debug.DrawRay(myVehicle.transform.position, myVehicle.transform.forward, Color.green);
        //Debug.DrawRay(myVehicle.transform.position, myVehicle.transform.forward * 100, Color.blue);
        //Debug.DrawRay(myVehicle.transform.position, myVehicle.transform.right * 50f);
        //we * .7 because .forward + .right will have a magnitude of 1.4 and we want it "normalized"... 1.4 = 7/5 and 5/7 = .7
        Debug.DrawRay(myVehicle.transform.position, (myVehicle.transform.forward + myVehicle.transform.right) * myVehicle.GetComponent<BoxCollider>().size.z * .7f);
        Debug.DrawRay(myVehicle.transform.position, (myVehicle.transform.forward + -myVehicle.transform.right) * myVehicle.GetComponent<BoxCollider>().size.z * .7f);
        //return a force that equals the wall normal * the overshoot length. it is added to by each of the 3 feelers
        return steeringForce;
    }

    //Interpose finds a midpoint between where 2 agents will be in the future(according to their velocity) and creates a force towards that point
    public Vector3 Interpose(Vehicle agentA, Vehicle agentB)
    {
        //calculate a time T to use to figure out where 2 agents will be in the future. to get this, we determine the time it will take to get
        //to the current midpoint of the 2 vehicles at our max speed.
        Vector3 midPoint = (agentA.transform.position + agentB.transform.position) / 2;

        float timeToReachMidPoint = Vector3.Distance(myVehicle.transform.position, midPoint)/myVehicle.MaxSpeed;

        //figure out there future points based on trajectory
        Vector3 aPos = agentA.transform.position + agentA.Velocity * timeToReachMidPoint;
        Vector3 bPos = agentB.transform.position + agentB.Velocity * timeToReachMidPoint;

        //update midpoint
        midPoint = (aPos + bPos) / 2;

        return Arrive(midPoint, 1f);//1 means we will arrive quickly    
    }

    public Vector3 Hide()//p.107(130) game ai
    { 
        //keep an array of all the obstacles in the scene.
        //when a puruer is nearby, cast a ray from him through all the obstacles, by a distance enough to get through to the other side (use radius) 
        //then determine which point is closest to the flee-er. 
        //and use that as a hiding spot. use Arrive to get to that point. 
        //if no points are found, evade. (if the distToclosest at the end is what we started with) return evade(target) otherwise return arrive(besthiding spot);

        //through each iteration of the obstacle, set a flaot distToClostest to some big number. then check each distance squared. if a lesser one comes up,
        //set distToClosest to the new value and set the bestHidingSpot to the current hidingSpot vector being checked.


        return Vector3.zero;
    }

    public Vector3 FollowPath()
    {
      //  Debug.Log("following a path");
        if (myVehicle.path.waypoints.Length == 0)
        {
       //     Debug.Log("No path to follow"); return Vector3.zero;
        }

       // Debug.Log("dist: " + Vector3.Distance(myVehicle.path.CurrentWaypoint(), myVehicle.transform.position));
        if (Vector3.Distance(myVehicle.path.CurrentWaypoint(), myVehicle.transform.position) < waypointAccuracy)
        {
            myVehicle.path.SetNextWaypoint();//increments our current waypoint
        //    Debug.Log("setting next point");
        }
      //  Debug.Log("seeking to: " + myVehicle.path.CurrentWaypoint());

        return Seek(myVehicle.path.CurrentWaypoint());
    }

    //offset relative to a leader's space. keeps formation. leader and offset should be assigned in vehicle's inspector
    public Vector3 OffsetPursuit(Vehicle leader, Vector3 offset)
    {
        //calculate offset according to leader's world position:
        Vector3 worldOffsetPos = leader.transform.TransformPoint(offset);

        //use the distance between us and our offset (position we should be at) to determine how fast to go there
        Vector3 toOffset = worldOffsetPos - myVehicle.transform.position;
        float lookAheadTime = toOffset.magnitude / (myVehicle.maxSpeed + leader.Velocity.magnitude);

        return Arrive(worldOffsetPos + leader.Velocity * lookAheadTime,1f);
    }

    //for this and Alignment() i could probably pass the transform instead of the vehicle. I dont think we use velocity... O_o only .forward
    //it just fails when 2 objects start at the exact same spot. 
    public Vector3 Separation(Vehicle[] vehicles)//see optimization note in ::Alignment()
    {      
        Vector3 steeringForce = Vector3.zero;

        foreach (Vehicle vehicle in vehicles)
        {
            Vector3 toAgent = myVehicle.transform.position - vehicle.transform.position;

            if (toAgent.magnitude <= vehicle.neighborRadius && vehicle != myVehicle)
            {
                //inversly proportional to its distance?
                //its fairly weak, and this causes a problem if they are traveling towards us. 
                //but its supposed to push harder away the closer they get... O_o
                steeringForce += toAgent.normalized / (toAgent.magnitude + .1f);//less separation the farther they are. and protect against /0
            }
        }

        return steeringForce;
    }

    //average out the neighbor's heading and adjust to it.
    //1 method is that each object checks through each vehicle in the scene (assigned in some managerial object). this way would require the for loop
    //to make sure that we are not including ourselves in the calculation of heading
    //another method (currently in use) is that each object assigns his own neighbors. 
    public Vector3 Alignment(Vehicle[] vehicles)
    {
        Vector3 averageHeading = Vector3.zero;
        int totalNeighbors = 0;//this is how many neighbors we detected and retrieved data from
        //iterate through every vehicle, determine if they are a neighbor, and if they are, get their heading
        //averageHeading += currentNeighbor.heading, totalVehicles++
        foreach (Vehicle vehicle in vehicles)
        {
            //calculating what vehicles are close enough could be done externally so that the same group of neighbors per frame
            //can be used for different methods. that approach would have neighbors tagged so instead of checking distances we check tags
            if ((vehicle.transform.position - myVehicle.transform.position).magnitude <= vehicle.neighborRadius && vehicle != myVehicle)
            {
           //     Debug.Log("averageHeading: " + averageHeading);
            //    Debug.Log("neighbor.forward: " + vehicle.transform.forward);                
                averageHeading += vehicle.transform.forward;
             //   Debug.Log("added: " + averageHeading);
                totalNeighbors++;
            }
        }

        if (totalNeighbors <= 0) return Vector3.zero;

        averageHeading /= (float)totalNeighbors;//we need to do this so that we only keep the vehicle heading at magnitude 1
        //>after dividing the heading, the returning vector is very small, sometimes 0 if there are no neighbors. therefore when called, teh vehicle
        //should have his own propulsion. (can be achieved by just adding .forward to this function call)
     //   Debug.Log("avHeading: " + averageHeading);
     //   Debug.Log("divided heading:" + averageHeading);
      //  Debug.Log("my.forward: " + myVehicle.transform.forward);
     //   Debug.Log("returning heading: " + (averageHeading - myVehicle.transform.forward));
        return averageHeading - myVehicle.transform.forward;//subtract in order to get the force required to move towards the heading we want
    }

    public Vector3 Cohesion(Vehicle[] neighbors)
    { 
        Vector3 centerOfMass = Vector3.zero;
        Vector3 steeringForce = Vector3.zero;
        int neighborCount = 0;
        foreach (Vehicle neighbor in neighbors)
        { 
            if ((neighbor.transform.position - myVehicle.transform.position).magnitude <= myVehicle.neighborRadius && neighbor != myVehicle)
            {
                centerOfMass += neighbor.transform.position;
                Debug.Log("position: " + neighbor.transform.position);
                neighborCount++;
            }
            
        }
       // Debug.Log("neighborcount" + neighborCount);

        if (neighborCount > 0)
        {
            centerOfMass /= (float)neighborCount;
            steeringForce = Seek(centerOfMass);
        }

        return steeringForce;
    }

    public Vector3 Flocking(Vehicle[] neighbors)
    {
        //adjust values to give different weights. a much more optimal method would be to rewrite than to call those functions
        Vector3 steeringForce = Wander()*5;
        steeringForce += Separation(neighbors)*10;
        steeringForce += Alignment(neighbors)*.8f;
        steeringForce += Cohesion(neighbors)*2;
        return steeringForce;
    }

    //demo, only portion of the if statements. the steeringForce vector should be zero'd in Vehicle.Update before calling this function
    /*public Vector3 Calculate()//p.122(145)
    {
        Vector3 steeringForce = Vector3.zero;

        if (myVehicle.on_WallAvoidance)
        {
            steeringForce += WallAvoidance();//could multiply by a multiplier
            //if we've gone above max alloted force (check the magnitude) then we just return force now and dont continue
        }

        if (myVehicle.on_ObstacleAvoidance)
        {
            steeringForce += WallAvoidance();//could multiply by a multiplier
            //if we've gone above max alloted force (check the magnitude) then we just return force now and dont continue
        }

        //etc...

        return steeringForce;
    }*/

    //i think instead of a generic<T>, i can set it as MovingEntity and all inheritences can be passed
    //this can also be replaced with a character controller. (remember cc uses its own .Move function)
    //I think this is meant to be called from the inner loop of another function, like Cohesion()
    public void EnforcePenetrationConstraint(Vehicle entity, Vehicle[] neighbors)//the entity passed is just ourselves from Vehicle.cs Update
    {
        foreach (Vehicle curEntity in neighbors)
        {
            if (curEntity != entity)
            {
                //calculate distance between their positions
                Vector3 toEntity = entity.transform.position - curEntity.transform.position;

                float distFromEachother = toEntity.magnitude;

                //if their distance is smaller than sum of their radii, we need to move 
                float amountOfOverlap = curEntity.nonPenRadius + entity.nonPenRadius - distFromEachother;

                if (amountOfOverlap >= 0)
                {
                    toEntity = new Vector3(toEntity.x, 0, toEntity.z);//zero out y
                    //Debug.Log("toEntity: " + toEntity);
                    entity.transform.position = entity.transform.position + (toEntity / distFromEachother) * amountOfOverlap;
                }
            }

        }
    }


    
}
