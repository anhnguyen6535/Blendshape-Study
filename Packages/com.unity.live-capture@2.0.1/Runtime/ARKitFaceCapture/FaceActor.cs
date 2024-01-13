using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text.RegularExpressions;

using Newtonsoft.Json;


namespace Unity.LiveCapture.ARKitFaceCapture
{


    /// <summary>
    /// A component used to apply face poses to a character in a scene.
    /// </summary>
    [DisallowMultipleComponent]
    [ExcludeFromPreset]
    [AddComponentMenu("Live Capture/ARKit Face Capture/ARKit Face Actor")]
    [ExecuteAlways]
    [RequireComponent(typeof(Animator))]
    [HelpURL(Documentation.baseURL + "ref-component-arkit-face-actor" + Documentation.endURL)]
    public class FaceActor : MonoBehaviour
    {

        internal static class PropertyNames
        {
            public const string BlendShapes = nameof(m_BlendShapes);
            public const string HeadPosition = nameof(m_HeadPosition);
            public const string HeadOrientation = nameof(m_HeadOrientation);
            public const string LeftEyeOrientation = nameof(m_LeftEyeOrientation);
            public const string RightEyeOrientation = nameof(m_RightEyeOrientation);
            public const string BlendShapesEnabled = nameof(m_BlendShapesEnabled);
            public const string HeadPositionEnabled = nameof(m_HeadPositionEnabled);
            public const string HeadOrientationEnabled = nameof(m_HeadOrientationEnabled);
            public const string EyeOrientationEnabled = nameof(m_EyeOrientationEnabled);
        }


        Dictionary<string, int> MappingOrder = new Dictionary<string, int>()
        {
            {"BrowDownLeft",0},
            {"BrowDownRight",1},
            {"BrowInnerUp",2},
            {"BrowOuterUpLeft",3},
            {"BrowOuterUpRight",4},
            {"CheekPuff",5},
            {"CheekSquintLeft",6},
            {"CheekSquintRight",7},
            {"EyeBlinkLeft",8},
            {"EyeBlinkRight",9},
            {"EyeLookDownLeft",10},
            {"EyeLookDownRight",11},
            {"EyeLookInLeft",12},
            {"EyeLookInRight",13},
            {"EyeLookOutLeft",14},
            {"EyeLookOutRight",15},
            {"EyeLookUpLeft",16},
            {"EyeLookUpRight",17},
            {"EyeSquintLeft",18},
            {"EyeSquintRight",19},
            {"EyeWideLeft",20},
            {"EyeWideRight",21},
            {"JawForward",22},
            {"JawLeft",23},
            {"JawOpen",24},
            {"JawRight",25},
            {"MouthClose",26},
            {"MouthDimpleLeft",27},
            {"MouthDimpleRight",28},
            {"MouthFrownLeft",29},
            {"MouthFrownRight",30},
            {"MouthFunnel",31},
            {"MouthLeft",32},
            {"MouthLowerDownLeft",33},
            {"MouthLowerDownRight",34},
            {"MouthPressLeft",35},
            {"MouthPressRight",36},
            {"MouthPucker",37},
            {"MouthRight",38},
            {"MouthRollLower",39},
            {"MouthRollUpper",40},
            {"MouthShrugLower",41},
            {"MouthShrugUpper",42},
            {"MouthSmileLeft",43},
            {"MouthSmileRight",44},
            {"MouthStretchLeft",45},
            {"MouthStretchRight",46},
            {"MouthUpperUpLeft",47},
            {"MouthUpperUpRight",48},
            {"NoseSneerLeft",49},
            {"NoseSneerRight",50},
            {"TongueOut",51},
        };

        List<string> BlendShapeKeyFromCSV = new List<string>();
        public int frameIndex = 0;
        public float timer;
        public float waitingTime;
        bool nowPlaying = false;

        Dictionary<string, bool> ActiveMap = new Dictionary<string, bool>();



        [SerializeField, Tooltip("The asset that configures how face pose data is mapped to this character's face rig.")]
        FaceMapper m_Mapper = null;

