﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OncePlayParticle : MonoBehaviour {

    ParticleSystem ptSys;
	// Use this for initialization
	void Start () {
        ptSys = this.GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {

        if (ptSys == null)
        {
            return;
        }
        else if (ptSys.isStopped) {
            Destroy(this.gameObject);
        }

    }
}
