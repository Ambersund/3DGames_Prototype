using UnityEngine;

namespace UnityStandardAssets.Vehicles.Aeroplane
{
    public class AeroplanePropellerAnimator : MonoBehaviour
    {
        [SerializeField] private Transform m_PropellerModel;                          // The model of the the aeroplane's propellor.
        [SerializeField] private Transform m_PropellerBlur;                           // The plane used for the blurred propellor textures.
        [SerializeField] private Texture2D[] m_PropellerBlurTextures;                 // An array of increasingly blurred propellor textures.
        [SerializeField] [Range(0f, 1f)] private float m_ThrottleBlurStart = 0.25f;   // The point at which the blurred textures start.
        [SerializeField] [Range(0f, 1f)] private float m_ThrottleBlurEnd = 0.5f;      // The point at which the blurred textures stop changing.
        [SerializeField] private float m_MaxRpm = 2000;                               // The maximum speed the propellor can turn at.

        /// <summary>
        /// Rotate propeller model around X axis instead of Y axis. Useful for Blender-imported models.
        /// </summary>
        public bool rotateAroundX = false;
        
        private AeroplaneController m_Plane;      // Reference to the aeroplane controller.
        private int m_PropellorBlurState = -1;    // To store the state of the blurred textures.
        private const float k_RpmToDps = 60f;     // For converting from revs per minute to degrees per second.
        private Renderer m_PropellorModelRenderer;
        private Renderer m_PropellorBlurRenderer;


        private void Awake()
        {
            // Set up the reference to the aeroplane controller.
            m_Plane = GetComponent<AeroplaneController>();

            m_PropellorModelRenderer = m_PropellerModel.GetComponent<Renderer>();
            m_PropellorBlurRenderer = m_PropellerBlur.GetComponent<Renderer>();

            // Set the propellor blur gameobject's parent to be the propellor.
            m_PropellerBlur.parent = m_PropellerModel;
        }


        private void Update()
        {
            // Rotate the propellor model at a rate proportional to the throttle.
            float rotation = m_MaxRpm*m_Plane.Throttle*Time.deltaTime*k_RpmToDps;
            if (rotateAroundX)
            {
                m_PropellerModel.Rotate(rotation, 0, 0);
            }
            else
            {
                m_PropellerModel.Rotate(0, rotation, 0);
            }

            // Create an integer for the new state of the blur textures.
            var newBlurState = 0;

            // choose between the blurred textures, if the throttle is high enough
            if (m_Plane.Throttle > m_ThrottleBlurStart)
            {
                var throttleBlurProportion = Mathf.InverseLerp(m_ThrottleBlurStart, m_ThrottleBlurEnd, m_Plane.Throttle);
                newBlurState = Mathf.FloorToInt(throttleBlurProportion*(m_PropellerBlurTextures.Length - 1));
            }

            // If the blur state has changed
            if (newBlurState != m_PropellorBlurState)
            {
                m_PropellorBlurState = newBlurState;

                if (m_PropellorBlurState == 0)
                {
                    // switch to using the 'real' propellor model
                    m_PropellorModelRenderer.enabled = true;
                    m_PropellorBlurRenderer.enabled = false;
                }
                else
                {
                    // Otherwise turn off the propellor model and turn on the blur.
                    m_PropellorModelRenderer.enabled = false;
                    m_PropellorBlurRenderer.enabled = true;

                    // set the appropriate texture from the blur array
                    m_PropellorBlurRenderer.material.mainTexture = m_PropellerBlurTextures[m_PropellorBlurState];
                }
            }
        }
    }
}
