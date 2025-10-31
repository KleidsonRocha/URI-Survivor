using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Importar para carregar cenas

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance; // Inst�ncia �nica do LevelManager

    [Tooltip("Nomes exatos das cenas das suas fases na ordem desejada.")]
    public string[] levelSceneNames; // Array com os nomes das cenas das fases (ex: "Main", "Fase2", "Fase3")

    [Tooltip("Nomes que ser�o exibidos para cada fase na tela de sele��o.")]
    public string[] levelDisplayNames; // NOVO: Nomes personalizados para as fases

    [Tooltip("Nome da cena do menu principal ou sele��o de fases.")]
    public string mainMenuSceneName = "MainMenu"; // Nome da sua cena de menu/sele��o de fases

    public bool gameActive;


    void Awake()
    {
        // Garante que haja apenas uma inst�ncia do LevelManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Mant�m este objeto entre as cenas
        }
        else
        {
            Destroy(gameObject); // Destr�i se j� existir uma inst�ncia
        }
    }

    void Start()
    {
        gameActive = false;
        // Inicializa o PlayerPrefs se for a primeira vez que o jogo est� rodando
        // A fase 0 (primeira fase) � sempre desbloqueada
        if (!PlayerPrefs.HasKey("CurrentUnlockedLevel"))
        {
            PlayerPrefs.SetInt("CurrentUnlockedLevel", 0); // Desbloqueia a primeira fase (�ndice 0)
            PlayerPrefs.Save(); // Salva imediatamente
        }
    }

    // M�todo para carregar uma fase espec�fica pelo �ndice
    public void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelSceneNames.Length)
        {
            SceneManager.LoadScene(levelSceneNames[levelIndex]);
            Debug.Log($"Carregando fase: {levelSceneNames[levelIndex]}");
        }
        else
        {
            Debug.LogError($"Tentativa de carregar �ndice de fase inv�lido: {levelIndex}");
        }
    }

    // M�todo para ser chamado quando uma fase � completada
    public void LevelCompleted()
    {
        // Pega o nome da cena atual
        string currentSceneName = SceneManager.GetActiveScene().name;
        int currentLevelIndex = -1;

        // Encontra o �ndice da fase atual no array levelSceneNames
        for (int i = 0; i < levelSceneNames.Length; i++)
        {
            if (levelSceneNames[i] == currentSceneName)
            {
                currentLevelIndex = i;
                break;
            }
        }

        if (currentLevelIndex != -1)
        {
            // Marca a fase atual como completada
            PlayerPrefs.SetInt("Level_" + currentLevelIndex + "_Completed", 1); // 1 para true, 0 para false
            Debug.Log($"Fase {currentLevelIndex + 1} completada!");

            // Desbloqueia a pr�xima fase, se houver
            int unlockedLevel = PlayerPrefs.GetInt("CurrentUnlockedLevel", 0);
            if (currentLevelIndex + 1 > unlockedLevel && currentLevelIndex + 1 < levelSceneNames.Length)
            {
                PlayerPrefs.SetInt("CurrentUnlockedLevel", currentLevelIndex + 1);
                Debug.Log($"Fase {currentLevelIndex + 2} desbloqueada!"); // +2 porque o �ndice � 0-based
            }
            PlayerPrefs.Save(); // Salva as mudan�as no PlayerPrefs
        }
        else
        {
            Debug.LogWarning($"A cena completada '{currentSceneName}' n�o est� registrada como uma fase em LevelManager.levelSceneNames.");
        }

        // Retorna para a tela de menu principal
        SceneManager.LoadScene(mainMenuSceneName);
    }


    public int GetCurrentUnlockedLevelIndex()
    {
        return PlayerPrefs.GetInt("CurrentUnlockedLevel", 0);
    }

    public bool IsLevelCompleted(int levelIndex)
    {
        return PlayerPrefs.GetInt("Level_" + levelIndex + "_Completed", 0) == 1;
    }

    public void EndLevel()
    {
        gameActive = false;

        UIController.instance.levelEndScreen.SetActive(true);
    }


    public void ResetGameProgress()
    {
        PlayerPrefs.DeleteAll(); // Apaga todas as chaves salvas
        PlayerPrefs.SetInt("CurrentUnlockedLevel", 0); // Redesbloqueia a primeira fase
        PlayerPrefs.Save();
        Debug.Log("Progresso do jogo resetado!");
    }
}