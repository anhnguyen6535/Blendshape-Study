using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.LiveCapture.ARKitFaceCapture;

using System.IO;
using Newtonsoft.Json;


[System.Serializable]
public class SerializableDictionaryBool: SerializableDictionary<string, bool> { 
    public Dictionary<string, bool> toDictionary()
    {
        Dictionary<string, bool> output = new Dictionary<string, bool>();

        foreach (var obj in this)
        {
            output.Add(obj.Key, obj.Value);
        }
        return output;
    }
}


[System.Serializable]
public class ConditionsData
{
    public List<string> condition1;
    public List<string> condition2;
    public List<string> condition3;
    public List<string> condition4;
    public List<string> condition5;
}

public class LoadData : MonoBehaviour
{

    public GameObject avatar;
    // public string filename;
    public Button button;
    AudioSource audioSource;

    public int condition;
    public FaceActor actor;
    public bool start = false;
    public string blendshapePath = "blendshapes/airpollution_actor";
    private Animator animator;

    public Dictionary<string, bool> ActiveMapOrigin = new Dictionary<string, bool>()
        {
            {"BrowDownLeft",true},
            {"BrowDownRight",true},
            {"BrowInnerUp",true},
            {"BrowOuterUpLeft",true},
            {"BrowOuterUpRight",true},
            {"CheekPuff",true},
            {"CheekSquintLeft",true},
            {"CheekSquintRight",true},
            {"EyeBlinkLeft",true},
            {"EyeBlinkRight",true},
            {"EyeLookDownLeft",true},
            {"EyeLookDownRight",true},
            {"EyeLookInLeft",true},
            {"EyeLookInRight",true},
            {"EyeLookOutLeft",true},
            {"EyeLookOutRight",true},
            {"EyeLookUpLeft",true},
            {"EyeLookUpRight",true},
            {"EyeSquintLeft",true},
            {"EyeSquintRight",true},
            {"EyeWideLeft",true},
            {"EyeWideRight",true},
            {"JawForward",true},
            {"JawLeft",true},
            {"JawOpen",true},
            {"JawRight",true},
            {"MouthClose",true},
            {"MouthDimpleLeft",true},
            {"MouthDimpleRight",true},
            {"MouthFrownLeft",true},
            {"MouthFrownRight",true},
            {"MouthFunnel",true},
            {"MouthLeft",true},
            {"MouthLowerDownLeft",true},
            {"MouthLowerDownRight",true},
            {"MouthPressLeft",true},
            {"MouthPressRight",true},
            {"MouthPucker",true},
            {"MouthRight",true},
            {"MouthRollLower",true},
            {"MouthRollUpper",true},
            {"MouthShrugLower",true},
            {"MouthShrugUpper",true},
            {"MouthSmileLeft",true},
            {"MouthSmileRight",true},
            {"MouthStretchLeft",true},
            {"MouthStretchRight",true},
            {"MouthUpperUpLeft",true},
            {"MouthUpperUpRight",true},
            {"NoseSneerLeft",true},
            {"NoseSneerRight",true},
            {"TongueOut",true},
        };
    public SerializableDictionaryBool ActiveMap;


    public void Initialize()
    {
        string fileName = "conditions.json";
        ConditionsData conditionsData = LoadJsonFile<ConditionsData>(fileName);

        // Output the loaded conditions data
        //Debug.Log("Condition 1: " + string.Join(", ", conditionsData.condition1));
        //Debug.Log("Condition 2: " + string.Join(", ", conditionsData.condition2));
        //Debug.Log("Condition 3: " + string.Join(", ", conditionsData.condition3));
        //Debug.Log("Condition 4: " + string.Join(", ", conditionsData.condition4));
        //Debug.Log("Condition 5: " + string.Join(", ", conditionsData.condition5));

        ActiveMap.Clear();
        Debug.Log($"Current condition is {condition}");

        List<string> activeList = new List<string>();
        switch (condition)
        {
            case 1:
                activeList = conditionsData.condition1;
                break;
            case 2:
                activeList = conditionsData.condition2;
                break;
            case 3:
                activeList = conditionsData.condition3;
                break;
            case 4:
                activeList = conditionsData.condition4;
                break;
            case 5:
                activeList = conditionsData.condition5;
                break;
            default:
                Debug.Log("No matching condition.");
                condition = 0;
                break;
        }
        Debug.Log(activeList);

        foreach (var obj in ActiveMapOrigin)
        {
            if (condition == 0)
            {
                ActiveMap.Add(obj.Key, obj.Value);
            }
            else
            {
                if (activeList.Contains(obj.Key))
                {
                    ActiveMap.Add(obj.Key, true);
                }
                else
                {
                    ActiveMap.Add(obj.Key, false);
                }
                
            }
        }
    }


    private T LoadJsonFile<T>(string fileName)
    {
        // Get the path for the JSON file
        string filePath = Path.Combine(Application.dataPath, fileName);

        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Read the JSON string from the file
            string jsonString = File.ReadAllText(filePath);

            // Deserialize the JSON string to an object of type T
            T loadedData = JsonConvert.DeserializeObject<T>(jsonString);

            Debug.Log("JSON file loaded from: " + filePath);

            return loadedData;
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
            return default(T);
        }
    }

    private void SaveJsonFile(Dictionary<string, bool> dictionary, string fileName)
    {
        // Convert the dictionary to a JSON string
        string jsonString = JsonConvert.SerializeObject(dictionary, Formatting.Indented);

        // Get the path for the JSON file
        string filePath = Path.Combine(Application.dataPath, fileName);

        // Write the JSON string to the file
        File.WriteAllText(filePath, jsonString);

        Debug.Log("JSON file saved at: " + filePath);
    }

    private void Awake()
    {
        Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {
        //AddListener�� jump �Լ� ����
        button.onClick.AddListener(triggerPlay);
        audioSource = gameObject.GetComponent<AudioSource>();
        animator = avatar.GetComponent<Animator>();    
    }

    // Update is called once per frame
    void Update()
    {
        if(start){
            triggerPlay();
            start = false;
        }
    }

    void PlayAudio()
    {
        audioSource.Play();
        animator.SetBool("talk", true);
        Debug.Log("true");
        StartCoroutine(AudioFinish()); 
    }

    public void triggerPlay()
    {
        // Save the dictionary to a JSON file
        string fileName = "config.json";

        Dictionary<string, bool> activeMapDict = ActiveMap.toDictionary();
        SaveJsonFile(activeMapDict, fileName);

        actor.triggerPlay(blendshapePath);
        Invoke("PlayAudio", 3f); 
    }

    IEnumerator AudioFinish() {
        yield return new WaitWhile(()=>audioSource.isPlaying);
        
        animator.SetBool("talk", false);
        Debug.Log("false");
    }

}
