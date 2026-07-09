using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject cubePrefab;

    public int cubeCount = 120;

    public Vector3 spawnCenter = new Vector3(-50, 0, -1);
    public Vector3 areaSize = new Vector3(140, 0, 140);
    public float wallPadding = 8f;
    public Vector3 spawnCheckHalfExtents = new Vector3(1.5f, 1f, 1.5f);
    public int maxSpawnAttempts = 50;

    private readonly int[] powerOf2Numbers = { 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024 };

    void Start()
    {
        SpawnCubes();
    }

    void SpawnCubes()
    {
        for (int i = 0; i < cubeCount; i++)
        {
            Vector3 randomPos;
            if (!TryGetRandomPosition(out randomPos))
            {
                Debug.LogWarning("Could not find a clear cube spawn position.");
                continue;
            }

            GameObject cubeObj = Instantiate(cubePrefab, randomPos, Quaternion.identity);

            Cube cube = cubeObj.GetComponent<Cube>();

            if (cube != null)
            {
                cube.SetNumber(GetSpawnNumber());
            }
        }
    }

    public void RespawnCubes()
    {
        Cube[] allCubes = FindObjectsOfType<Cube>();
        foreach (Cube cube in allCubes)
        {
            if (cube.GetComponent<PlayerMovement>() == null)
            {
                Destroy(cube.gameObject);
            }
        }

        SpawnCubes();
    }

    bool TryGetRandomPosition(out Vector3 position)
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector3 candidate = GetRandomPosition();
            if (IsSpawnPositionClear(candidate))
            {
                position = candidate;
                return true;
            }
        }

        position = Vector3.zero;
        return false;
    }

    Vector3 GetRandomPosition()
    {
        float halfWidth = Mathf.Max(0f, (areaSize.x / 2f) - wallPadding);
        float halfDepth = Mathf.Max(0f, (areaSize.z / 2f) - wallPadding);

        float x = Random.Range(spawnCenter.x - halfWidth, spawnCenter.x + halfWidth);
        float z = Random.Range(spawnCenter.z - halfDepth, spawnCenter.z + halfDepth);

        return new Vector3(x, 0.5f, z);
    }

    bool IsSpawnPositionClear(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapBox(position, spawnCheckHalfExtents, Quaternion.identity, ~0, QueryTriggerInteraction.Collide);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (IsBlockedCollider(colliders[i]))
            {
                return false;
            }
        }

        return true;
    }

    bool IsBlockedCollider(Collider collider)
    {
        if (collider.CompareTag("Obstacle") || collider.CompareTag("Wall") || collider.CompareTag("Cube"))
        {
            return true;
        }

        if (collider.GetComponentInParent<PlayerMovement>() != null)
        {
            return true;
        }

        return collider.name.Contains("Obstacle");
    }

    int GetSpawnNumber()
    {
        return powerOf2Numbers[Random.Range(0, powerOf2Numbers.Length)];
    }
}
