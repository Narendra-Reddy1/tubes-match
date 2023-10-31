using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [CreateAssetMenu(fileName = "newAudioAsset", menuName = "ScriptableObjects/AudioAsset", order = 1)]
    public class AudioAsset : ScriptableObject
    {
        [SerializeField]
        private SerializedDictionary<AudioID, AudioClip> m_audioDictionary;

        public AudioClip GetAudioClipByID(AudioID audioID) => m_audioDictionary[audioID];
    }
    public enum AudioID
    { 
        ButtonClickSFX,
        BallClickSFX,
        BallMatchSFX,
    }