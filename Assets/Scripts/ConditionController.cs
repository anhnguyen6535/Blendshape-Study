using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.LiveCapture.ARKitFaceCapture;

public class ConditionController : MonoBehaviour
{
    public bool isShortDistance = true;
    public bool isOnlyHeadMovement = true;
    public bool start = false;
    public string fileName = "blendshapes/airpollution_actor";
    private AudioSource audioSource;

    [SerializeField] private GameObject[] shortDistance;
    [SerializeField] private GameObject[] longDistance;
    [SerializeField] private FaceActor actor;


    // Start is called before the first frame update
    void Start()
    {
        conditionTrigger();
        audioSource = gameObject.GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if(start){
            actor.triggerPlay(fileName);
            GetComponent<LoadData>().triggerPlay();
            start = false;
        }
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
