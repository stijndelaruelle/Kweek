using System.Collections.Generic;
using UnityEngine;

namespace Kweek
{
    public class BlinkDetector : MonoBehaviour
    {
        [SerializeField]
        private WebCamTextureDownScaleExampleCustom m_FaceLandmarkDetector = null;

        private void Awake()
        {
            if (m_FaceLandmarkDetector != null)
                m_FaceLandmarkDetector.FaceLandmarksDetectedEvent += OnFaceLandmarksDetected;
        }

        private void OnDestroy()
        {
            if (m_FaceLandmarkDetector != null)
                m_FaceLandmarkDetector.FaceLandmarksDetectedEvent -= OnFaceLandmarksDetected;
        }

        private void OnFaceLandmarksDetected(List<FaceLandmarkData> faceLandmarkData)
        {
            //Analyse eyes
        }
    }
}
