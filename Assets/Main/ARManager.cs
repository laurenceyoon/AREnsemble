using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
        public List<GameObject> prefabList;
        public GameObject InstructionObject, menuObject;
        public Text InstructionText;
        private int counter = 0;
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

        private Instrument selectedInstrument = null;
        private int selectedIndex;

        private bool holding;

        public Canvas m_canvas;
        GraphicRaycaster m_gr;
        PointerEventData m_ped;

        public MIDIManager midiManager;

        //public List<ParticleSystem> psList;
        public int particleIndex = 0;
        private enum ParticleType
        {
            fxWaveC,
            fxWaveE,
        }

        void Awake()
        {
            m_RaycastManager = GetComponent<ARRaycastManager>();
            instances = new List<FMOD.Studio.EventInstance>();
            InstrumentPrefabs = new List<GameObject>();
            counter = 0;
        }

        private void Start()
        {
            m_gr = m_canvas.GetComponent<GraphicRaycaster>();
            m_ped = new PointerEventData(null);
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
                        var instrumentDetection = hit.transform.GetComponent<Instrument>();
                        holding = instrumentDetection != null;
                        if (holding) selectedInstrument = instrumentDetection;
                        /*
                        if (selectedInstrument == null )
                        {
                            var isTouchingSlider = false;
                            m_ped.position = Input.GetTouch(0).position;
                            List<RaycastResult> results = new List<RaycastResult>();
                            m_gr.Raycast(m_ped, results);

                            if (results.Count > 0)
                            {
                                if (results[0].gameObject.tag=="slider")
                                    isTouchingSlider = true;
                            }
                            if (!isTouchingSlider) instrumentSizeSlider.gameObject.SetActive(false);
                        }
                        */
                    }
                }
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    if (holding)
                    {
                        instrumentSizeSlider.gameObject.SetActive(true);
                        instrumentSizeSlider.value = (selectedInstrument.sizeScale - 0.5f) / 1.0f;
                    }
                    else
                    {
                        instrumentSizeSlider.gameObject.SetActive(false);
                    }
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
                    var rot = hitPose.rotation.eulerAngles + 180f * Vector3.up;
                    spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, Quaternion.Euler(rot));
                    spawnedObject.SetActive(true);
                    InstrumentPrefabs.Add(spawnedObject);
                    var instance = FMODUnity.RuntimeManager.CreateInstance("event:/Test/AR-" + names[counter]);
                    var instrument = spawnedObject.GetComponent<Instrument>();
                    instrument.Init(instance, names[counter], particleIndex);
                    midiManager.midiInit(instrument);
                    instances.Add(instance);
                    counter++;
                }
                else
                {
                    var isTouchingSlider = false;
                    m_ped.position = Input.GetTouch(0).position;
                    List<RaycastResult> results = new List<RaycastResult>();
                    m_gr.Raycast(m_ped, results);

                    if (results.Count > 0)
                    {
                        if (results[0].gameObject.tag == "slider")
                            isTouchingSlider = true;
                    }
                    if (holding && !isTouchingSlider)
                    {
                        var rot = hitPose.rotation.eulerAngles + 180f * Vector3.up;
                        Move(hitPose.position, Quaternion.Euler(rot));
                    }
                }
            }
        }

        void Move(Vector3 position, Quaternion rotation)
        {
            selectedInstrument.transform.position = position;
            selectedInstrument.transform.rotation = rotation;
            //instances[selectedIndex].set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(spawnedObject));
        }

        public void setNextObject()
        {
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

        public void showMenu()
        {
            InstructionObject.SetActive(false);
            menuObject.SetActive(true);
        }

        public void onValueChange()
        {
            float newScale = instrumentSizeSlider.value + 0.5f;
            Vector3 newSize = new Vector3(newScale * selectedInstrument.originalSize.x, newScale * selectedInstrument.originalSize.y, newScale * selectedInstrument.originalSize.z);
            selectedInstrument.sizeScale = newScale;
            selectedInstrument.transform.localScale = newSize;
            //InstructionText.text = selectedInstrument.name +" : "+ holding.ToString()+", "+ newScale.ToString()+", "+ instrumentSizeSlider.value.ToString();
        }

        public void musicPlay()
        {
            foreach (var instance in InstrumentPrefabs) {
                instance.GetComponent<Instrument>().play();
            }
        }

        public void musicPause()
        {
            foreach (var instance in InstrumentPrefabs)
            {
                instance.GetComponent<Instrument>().pause();
            }
        }

        public void echoSmallSet(bool isOn)
        {
            if(isOn)
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Reverb", 1);
                //Debug.Log(1.ToString());
        }
        public void echoMidSet(bool isOn)
        {
            if (isOn)
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Reverb", 2);
                //Debug.Log(2.ToString());
        }
        public void echoBigSet(bool isOn)
        {
            if (isOn)
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Reverb", 3);
                //Debug.Log(3.ToString());
        }

        static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

        ARRaycastManager m_RaycastManager;
    }
}