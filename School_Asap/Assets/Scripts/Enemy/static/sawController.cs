using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sawController : MonoBehaviour
{
	[SerializeField]
	private float speed = -500f;
	[SerializeField]
	private SpriteRenderer myImage;
	[SerializeField]
	private Sprite bloodSaw;
	private bool oneFunc = true;

	void Update()
	{
		transform.Rotate(new Vector3(0f, 0f, speed * Time.deltaTime));
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if(oneFunc)
		{
			myImage.sprite = bloodSaw;
			oneFunc = false;
		}	
	}
}
