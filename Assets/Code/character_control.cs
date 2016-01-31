using UnityEngine;
using System.Collections;

public class character_control : MonoBehaviour {

    public float gravity_strength = 5f;

    Rigidbody rb;
    Vector3 gravity;
    float drag;
    string state; // "flying" or "walking"

    bool invert_world;

	void Start () {
        rb = gameObject.GetComponent<Rigidbody>();
        gravity = new Vector3(0,0,0);
        gravity.y = gravity_strength*-1f;
        drag = 0.98f;
        state = "flying";
        invert_world = false;
    }
	
	void Update () {

        if (state == "flying"){
	        rb.AddForce(gravity);
        }
        

	}
}
