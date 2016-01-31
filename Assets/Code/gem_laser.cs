using UnityEngine;
using System.Collections;

public class gem_laser : MonoBehaviour {

    Vector3 loc;
    bool launched;
    public GameObject laser;
    
    void Start () {
	    loc = transform.position;
        transform.position = new Vector3(transform.position.x, 110f ,transform.position.z);
        laser.SetActive(false);
	}
	
	void Update () {
	    if (launched){
            transform.position = Vector3.Lerp(transform.position,loc,0.03f);
        }
        if (Vector3.Distance(transform.position, loc) < 0.5f){
            laser.SetActive(true);
        }
	}

    void launch (){
        launched = true;
    }
}