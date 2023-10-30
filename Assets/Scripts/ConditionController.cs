using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionController : MonoBehaviour
{
    public bool isShortDistance = true;
    public bool isOnlyHeadMovement = true;

    [SerializeField] private GameObject[] shortDistance;
    [SerializeField] private GameObject[] longDistance;


    // Start is called before the first frame update
    void Start()
    {
        conditionTrigger();
    }

    // Update is called once per frame
    void Update()
    {
        conditionTrigger();
    }

    void conditionTrigger(){
        for (int i = 0; i < shortDistance.Length; i++){
            shortDistance[i].SetActive(isShortDistance);
        }
        for (int i = 0; i < longDistance.Length; i++){
            longDistance[i].SetActive(!isShortDistance);
        }    
    }
}
