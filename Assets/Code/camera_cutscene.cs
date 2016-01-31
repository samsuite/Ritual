using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class camera_cutscene : MonoBehaviour {

    public List<cutscene_node> nodes;
    public int next_node;
    bool animating = false;
    bool waiting = false;
    float timer = 0f;

    Vector3 init_pos;
    Quaternion init_rot;

    cutscene_node n;
	
	void FixedUpdate () { // fixed update time is 0.02 sec (50 fps)
	    if (animating){
            if (waiting){
                timer -= 0.02f;
                if (timer <= 0){
                    timer = 0;
                    waiting = false;
                }
            }
            else if (n.cut_to){
                Camera.main.transform.position = n.gameObject.transform.position;
                Camera.main.transform.rotation = n.gameObject.transform.rotation;
            }
            else if (n.lerp_to){
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position,n.gameObject.transform.position,0.01f);
                Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation,n.gameObject.transform.rotation,0.01f);
            }
            else {
                float rate = 1f/(n.time_to * 50f);
                Vector3 disp = (n.gameObject.transform.position - init_pos)*rate;
                float angle = Quaternion.Angle(init_rot, n.gameObject.transform.rotation) *rate;

                Camera.main.transform.position += disp;
                Camera.main.transform.rotation = Quaternion.RotateTowards(Camera.main.transform.rotation, n.gameObject.transform.rotation, angle);
            }

            if (Vector3.Distance(Camera.main.transform.position,n.gameObject.transform.position) < 2f && !waiting){
                if (nodes.Count >= next_node+2){
                    next_node += 1;
                    n = nodes[next_node];

                    init_pos = Camera.main.transform.position;
                    init_rot = Camera.main.transform.rotation;


                    if (n.wait_before){
                        waiting = true;
                        timer = n.wait_time;
                    }
                }
                else {
                    animating = false;
                }
            }
        }
	}

    public void animate () {
        next_node = 0;
        n = nodes[next_node];
        Camera.main.transform.position = n.gameObject.transform.position;
        Camera.main.transform.rotation = n.gameObject.transform.rotation;
        if (nodes.Count >= next_node+2){
            animating = true;
            next_node += 1;
            n = nodes[next_node];

            init_pos = Camera.main.transform.position;
            init_rot = Camera.main.transform.rotation;
        }
    }
}
