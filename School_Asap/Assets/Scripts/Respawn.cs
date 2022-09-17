using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
	public float loseHeight = -20f;
	public VFXDeath death;
	private GameObject player;
	public Transform start;

	void Update()
	{
		if(player == null)
		{ 
			player = GameObject.FindWithTag("Player");
		}
		else
		{
			if (player.transform.position.y <= loseHeight && player != null)
			{
				death.Death();
			}
		}
	}
}
