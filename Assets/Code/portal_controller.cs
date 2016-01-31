using UnityEngine;
using System.Collections;

public class portal_controller : MonoBehaviour {

    [Range(0.0f, 1.0f)]
    public float hole_size = 0;

    public AnimationCurve anim = AnimationCurve.Linear(0,0,5,1);

	public GameObject tube;
    public MeshRenderer ren;
    public ParticleSystem part;

    float tube_width;
    float ren_size;

    public bool is_opening {
        get; private set;
    }
    public bool is_open {
        get; private set;
    }
    float timer = 0f;

	void Start () {
        part.enableEmission = false;
	    tube_width = tube.transform.localScale.x;
        ren_size = ren.material.GetFloat("_HoleSize");
	}
	
	void Update () {
	    tube.transform.localScale = new Vector3(hole_size*tube_width,tube.transform.localScale.y,hole_size*tube_width);
        ren.material.SetFloat("_HoleSize", ren_size*hole_size);

        if (is_opening){

            timer += Time.deltaTime;

            if (timer >= 5f){
                is_opening = false;
                is_open = true;
                timer = 0f;
            }
            else {
                hole_size = anim.Evaluate(timer);
            }

        }
	}

    public void open(){
        is_opening = true;
        part.enableEmission = true;
    }

    public void close(){
        is_open = false;
        part.enableEmission = false;
        hole_size = 0f;
    }

}
