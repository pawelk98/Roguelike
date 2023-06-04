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
        SoundController.Instance.PlaySound(SoundController.Instance.itemChange, transform.position, 0.3f);

    }

    public bool IsActivated()
    {
        return isActivated;
    }
}
