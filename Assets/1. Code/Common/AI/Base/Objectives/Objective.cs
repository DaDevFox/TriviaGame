using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.AI
{
    public abstract class Objective
    {
        public AIBase source { get; internal set; }

        public int priority { get; protected set; } = 1;
        public List<Objective> SubObjectives { get; private set; } = new List<Objective>();
        public Objective parent { get; private set; }

        internal int weightWhenUsed = 0;

        /// <summary>
        /// Calculate the amount of effort required to prepare for this objective. 
        /// </summary>
        /// <returns></returns>
        public virtual int CalculateFeasability()
        {
            return 0;
        }

        /// <summary>
        /// Progress towards goal, returns true when complete. 
        /// </summary>
        /// <returns></returns>
        public virtual bool Tick()
        {
            return true;
        }

        /// <summary>
        /// Use this to initialize
        /// </summary>
        public virtual void Init()
        {

        }


        /// <summary>
        /// Called when objective is complete
        /// </summary>
        public virtual void OnComplete()
        {

        }

        public void AddSubObjective(Objective objective)
        {
            this.SubObjectives.Add(objective);
            objective.parent = this;
            objective.source = this.source;
        }

        public void RemoveSubObjective(Objective objective) => objective.Dispose();

        public virtual void Dispose()
        {
            this.parent.SubObjectives.Remove(this);
            this.parent = null;
            this.source = null;
        }
    }

    /// <summary>
    /// Indicates an objective that isn't just a single task that will be deleted upon fullfilment, a master objective is a growing and always present task, such as 
    /// maintaining economic stability, threat assessment, or territory expansion
    /// </summary>
    public abstract class MasterObjective : Objective
    {

    }
}
