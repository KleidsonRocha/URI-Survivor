using NUnit.Framework; // Remover se não estiver usando para testes, pode causar warning se não for referência real
using System.Collections.Generic;
using UnityEngine;

public class ExperienceLevelController : MonoBehaviour
{
    public static ExperienceLevelController instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public int currentExperience;

    public ExpPickup pickup;

    public List<int> expLevels;
    public int currentLevel = 1, levelCount = 100;

    void Start()
    {
        // Certifique-se de que a lista de níveis não está vazia para evitar erros
        if (expLevels.Count == 0)
        {
            expLevels.Add(0); // Nível 0 precisa de 0 EXP para começar
            expLevels.Add(10); // Nível 1 precisa de 10 EXP (primeiro nível real)
        }

        while (expLevels.Count < levelCount)
        {
            // Calcula o próximo nível baseado no anterior, com um aumento de 10%
            expLevels.Add(Mathf.CeilToInt(expLevels[expLevels.Count - 1] * 1.1f));
        }

        if (UIController.instance != null)
        {
            UIController.instance.UpdateExperience(currentExperience, expLevels[currentLevel], currentLevel);
        }
    }

    void Update()
    {

    }

    public void GetExp(int amountToGet)
    {
        currentExperience += amountToGet;


        if (UIController.instance != null)
        {
            UIController.instance.AnimateExpBarGain();
        }

        if (currentExperience >= expLevels[currentLevel])
        {
            LevelUp();
        }

        else if (UIController.instance != null)
        {
            UIController.instance.UpdateExperience(currentExperience, expLevels[currentLevel], currentLevel);
        }
    }

    public void SpawnExp(Vector3 position, int xpValue)
    {
        if (pickup != null)
        {
            Instantiate(pickup, position, Quaternion.identity).expValue = xpValue;
        }
        else
        {
            Debug.LogWarning("ExperienceLevelController: ExpPickup prefab não atribuído. Não é possível spawnar EXP.");
        }
    }

    void LevelUp()
    {
        // Remove a EXP necessária para o nível atual
        currentExperience -= expLevels[currentLevel];
        currentLevel++; 

        // Impede que o nível exceda o máximo definido
        if (currentLevel >= expLevels.Count)
        {
            currentLevel = expLevels.Count - 1;
            currentExperience = 0; 
        }


        if (UIController.instance != null)
        {
            UIController.instance.AnimateExpBarLevelUp();
           
            UIController.instance.UpdateExperience(currentExperience, expLevels[currentLevel], currentLevel);
        }

        // Você pode adicionar mais lógica aqui, como conceder pontos de habilidade, restaurar vida, etc.

        Debug.Log($"Parabéns! Você subiu para o nível {currentLevel}!");
    }
}