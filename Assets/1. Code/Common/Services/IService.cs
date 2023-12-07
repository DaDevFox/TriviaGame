using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    /// <summary>
    /// A special class that has 1 instance in the game at all times and recieves Unity messages without implementing the GameObject pattern; Also useful for initialization before most game functions/visuals begin
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// The latest service signal, Game.PostInitialize will be called after all service OnInit functions have been called; called after Start; use this to initialize before anything else in the game, except other services
        /// </summary>
        void OnInit();

        void OnDeregester();

        /// <summary>
        /// Called every frame
        /// </summary>
        void Update();

        /// <summary>
        /// Called after every physics frame, which is at a different framerate than the Update method, use <c>Time.fixedDeltaTime</c> to access the delta value for physics calculations. 
        /// </summary>
        void FixedUpdate();

        /// <summary>
        /// Called after Update
        /// </summary>
        void LateUpdate();

        /// <summary>
        /// Use this to initialize before Start
        /// </summary>
        void Awake();

        /// <summary>
        /// Called after Awake, if the ServiceManager object is enabled
        /// </summary>
        void Start();
    }
}
