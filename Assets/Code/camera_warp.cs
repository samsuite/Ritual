using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class camera_warp : MonoBehaviour {

    ColorCorrectionCurves ccc;
    Camera cam;

    public float BPM = 60f;
    float len;
    float timer = 0f;

	void Start () {
	    ccc = gameObject.GetComponent<ColorCorrectionCurves>();
        cam = gameObject.GetComponent<Camera>();
        len = 60f/BPM;
	}
	
	void Update () {
	    timer = (timer + Time.deltaTime)%len;
        ccc.saturation = 1.05f + 0.1f * Mathf.Sin(Mathf.PI*2f*(timer/len));
        cam.fieldOfView = 100f + Mathf.Sin(Mathf.PI*2f*(timer/len));
	}
}
