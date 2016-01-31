using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Shrine : MonoBehaviour {

    [SerializeField] private int followersNeeded;

    public portal_controller portal;
    private bool portalOpening;

    private static int activeShrines;
    private Collider prayerTrigger;
    private int followersPraying;
    private bool shrineActive;

    public int FollowersPraying
    {
        get { return followersPraying; }
        set { followersPraying = value; }
    }

	void Awake () {
        shrineActive = false;
        prayerTrigger = this.GetComponent<Collider>();
	}
	
	// Update is called once per frame
	void Update () {
        if (followersPraying >= followersNeeded && !shrineActive)
        {
            prayerTrigger.enabled = false;
            shrineActive = true;
            ActivateShrine();
        }
	}

    void ActivateShrine()
    {
        print("Shrine Active!");
        activeShrines++;
        if (activeShrines >= 5) {
            portal.open();
        }
    }

}
