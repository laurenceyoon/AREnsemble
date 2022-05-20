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

        [SerializeField]
        private Camera arCamera;

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

        public Slider instrumentSizeSlider;

        private Instrument selectedInstrument=null;
        private int selectedIndex;

        private bool holding;

        void Awake()
        {
            m_RaycastManager = GetComponent<ARRaycastManager>();
            instances = new List<FMOD.Studio.EventInstance>();
            InstrumentPrefabs = new List<GameObject>();
            counter = 0;
        }

        void Update()
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Ray ray = arCamera.ScreenPointToRay(Input.GetTouch(0).position);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        /*
                        var index= 0;
                        foreach (var spwnedObject in InstrumentPrefabs)
                        {   
                            if (hit.transform == spawnedObject.transform)
                            {
                                selectedObject = spawnedObject;
                                selectedIndex = index;
                                holding = true;
                            }
                            index++;
                        }*/
                        selectedInstrument = hit.transform.GetComponent<Instrument>();
                        holding = selectedInstrument != null;
                    }
                }
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    holding = false;
                }
            }

            if (Input.touchCount > 0 && !isDelay && m_RaycastManager.Raycast(Input.GetTouch(0).position, s_Hits))
            {
                var hitPose = s_Hits[0].pose;
                if (counter < 4)
                {
                    isDelay = true;
                    Invoke("setNextObject", 1.0f);
                    spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                    spawnedObject.SetActive(true);
                    InstrumentPrefabs.Add(spawnedObject);
                    var instance = FMODUnity.RuntimeManager.CreateInstance("event:/Test/AR-" + names[counter]);
                    //var instance = FMODUnity.RuntimeManager.CreateInstance("event:/Test/AR-Instrument1");
                    /*
                    instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(spawnedObject));
                    instance.start();
                    instance.setPaused(true);
                    */
                    spawnedObject.GetComponent<Instrument>().Init(instance);
                    instances.Add(instance);
                    counter++;
                }
                else
                {
                    if (holding)
                        Move(hitPose.position, hitPose.rotation);
                }
            }
        }

        void Move(Vector3 position, Quaternion rotation)
        {
            selectedInstrument.transform.position = position;
            selectedInstrument.transform.rotation = rotation;
            //instances[selectedIndex].set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(spawnedObject));
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
            /*foreach (var instance in instances) {
                instance.setPaused(false);
            }*/
            foreach (var instance in InstrumentPrefabs) {
                instance.GetComponent<Instrument>().play();
            }
        }

        public void musicPause()
        {
            /*foreach (var instance in instances)
            {
                instance.setPaused(true);
            }*/
            foreach (var instance in InstrumentPrefabs)
            {
                instance.GetComponent<Instrument>().pause();
            }
        }

        static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

        ARRaycastManager m_RaycastManager;
    }
}
