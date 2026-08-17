using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHistory.Data
{
    /// <summary>
    /// Définit une ère du jeu (historique ou spéculative/procédurale).
    /// Principe data-driven : une nouvelle ère = un nouvel asset, jamais une modification de code.
    /// Les ~8 grandes ères historiques sont des assets écrits à la main.
    /// Les ères procédurales (post-2026, paliers 3000+) sont générées via ProceduralEraGenerator
    /// mais respectent EXACTEMENT la même interface que celle-ci, pour que le reste du jeu
    /// (UI, IA, économie) n'ait jamais à distinguer les deux.
    /// </summary>
    [CreateAssetMenu(fileName = "Era_", menuName = "AgeOfHistory/Era Definition")]
    public class EraDefinition : ScriptableObject
    {
        [Header("Identité")]
        public string eraId;                 // ex: "era_stone_age", "era_procedural_3120"
        public int eraIndex;                 // numéro d'ordre global (1, 2, 3 ... 3000, 3001...)
        public string displayName;
        [TextArea] public string description;

        [Header("Contenu")]
        public List<TechnologyDefinition> availableTechnologies;
        public List<UnitDefinition> availableUnits;
        public List<BuildingDefinition> availableBuildings;
        public List<ResourceType> resourcesIntroduced;

        [Header("Progression")]
        public EraDefinition previousEra;    // null pour la toute première ère
        public List<TechnologyRequirement> advancementRequirements; // pour passer à l'ère suivante

        [Header("Économie (voir section 3bis du cahier des charges)")]
        public bool marketEnabled;           // actif à partir de l'ère industrielle
        public bool cryptoEnabled;           // actif à partir de l'époque moderne
        public bool quantumFinanceEnabled;   // actif en ères spéculatives

        [Header("Origine")]
        public bool isProcedurallyGenerated; // false pour les ~8 ères écrites à la main
    }

    [System.Serializable]
    public struct TechnologyRequirement
    {
        public TechnologyDefinition technology;
        public int minimumLevel;
    }
}
