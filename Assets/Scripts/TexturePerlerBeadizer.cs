using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PerlerBeadizer
{
    [ExecuteInEditMode]
    public class TexturePerlerBeadizer : MonoBehaviour
    {
        public class Bead
        {
            public GameObject BeadObject {get; private set;}
            public MeshRenderer MRend {get; private set;}
            
            private Material _mat;

            private Vector3 _scale;

            private Vector3 _origScale;
            
            public Bead(GameObject beadPrefab, Vector3 position, Color beadColor, Transform parent)
            {
                BeadObject = Instantiate(beadPrefab, position, Quaternion.Euler(Vector3.zero));
                BeadObject.transform.SetParent(parent);
                BeadObject.transform.name = "Bead";
                BeadObject.transform.tag = "Bead";
                _scale = BeadObject.transform.localScale;
                _origScale = _scale;
                MRend = BeadObject.GetComponentInChildren<MeshRenderer>();
                _mat = new Material(MRend.sharedMaterial);
                SetBeadColor(beadColor);
            }

            public void SetBeadSize(float size)
            {
                if(size > 0)
                {
                    _scale = _origScale * size;
                    BeadObject.transform.localScale = _scale;
                }
            }

            public void SetBeadColor(Color color)
            {
                _mat.color = color;
                MRend.material = _mat;
            }

            public void SetBeadSmoothness(float smoothness)
            {
                _mat.SetFloat("_Glossiness", smoothness);
            }
            public void SetBeadMetallicness(float metallicness)
            {
                _mat.SetFloat("_Metallic", metallicness);
            }

            public void DestroyBead()
            {
                Destroy(BeadObject);
                _mat = null;
            }
        }

        public Texture2D TextureToUse;
        public GameObject BeadPrefab;

        public float BeadSize;

        public float BeadMetallicness;
        public float BeadSmoothness;

        public bool AutoRefresh;

        public bool AutoPositionCamera;

        private Texture2D _tex => TextureToUse;
        private GameObject _bead => BeadPrefab;
        private GameObject _cam;
        private GameObject _beadContainer;
        private List<Bead> _beads;
        private float _scale => BeadSize;
        private float _metal => BeadMetallicness;
        private float _smooth => BeadSmoothness;

        private bool _run = false;

        private bool _setUp = false;

        private bool _autorefresh => AutoRefresh;
        private bool _posCam => AutoPositionCamera;

        private int _curWidth;
        private int _curHeight;

        public void Generate()
        {
            CheckReferences();
            GeneratePerlerBeads();
            RefreshBeads();
        }

        void Start()
        {
            if(!_setUp)
            {
                CheckReferences();
            }
        }

        

        void CheckReferences()
        {
            _cam = transform.Find("Camera").gameObject;
            if(_beadContainer) { DestroyImmediate(_beadContainer); }
            _beadContainer = new GameObject("Bead Container");
            _beadContainer.transform.parent = this.transform;
            _setUp = true;
        }

        void GeneratePerlerBeads()
        {
            if (!_tex.isReadable)
            {
                Debug.LogWarning($"Cannot read texture \"{_tex.name}\" - check its Read/Write import flag.");
                _run = false;
                return;
            }

            GameObject[] existingBeads = GameObject.FindGameObjectsWithTag("Bead");
            foreach(GameObject bead in existingBeads)
            {
                DestroyImmediate(bead);
            }
            _beads = new List<Bead>();
            _tex.filterMode = FilterMode.Point;
            _curWidth = _tex.width;
            _curHeight = _tex.height;
            Vector2[] _coords = new Vector2[_curWidth * _curHeight];
            Color[] _colors = new Color[_curWidth * _curHeight];

            if(_curWidth > _curHeight)
        {
            for(int x = 0; x < _curWidth; x++)
            {
                for(int y = 0; y < _curHeight; y++)
                {
                    _colors[x + y * _curWidth] = _tex.GetPixel(x,y);
                    _coords[x + y * _curWidth] = new Vector2(x,y);
                }
            }
        }
            else 
        {
            for(int y = 0; y < _curHeight; y++)
            {
                for(int x = 0; x < _curWidth; x++)
                {
                    _colors[x + y * _curWidth] = _tex.GetPixel(x,y);
                    _coords[x + y * _curWidth] = new Vector2(x,y);
                }
            }
        }

            for(int px = 0; px < _colors.Length; px++)
            {
                if(_colors[px].a < float.Epsilon) continue;
                Vector3 _pos = transform.position + new Vector3(_coords[px].x, 0, _coords[px].y);
                Bead _newBead = new(_bead, _pos, _colors[px], _beadContainer.transform);
                _beads.Add(_newBead);
            }

            if(_posCam) { PositionCamera(_curWidth, _curHeight); }
            
            _run = true;
        }

        void PositionCamera(int texWidth, int texHeight)
        {
            _cam.transform.position = new Vector3(texWidth / 2, (texWidth + texHeight) / 2, texHeight / 2);
        }

        public void RefreshBeads()
        {
            if(_posCam) { PositionCamera(_curWidth, _curHeight); }
            if(_beads == null) { return; }
            if(_beads.Count == 0) { return; }

            foreach(Bead b in _beads)
            {
                b.SetBeadSize(_scale);
                b.SetBeadMetallicness(_metal);
                b.SetBeadSmoothness(_smooth);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(!_run || !_setUp ||!_autorefresh) {return;}
            RefreshBeads();
        }
    }
}
