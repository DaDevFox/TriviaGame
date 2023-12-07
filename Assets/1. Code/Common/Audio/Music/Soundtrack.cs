using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Common.Audio.Music
{
    [CreateAssetMenu(menuName = "Soundtrack")]
    public class Soundtrack : ScriptableObject
    {
        public string track_tag;
        public string track_name;
        public bool hidden = false;

        public AudioClip[] music;
    }
}
