using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Listens for touch events and performs an AR raycast from the screen touch point.
    /// AR raycasts will only hit detected trackables like feature points and planes.
    ///
    /// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
    /// and moved to the hit position.
    /// </summary>
    [RequireComponent(typeof(ARRaycastManager))]

    public class ARManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Instantiates this prefab on a plane at the touch location.")]
        GameObject m_PlacedPrefab;

        public GameObject InstructionObject, menuObject;
        public Text InstructionText;
        private int counter=0;
        private bool isDelay = false;

        /// <summary>
        /// The prefab to instantiate on touch.
        /// </summary>
        /// 
        public GameObject placedPrefab
        {
            get { return m_PlacedPrefab; }
            set { m_PlacedPrefab = value; }
        }

        /// <summary>
        /// The object instantiated as a result of a successful raycast intersection with a plane.
        /// </summary>
        public GameObject spawnedObject { get; private set; }

        private List<FMOD.Studio.EventInstance> instances;
        private List<string> names = new List<string>()
        {
            "Violin",
            "Viola",
            "Cello",
            "Piano"
        };
        private List<GameObject> InstrumentPrefabs;


        void Awake()
        {
            m_RaycastManager = GetComponent<ARRaycastManager>();
            instances = new List<FMOD.Studio.EventInstance>();
            InstrumentPrefabs = new List<GameObject>();
            counter = 0;
        }

        bool TryGetTouchPosition(out Vector2 touchPosition)
        {
            if (Input.touchCount > 0)
            {
                touchPosition = Input.GetTouch(0).position;
                return true;
            }

            touchPosition = default;
            return false;
        }

        void Update()
        {
            if (!TryGetTouchPosition(out Vector2 touchPosition))
                return;
            
            if (!isDelay && m_RaycastManager.Raycast(touchPosition, s_Hits))
            {
                var hitPose = s_Hits[0].pose;
                if (counter<4)
                {
                    isDelay = true;
                    Invoke("setNextObject", 1.0f);
                    spawnedObject =Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                    spawnedObject.SetActive(true);
                    InstrumentPrefabs.Add(spawnedObject);
                    var instance = FMODUnity.RuntimeManager.CreateInstance("event:/Test/AR-"+ names[counter]);
                    //var instance = FMODUnity.RuntimeManager.CreateInstance("event:/Test/AR-Instrument1");
                    instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(spawnedObject));
                    instance.start();
                    instance.setPaused(true);
                    instances.Add(instance);
                    counter++;
                }
                /*
                else
                {
                    spawnedObject.transform.position=hitPose.position;
                }*/
            }
        }

        public void setNextObject() {
            isDelay = false;
            if (counter == 4)
            {
                showMenu();
            }
            else
            {
                InstructionText.text = "Place the " + names[counter];
            }
        }

        public void showMenu() {
            InstructionObject.SetActive(false);
            menuObject.SetActive(true);
        }

        public void musicStart()
        {
            foreach (var instance in instances) {
                instance.setPaused(false);
            }
        }

        public void musicPause()
        {
            foreach (var instance in instances)
            {
                instance.setPaused(true);
            }
        }

        static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

        ARRaycastManager m_RaycastManager;
    }
}
