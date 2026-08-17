using System.Collections.Generic;
using System.Linq;
using AgeOfHistory.Data;

namespace AgeOfHistory.Simulation
{
    /// <summary>
    /// État de simulation pur (POCO), volontairement SÉPARÉ des GameObjects Unity.
    /// C'est cet état qui doit être strictement identique sur tous les clients à un tick donné.
    /// La représentation visuelle (GameObjects, animations, effets) est synchronisée en LECTURE
    /// SEULE à partir de cet état par une couche de "vue" séparée (voir EntityViewSync) —
    /// jamais l'inverse, et jamais de logique de gameplay dans la couche de vue.
    ///
    /// DÉTERMINISME : les positions utilisent FixedPoint (virgule fixe Q16.16), pas float —
    /// voir FixedPoint.cs. CurrentHitPoints est encore en float ; à convertir en FixedPoint
    /// avant d'implémenter le combat (les calculs de dégâts doivent être déterministes eux aussi).
    /// </summary>
    public class SimWorld
    {
        private readonly Dictionary<int, SimEntity> _entities = new Dictionary<int, SimEntity>();
        private readonly Dictionary<int, PlayerState> _players = new Dictionary<int, PlayerState>();
        private int _nextEntityId = 1;

        public IReadOnlyDictionary<int, SimEntity> Entities => _entities;

        public void RegisterPlayer(int playerId, EraDefinition startingEra)
        {
            _players[playerId] = new PlayerState(playerId, startingEra);
        }

        public PlayerState GetPlayer(int playerId) => _players[playerId];

        /// <summary>ID déterministe : toujours attribué dans le même ordre sur tous les clients
        /// car appelé uniquement depuis l'exécution ordonnée des commandes (SimCommandQueue).</summary>
        public SimEntity SpawnEntity(int ownerPlayerId, UnitDefinition definition, FixedPoint x, FixedPoint y)
        {
            var entity = new SimEntity
            {
                EntityId = _nextEntityId++,
                OwnerPlayerId = ownerPlayerId,
                UnitDefinitionId = definition.unitId,
                PositionX = x,
                PositionY = y,
                CurrentHitPoints = FixedPoint.FromDesignValue(definition.hitPoints)
            };
            _entities[entity.EntityId] = entity;
            return entity;
        }

        public void RemoveEntity(int entityId) => _entities.Remove(entityId);

        /// <summary>Itération toujours dans le même ordre (trié par EntityId) — jamais l'ordre
        /// naturel d'un Dictionary, qui n'est pas garanti identique entre exécutions/plateformes.</summary>
        public IEnumerable<SimEntity> EntitiesOrdered() => _entities.Values.OrderBy(e => e.EntityId);
    }

    public class SimEntity
    {
        public int EntityId;
        public int OwnerPlayerId;
        public string UnitDefinitionId;
        public FixedPoint PositionX;
        public FixedPoint PositionY;
        public FixedPoint CurrentHitPoints;

        public bool IsAlive => CurrentHitPoints > FixedPoint.Zero;
    }

    public class PlayerState
    {
        public int PlayerId { get; }
        public EraDefinition CurrentEra { get; set; }
        public Dictionary<string, int> ResourceStock { get; } = new Dictionary<string, int>();

        public PlayerState(int playerId, EraDefinition startingEra)
        {
            PlayerId = playerId;
            CurrentEra = startingEra;
        }

        public int GetResource(string resourceId) =>
            ResourceStock.TryGetValue(resourceId, out var amount) ? amount : 0;

        public void AddResource(string resourceId, int amount)
        {
            ResourceStock[resourceId] = GetResource(resourceId) + amount;
        }

        public bool TrySpend(IEnumerable<ResourceCost> costs)
        {
            var costList = costs.ToList();
            if (costList.Any(c => GetResource(c.resource.resourceId) < c.amount))
                return false;

            foreach (var cost in costList)
                ResourceStock[cost.resource.resourceId] = GetResource(cost.resource.resourceId) - cost.amount;
            return true;
        }
    }
}
