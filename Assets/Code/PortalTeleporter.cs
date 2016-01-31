using UnityEngine;
using System.Collections;

public class PortalTeleporter : MonoBehaviour {

    public SpiritController player;
    public portal_controller portal;
    public float radius = 0.6f;
	
	// Update is called once per frame
	void Update () {
	    if (portal.is_open && player.isActiveAndEnabled && (player.transform.position - this.transform.position).sqrMagnitude <= radius * radius) {
            Application.Quit ();
        }
	}
}
