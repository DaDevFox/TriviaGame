using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Common.Utils.Extensions;

namespace Common.Graphics
{
    public class MultiMeshSystem
    {
        private Dictionary<string, InstanceMeshSystemV2> _systems = new Dictionary<string, InstanceMeshSystemV2>();


        public void AddSystem(string id, Mesh mesh, Material mat) => _systems.Add(id, new InstanceMeshSystemV2(mesh, mat));

        public bool HasSystem(string id) => _systems.ContainsKey(id);

        public void Update()
        {
            foreach(KeyValuePair<string, InstanceMeshSystemV2> pair in _systems)
                pair.Value.Update();
        }


        public InstanceMeshSystemV2 this[string systemId]
        {
            get => _systems.ContainsKey(systemId) ? _systems[systemId] : null;
        }
    }


    public class InstanceMeshSystem
    {
        public Mesh mesh;
        public Material material;

        private int startSize = 50;

        private List<ItemData[]> _items = new List<ItemData[]>();
        private List<Matrix4x4[]> _matrices = new List<Matrix4x4[]>();

        private int currentIdx = 0;
        private int currentMatrix = 0;
        
        public InstanceMeshSystem(Mesh mesh, Material mat)
        {
            this.mesh = mesh;
            this.material = mat;

            _items.Add(new ItemData[startSize]);
            _matrices.Add(new Matrix4x4[startSize]);
        }


        public ItemData Add()
        {
            if(currentIdx > 1021)
            {
                currentIdx = 0;
                currentMatrix++;
            }

            if (_matrices.Count - 1 < currentMatrix)
            {
                _matrices.Add(new Matrix4x4[0]);
                _items.Add(new ItemData[0]);
            }

            ItemData data = default;

            data.matrix = currentMatrix;
            data.id = currentIdx;

            data.position = new Vector3(0f, 0f, 0f);
            data.rotation = Quaternion.identity;
            data.scale = new Vector3(1f, 1f, 1f);

            if (_items[currentMatrix].Length - 1 > currentIdx)
            {
                _items[currentMatrix][currentIdx] = data;
            }
            else
            {
                _items[currentMatrix] = _items[currentMatrix].Add(data);
                _matrices[currentMatrix] = _matrices[currentMatrix].Add();
            }


            UpdateMatrix(data.matrix, data.id);

            currentIdx++;
            return data;
        }
        
        public ItemData Add(Vector3 position)
        {
            ItemData data = Add();

            _items[data.matrix][data.id].position = position;

            UpdateMatrix(data.matrix, data.id);

            return data;
        }

        public ItemData Add(Vector3 position, Quaternion rotation)
        {
            ItemData data = Add();

            _items[data.matrix][data.id].position = position;
            _items[data.matrix][data.id].rotation = rotation;

            UpdateMatrix(data.matrix, data.id);

            return data;
        }

        public ItemData Add(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            ItemData data = Add();

            _items[data.matrix][data.id].position = position;
            _items[data.matrix][data.id].rotation = rotation;
            _items[data.matrix][data.id].scale = scale;


            UpdateMatrix(data.matrix, data.id);

            return data;
        }

        public void Update()
        {
            if (SystemInfo.supportsInstancing)
                foreach(Matrix4x4[] matrices in _matrices)
                    UnityEngine.Graphics.DrawMeshInstanced(mesh, 0, material, matrices, matrices.Length);
            else
                foreach(Matrix4x4[] matrices in _matrices)
                    foreach (Matrix4x4 matrix in matrices)
                        UnityEngine.Graphics.DrawMesh(mesh, matrix, material, 0);
        }

        private bool Valid(int matrix, int idx)
        {
            return _items.Count - 1 >= matrix && (idx >= 0 && idx < _items[matrix].Length);
        }


        public void Set(int matrix, int idx, Vector3 position)
        {
            if (Valid(matrix, idx))
                _items[matrix][idx].position = position;
        }

        public void Set(int matrix, int idx, Vector3 position, Quaternion rotation)
        {
            if (Valid(matrix, idx))
            {
                _items[matrix][idx].position = position;
                _items[matrix][idx].rotation = rotation;
            }
        }

        public void Set(int matrix, int idx, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            if (Valid(matrix, idx))
            {
                _items[matrix][idx].position = position;
                _items[matrix][idx].rotation = rotation;
                _items[matrix][idx].scale = scale;
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

            foreach (ItemData[] array in _items)
                foreach (ItemData item in array)
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

            foreach (ItemData[] array in _items)
            {
                foreach (ItemData item in array)
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
