using AgeOfHistory.Data;

namespace AgeOfHistory.Simulation
{
    /// <summary>
    /// Une unité attaque une autre. Squelette Phase 0 volontairement simple, dans le même esprit
    /// que GatherResourceCommand : pas encore de portée/ligne de vue/déplacement vers la cible,
    /// les dégâts sont appliqués instantanément à l'exécution pour valider la boucle complète.
    /// La portée, les temps d'attaque et le pathfinding viendront enrichir cette commande en
    /// Phase 1, sans changer son principe (rester une commande, jamais une synchronisation d'état).
    ///
    /// La définition de l'attaquant (attackerDefinition) est passée en paramètre plutôt que
    /// récupérée depuis une table globale : elle vient d'un ScriptableObject identique sur tous
    /// les clients (même build), donc la référence est sûre et déterministe.
    /// </summary>
    public class AttackCommand : ISimCommand
    {
        public int TargetTick { get; }
        public int IssuerPlayerId { get; }
        private readonly int _attackerEntityId;
        private readonly int _targetEntityId;
        private readonly UnitDefinition _attackerDefinition;

        public AttackCommand(int targetTick, int issuerPlayerId, int attackerEntityId, int targetEntityId, UnitDefinition attackerDefinition)
        {
            TargetTick = targetTick;
            IssuerPlayerId = issuerPlayerId;
            _attackerEntityId = attackerEntityId;
            _targetEntityId = targetEntityId;
            _attackerDefinition = attackerDefinition;
        }

        public void Execute(SimulationContext context)
        {
            if (!context.World.Entities.TryGetValue(_attackerEntityId, out var attacker) || !attacker.IsAlive)
                return; // l'attaquant a pu mourir entre l'émission et l'exécution de la commande

            if (!context.World.Entities.TryGetValue(_targetEntityId, out var target) || !target.IsAlive)
                return; // la cible a pu déjà mourir (ex. tuée par une autre commande au même tick)

            var damage = FixedPoint.FromDesignValue(_attackerDefinition.attackPower);
            target.CurrentHitPoints = target.CurrentHitPoints - damage;

            if (!target.IsAlive)
                context.World.RemoveEntity(target.EntityId);
        }
    }
}
