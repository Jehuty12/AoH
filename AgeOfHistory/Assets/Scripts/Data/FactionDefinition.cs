using UnityEngine;

namespace AgeOfHistory.Data
{
    /// <summary>
    /// Regroupement régional (Europe, Asie, Afrique, Amérique) qui structure le mode Histoire.
    /// Ne porte pas de bonus de gameplay lui-même : c'est un simple conteneur d'organisation
    /// pour les civilisations (pays) qu'il regroupe. Voir CivilizationDefinition pour les bonus.
    /// </summary>
    [CreateAssetMenu(fileName = "Faction_", menuName = "AgeOfHistory/Faction Definition")]
    public class FactionDefinition : ScriptableObject
    {
        public string factionId;    // ex: "faction_europe"
        public string displayName;
        [TextArea] public string description;
    }
}
