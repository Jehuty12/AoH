using AgeOfHistory.Data;
using UnityEngine;

namespace AgeOfHistory.Simulation
{
    /// <summary>
    /// Point d'entrée de test pour la Phase 0 : assemble SimulationClock + SimCommandQueue + SimWorld
    /// et fait tourner une boucle locale à un seul joueur, SANS réseau — utile pour tester
    /// rapidement une commande de gameplay en isolation. Une fois Netcode branché, le chemin de jeu
    /// réel passe par Networking/NetworkGameBootstrap.cs (qui pose les mêmes briques mais fait
    /// transiter les commandes par NetworkCommandRelay). Garder ce script à jour comme test
    /// d'isolation, ne pas le supprimer une fois le réseau branché.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private SimulationClock clock;
        [SerializeField] private EraDefinition startingEra;
        [SerializeField] private UnitDefinition testSettlerDefinition;
        [SerializeField] private UnitDefinition testMilitaryDefinition;
        [SerializeField] private ResourceType testResource;
        [SerializeField] private EntityViewSync viewSync;

        private SimWorld _world;
        private SimCommandQueue _commandQueue;
        private const int LocalPlayerId = 0;
        private const uint TestSeed = 12345; // en partie réelle : générée et distribuée par l'hôte

        private void Awake()
        {
            _world = new SimWorld();
            _world.RegisterPlayer(LocalPlayerId, startingEra);

            var context = new SimulationContext(TestSeed, _world);
            _commandQueue = new SimCommandQueue(context);

            clock.OnSimulationTick += _commandQueue.ExecuteTick;

            if (viewSync != null)
                viewSync.Initialize(_world);
        }

        private void Start()
        {
            // Test de bout en bout, partie 1 : un colon récolte, puis produit une unité.
            var settler = _world.SpawnEntity(LocalPlayerId, testSettlerDefinition, FixedPoint.Zero, FixedPoint.Zero);

            _commandQueue.Enqueue(new GatherResourceCommand(
                targetTick: clock.CurrentTick + 5, LocalPlayerId, settler.EntityId, testResource, amount: 50));

            _commandQueue.Enqueue(new ProduceUnitCommand(
                targetTick: clock.CurrentTick + 10, LocalPlayerId, testMilitaryDefinition,
                spawnX: FixedPoint.FromInt(2), spawnY: FixedPoint.Zero));

            // Test de bout en bout, partie 2 : combat, sur deux entités créées directement (pas
            // celle produite ci-dessus) pour ne pas dépendre du timing d'exécution de la commande
            // de production. Une fois Netcode branché, l'EntityId d'une unité produite sera connu
            // côté client via l'écho serveur de la commande — pas à deviner comme ici.
            const int EnemyPlayerId = 1;
            _world.RegisterPlayer(EnemyPlayerId, startingEra);
            var attacker = _world.SpawnEntity(LocalPlayerId, testMilitaryDefinition, FixedPoint.FromInt(4), FixedPoint.Zero);
            var enemyTarget = _world.SpawnEntity(EnemyPlayerId, testMilitaryDefinition, FixedPoint.FromInt(5), FixedPoint.Zero);

            _commandQueue.Enqueue(new AttackCommand(
                targetTick: clock.CurrentTick + 15, LocalPlayerId,
                attacker.EntityId, enemyTarget.EntityId, testMilitaryDefinition));
        }
    }
}
