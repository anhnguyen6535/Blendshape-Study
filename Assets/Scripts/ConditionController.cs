using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.LiveCapture.ARKitFaceCapture;

public class ConditionController : MonoBehaviour
{
    public bool isShortDistance = true;
    public bool isOnlyHeadMovement = true;

    [SerializeField] private GameObject[] shortDistance;
    [SerializeField] private GameObject[] longDistance;
    [SerializeField] private GameObject xrCamera;

    // Start is called before the first frame update
    void Start()
    {

        // make sure that the first element in each array is user's chair
        if(isShortDistance){
            xrCamera.transform.position = shortDistance[0].transform.position;
        }else{
            xrCamera.transform.position = longDistance[0].transform.position;
        }
        conditionTrigger();
    }

    // Update is called once per frame
    void Update()
    {
        
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
