using UnityEngine;

namespace FFM.Audio {

    [CreateAssetMenu(fileName = "Audiodata", menuName = "Scriptable/Audio/New Audio")]
    public class AudioData : ScriptableObject {

        [SerializeField] 
        private bool m_MultiClip = false;
        
        [SerializeField] 
        private bool m_RandomPitch = false;


        [SerializeField]
        private float m_Pitch = 1f;

        [SerializeField]
        private Vector2 m_PitchRange = new Vector2(-2, 2);

        [SerializeField]
        private AudioClip m_Clip;

        [SerializeField]
        private AudioClip[] m_Clips;


        public AudioClip Clip => m_MultiClip ? m_Clips[Random.Range(0, m_Clips.Length)] : this.m_Clip;

        public float Pitch => m_RandomPitch ? Random.Range(m_PitchRange.x, m_PitchRange.y) : this.m_Pitch;
    }
}
