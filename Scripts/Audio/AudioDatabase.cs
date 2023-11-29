using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FFM.Audio {
    public class AudioDatabase : Singleton<AudioDatabase> {
        [SerializeField] private List<AudioData> m_AudioDatabase = new List<AudioData>();


        public AudioData AudioByName(string clipName) {
            return m_AudioDatabase.FirstOrDefault(x => x.name == clipName);
        }

        public void PlayAudio(string clipName, float volume, AudioSource source, bool loop = false) {
            var _audio = this.AudioByName(clipName); 

            if (_audio != null) {
                source.clip = _audio.Clip;
                source.volume = volume;
                source.pitch = _audio.Pitch;
                source.loop = loop;
                source.Play();
            } else {
                Debug.LogWarning("Audio clip not found in the database: " + clipName);
            }
        }
    }
}
