using UnityEngine;
using UnityEngine.UI; // Para elementos de UI (Button, Text)
using TMPro; 

public class LevelSelectManager : MonoBehaviour
{
    [Tooltip("Arrastar aqui os botões de cada fase na ordem, do Level 1 ao último.")]
    public Button[] levelButtons; // Array para arrastar seus botões de fase no Inspector

    [Tooltip("Opcional: Arrastar aqui os componentes TextMeshProUGUI (ou Text) dos botões, se necessário. Se deixado vazio, ele tentará encontrar automaticamente.")]
    public TextMeshProUGUI[] levelTexts; // Para atualizar o texto dos botões (TextMeshPro)
                                         // Se não usar TextMeshPro, mude para: public Text[] levelTexts;

    void OnEnable() // Usar OnEnable para garantir que a UI seja atualizada sempre que a cena for ativada
    {
        UpdateLevelButtons();
    }

    void UpdateLevelButtons()
    {
        if (LevelManager.instance == null)
        {
            Debug.LogError("LevelManager não encontrado! Certifique-se de que ele está carregado antes da cena do MainMenu.");
            return;
        }

        int currentUnlockedLevel = LevelManager.instance.GetCurrentUnlockedLevelIndex();

        for (int i = 0; i < levelButtons.Length; i++)
        {
            Button button = levelButtons[i];
            TextMeshProUGUI buttonText = null; // Inicializa como null

            // Tenta obter o componente de texto do botão
            if (levelTexts != null && levelTexts.Length > i && levelTexts[i] != null)
            {
                buttonText = levelTexts[i];
            }
            else
            {
                // Fallback: tenta encontrar o TextMeshProUGUI (ou Text) como filho do botão
                buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                // Se não usar TextMeshPro, use: buttonText = button.GetComponentInChildren<Text>();
            }

            if (buttonText == null)
            {
                Debug.LogWarning($"Botão '{button.name}' não possui um componente TextMeshProUGUI (ou Text) como filho. Não é possível atualizar o texto.");
            }

            // Desativa todos os listeners para evitar duplicações
            button.onClick.RemoveAllListeners();

            // NOVO: Obtém o nome de exibição da fase do LevelManager
            string levelDisplayName = "";
            if (LevelManager.instance.levelDisplayNames != null && i < LevelManager.instance.levelDisplayNames.Length)
            {
                levelDisplayName = LevelManager.instance.levelDisplayNames[i];
            }
            else
            {
                // Fallback caso o levelDisplayNames não esteja configurado para esta fase
                levelDisplayName = $"Fase {i + 1}";
                Debug.LogWarning($"LevelManager.levelDisplayNames não possui um nome para o índice {i}. Usando fallback '{levelDisplayName}'.");
            }


            // Verifica se a fase está desbloqueada
            if (i <= currentUnlockedLevel)
            {
                button.interactable = true; // Habilita o botão

                // Verifica se a fase já foi completada
                if (LevelManager.instance.IsLevelCompleted(i))
                {
                    if (buttonText != null)
                    {
                        buttonText.text = $"{levelDisplayName} (✓)"; // AGORA USA O NOME PERSONALIZADO
                        buttonText.color = Color.green; // Opcional: muda a cor
                    }
                }
                else
                {
                    if (buttonText != null)
                    {
                        buttonText.text = levelDisplayName; // AGORA USA O NOME PERSONALIZADO
                        buttonText.color = Color.white; // Reseta a cor
                    }
                }

                // Adiciona o listener para carregar a fase
                int levelIndexToLoad = i; // Variável local para closure
                button.onClick.AddListener(() => LevelManager.instance.LoadLevel(levelIndexToLoad));
            }
            else
            {
                // Fase bloqueada
                button.interactable = false; // Desabilita o botão
                if (buttonText != null)
                {
                    buttonText.text = $"{levelDisplayName} (Bloqueada)"; // AGORA USA O NOME PERSONALIZADO
                    buttonText.color = Color.gray; // Escurece o texto
                }
            }
        }
    }

    public void OnClickResetProgress()
    {
        if (LevelManager.instance != null)
        {
            LevelManager.instance.ResetGameProgress();
            UpdateLevelButtons(); // Atualiza o estado dos botões após o reset
            Debug.Log("Progresso do jogo resetado.");
        }
    }
}