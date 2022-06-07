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
        private Camera arCamera;
        public List<GameObject> prefabList;
        public GameObject InstructionObject, menuObject, playButton;
        public Text InstructionText;
        private int counter = 0;
        private bool isDelay = false;

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
        private List<float> rotDeg = new List<float>()
        {
            180f,
            180f,
            180f,
            0f
        };
        private List<GameObject> InstrumentPrefabs;

        public Slider instrumentSizeSlider;

        private Instrument selectedInstrument = null;
        public Instrument toggleInstrument = null;
        private int toggleID;
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

        public enum UserState {
            Initializing,
            Ready,
            Playing,
            InstrumentSettings
        }

        public UserState currentState;

        public MenuController menuController;
        public GameObject InstrumentDeactivateButton;

        void Awake()
        {
            m_RaycastManager = GetComponent<ARRaycastManager>();
            instances = new List<FMOD.Studio.EventInstance>();
            InstrumentPrefabs = new List<GameObject>();
            counter = 0;
            currentState = UserState.Initializing;
            
        }

        public void initInstruments() {
            for (int i = 0; i < 4; i++)
            {
                spawnedObject = Instantiate(prefabList[i]);
                spawnedObject.SetActive(false);
                InstrumentPrefabs.Add(spawnedObject);
                var instance = FMODUnity.RuntimeManager.CreateInstance("event:/Test/AR-" + names[i]);
                var instrument = spawnedObject.GetComponent<Instrument>();
                instrument.Init(instance, names[i], particleIndex);
                midiManager.midiInit(instrument);
                instances.Add(instance);
            }
        }

        private void Start()
        {
            m_gr = m_canvas.GetComponent<GraphicRaycaster>();
            m_ped = new PointerEventData(null);
        }

        void Update()
        {
            if (Input.touchCount > 0 && currentState == UserState.InstrumentSettings)
            {
                InstructionText.text = Input.GetTouch(0).position.ToString();
                if (toggleInstrument != null && m_RaycastManager.Raycast(Input.GetTouch(0).position, s_Hits) && Input.GetTouch(0).position.y>450 && Input.GetTouch(0).position.y < 2000)
                {
                    var hitPose = s_Hits[0].pose;
                    var rot = hitPose.rotation.eulerAngles + rotDeg[toggleID] * Vector3.up;
                    toggleInstrument.gameObject.SetActive(true);
                    toggleInstrument.transform.position = hitPose.position;
                    toggleInstrument.transform.rotation = Quaternion.Euler(rot);
                    toggleInstrument.activate();
                    toggleInstrument = null;
                    menuController.deselectToggle();
                }

                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Ray ray = arCamera.ScreenPointToRay(Input.GetTouch(0).position);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        var instrumentDetection = hit.transform.GetComponent<Instrument>();
                        holding = instrumentDetection != null;
                        if (holding)
                        {
                            selectedInstrument = instrumentDetection;
                            toggleID = InstrumentPrefabs.FindIndex(obj=>obj==selectedInstrument.gameObject);
                        }
                    }
                }
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    if (holding)
                    {
                        instrumentSizeSlider.gameObject.SetActive(true);
                        InstrumentDeactivateButton.SetActive(true);
                        instrumentSizeSlider.value = (selectedInstrument.sizeScale - 0.5f) / 1.0f;
                    }
                    else
                    {
                        InstrumentDeactivateButton.SetActive(false);
                        instrumentSizeSlider.gameObject.SetActive(false);
                    }
                    holding = false;
                }
                if (m_RaycastManager.Raycast(Input.GetTouch(0).position, s_Hits))
                {
                    var hitPose = s_Hits[0].pose;
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
                        var rot = hitPose.rotation.eulerAngles + rotDeg[toggleID] * Vector3.up;
                        Move(hitPose.position, Quaternion.Euler(rot));
                    }
                }
            }
            transform.GetComponent<ARPlaneManager>().SetTrackablesActive(currentState == UserState.InstrumentSettings);
        }

        void Move(Vector3 position, Quaternion rotation)
        {
            selectedInstrument.transform.position = position;
            selectedInstrument.transform.rotation = rotation;
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
            playButton.SetActive(false);
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

        public void setSelectedInstrument(int id) {
            toggleInstrument = InstrumentPrefabs[id].GetComponent<Instrument>();
            toggleID = id;
            //selectedInstrument.play();
        }

        public void closeInstrumentSettings()
        {
            currentState = ARManager.UserState.Playing;
            instrumentSizeSlider.gameObject.SetActive(false);
            InstrumentDeactivateButton.SetActive(false);
        }

        public void deactivateInstrument()
        {
            instrumentSizeSlider.gameObject.SetActive(false);
            InstrumentDeactivateButton.SetActive(false);
            selectedInstrument.deactivate();
            selectedInstrument.gameObject.SetActive(false);
        }

        static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

        ARRaycastManager m_RaycastManager;
    }
}