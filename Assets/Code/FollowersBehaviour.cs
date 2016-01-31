using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class FollowersBehaviour : MonoBehaviour {

    private const string FOLLOWER_JUMP_PLAY = "FollowerPlay";   //For animation.

    [SerializeField] private GameObject god;                    //Player character. Assigned in village object.
    [SerializeField] private GameObject home;                   //Home village. Assigned in village object.
    [SerializeField] private float minFollowDist;               //How close a follower will get to any object.
    [SerializeField] private float minMoveSpeed;                // Min and max speeds, follower will chose a random speed between these two.
    [SerializeField] private float maxMoveSpeed;                   
    [SerializeField] private float idleTimer;                   //Time allowed to be idle (In seconds.)

    private GameObject shrine;                                  //Praying object, assigned once in trigger.
    private Animator followerAnimator;                          //Animator object.
    private float moveSpeed;                                    //Speed calculated from min and max.
    private bool followGod;                                     //Follow player.
    private bool atHome;                                        //At home village.
    private bool praying;                                       //Praying at shrine.

    public GameObject God 
    {
        get { return god; }
        set { god = value; }
    }

    public GameObject Home
    {
        get { return home; }
        set { home = value; }
    }

	void Awake () 
    {
        praying = false;
        followGod = god.GetComponent<SpiritController>().IsEnchanting;
        followerAnimator = GetComponentInChildren<Animator>();

        //Get random speed from the range of min and max.
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);

	}
	
	// Update is called once per frame
	void Update () 
    {

        /*   Following god works, need to set button on player that will set "followGod" to true.
         *   Would be better to move followGod to a global script accessable by both player and followers?
         *   (Set by player, accessed by followers.)
         */

        //Follower at shrine
        if (praying)
        {
            //Logic done on Shine.cs, this can be changed as needed.
            FollowObject(shrine);
        }

        //Follower at home village
        if (atHome)
        {
            //Logic done on Village.cs, this can be changed as needed.
            FollowObject(home);
        }

        //Follow god. (Change bool conditions.)
        if (followGod && !praying)
        {
            FollowObject(god);
        }
        else if(!followGod && !praying)
        {
            StartCoroutine(IdleTimer());
        }
	}

    //Simple follow script
    void FollowObject(GameObject target)
    {
        //Animates walk cycle.
        followerAnimator.SetBool(FOLLOWER_JUMP_PLAY, true);

        //Vector3 targetPosition = new Vector3(target.transform.position.x, this.transform.position.y, target.transform.position.z);

        //transform.LookAt(targetPosition);

        transform.LookAt(target.transform);

        if(Vector3.Distance(transform.position, target.transform.position) >= minFollowDist)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.Equals(god))
        {
            atHome = false;
        }

        if (collider.gameObject.Equals(home))
        {
            atHome = true;
        }

        if(collider.gameObject.tag.Equals("TargetObj"))
        {
            //Set shrine and add one to follower count.
            shrine = collider.gameObject;
            praying = true;

            //(Not a fan of doing this, but it works for now.)
            collider.gameObject.GetComponent<Shrine>().FollowersPraying++;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.Equals(god))
        {
            //Stop animation walk cycle.
            followerAnimator.SetBool(FOLLOWER_JUMP_PLAY, false);
        }

        if (collider.gameObject.Equals(home))
        {
            atHome = false;
        }
    }

    IEnumerator IdleTimer()
    {
        yield return new WaitForSeconds(idleTimer);
        FollowObject(home);
    }
}
