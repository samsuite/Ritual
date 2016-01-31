using UnityEngine;
using System.Collections;

public class rotate : MonoBehaviour {

    public Vector3 spin;
	
	void Update () {
	    transform.Rotate(spin*Time.deltaTime);
	}
}
