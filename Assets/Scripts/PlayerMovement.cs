using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    private object uiManager;

    [Header("Movement")] // [] similar to annotation in Java or dart 
    public float speed = 8f;

    [Header("Chain")]
    public List<Cube> chain = new List<Cube>();
    [Header("Chain Visuals")]
    public bool useAutoChainSpacing = true;
    public float chainSurfaceGap = 0.25f;
    public float firstChainCenterSpacing = 5f;
    public float chainCenterSpacing = 5f;
    public float chainVerticalOffset = 0f;

    private Cube playerCube;
    private bool isMerging;

    void Awake()
    {
        uiManager = FindObjectOfType<AutoUIManager>();
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<AutoUIManagerLegacy>();
        }
    }

    void Start()
    {
        playerCube = GetComponent<Cube>();
    }

    void Update()
    {
        MoveForward();
        RotateToMouse();
    }

    void LateUpdate()
    {
        RepositionChain();
    }

    void MoveForward()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);// Time.delatTime gives the time in second between the last frame and current frame 
    }

    void RotateToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 target = hit.point;
            target.y = transform.position.y;// This transform is inherited from the "Component" class which is the base class of "Monobehaviour" class 

            Vector3 direction = target - transform.position;

            if (direction != Vector3.zero)
                transform.forward = direction;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        HandleHit(other);
    }

    void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.collider);
    }

    void HandleHit(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            Cube otherCube = other.GetComponent<Cube>();

            if (otherCube == null) return;

            if (otherCube.number > playerCube.number)
            {
                GameOver();
            }
            else
            {
                CollectCube(otherCube);
            }
        }

        if (IsObstacleOrWall(other))
        {
            GameOver();
        }
    }

    bool IsObstacleOrWall(Collider other)
    {
        return other.CompareTag("Obstacle") || other.CompareTag("Wall") || other.name.Contains("Obstacle");
    }

    void CollectCube(Cube newCube)
    {
        int collectedValue = newCube.number;

        // Disable collider and rigidbody to prevent physics issues
        Collider cubeCollider = newCube.GetComponent<Collider>();
        if (cubeCollider != null)
        {
            cubeCollider.enabled = false;
        }

        Rigidbody cubeRigidbody = newCube.GetComponent<Rigidbody>();
        if (cubeRigidbody != null)
        {
            cubeRigidbody.isKinematic = true;
        }

        newCube.gameObject.SetActive(true);
        newCube.transform.SetParent(transform, false);
        newCube.transform.localScale = Vector3.one;
        newCube.transform.localRotation = Quaternion.identity;
        SetCubeRenderersEnabled(newCube, true);

        InsertCubeInChain(newCube);
        RepositionChain();

        if (uiManager != null)
        {
            if (uiManager is AutoUIManager)
                ((AutoUIManager)uiManager).AddScore(collectedValue);
            else if (uiManager is AutoUIManagerLegacy)
                ((AutoUIManagerLegacy)uiManager).AddScore(collectedValue);
        }

        if (!isMerging)
        {
            StartCoroutine(MergeRoutine());
        }
    }

    void InsertCubeInChain(Cube newCube)
    {
        for (int i = 0; i < chain.Count; i++)
        {
            if (newCube.number > chain[i].number)
            {
                chain.Insert(i, newCube);
                return;
            }
        }

        chain.Add(newCube);
    }

    IEnumerator MergeRoutine()
    {
        isMerging = true;
        yield return new WaitForSeconds(0.2f);

        bool merged = true;
        while (merged)
        {
            merged = false;

            // Merge from the tail toward the player so collected 2s carry forward:
            // 4-2-2 -> 4-4 -> 8.
            for (int i = chain.Count - 1; i >= 0; i--)
            {
                Cube frontCube = GetCubeAtChainIndex(i - 1);
                Cube backCube = chain[i];

                if (frontCube != null && backCube != null && frontCube.number == backCube.number)
                {
                    frontCube.AddNumber(backCube.number);

                    Destroy(backCube.gameObject);
                    chain.RemoveAt(i);
                    RepositionChain();

                    merged = true;
                    yield return new WaitForSeconds(0.2f);
                    break; // Start over after a merge
                }
            }
        }

        isMerging = false;
    }

    Cube GetCubeAtChainIndex(int index)
    {
        if (index < 0)
        {
            return playerCube;
        }

        if (index >= chain.Count)
        {
            return null;
        }

        return chain[index];
    }

    void RepositionChain()
    {
        Vector3 baseCenter = GetVisualCenter(playerCube);
        Vector3 chainDirection = -transform.forward;
        float previousHalfDepth = GetProjectedHalfDepth(playerCube, chainDirection);
        float distanceFromPlayer = 0f;

        for (int i = 0; i < chain.Count; i++)
        {
            if (chain[i] == null) continue;

            chain[i].transform.rotation = transform.rotation;
            chain[i].transform.localScale = Vector3.one;

            if (useAutoChainSpacing)
            {
                float currentHalfDepth = GetProjectedHalfDepth(chain[i], chainDirection);
                distanceFromPlayer += previousHalfDepth + chainSurfaceGap + currentHalfDepth;
                previousHalfDepth = currentHalfDepth;
            }
            else
            {
                distanceFromPlayer = firstChainCenterSpacing + (i * chainCenterSpacing);
            }

            Vector3 targetCenter = baseCenter + chainDirection * distanceFromPlayer;
            targetCenter.y += chainVerticalOffset;

            MoveVisualCenterTo(chain[i], targetCenter);
        }
    }

    Vector3 GetVisualCenter(Cube cube)
    {
        if (cube == null || cube.cubeRenderer == null)
        {
            return transform.position;
        }

        return cube.cubeRenderer.bounds.center;
    }

    void MoveVisualCenterTo(Cube cube, Vector3 targetCenter)
    {
        if (cube.cubeRenderer == null)
        {
            cube.transform.position = targetCenter;
            return;
        }

        cube.transform.position += targetCenter - cube.cubeRenderer.bounds.center;
    }

    float GetProjectedHalfDepth(Cube cube, Vector3 direction)
    {
        if (cube == null || cube.cubeRenderer == null)
        {
            return 0.5f;
        }

        Bounds localBounds = cube.cubeRenderer.localBounds;
        Transform rendererTransform = cube.cubeRenderer.transform;
        Vector3 center = localBounds.center;
        Vector3 extents = localBounds.extents;
        direction = direction.normalized;

        float min = float.PositiveInfinity;
        float max = float.NegativeInfinity;

        for (int x = -1; x <= 1; x += 2)
        {
            for (int y = -1; y <= 1; y += 2)
            {
                for (int z = -1; z <= 1; z += 2)
                {
                    Vector3 localCorner = center + Vector3.Scale(extents, new Vector3(x, y, z));
                    float projected = Vector3.Dot(rendererTransform.TransformPoint(localCorner), direction);
                    min = Mathf.Min(min, projected);
                    max = Mathf.Max(max, projected);
                }
            }
        }

        return (max - min) * 0.5f;
    }

    void SetCubeRenderersEnabled(Cube cube, bool enabled)
    {
        Renderer[] renderers = cube.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = enabled;
        }
    }

    void GameOver()
    {
        if (uiManager != null)
        {
            if (uiManager is AutoUIManager)
                ((AutoUIManager)uiManager).GameOver();
            else if (uiManager is AutoUIManagerLegacy)
                ((AutoUIManagerLegacy)uiManager).GameOver();
        }
        else
        {
            Debug.Log("Game Over");
            Time.timeScale = 0f;
        }
    }

    public void ResetPlayer()
    {
        transform.position = Vector3.zero;
        transform.forward = Vector3.forward;

        for (int i = chain.Count - 1; i >= 0; i--)
        {
            if (chain[i] != null)
            {
                Destroy(chain[i].gameObject);
            }
        }
        chain.Clear();

        if (playerCube != null)
        {
            playerCube.SetNumber(2);
        }
    }
}
