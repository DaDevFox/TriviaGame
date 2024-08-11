using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Common.Utils.Extensions;

namespace Common.Graphics
{
    public class InstanceMeshSystemV2
    {
        public Mesh mesh;
        public Material material;

        private int startSize = 50;

        private Dictionary<int, Dictionary<int, ItemData>> _items = new Dictionary<int, Dictionary<int, ItemData>>();
        private Dictionary<int, Dictionary<int, Matrix4x4>> _matrices = new Dictionary<int, Dictionary<int, Matrix4x4>>();

        public InstanceMeshSystemV2(Mesh mesh, Material mat)
        {
            this.mesh = mesh;
            this.material = mat;

            _items.Add(0, new Dictionary<int, ItemData>());
            _matrices.Add(0, new Dictionary<int, Matrix4x4>());
        }

        public Tuple<int, int> GetFreePos()
        {
            int foundIdx = -1;
            int foundMatrix = -1;
            int matrix = 0;
            int idx = 0;
            while (foundIdx == -1)
            {
                if (_items[matrix].Count >= 1023)
                {
                    idx = 0;
                    matrix++;
                }

                if (!Valid(matrix, idx))
                {
                    foundIdx = idx;
                    foundMatrix = matrix;
                }

                if (idx > 1021)
                {
                    idx = 0;
                    matrix++;
                }

                idx++;
            }

            return new Tuple<int, int>(foundMatrix, foundIdx);
        }


        public ItemData Add()
        {
            Tuple<int, int> found = GetFreePos();

            if (found == null)
                throw new Exception("Could not find free id");

            int matrix = found.Item1;
            int idx = found.Item2;

            ItemData data = new ItemData();

            data.matrix = matrix;
            data.id = idx;

            data.position = new Vector3(0f, 0f, 0f);
            data.rotation = Quaternion.identity;
            data.scale = new Vector3(1f, 1f, 1f);

            if (!_matrices.ContainsKey(matrix))
            {
                _matrices.Add(matrix, new Dictionary<int, Matrix4x4>());
                _items.Add(matrix, new Dictionary<int, ItemData>());
            }


            if (Valid(matrix, idx))
            {
                _items[matrix][idx] = data;
            }
            else
            {
                _items[matrix].Add(idx, data);
                _matrices[matrix].Add(idx, new Matrix4x4());
            }

            //Mod.dLog($"adding at {matrix}:{idx} with lengths {_matrices.Count}:{_items[matrix].Count}");
            // DONE: Error when switching to next matrix up: KeyNotFoundException

            UpdateMatrix(data.matrix, data.id);

            return data;
        }

        public ItemData Add(Vector3 position)
        {
            ItemData data = Add();

            data.position = position;

            _items[data.matrix][data.id] = data;

            UpdateMatrix(data.matrix, data.id);

            return data;
        }

        public ItemData Add(Vector3 position, Quaternion rotation)
        {
            ItemData data = Add();

            data.position = position;
            data.rotation = rotation;

            _items[data.matrix][data.id] = data;

            UpdateMatrix(data.matrix, data.id);

            return data;
        }

        public ItemData Add(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            ItemData data = Add();

            data.position = position;
            data.rotation = rotation;
            data.scale = scale;

            _items[data.matrix][data.id] = data;

            UpdateMatrix(data.matrix, data.id);

            return data;
        }

        public void Update()
        {
            if (SystemInfo.supportsInstancing)
                foreach (Dictionary<int, Matrix4x4> matrices in _matrices.Values)
                    UnityEngine.Graphics.DrawMeshInstanced(mesh, 0, material, matrices.Values.ToArray(), matrices.Count);
            else
                foreach (Dictionary<int, Matrix4x4> matrices in _matrices.Values)
                    foreach (Matrix4x4 matrix in matrices.Values)
                        UnityEngine.Graphics.DrawMesh(mesh, matrix, material, 0);
        }

        private bool Valid(int matrix, int idx)
        {
            return _items.ContainsKey(matrix) && (_items[matrix].ContainsKey(idx));
        }


        public void Set(int matrix, int idx, Vector3 position)
        {
            if (Valid(matrix, idx))
            {
                ItemData data = _items[matrix][idx];
                data.position = position;
            }
        }

        public void Set(int matrix, int idx, Vector3 position, Quaternion rotation)
        {
            if (Valid(matrix, idx))
            {
                ItemData data = _items[matrix][idx];
                data.position = position;
                data.rotation = rotation;
            }
        }

        public void Set(int matrix, int idx, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            if (Valid(matrix, idx))
            {
                ItemData data = _items[matrix][idx];
                data.position = position;
                data.rotation = rotation;
                data.scale = scale;
            }

            UpdateMatrix(matrix, idx);
        }

        public void Set(string tag, Vector3 position)
        {
            if (Get(tag, out ItemData found))
            {
                Set(found.matrix, found.id, position);
            }
            else
            {
                ItemData created = Add(position);
                created.tag = tag;
            }
        }

        public void Set(string tag, Vector3 position, Quaternion rotation)
        {
            if (Get(tag, out ItemData found))
            {
                Set(found.matrix, found.id, position, rotation);
            }

            else
            {
                ItemData created = Add(position, rotation);
                created.tag = tag;
            }
        }

        public void Set(string tag, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            if (Get(tag, out ItemData found))
            {
                Set(found.matrix, found.id, position, rotation, scale);
            }
            else
            {
                ItemData created = Add(position, rotation, scale);
                created.tag = tag;
            }
        }


        public bool Has(int matrix, int idx) => Valid(matrix, idx);

        public bool Has(string tag)
        {
            bool flag = false;

            foreach (Dictionary<int, ItemData> dict in _items.Values)
                foreach (ItemData item in dict.Values)
                    if (item.tag == tag)
                        flag = true;

            return flag;
        }


        public bool Get(int matrix, int idx, out ItemData result)
        {
            result = new ItemData();
            if (Valid(matrix, idx))
            {
                result = _items[matrix][idx];
                return true;
            }
            return false;
        }

        public bool Get(string tag, out ItemData result)
        {
            int matrix = -1;
            int idx = -1;

            foreach (Dictionary<int, ItemData> dict in _items.Values)
            {
                foreach (ItemData item in dict.Values)
                {
                    if (item.tag == tag)
                    {
                        matrix = item.matrix;
                        idx = item.id;
                    }
                }
            }

            return Get(matrix, idx, out result);
        }

        //NOTE: Potential inefficiency; not shifting down when removing and therefore requiring a loop to search for the next free id (see also GetFreeId)
        public void RemoveAt(int matrix, int idx)
        {
            if (!Valid(matrix, idx))
                return;

            _matrices[matrix].Remove(idx);
        }


        public void UpdateMatrix(int matrix, int idx)
        {
            if (Valid(matrix, idx))
                _matrices[matrix][idx] = Matrix4x4.TRS(_items[matrix][idx].position, _items[matrix][idx].rotation, _items[matrix][idx].scale);
            else
                Debug.LogError($"matrix {matrix} with index {idx} not valid");
        }






        public struct ItemData
        {
            /// <summary>
            /// Used by user to identify ItemDatas easily and readably
            /// </summary>
            public string tag;
            public int matrix { get; internal set; }
            public int id { get; internal set; }

            public Vector3 position;
            public Quaternion rotation;
            public Vector3 scale;
        }
    }
}