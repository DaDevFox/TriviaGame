using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Common.Utils
{
    public class GameObjectProxy : MonoBehaviour
    {
        public static event Action _Start;
        public static event Action _Update;
        public static event Action _FixedUpdate;
        public static event Action _LateUpdate;

        public static GameObjectProxy proxy { get; private set; } = new GameObject("GameObjectProxy").AddComponent<GameObjectProxy>();

        private void Start()
        {
            _Start?.Invoke();
        }

        private void FixedUpdate()
        {
            _FixedUpdate?.Invoke();
        }

        private void Update()
        {
            _Update?.Invoke();
        }

        private void LateUpdate()
        {
            _LateUpdate?.Invoke();
        }

    }
}