        [SerializeField, Tooltip("The channels of face capture data to apply to this actor. " +
            "This allows for recording all channels of a face capture, while later being able to use select parts of the capture.")]
        [EnumFlagButtonGroup(100f)]
        FaceChannelFlags m_EnabledChannels = FaceChannelFlags.All;

        [SerializeField, Tooltip("The blend shapes weights that define the face expression.")]
        FaceBlendShapePose m_BlendShapes;
        [SerializeField, Tooltip("The position of the head.")]
        Vector3 m_HeadPosition;
        [SerializeField, Tooltip("The rotation of the head.")]
        Vector3 m_HeadOrientation = Vector3.zero;
        [SerializeField, Tooltip("The rotation of the left eye.")]
        Vector3 m_LeftEyeOrientation = Vector3.zero;
        [SerializeField, Tooltip("The rotation of the right eye.")]
        Vector3 m_RightEyeOrientation = Vector3.zero;
        [SerializeField]
        bool m_BlendShapesEnabled;
        [SerializeField]
        bool m_HeadPositionEnabled;
        [SerializeField]
        bool m_HeadOrientationEnabled;
        [SerializeField]
        bool m_EyeOrientationEnabled;

        FaceMapperCache m_Cache;
        FaceChannelFlags m_LastChannels;

        /// <summary>
        /// The Animator component used by the device for playing takes on this actor.
        /// </summary>

        public List<List<float>> wholeData;
        public Animator Animator { get; private set; }

        /// <summary>
        /// The asset that configures how face pose data is mapped to this character's face rig.
        /// </summary>
        public FaceMapper Mapper => m_Mapper;

        internal FaceBlendShapePose BlendShapes
        {
            get => m_BlendShapes;
            set => m_BlendShapes = value;
        }

        internal Vector3 HeadPosition
        {
            get => m_HeadPosition;
            set => m_HeadPosition = value;
        }

        internal Vector3 HeadOrientation
        {
            get => m_HeadOrientation;
            set => m_HeadOrientation = value;
        }

        internal Vector3 LeftEyeOrientation
        {
            get => m_LeftEyeOrientation;
            set => m_LeftEyeOrientation = value;
        }

        internal Vector3 RightEyeOrientation
        {
            get => m_RightEyeOrientation;
            set => m_RightEyeOrientation = value;
        }

        internal bool BlendShapesEnabled
        {
            get => m_BlendShapesEnabled;
            set => m_BlendShapesEnabled = value;
        }

        internal bool HeadPositionEnabled
        {
            get => m_HeadPositionEnabled;
            set => m_HeadPositionEnabled = value;
        }

        internal bool HeadOrientationEnabled
        {
            get => m_HeadOrientationEnabled;
            set => m_HeadOrientationEnabled = value;
        }

        internal bool EyeOrientationEnabled
        {
            get => m_EyeOrientationEnabled;
            set => m_EyeOrientationEnabled = value;
        }

        void Awake()
        {
            Animator = GetComponent<Animator>();
        }

        public void Start()
        {
            

            timer = 0.0f;
            waitingTime = 1f / 60;
            // playbackWithBlendshapes();
        }

        void OnDisable()
        {
            ClearCache();
        }


        void FixedUpdate()
        {
            if (nowPlaying)
            {

                frameIndex += 1;

                List<float> frame = wholeData[frameIndex];

                // Debug.Log($"{frameIndex}, {frame.Count}");

                setBlendshapes(frame);
                UpdateRig(true);

                if (frameIndex > wholeData.Count - 10)
                {
                    nowPlaying = false;
                    Debug.Log("Now playing: " + nowPlaying);
                    frameIndex = 0;
                }
                
            }

            //if (nowPlaying)
            //{
            //    timer += Time.deltaTime;

            //    if (timer > waitingTime)
            //    {
            //        frameIndex += 1;

            //        List<float> frame = wholeData[frameIndex];

            //        Debug.Log($"{frameIndex}, {frame.Count}");

            //        setBlendshapes(frame);
            //        UpdateRig(true);

            //        timer = 0;

            //        if (frameIndex > wholeData.Count - 10)
            //        {
            //            nowPlaying = false;
            //        }
            //    }
            //}
        }

