using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Village : MonoBehaviour {

    [SerializeField] private GameObject villager;
    [SerializeField] private GameObject god;
    [SerializeField] private int villageResidents;
    [SerializeField] private float spawnTimer;          //Spawn time (in seconds.)
    [SerializeField] private float modifier;

    private Vector3 spawnPosition;
    private bool readyToSpawn;
    private bool initialSpawn;
    private int activeVillagers;

    public int ActiveVillagers
    {
        get { return activeVillagers; }
        set { activeVillagers = value; }
    }

    void Awake()
    {
        readyToSpawn = initialSpawn = true;
        
        //This is crap
    }
	
	// Update is called once per frame
	void Update () {

        //This is for initial spawn, once the limit is reached this shouldn't trigger again.
        if(activeVillagers < villageResidents && readyToSpawn && initialSpawn)
        {
            if (activeVillagers == villageResidents)
            {
                initialSpawn = false;
            }

            SpawnVillager();
        }

        //When village is full, activate it.
        if (activeVillagers >= villageResidents)
        {
            ActiveVillage();
        }
	}

    void ActiveVillage()
    {
        //print("Village Active!");
        //Whatever effects and logic that happens when village is full.
    }

    void SpawnVillager()
    {
        spawnPosition = new Vector3(
            Random.Range(transform.position.x, transform.position.x + modifier),
            this.transform.position.y+(modifier*0.25f),
            Random.Range(transform.position.z, transform.position.z + modifier)
        );

        GameObject go = (GameObject)Instantiate(villager, spawnPosition, transform.rotation);
        go.GetComponent<FollowersBehaviour>().God = god;
        go.GetComponent<FollowersBehaviour>().Home = this.gameObject;

        //This is so they don't spawn on top of each other. Modify as you need to.
        //go.GetComponent<Rigidbody>().AddForce(transform.forward * 500, ForceMode.Force);
        
        StartCoroutine(SpawnVillagerTimer());
    }

    //Simple timer for spawning, 
    IEnumerator SpawnVillagerTimer()
    {
        readyToSpawn = false;
        yield return new WaitForSeconds(spawnTimer);
        readyToSpawn = true;
        activeVillagers++;
    }


}
