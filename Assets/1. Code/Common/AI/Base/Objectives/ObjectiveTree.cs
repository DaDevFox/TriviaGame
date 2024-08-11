//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Common.AI
//{
//    public class ObjectiveTree : IEnumerable
//    {
//        public AIBase source { get; internal set; }

//        public Objective current { get; private set; }
//        private List<Objective> topLevel = new List<Objective>();

//        public void Add(Objective item)
//        {
//            topLevel.Add(item);
//            item.source = source;
//            item.Init();
//        }

//        public void Progress()
//        {
//            if (current == null)
//                if (topLevel.Count > 0)
//                    current = topLevel[0];

//            if (current.Tick())
//            {
//                current.OnComplete();
//                current.Dispose();
//                source.attentionBudget += current.weightWhenUsed;
//            }
//        }



//        public IEnumerator GetEnumerator()
//        {
//            ObjectiveEnumerator enumerator = new ObjectiveEnumerator();
//            enumerator.Init(topLevel);
//            return (IEnumerator)enumerator;
//        }

//        public class ObjectiveEnumerator : IEnumerator
//        {
//            private List<Objective> _objectives;
//            private int idx = -1;

//            public void Init(List<Objective> topLevel)
//            {
//                foreach (Objective objective in topLevel)
//                    Add(objective);
//            }

//            private void Add(Objective objective)
//            {
//                _objectives.Add(objective);
//                foreach(Objective _objective in objective.SubObjectives)
//                    Add(_objective);
//            }

//            public object Current
//            {
//                get
//                {
//                    try
//                    {
//                        return _objectives[idx];
//                    }
//                    catch(IndexOutOfRangeException)
//                    {
//                        throw new InvalidOperationException();
//                    }
//                }
//            }

//            public bool MoveNext()
//            {
//                idx++;
//                return idx < _objectives.Count;
//            }

//            public void Reset()
//            {
//                idx = -1;
//            }
//        }
//    }
//}
