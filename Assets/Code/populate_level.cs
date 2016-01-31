using UnityEngine;
using System.Collections;

public class populate_level : MonoBehaviour {

    public int num_objects;
    public GameObject obj;
    public float radius;

    public LayerMask terrainLayer;

	void Start () {
	    for (int i = 0; i < num_objects; i++){

            Vector2 offset = Random.insideUnitCircle * radius;
		    Vector3 combined = new Vector3 (transform.position.x + offset.x, transform.position.y, transform.position.z + offset.y);
		    RaycastHit hit;
            if (Physics.Raycast (combined, Vector3.down, out hit, 10000f, terrainLayer)) {
                GameObject g = (GameObject)Instantiate(obj, hit.point, Quaternion.Euler(0f,Random.Range(0f,360f),0f));
                float rand = Random.Range(-0.2f,0.2f);
                g.transform.localScale = new Vector3(g.transform.localScale.x+rand,g.transform.localScale.y+rand,g.transform.localScale.z+rand);
		    }
        }
	}
	
	void Update () {
	
	}
}
