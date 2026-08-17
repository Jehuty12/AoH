using System.Collections.Generic;
using AgeOfHistory.Data;
using UnityEngine;

namespace AgeOfHistory.Networking
{
    /// <summary>
    /// Un ISimCommand peut référencer un UnitDefinition/ResourceType directement en mémoire locale
    /// (Phase 0, test en un seul client), mais un ScriptableObject ne peut pas être envoyé tel quel
    /// sur le réseau. Ce registre fait le pont : les commandes réseau transportent des IDs string
    /// (déjà présents sur chaque définition, ex. UnitDefinition.unitId), et ce registre les résout
    /// vers l'asset local correspondant à réception.
    ///
    /// Condition de correction : IMPÉRATIF que tous les clients chargent exactement le même
    /// contenu (même build/mêmes assets), sinon un même ID pourrait résoudre vers des définitions
    /// différentes selon le client — source de désynchronisation silencieuse.
    /// </summary>
    public class DataRegistry : MonoBehaviour
    {
        public static DataRegistry Instance { get; private set; }

        [SerializeField] private List<UnitDefinition> allUnits;
        [SerializeField] private List<ResourceType> allResources;
        [SerializeField] private List<EraDefinition> allEras;

        private Dictionary<string, UnitDefinition> _unitsById;
        private Dictionary<string, ResourceType> _resourcesById;
        private Dictionary<string, EraDefinition> _erasById;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _unitsById = new Dictionary<string, UnitDefinition>();
            foreach (var unit in allUnits) _unitsById[unit.unitId] = unit;

            _resourcesById = new Dictionary<string, ResourceType>();
            foreach (var resource in allResources) _resourcesById[resource.resourceId] = resource;

            _erasById = new Dictionary<string, EraDefinition>();
            foreach (var era in allEras) _erasById[era.eraId] = era;
        }

        public UnitDefinition GetUnit(string unitId) =>
            _unitsById.TryGetValue(unitId, out var def) ? def : null;

        public ResourceType GetResource(string resourceId) =>
            _resourcesById.TryGetValue(resourceId, out var def) ? def : null;

        public EraDefinition GetEra(string eraId) =>
            _erasById.TryGetValue(eraId, out var def) ? def : null;
    }
}
