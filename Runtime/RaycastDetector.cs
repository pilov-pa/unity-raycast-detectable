using R3;
using UnityEngine;

namespace Pilov.RaycastDetectable
{
    public class RaycastDetector : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float maxDistance = 3;
        [SerializeField] private Camera detectorCamera;
        private ReactiveProperty<GameObject> _detectedObject = new();
        public ReadOnlyReactiveProperty<GameObject> DetectedObject => _detectedObject;

        private void Awake()
        {
            if (detectorCamera == null)
            {
                detectorCamera = Camera.main;
            }
        }

        private void Update()
        {
            Detect(new Vector2(Screen.width / 2f, Screen.height / 2f));
        }

        private void Detect(Vector2 screenCoords)
        {
            Ray ray = detectorCamera.ScreenPointToRay(screenCoords);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
            {
                var raycastDetectables = hit.collider.GetComponents<IRaycastDetectable>();
                if (_detectedObject.Value != null && _detectedObject.Value != hit.transform.gameObject)
                {
                    var oldRaycastDetectables= _detectedObject.Value.GetComponents<IRaycastDetectable>();
                    foreach (var raycastDetectable in oldRaycastDetectables)
                    {
                        raycastDetectable.OnUndetect();
                    }
                    _detectedObject.Value = null;
                }
                
				if (hit.transform.gameObject != _detectedObject.Value)
                {
	                foreach (var raycastDetectable in raycastDetectables)
	                {
	                    raycastDetectable.OnDetect();
 	               }
  	               _detectedObject.Value = hit.transform.gameObject;
				}
                return;
            }

            if (_detectedObject.Value != null)
            {
                var raycastDetectables= _detectedObject.Value.GetComponents<IRaycastDetectable>();
                foreach (var raycastDetectable in raycastDetectables)
                {
                    raycastDetectable.OnUndetect();
                }
            }
            _detectedObject.Value = null;
        }
    }
}