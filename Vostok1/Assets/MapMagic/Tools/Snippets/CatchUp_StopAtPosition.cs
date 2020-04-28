using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Den.Tools
{
	public class CatchUp_StopAtPosition 
	{
		public float acceleration;
		public float deltaTime;

		public void CatchUpVelocity (Vector3 target, Vector3 position, ref Vector3 velocity)
		{
			//vectors and speeds
			float distance = (target-position).magnitude;
			Vector3 direction = (position-target).normalized; // /distance;
			float speedFront = Vector3.Dot(velocity,direction);

			Vector3 upwards = Vector3.Cross(direction, velocity).normalized;
			Vector3 sideways = Vector3.Cross(upwards, direction).normalized;
			float speedSide = Vector3.Dot(velocity, sideways);


			//calculating acceleration ratio based on catch up time
			float timeFront = CatchUpTime(distance, speedFront);
			float timeSide = speedSide / acceleration * 2;
			float accelFactorFront = timeFront / (timeFront+timeSide);

		//	if (Mathf.Abs(speedSide) > 1) accelFactorFront = 0;
		//	else accelFactorFront = 1;

			float accelTowards = acceleration * accelFactorFront;
			float accelSide = acceleration * (1-accelFactorFront);

			


			//braking the side speed
//			if (speedSide > accelSide*deltaTime) //stopping if still moving
				speedSide -= accelSide*deltaTime;
//			else  //if stopped
//				speedSide = 0;
			
			
			//braking the front speed
//			if (speedFront > accelTowards*deltaTime)
			{
				if (speedFront > 0)  //going from point - braking inverse (ie accelerating)
					speedFront -= accelTowards*deltaTime; 

				else if (timeFront-deltaTime > -speedFront/accelTowards)  //if can brake before catch time - accelerating
					speedFront -= accelTowards*deltaTime; 

				else  //if cannot stop within catch time ----braking with higher acceleration--- nope, break with the standard one, just a bit ahead
				{
					speedFront += accelTowards*deltaTime; 
					if (speedFront > 0) speedFront = 0;
				}
			}
//			else
//				speedFront = 0;


			Debug.Log(speedSide);

			//returning speed vectors
			Vector3 newVelocity = direction*speedFront + sideways*speedSide;

			Debug.Log(velocity + " " + newVelocity + " " + (velocity-newVelocity).magnitude);

			velocity = newVelocity;
		}

		public float CatchUpTime (float distance, float velocity)
		///calculates the time of position to get to target with a given acceleration
		{
			if (distance < 0)  //inverting negative distance
			{
				distance = -distance;
				velocity = -velocity;
			}

			//opposite velocity
			if (velocity >= 0)
			{
				float breakTime = velocity / acceleration;
				distance += acceleration*breakTime*breakTime / 2;

				return Mathf.Sqrt( distance/acceleration )*2 +  breakTime;
			}

			//towards velocity
			else
			{
				float breakTime = -velocity / acceleration;
				distance -= acceleration*breakTime*breakTime / 2;  //it is the FINAL stage, stopping from current speed

				float d = velocity*velocity + acceleration*distance;

				float accelTime = (velocity + Mathf.Sqrt(d)) / acceleration;
				return accelTime*2 + breakTime;
			}

		}
	}
}
