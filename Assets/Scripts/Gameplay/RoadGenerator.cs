using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    internal sealed class RoadGenerator : MonoBehaviour
    {
        private struct Segment
        {
            public int Index { get; set; }
            public MeshFilter MeshFilter { get; set; }
        }

        private const int MeshCount = 3;
        private const float Epsilon = 0.5f;

        [SerializeField]
        private MeshFilter _segmentPrefab;
        [SerializeField]
        private float _segmentLength = 25;
        [SerializeField]
        private int _segmentResolution = 100;

        private Vector3[] _vertexArray;
        private List<MeshFilter> _freeMeshFilters = new List<MeshFilter>();
        private List<Segment> _visibleSegments = new List<Segment>();
        private Camera _mainCamera;

        private Camera MainCamera
        {
            get
            {
                if (_mainCamera == null)
                {
                    _mainCamera = Camera.main;
                }

                return _mainCamera;
            }
        }

        private void Awake()
        {
            InitializeMeshFilters();
        }

        private void Update()
        {
            var cameraWorldCenter = MainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
            var currentSegment = (int)(cameraWorldCenter.x / _segmentLength);

            for (int i = 0; i < _visibleSegments.Count;)
            {
                int segmentIndex = _visibleSegments[i].Index;

                if (!IsSegmentInViewport(segmentIndex))
                {
                    MakeSegmentInvisible(segmentIndex);
                }
                else
                {
                    i++;
                }
            }

            for (int i = currentSegment - 1; i <= currentSegment + 1; i++)
            {
                if (IsSegmentInViewport(i))
                {
                    MakeSegmentVisible(i);
                }
            }
        }

        public void ResetRoad()
        {
            for (int i = 0; i < _visibleSegments.Count; i++)
            {
                var filter = _visibleSegments[i].MeshFilter;
                filter.gameObject.SetActive(false);

                _freeMeshFilters.Add(filter);
            }

            _visibleSegments.Clear();
        }

        private void InitializeMeshFilters()
        {
            _vertexArray = new Vector3[_segmentResolution * 2];

            var iterationsCount = _vertexArray.Length / 2 - 1;
            var triangles = new int[(_vertexArray.Length - 2) * 3];

            for (var i = 0; i < iterationsCount; i++)
            {
                var k = i * 6;
                var j = i * 2;

                triangles[k] = j + 2;
                triangles[k + 1] = j + 1;
                triangles[k + 2] = j + 0;
                triangles[k + 3] = j + 2;
                triangles[k + 4] = j + 3;
                triangles[k + 5] = j + 1;
            }

            for (int i = 0; i < MeshCount; i++)
            {
                var filter = Instantiate(_segmentPrefab);
                var mesh = filter.mesh;

                mesh.Clear();
                mesh.vertices = _vertexArray;
                mesh.triangles = triangles;

                filter.gameObject.SetActive(false);
                _freeMeshFilters.Add(filter);
            }
        }

        private float GetHeight(float position, float parameter)
        {
            return
               (Mathf.Cos(Mathf.Pow(position, 2) * 0.0002f) +
                Mathf.Sin(Mathf.Pow(position, 2) * 0.0010f) +
                Mathf.Cos(Mathf.Pow(position, 2) * 0.0006f) + 3f) * 0.5f;
        }

        public void GenerateSegment(int index, ref Mesh mesh, GameObject segmentGameObject)
        {
            var startPosition = index * _segmentLength;
            var step = _segmentLength / (_segmentResolution - 1);

            for (int i = 0; i < _segmentResolution; i++)
            {
                float xPos = step * i;
                float yPosTop = GetHeight(startPosition + xPos, index);

                _vertexArray[i * 2] = new Vector3(xPos, yPosTop, 0);
                _vertexArray[i * 2 + 1] = new Vector3(xPos, 0, 0);
            }

            mesh.vertices = _vertexArray;
            mesh.RecalculateBounds();

            SetupColliders(segmentGameObject, startPosition, step, index);
        }

        private void SetupColliders(GameObject segmentGameObject, float startPosition, float step, float index)
        {
            var collidersCount = _segmentResolution - 1;
            var colliders = segmentGameObject.GetComponents<BoxCollider2D>();

            if (colliders == null || colliders.Length == 0)
            {
                colliders = AddColliders(segmentGameObject, collidersCount);
            }

            for (int i = 0; i < collidersCount; i++)
            {
                var collider = colliders[i];
                var height = GetHeight(startPosition + step * i, index);

                collider.size = new Vector2(collider.size.x, height);
                collider.offset = new Vector2(collider.offset.x, height * 0.5f);
            }
        }

        private BoxCollider2D[] AddColliders(GameObject segmentGameObject, int collidersCount)
        {
            for (int i = 0; i < collidersCount; i++)
            {
                var collider = segmentGameObject.AddComponent<BoxCollider2D>();

                collider.size = new Vector2(collider.size.x / collidersCount, collider.size.y);
                collider.offset = new Vector2(collider.offset.x / collidersCount + collider.size.x * i, collider.offset.y);
            }

            return segmentGameObject.GetComponents<BoxCollider2D>();
        }

        private bool IsSegmentInViewport(int index)
        {
            var worldLeftBorder = MainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
            var worldRightBorder = MainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0));

            float x1 = index * _segmentLength;
            float x2 = x1 + _segmentLength;

            // Epsilon is used to start segment generation a bit earlier. Empty space shouldn't appears in viewport.
            return x1 <= (worldRightBorder.x + Epsilon) && x2 >= (worldLeftBorder.x - Epsilon);
        }

        private void MakeSegmentVisible(int index)
        {
            if (IsSegmentVisible(index))
            {
                return;
            }

            var meshIndex = _freeMeshFilters.Count - 1;
            var filter = _freeMeshFilters[meshIndex];
            _freeMeshFilters.RemoveAt(meshIndex);

            var mesh = filter.mesh;
            GenerateSegment(index, ref mesh, filter.gameObject);

            filter.transform.position = new Vector3(index * _segmentLength, 0, 0);
            filter.gameObject.SetActive(true);

            var segment = new Segment
            {
                Index = index,
                MeshFilter = filter
            };

            _visibleSegments.Add(segment);
        }

        private void MakeSegmentInvisible(int index)
        {
            if (!IsSegmentVisible(index))
            {
                return;
            }

            var listIndex = FindUsedSegmentIndex(index);
            var segment = _visibleSegments[listIndex];
            _visibleSegments.RemoveAt(listIndex);

            var filter = segment.MeshFilter;
            filter.gameObject.SetActive(false);

            _freeMeshFilters.Add(filter);
        }

        private bool IsSegmentVisible(int index)
        {
            return FindUsedSegmentIndex(index) != -1;
        }

        private int FindUsedSegmentIndex(int index)
        {
            for (int i = 0; i < _visibleSegments.Count; ++i)
            {
                if (_visibleSegments[i].Index == index)
                {
                    return i;
                }
            }

            return -1;
        }
    }

}
