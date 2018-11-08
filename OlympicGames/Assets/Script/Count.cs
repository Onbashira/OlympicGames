using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Count : MonoBehaviour {

    private int num;
    public int num_max = 3;
    private float time = 0;
	// Use this for initialization
	void Start ()
    {
        num = num_max;
        this.GetComponent<Text>().text = num.ToString();
    }
	
	// Update is called once per frame
	void Update () {

        if(num == 0)
        {
            this.GetComponent<Text>().enabled = false;
            return;
        }
        time += Time.deltaTime;
        if(time > 1)
        {
            time -= 1;
            num--;
            this.GetComponent<Text>().text = "" + num;// num.ToString();
        }
        
	}
}
