using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : MonoBehaviour
{

    [SerializeField]
    private uint maxNum = 0;
    // Use this for initialization
    void Start()
    {
        ControllerFetcher.Initialize();
        maxNum = ControllerFetcher.GetMaxConectedController();
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 1; i <= ControllerFetcher.GetMaxConectedController(); ++i)
        {
            if(GamepadInput.GamePad.GetButton(GamepadInput.GamePad.Button.A, (GamepadInput.GamePad.Index)i))
            {
                Debug.Log( i + " A is Press");
            }
        }

    }
}
