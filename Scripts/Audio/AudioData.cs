using Sirenix.OdinInspector;
using UnityEngine;

namespace FFM.Audio {

    [CreateAssetMenu(fileName = "Audiodata", menuName = "Scriptable/Audio/New Audio")]
    public class AudioData : ScriptableObject {

        [BoxGroup("Settings"), HorizontalGroup("Settings/Line"), LabelWidth(90), SerializeField] 
        private bool m_MultiClip = false;
        
        [BoxGroup("Settings"), HorizontalGroup("Settings/Line"), LabelWidth(90), SerializeField] 
        private bool m_RandomPitch = false;


        [SerializeField, HideIf("m_RandomPitch"), BoxGroup("Audio")]
        private float m_Pitch = 1f;

        [SerializeField, ShowIf("m_RandomPitch"), MinMaxSlider(-2, 2, true), BoxGroup("Audio")]
        private Vector2 m_PitchRange = new Vector2(-2, 2);

        [SerializeField, HideIf("m_MultiClip"), BoxGroup("Audio")]
        private AudioClip m_Clip;

        [SerializeField, ShowIf("m_MultiClip"), BoxGroup("Audio")]
        private AudioClip[] m_Clips;


        public AudioClip Clip => m_MultiClip ? m_Clips[Random.Range(0, m_Clips.Length)] : this.m_Clip;

        public float Pitch => m_RandomPitch ? Random.Range(m_PitchRange.x, m_PitchRange.y) : this.m_Pitch;
    }
}