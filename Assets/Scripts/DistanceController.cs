using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DistanceController : MonoBehaviour
{

    private GameObject avatar;
    private GameObject chair;
    private string csvPath = "";
    public float movingSpeed = 0.5f;
    public int condition = 1;
    [SerializeField] Vector3 initialPosition = new(0,0,1);
    public bool log = false;

    //------ CONDITION VARIABLES -----//
    public GameObject female;
    public GameObject male;
    private bool isStanding = false;
    [SerializeField] GameObject femchair;
    [SerializeField] GameObject mchair;

    // Start is called before the first frame update
    void Start()
    {
        // condition setup
        ConditionSetup();

        // create new csv file for this scene
        csvPath = Application.dataPath + "/Results/DistanceLog/" + DateTime.Now.ToString("dd-MM-HH-mm") + ".csv";
        TextWriter tw = new StreamWriter(csvPath, false);
        tw.WriteLine("Condition, x, y, z");
        tw.Close();
    }

    // Update is called once per frame
    void Update()
    {
        // log data when clicked
        if(log){
            LogDistanceToCSV();
            log = false;
        }
    }

    void MoveAvatarCloser(){
        avatar.transform.position -= new Vector3(0,0,movingSpeed);
    }

    void MoveAvatarFurther(){
        avatar.transform.position += new Vector3(0,0,movingSpeed);
    }

    void LogDistanceToCSV(){
        TextWriter tw = new StreamWriter(csvPath, true);
        tw.WriteLine(condition + "," + avatar.transform.position);
        tw.Close();
    }
    

    void ConditionSetup(){
        // clear old avatar
        female.SetActive(false);
        male.SetActive(false);

        // reset setup based on codition
        SwitchCondition(condition);
        avatar.SetActive(true);
        avatar.GetComponent<Animator>().SetBool("isStanding", isStanding);
        avatar.transform.position = initialPosition;
        chair.SetActive(!isStanding);
    }

    void SwitchCondition(int cond){
        switch(cond){
            case 1:
                avatar = male;
                chair = mchair;
                isStanding = false;
                break;
            case 2:
                avatar = female;
                chair = femchair;
                isStanding = false;
                break;
            case 3:
                avatar = male;
                chair = mchair;
                isStanding = true;
                break;
            case 4:
                avatar = female;
                chair = femchair;
                isStanding = true;
                break;
            default:
                break;
        }
    }
}