        void LateUpdate()
        {
            if (!nowPlaying)
            {
                UpdateRig(true);
            }
            
        }

        /// <summary>
        /// Sets the face mapper used by this face rig.
        /// </summary>
        /// <param name="mapper">The mapper to use, or null to clear the current mapper.</param>
        public void SetMapper(FaceMapper mapper)
        {
            m_Mapper = mapper;
            ClearCache();
        }

        /// <summary>
        /// Clears the mapper state cache for the actor, causing it to rebuild the next
        /// time the face rig updates.
        /// </summary>
        public void ClearCache()
        {
            m_Cache?.Dispose();
            m_Cache = null;
        }

        /// <summary>
        /// Updates the face rig from the current pose.
        /// </summary>
        /// <param name="continuous">When true, the new pose follows the current pose and they
        /// can be smoothed between, while false corresponds to a seek in the animation where the
        /// previous pose is invalidated and should not influence the new pose.</param>
        public void UpdateRig(bool continuous)
        {
            if (m_Mapper != null)
            {
                if (m_Cache == null)
                {
                    m_Cache = m_Mapper.CreateCache(this);

                    // we can't use any previous state since there is none yet
                    continuous = false;
                }

                // determine which face channels are enabled and have data to use
                var channels = m_EnabledChannels;

                if (!m_BlendShapesEnabled)
                {
                    channels &= ~FaceChannelFlags.BlendShapes;
                }
                if (!m_HeadPositionEnabled)
                {
                    channels &= ~FaceChannelFlags.HeadPosition;
                }
                if (!m_HeadOrientationEnabled)
                {
                    channels &= ~FaceChannelFlags.HeadRotation;
                }
                if (!m_EyeOrientationEnabled)
                {
                    channels &= ~FaceChannelFlags.Eyes;
                }

                // apply the active face channels
                if (channels.HasFlag(FaceChannelFlags.BlendShapes))
                {
                    m_Mapper.ApplyBlendShapesToRig(
                        this,
                        m_Cache,
                        ref m_BlendShapes,
                        continuous && m_LastChannels.HasFlag(FaceChannelFlags.BlendShapes)
                    );
                }
                if (channels.HasFlag(FaceChannelFlags.HeadPosition))
                {
                    m_Mapper.ApplyHeadPositionToRig(
                        this,
                        m_Cache,
                        ref m_HeadPosition,
                        continuous && m_LastChannels.HasFlag(FaceChannelFlags.HeadPosition)
                    );
                }
                if (channels.HasFlag(FaceChannelFlags.HeadRotation))
                {
                    var headRotation = Quaternion.Euler(m_HeadOrientation);

                    m_Mapper.ApplyHeadRotationToRig(
                        this,
                        m_Cache,
                        ref headRotation,
                        continuous && m_LastChannels.HasFlag(FaceChannelFlags.HeadRotation)
                    );
                }
                if (channels.HasFlag(FaceChannelFlags.Eyes))
                {
                    var leftEyeRotation = Quaternion.Euler(m_LeftEyeOrientation);
                    var rightEyeRotation = Quaternion.Euler(m_RightEyeOrientation);

                    m_Mapper.ApplyEyeRotationToRig(
                        this,
                        m_Cache,
                        ref m_BlendShapes,
                        ref leftEyeRotation,
                        ref rightEyeRotation,
                        continuous && m_LastChannels.HasFlag(FaceChannelFlags.Eyes)
                    );
                }

                m_LastChannels = channels;
            }
        }

