using UnityEngine;
using System.Collections;

public class lerp_to_point : MonoBehaviour {

    public Vector3 dest;
    bool playing = false;

    public AudioClip miro;

	void Update () {
        if (playing){
            transform.position = Vector3.Lerp(transform.position,dest,0.02f);
        }
        if (Vector3.Distance(transform.position, dest) < 0.5f){
            playing = false;
            AudioSource.PlayClipAtPoint (miro, Camera.main.transform.position, 0.2f);
        }
	}

    public void play(){
        playing = true;
    }
}
