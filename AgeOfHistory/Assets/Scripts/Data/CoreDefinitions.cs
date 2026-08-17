using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHistory.Data
{
    /// <summary>Type de ressource (bois, or, pétrole, cryptomonnaie...). Extensible sans code.</summary>
    [CreateAssetMenu(fileName = "Resource_", menuName = "AgeOfHistory/Resource Type")]
    public class ResourceType : ScriptableObject
    {
        public string resourceId;
        public string displayName;
        public bool isSpeculative; // true pour crypto/finance quantique (volatilité de marché)
    }

    /// <summary>Définition data-driven d'une unité, valable pour toutes les ères qui la référencent.</summary>
    [CreateAssetMenu(fileName = "Unit_", menuName = "AgeOfHistory/Unit Definition")]
    public class UnitDefinition : ScriptableObject
    {
        public string unitId;
        public string displayName;
        public GameObject prefab;
        public int populationCost;
        public List<ResourceCost> productionCost;
        public float hitPoints;
        public float attackPower;
        public float moveSpeed;
    }

    /// <summary>Définition data-driven d'un bâtiment.</summary>
    [CreateAssetMenu(fileName = "Building_", menuName = "AgeOfHistory/Building Definition")]
    public class BuildingDefinition : ScriptableObject
    {
        public string buildingId;
        public string displayName;
        public GameObject prefab;
        public List<ResourceCost> constructionCost;
        public float hitPoints;
    }

    /// <summary>Définition data-driven d'une technologie de recherche.</summary>
    [CreateAssetMenu(fileName = "Tech_", menuName = "AgeOfHistory/Technology Definition")]
    public class TechnologyDefinition : ScriptableObject
    {
        public string technologyId;
        public string displayName;
        [TextArea] public string effectDescription; // effet à interpréter par les systèmes concernés
        public List<ResourceCost> researchCost;
        public float researchTimeSeconds;
    }

    [System.Serializable]
    public struct ResourceCost
    {
        public ResourceType resource;
        public int amount;
    }

    /// <summary>
    /// Un scénario du mode Histoire. Peut être partagé entre plusieurs pays (scénarios "multi-perspectives",
    /// cf. section 4bis du cahier des charges) via ScenarioCountryVariant : une trame commune + un jeu de
    /// paramètres par pays, jamais des scénarios dupliqués.
    /// </summary>
    [CreateAssetMenu(fileName = "Scenario_", menuName = "AgeOfHistory/Scenario Definition")]
    public class ScenarioDefinition : ScriptableObject
    {
        public string scenarioId;
        public string displayName;
        [TextArea] public string historicalContext;

        [Header("Trame commune")]
        public string mapId;
        public List<ScenarioObjective> sharedObjectives;

        [Header("Variantes par pays (scénario partagé) — laisser vide si scénario mono-pays")]
        public List<ScenarioCountryVariant> countryVariants;

        [Header("Branchement narratif")]
        public List<ScenarioDefinition> nextScenarios; // divergences possibles après ce scénario
    }

    [System.Serializable]
    public struct ScenarioCountryVariant
    {
        public CivilizationDefinition country;
        public Vector2 startPosition;
        public List<UnitDefinition> startingForces;
        [TextArea] public string objectiveNuance; // différence d'objectif propre à ce pays
    }

    [System.Serializable]
    public struct ScenarioObjective
    {
        public string objectiveId;
        [TextArea] public string description;
        public bool isVictoryCondition;
        public bool isDefeatCondition;
    }
}
