//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using Common.Assets;
//using Common.Utils;

//namespace Common.AI
//{
//    public class AIBase : MonoBehaviour
//    {
//        /// <summary>
//        /// How often in seconds iterations occur
//        /// </summary>
//        public float iterationFrequency { get; protected set; } = 10f;

//        /// <summary>
//        /// A measure of how many different objectives the AI can attempt to try at once
//        /// </summary>
//        public int maxAttentionBudget { get; internal set; } = 100;
//        public int attentionBudget { get; internal set; } = -1;

//        protected List<Prompt> validConsiderations = new List<Prompt>();
//        private List<Objective> _inactive = new List<Objective>();
//        private ObjectiveTree _active = new ObjectiveTree();

//        private float iterationProgress = 0f;

//        public Metadata metadata { get; } = new Metadata();

//        #region Initialization

//        private void Awake()
//        {
//            InitConsiderations();
//        }

//        private void InitConsiderations()
//        {
//            foreach (Prompt consideration in validConsiderations)
//                consideration.source = this;
//        }

//        #endregion

//        #region Iteration

//        public void ForceIteration()
//        {
//            Iterate();
//        }


//        private void Update()
//        {
//            UpdateIterationProgress();
//        }

//        private void UpdateIterationProgress()
//        {
//            iterationProgress += Time.deltaTime;
//            if (iterationProgress >= iterationFrequency)
//            {
//                Iterate();
//                iterationProgress = 0;
//            }
//        }

//        private void Iterate()
//        {
//            IterateConsiderations();
//            IterateObjectives();
//        }

//        private void IterateConsiderations()
//        {
//            foreach (Prompt consideration in validConsiderations)
//                if (consideration.Assess())
//                    consideration.OnAction();
//        }

//        private void IterateObjectives()
//        {
//            ProcessInactive();
//            ProgressActive();
//        }

//        private void ProcessInactive()
//        {
//            _inactive.OrderBy((objective) =>
//            {
//                return objective.CalculateFeasability() + objective.priority;
//            });

//            if (attentionBudget == -1)
//                attentionBudget = maxAttentionBudget;

//            foreach (Objective objective in _inactive)
//            {
//                int weight = objective.CalculateFeasability();
//                if (weight < attentionBudget)
//                {
//                    attentionBudget -= weight;
//                    _active.Add(objective);
//                    _inactive.Remove(objective);

//                    objective.weightWhenUsed = weight;
//                }
//            }
//        }

//        private void ProgressActive()
//        {
//            _active.Progress();
//        }


//        #endregion

//        #region Accessors

//        public void AddObjective(Objective objective)
//        {
//            _inactive.Add(objective);
//        }

//        public T GetConsideration<T>() where T : Prompt
//        {
//            foreach (Prompt consideration in validConsiderations)
//                if (consideration is T)
//                    return consideration as T;
//            return null;
//        }

//        public ObjectiveTree GetActive()
//        {
//            return _active;
//        }

//        #endregion

//    }
//}
