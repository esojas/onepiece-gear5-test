//Credit to: Small Hedge Games
using UnityEngine;

namespace SmallHedge.SoundManager
{
    [CreateAssetMenu(menuName = "Small Hedge/Sounds SO", fileName = "Sounds SO")]
    public class SoundsSO : ScriptableObject
    {
        public SoundList[] sounds;
        public SoundList[] music;
#if UNITY_EDITOR
        private void OnValidate()
        {
            string[] enumNames = System.Enum.GetNames(typeof(SoundType));
            for (int i = 0; i < sounds.Length; i++)
                sounds[i].name = i < enumNames.Length ? enumNames[i] : "Unassigned";
        }
#endif
    }
}