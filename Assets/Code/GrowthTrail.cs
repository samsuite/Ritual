using UnityEngine;
using System.Collections.Generic;

public class GrowthTrail : MonoBehaviour {

	public LayerMask terrainLayer;
	public GameObject[] plantPrefabs;
	public int poolSize = 120;
	public float maxDistanceToGround = 30;
	public float maxRandOffset = 10f;
	public float baseGrowthIntDistance = 0.5f;
	public float randGrowthIntDistance = 0.1f;
	public float baseGrowthIntTime = 0.05f;
	public float randGrowthIntTime = 0.01f;
	public int safetyBuffer = 30;
	public float timeToDie = 0.1f;
	public float timeToGrow = 0.1f;
	public float marginOfError = 0.0001f;

	private float currGrowthDistance;
	private float currGrowthTime;
	private Vector3 lastPosition;
	private float distanceTraveled;
	private int desiredPlants;
	private bool killingAllPlants;

	private InternalPlantScript_DO_NOT_TOUCH[] plantPool;
	private int nextReadyPlant;
	private int numActivePlants;
	private HashSet<InternalPlantScript_DO_NOT_TOUCH> dyingPlants;
	private HashSet<InternalPlantScript_DO_NOT_TOUCH> growingPlants;

	void Start () {
		if (safetyBuffer > poolSize) {
			Debug.LogWarning ("Do not make pool smaller than safety buffer! Buffer has been disabled.");
		}
		dyingPlants = new HashSet<InternalPlantScript_DO_NOT_TOUCH> ();
		growingPlants = new HashSet<InternalPlantScript_DO_NOT_TOUCH> ();
		refreshPlantPool ();
		lastPosition = transform.position;
		currGrowthDistance = baseGrowthIntDistance + Random.Range (-randGrowthIntDistance, randGrowthIntDistance);
		currGrowthTime = Time.timeSinceLevelLoad + baseGrowthIntTime + Random.Range (-randGrowthIntTime, randGrowthIntTime);
	}
	
	void Update () {

		if (killingAllPlants) {
			if (dyingPlants.Count == 0) {
				killingAllPlants = false;
				nextReadyPlant = 0;
				numActivePlants = 0;
			} else return;
		}

		distanceTraveled += Vector3.Distance (lastPosition, transform.position);
		lastPosition = transform.position;

		if (distanceTraveled >= currGrowthDistance && numActivePlants != plantPool.Length) {
			currGrowthDistance = baseGrowthIntDistance + Random.Range (-randGrowthIntDistance, randGrowthIntDistance);
			distanceTraveled = 0f;
			GrowPlant (plantPool [nextReadyPlant]);
		}
		if (Time.timeSinceLevelLoad >= currGrowthTime && numActivePlants != plantPool.Length) {
			currGrowthTime = Time.timeSinceLevelLoad + baseGrowthIntTime + Random.Range (-randGrowthIntTime, randGrowthIntTime);
			GrowPlant (plantPool [nextReadyPlant]);
		}
		MaintainSafetyBuffer (desiredPlants);

		dyingPlants.RemoveWhere (keepDying);

		growingPlants.RemoveWhere (keepGrowing);

	}

	void KillPlant (InternalPlantScript_DO_NOT_TOUCH plant) {
		dyingPlants.Add (plant);
		growingPlants.Remove (plant);
	}

	void GrowPlant (InternalPlantScript_DO_NOT_TOUCH plant) {
		Vector2 offset = Random.insideUnitCircle * maxRandOffset;
		Vector3 combined = new Vector3 (transform.position.x + offset.x, transform.position.y, transform.position.z + offset.y);
		RaycastHit hit;
		if (Physics.Raycast (combined, Vector3.down, out hit, maxDistanceToGround, terrainLayer)) {
			plant.transform.position = hit.point;
		} else {
			MaintainSafetyBuffer (numActivePlants - dyingPlants.Count - 1);
			return;
		}
		growingPlants.Add (plant);
		plant.transform.localScale = Vector3.zero;
		plant.gameObject.SetActive (true);
		numActivePlants++;
		nextReadyPlant = (nextReadyPlant + 1) % plantPool.Length;
	}

	bool keepDying (InternalPlantScript_DO_NOT_TOUCH plant) {
		Vector3 newSize = Vector3.SmoothDamp (plant.transform.localScale, Vector3.zero, ref plant.scaleVelocity, timeToDie);
		if (newSize.sqrMagnitude <= marginOfError) {
			plant.gameObject.SetActive (false);
			plant.scaleVelocity = Vector3.zero;
			numActivePlants--;
			return true;
		} else {
			plant.transform.localScale = newSize;
			return false;
		}
	}

	bool keepGrowing (InternalPlantScript_DO_NOT_TOUCH plant) {
		Vector3 newSize = Vector3.SmoothDamp (plant.transform.localScale, plant.fullSize, ref plant.scaleVelocity, timeToGrow);
		if (newSize.sqrMagnitude >= plant.sqrMagFullSizeMinusBorder) {
			plant.transform.localScale = plant.fullSize;
			plant.scaleVelocity = Vector3.zero;
			return true;
		} else {
			plant.transform.localScale = newSize;
			return false;
		}
	}

	public void refreshPlantPool () {
		
		killAllPlantsImmediate ();
		
		plantPool = new InternalPlantScript_DO_NOT_TOUCH [poolSize];
		
		for (int i = 0; i < plantPool.Length; i++) {
			plantPool[i] = Instantiate (plantPrefabs[i % plantPrefabs.Length]).AddComponent<InternalPlantScript_DO_NOT_TOUCH> ();
			plantPool[i].fullSize = plantPool[i].transform.localScale;
			plantPool[i].sqrMagFullSize = plantPool[i].fullSize.sqrMagnitude;
			plantPool[i].sqrMagFullSizeMinusBorder = plantPool[i].sqrMagFullSize - marginOfError;
			plantPool[i].scaleVelocity = Vector3.zero;
			plantPool[i].gameObject.SetActive (false);
		}
		
		desiredPlants = plantPool.Length - safetyBuffer;
	}

	void MaintainSafetyBuffer (int desiredPlants) {
		if (desiredPlants < 0) {
            desiredPlants = 0;
		}
		int oldestPlant = (nextReadyPlant - numActivePlants) % plantPool.Length;
		if (oldestPlant < 0) oldestPlant += plantPool.Length;
		while (numActivePlants - dyingPlants.Count > desiredPlants) {
			KillPlant (plantPool [oldestPlant]);
			oldestPlant = (oldestPlant + 1) % plantPool.Length;
		}
	}

	public void killAllPlantsImmediate () {

		if (plantPool != null) {
			foreach (InternalPlantScript_DO_NOT_TOUCH plant in plantPool) {
				Destroy (plant.gameObject);
			}
		}

		dyingPlants.Clear ();
		growingPlants.Clear ();

		nextReadyPlant = 0;
		numActivePlants = 0;
	}

	public void killAllPlantsPretty () {
		foreach (InternalPlantScript_DO_NOT_TOUCH plant in plantPool) {
			KillPlant (plant);
		}
		killingAllPlants = true;
	}

	public int getNumPlantsInTrail () {
		return numActivePlants;
	}
	
}