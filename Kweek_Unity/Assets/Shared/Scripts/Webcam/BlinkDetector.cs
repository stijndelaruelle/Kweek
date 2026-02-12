using System.Collections.Generic;
using UnityEngine;
using Stijn.Utilities;

namespace Kweek
{
    public class BlinkDetector : MonoBehaviour
    {
        [SerializeField]
        private WebCamTextureDownScaleExampleCustom m_FaceLandmarkDetector = null;

        [SerializeField]
        private float m_BlinkEARRatio = 0.0f;

        [SerializeField]
        private int m_HistorySize = 0;

        [SerializeField]
        private PhysicsParticle m_PopupPrefab = null; //TEMP

        private LinkedList<float> m_LeftEyeEARRatioHistory = null;

        //Events
        public VoidDelegate BlinkEvent = null;

        private void Awake()
        {
            if (m_FaceLandmarkDetector != null)
                m_FaceLandmarkDetector.FaceLandmarksDetectedEvent += OnFaceLandmarksDetected;

            m_LeftEyeEARRatioHistory = new LinkedList<float>();
        }

        private void OnDestroy()
        {
            if (m_FaceLandmarkDetector != null)
                m_FaceLandmarkDetector.FaceLandmarksDetectedEvent -= OnFaceLandmarksDetected;
        }

        private void AnalyseEyes(FaceLandmarkData faceLandmarkData)
        {
            //This means no face was detected this frame
            float leftEyeAspectRatio = 0.0f;

            if (faceLandmarkData != null)
                leftEyeAspectRatio = faceLandmarkData.CalculateEyeAspectRatio(FaceLandmarkData.EyeType.Left);

            //Check if we're blinking
            if (leftEyeAspectRatio <= m_BlinkEARRatio)
            {
                //Check if our eyes were not already closed last frame
                if (m_LeftEyeEARRatioHistory == null || m_LeftEyeEARRatioHistory.Count == 0 || m_LeftEyeEARRatioHistory.Last.Value > m_BlinkEARRatio)
                {
                    //Fire blink event
                    if (BlinkEvent != null)
                        BlinkEvent();

                    if (m_PopupPrefab != null)
                    {
                        PhysicsParticle partcile = GameObject.Instantiate(m_PopupPrefab);
                        partcile.Animate();
                    }
                }
            }

            //Update the history
            if (m_LeftEyeEARRatioHistory == null)
                m_LeftEyeEARRatioHistory = new LinkedList<float>();

            if (m_LeftEyeEARRatioHistory.Count + 1 > m_HistorySize) //Pop one value if the history would go out of range
                m_LeftEyeEARRatioHistory.RemoveFirst();

            m_LeftEyeEARRatioHistory.AddLast(leftEyeAspectRatio);
        }

        private void OnFaceLandmarksDetected(List<FaceLandmarkData> faceLandmarkData)
        {
            if (faceLandmarkData == null || faceLandmarkData.Count == 0)
            {
                AnalyseEyes(null);
                return;
            }

            //We only analyse the first face
            AnalyseEyes(faceLandmarkData[0]);
        }
    }
}
