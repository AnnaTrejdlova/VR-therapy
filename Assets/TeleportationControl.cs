using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportationControl : MonoBehaviour
{
    public GameObject leftTeleportationRay;
    public GameObject rightTeleportationRay;

    // Start is called before the first frame update
    void Start()
    {
        DominantHand dominantHand = ApplicationModel.dominantHand;
        Debug.Log(dominantHand);
        if (dominantHand == DominantHand.Right)
        {
            leftTeleportationRay.SetActive(false);
            rightTeleportationRay.SetActive(true);
        }
        else
        {
            leftTeleportationRay.SetActive(true);
            rightTeleportationRay.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