        public List<List<float>> ReadFullData(string file)
        {
            var list = new List<List<float>>();
            StreamReader sr = new StreamReader(Application.dataPath + "/Resources/" + file);

            bool endOfFile = false;
            int index = 0;

            BlendShapeKeyFromCSV = new List<string>();

            while (!endOfFile)
            {
                string data_String = sr.ReadLine();
                if (data_String == null)
                {
                    endOfFile = true;
                    break;
                }
                var data_values = data_String.Split(','); //string, stringŸ��

                var tmp = new List<float>();
                for (int i = 2; i < data_values.Length - 9; i++)
                {
                    if (index == 0)
                    {
                        BlendShapeKeyFromCSV.Add(data_values[i]);
                    }
                    else
                    {
                        // Debug.Log(data_values[i]);
                        float value = float.Parse(data_values[i]);
                        tmp.Add(value);
                    }

                }

                // if (index == 0)
                // {
                //    foreach (string key in BlendShapeKeyFromCSV)
                //     {
                //         Debug.Log(key);
                //     }
                //     Debug.Log($"Number of Key : {BlendShapeKeyFromCSV.Count}");
                // }

                list.Add(tmp);
                index += 1;
            }

            list.RemoveAt(0);

            return list;
        }

        public void setBlendshapes(List<float> frame)
        {
            // Debug.Log(frame.Count);
            int blendshapeIndex = 0;

            foreach (string shapeKey in BlendShapeKeyFromCSV)
            {
                
                if (ActiveMap[shapeKey] == true)
                {
                    int realKey = MappingOrder[shapeKey];
                    // Debug.Log($"real key:  {realKey}, blendshape index: {blendshapeIndex}");
                    m_BlendShapes[realKey] = frame[blendshapeIndex];
                    
                    
                    if (shapeKey == "JawOpen")
                    {
                        // Debug.Log($"{frame[blendshapeIndex]}");
                    }
                    
                }
                else
                {
                    int realKey = MappingOrder[shapeKey];
                    m_BlendShapes[realKey] = DefaultfacialExpressions[shapeKey];
                    // Debug.Log($"{realKey}, {blendshapeIndex}");
                }
                blendshapeIndex += 1;

            }

        }

        private Dictionary<string, bool> LoadJsonFile(string fileName)
        {
            // Get the path for the JSON file
            string filePath = Path.Combine(Application.dataPath, fileName);

            // Check if the file exists
            if (File.Exists(filePath))
            {
                // Read the JSON string from the file
                string jsonString = File.ReadAllText(filePath);

                // Deserialize the JSON string to a Dictionary<string, bool>
                Dictionary<string, bool> loadedDictionary = JsonConvert.DeserializeObject<Dictionary<string, bool>>(jsonString);

                // Debug.Log("JSON file loaded from: " + filePath);

                return loadedDictionary;
            }
            else
            {
                Debug.LogError("File not found: " + filePath);
                return null;
            }
        }


        private Dictionary<string, float> DefaultfacialExpressions;


        private void LoadJsonDefaultJsonFile(string fileName)
        {
            // Get the path for the JSON file
            string filePath = Path.Combine(Application.dataPath, fileName);

            string jsonContent = File.ReadAllText(filePath);
            DefaultfacialExpressions = JsonConvert.DeserializeObject<Dictionary<string, float>>(jsonContent);

            // foreach (var obj in DefaultfacialExpressions)
            // {
            //     Debug.Log($"Facial Expressions Loaded: {obj.Key}, {obj.Value}");
            // }
            
        }


        private void toPlayMode()
        {
            nowPlaying = true;

        }

        private void loadMap()
        {
            ActiveMap = LoadJsonFile("config.json");
            LoadJsonDefaultJsonFile("default_blendshapes.json");
            // Debug.Log($"Active, {ActiveMap}");

            // foreach(KeyValuePair<string, bool> items in ActiveMap)
            // {
            //     if (items.Value)
            //     {
            //         Debug.Log($"{items.Key}, {items.Value}");
            //     }
            // }
        }

        public void triggerPlay(string filename)
        {
            
            // Debug.Log("Triggered playback!");

            Invoke("loadMap", 2f);

            wholeData = ReadFullData(filename + ".csv");
            Invoke("toPlayMode", 3f);
        }
    }
}
