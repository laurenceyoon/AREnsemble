using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

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

        void Awake()
        {
            m_RaycastManager = GetComponent<ARRaycastManager>();
            instances = new List<FMOD.Studio.EventInstance>();
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
            
            if (m_RaycastManager.Raycast(touchPosition, s_Hits))
            {
                var hitPose = s_Hits[0].pose;
                if (spawnedObject == null)
                {
                    spawnedObject=Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                    spawnedObject.SetActive(true);
                    var instance = FMODUnity.RuntimeManager.CreateInstance("event:/Test/AR-Instrument1");
                    instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(spawnedObject));
                    instance.start();
                    instances.Add(instance);
                }
                else
                {
                    spawnedObject.transform.position=hitPose.position;
                }
            }
        }

        static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

        ARRaycastManager m_RaycastManager;
    }
}
