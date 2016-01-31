using UnityEngine;
using System.Collections;

public class cutscene_node : MonoBehaviour {

    public bool cut_to;
    public bool lerp_to;
    public float time_to = 2f;
    public bool wait_before = false;
    public float wait_time = 1f;
    Camera cam;

	void Start () {
	    cam = gameObject.GetComponent<Camera>();
        Destroy(cam);
	}
	
	void Update () {
	
	}
}
