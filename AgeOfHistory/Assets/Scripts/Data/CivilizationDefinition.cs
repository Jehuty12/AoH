using System.Collections.Generic;
using UnityEngine;

namespace AgeOfHistory.Data
{
    /// <summary>
    /// Un pays jouable (= la civilisation au sens strict, cf. section 5bis du cahier des charges).
    /// Regroupé sous une faction mondiale (Europe/Asie/Afrique/Amérique) pour l'organisation
    /// du mode Histoire, mais jouable individuellement en partie libre/infinie.
    /// </summary>
    [CreateAssetMenu(fileName = "Civ_", menuName = "AgeOfHistory/Civilization Definition")]
    public class CivilizationDefinition : ScriptableObject
    {
        [Header("Identité")]
        public string civilizationId;   // ex: "civ_france"
        public string displayName;
        public FactionDefinition faction;

        [Header("Bonus (appliqués par-dessus la base commune à toutes les ères)")]
        public List<CivBonus> resourceBonuses;
        public List<UnitDefinition> uniqueUnits;
        public List<BuildingDefinition> uniqueBuildings;
        public List<TechnologyDefinition> exclusiveTechnologies;

        [Header("Mode Histoire")]
        public List<ScenarioDefinition> campaignScenarios; // propres à ce pays (branches divergentes incluses)
    }

    [System.Serializable]
    public struct CivBonus
    {
        public ResourceType resource;
        public float multiplier;        // ex: 1.15 = +15%
        [TextArea] public string note;  // justification/description du bonus
    }
}
