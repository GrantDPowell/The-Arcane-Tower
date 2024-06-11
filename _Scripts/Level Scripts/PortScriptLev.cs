using System.Collections;
using System.Collections.Generic;
using UnityEngine;


	public class PlayerPortal : MonoBehaviour
	{
		bool triggered = false;
		public void OnTriggerEnter(Collider other)
		{
			if (triggered)
				return;
				
			triggered = true;
			
			Debug.Log("Portal reached - Generate new dungeon");
			


			// will use later
			// Generate new dungeon
			//DungeonGameManager.Instance.ExitReached();
			
			
		}
	}
