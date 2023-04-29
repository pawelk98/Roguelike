using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchController : MonoBehaviour
{
    public GameObject lightBall;
    bool isActivated;

    void Start()
    {
        lightBall.SetActive(false);
    }

    public void Activate()
    {
        lightBall.SetActive(true);
        isActivated = true;
    }

    public bool IsActivated()
    {
        return isActivated;
    }
}
