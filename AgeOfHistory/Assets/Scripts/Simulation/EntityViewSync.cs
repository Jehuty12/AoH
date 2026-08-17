using System.Collections.Generic;
using AgeOfHistory.Data;
using UnityEngine;

namespace AgeOfHistory.Simulation
{
    /// <summary>
    /// Fait le pont entre SimWorld (état déterministe, sans Unity) et les GameObjects affichés
    /// à l'écran. Cette classe est en LECTURE SEULE sur SimWorld : elle ne doit jamais modifier
    /// l'état de simulation directement (toute action du joueur doit passer par une ISimCommand
    /// mise en file dans SimCommandQueue). Mélanger logique de gameplay et rendu ici casse la
    /// séparation nécessaire au lockstep déterministe.
    /// </summary>
    public class EntityViewSync : MonoBehaviour
    {
        [SerializeField] private SimulationClock clock;
        [SerializeField] private List<UnitDefinition> knownUnitDefinitions; // pour retrouver le bon prefab

        private SimWorld _world;
        private readonly Dictionary<int, GameObject> _views = new Dictionary<int, GameObject>();

        public void Initialize(SimWorld world)
        {
            _world = world;
            clock.OnSimulationTick += _ => SyncViews();
        }

        private void SyncViews()
        {
            var seenThisTick = new HashSet<int>();

            foreach (var entity in _world.EntitiesOrdered())
            {
                seenThisTick.Add(entity.EntityId);

                if (!_views.TryGetValue(entity.EntityId, out var view))
                {
                    view = CreateView(entity);
                    _views[entity.EntityId] = view;
                }

                view.transform.position = new Vector3(entity.PositionX.ToFloat(), 0f, entity.PositionY.ToFloat());
            }

            // Nettoyage des vues dont l'entité de simulation a disparu.
            var toRemove = new List<int>();
            foreach (var kvp in _views)
            {
                if (!seenThisTick.Contains(kvp.Key))
                {
                    Destroy(kvp.Value);
                    toRemove.Add(kvp.Key);
                }
            }
            foreach (var id in toRemove) _views.Remove(id);
        }

        private GameObject CreateView(SimEntity entity)
        {
            var definition = knownUnitDefinitions.Find(d => d.unitId == entity.UnitDefinitionId);
            var prefab = definition != null ? definition.prefab : null;

            return prefab != null
                ? Instantiate(prefab)
                : GameObject.CreatePrimitive(PrimitiveType.Cube); // repli visuel si le prefab n'est pas encore assigné
        }
    }
}
