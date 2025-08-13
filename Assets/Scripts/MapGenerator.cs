using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [Header("Refer�ncias Obrigat�rias")]
    public Tilemap tilemap;
    public Tile[] groundTiles; // Tiles para o ch�o
    public Tile wallTile;      // Tile para paredes

    [Header("Tamanho do Mapa")]
    [Range(1.0f, 3.0f)]
    public float mapHeightMultiplier = 1.5f; // 1.5x a altura da tela
    [Range(1.0f, 5.0f)]
    public float mapWidthMultiplier = 3.0f;  // 3x a largura da tela

    [Header("Configura��es")]
    [Range(1, 3)]
    public int wallThickness = 1; // Espessura da parede
    public int randomSeed = 0;    // 0 = aleat�rio

    private Camera mainCamera;

    void Start()
    {
        // Encontra a c�mera principal
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("C�mera principal n�o encontrada!");
            return;
        }

        // Verifica se tudo est� configurado
        if (!ValidateSetup())
        {
            Debug.LogError("MapGenerator n�o est� configurado corretamente!");
            return;
        }

        // Gera o mapa
        GenerateMap();
    }

    bool ValidateSetup()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap n�o foi atribu�do ao MapGenerator!");
            return false;
        }

        if (groundTiles == null || groundTiles.Length == 0)
        {
            Debug.LogError("Ground Tiles n�o foram atribu�dos!");
            return false;
        }

        if (wallTile == null)
        {
            Debug.LogError("Wall Tile n�o foi atribu�do!");
            return false;
        }

        return true;
    }

    void GenerateMap()
    {
        // Define seed se especificado
        if (randomSeed != 0)
        {
            Random.InitState(randomSeed);
        }

        // Limpa o mapa atual
        tilemap.ClearAllTiles();

        // Calcula o tamanho do mapa baseado na c�mera
        Vector2 cameraSize = GetCameraSize();
        int mapWidth = Mathf.RoundToInt(cameraSize.x * mapWidthMultiplier);
        int mapHeight = Mathf.RoundToInt(cameraSize.y * mapHeightMultiplier);

        // Calcula os limites
        int halfWidth = mapWidth / 2;
        int halfHeight = mapHeight / 2;

        // Gera o mapa
        for (int x = -halfWidth - wallThickness; x <= halfWidth + wallThickness; x++)
        {
            for (int y = -halfHeight - wallThickness; y <= halfHeight + wallThickness; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                // Verifica se est� na �rea de parede
                bool isWall = (x < -halfWidth || x > halfWidth || y < -halfHeight || y > halfHeight);

                if (isWall)
                {
                    // Coloca tile de parede
                    tilemap.SetTile(position, wallTile);
                }
                else
                {
                    // Coloca tile de ch�o aleat�rio
                    Tile randomGroundTile = groundTiles[Random.Range(0, groundTiles.Length)];
                    tilemap.SetTile(position, randomGroundTile);
                }
            }
        }

        Debug.Log($"Mapa gerado: {mapWidth}x{mapHeight} tiles (�rea jog�vel)");
    }

    Vector2 GetCameraSize()
    {
        float height = mainCamera.orthographicSize * 2f;
        float width = height * mainCamera.aspect;
        return new Vector2(width, height);
    }

    // M�todo para regenerar o mapa (�til para testes)
    [ContextMenu("Regenerar Mapa")]
    public void RegenerateMap()
    {
        if (Application.isPlaying)
        {
            GenerateMap();
        }
    }
}