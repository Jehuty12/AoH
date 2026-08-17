using AgeOfHistory.Data;

namespace AgeOfHistory.Simulation
{
    /// <summary>
    /// Un colon récolte une ressource. Commande volontairement simple pour la Phase 0 : pas encore
    /// de trajet/pathfinding, la récolte est instantanée à l'exécution pour valider la boucle
    /// complète (commande réseau -> exécution déterministe -> effet sur l'état du joueur).
    /// Le trajet/temps de récolte réaliste viendra remplacer ce squelette en Phase 1.
    /// </summary>
    public class GatherResourceCommand : ISimCommand
    {
        public int TargetTick { get; }
        public int IssuerPlayerId { get; }
        private readonly int _gathererEntityId;
        private readonly ResourceType _resource;
        private readonly int _amount;

        public GatherResourceCommand(int targetTick, int issuerPlayerId, int gathererEntityId, ResourceType resource, int amount)
        {
            TargetTick = targetTick;
            IssuerPlayerId = issuerPlayerId;
            _gathererEntityId = gathererEntityId;
            _resource = resource;
            _amount = amount;
        }

        public void Execute(SimulationContext context)
        {
            if (!context.World.Entities.ContainsKey(_gathererEntityId))
                return; // l'entité a pu être détruite entre l'émission et l'exécution de la commande

            context.World.GetPlayer(IssuerPlayerId).AddResource(_resource.resourceId, _amount);
        }
    }

    /// <summary>
    /// Un joueur commande la production d'une unité depuis un bâtiment. Vérifie le coût et
    /// débite les ressources de façon déterministe avant de faire apparaître l'unité.
    /// </summary>
    public class ProduceUnitCommand : ISimCommand
    {
        public int TargetTick { get; }
        public int IssuerPlayerId { get; }
        private readonly UnitDefinition _unitDefinition;
        private readonly FixedPoint _spawnX;
        private readonly FixedPoint _spawnY;

        public ProduceUnitCommand(int targetTick, int issuerPlayerId, UnitDefinition unitDefinition, FixedPoint spawnX, FixedPoint spawnY)
        {
            TargetTick = targetTick;
            IssuerPlayerId = issuerPlayerId;
            _unitDefinition = unitDefinition;
            _spawnX = spawnX;
            _spawnY = spawnY;
        }

        public void Execute(SimulationContext context)
        {
            var player = context.World.GetPlayer(IssuerPlayerId);
            if (!player.TrySpend(_unitDefinition.productionCost))
                return; // ressources insuffisantes : commande ignorée silencieusement (à remonter en UI plus tard)

            context.World.SpawnEntity(IssuerPlayerId, _unitDefinition, _spawnX, _spawnY);
        }
    }
}
