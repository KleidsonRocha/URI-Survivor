using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [Header("Referências Obrigatórias")]
    public Tilemap tilemap;
    public Tile[] groundTiles; // Tiles para o chão
    public Tile wallTile;      // Tile para paredes

    [Header("Tamanho do Mapa")]
    [Range(1.0f, 3.0f)]
    public float mapHeightMultiplier = 1.5f; // 1.5x a altura da tela
    [Range(1.0f, 5.0f)]
    public float mapWidthMultiplier = 3.0f;  // 3x a largura da tela

    [Header("Configurações")]
    [Range(1, 3)]
    public int wallThickness = 1; // Espessura da parede
    public int randomSeed = 0;    // 0 = aleatório

    private Camera mainCamera;

    void Start()
    {
        // Encontra a câmera principal
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Câmera principal não encontrada!");
            return;
        }

        // Verifica se tudo está configurado
        if (!ValidateSetup())
        {
            Debug.LogError("MapGenerator não está configurado corretamente!");
            return;
        }

        // Gera o mapa
        GenerateMap();
    }

    bool ValidateSetup()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap não foi atribuído ao MapGenerator!");
            return false;
        }

        if (groundTiles == null || groundTiles.Length == 0)
        {
            Debug.LogError("Ground Tiles não foram atribuídos!");
            return false;
        }

        if (wallTile == null)
        {
            Debug.LogError("Wall Tile não foi atribuído!");
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

        // Calcula o tamanho do mapa baseado na câmera
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

                // Verifica se está na área de parede
                bool isWall = (x < -halfWidth || x > halfWidth || y < -halfHeight || y > halfHeight);

                if (isWall)
                {
                    // Coloca tile de parede
                    tilemap.SetTile(position, wallTile);
                }
                else
                {
                    // Coloca tile de chão aleatório
                    Tile randomGroundTile = groundTiles[Random.Range(0, groundTiles.Length)];
                    tilemap.SetTile(position, randomGroundTile);
                }
            }
        }

        Debug.Log($"Mapa gerado: {mapWidth}x{mapHeight} tiles (área jogável)");
    }

    Vector2 GetCameraSize()
    {
        float height = mainCamera.orthographicSize * 2f;
        float width = height * mainCamera.aspect;
        return new Vector2(width, height);
    }

    // Método para regenerar o mapa (útil para testes)
    [ContextMenu("Regenerar Mapa")]
    public void RegenerateMap()
    {
        if (Application.isPlaying)
        {
            GenerateMap();
        }
    }
}