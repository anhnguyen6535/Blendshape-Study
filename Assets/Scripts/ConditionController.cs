using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.LiveCapture.ARKitFaceCapture;

public class ConditionController : MonoBehaviour
{
    public bool isShortDistance = true;

    [SerializeField] private GameObject[] shortDistance;
    [SerializeField] private GameObject[] longDistance;
    [SerializeField] private GameObject xrCamera;
    public GameObject avatar;
    private GameObject cameraPlaceholder;

    // Start is called before the first frame update
    void Start()
    {
        cameraPlaceholder = GameObject.Find("CameraPlaceholder");
        xrCamera.transform.position = cameraPlaceholder.transform.position;
        // make sure that the first element in each array is user's chair
        if(isShortDistance){
            avatar.transform.position = new Vector3(
                shortDistance[0].transform.position.x,
                shortDistance[0].transform.position.y,
                shortDistance[0].transform.position.z  - 0.3f
            );
        }else{
            avatar.transform.position = new Vector3(
                longDistance[0].transform.position.x,
                longDistance[0].transform.position.y,
                longDistance[0].transform.position.z  - 0.3f
            );
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
