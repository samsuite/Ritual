using UnityEngine;
using System.Collections;

public class intro_cutscene : MonoBehaviour {

    camera_cutscene cc;
    int last_node = 0;
    public portal_controller portal;
    public lerp_to_point l;
    public GameObject logo;
    public SpiritController sc;

	void Start () {
	    cc = gameObject.GetComponent<camera_cutscene>();
        cc.animate();
        logo.SetActive(false);
        sc.enabled = false;
	}
	
	void Update () {
	    if (last_node != cc.next_node){
            last_node = cc.next_node;
            if (last_node == 5){
                portal.open();
                logo.SetActive(true);
            }
            else if (last_node == 6){
                l.play();
            }
            else if (last_node == 8){
                portal.close();
            }
            else if (last_node == 9){
                logo.SetActive(false);
                sc.enabled = true;
                Camera.main.transform.parent = sc.gameObject.transform;
            }
        }
	}
}
