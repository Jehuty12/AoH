using AgeOfHistory.Data;
using AgeOfHistory.Simulation;
using Unity.Netcode;
using UnityEngine;

namespace AgeOfHistory.Networking
{
    /// <summary>
    /// Remplace GameBootstrap (Simulation/GameBootstrap.cs, qui reste utile comme test local
    /// sans réseau) une fois Netcode for GameObjects branché. Chaque instance de ce script tourne
    /// sur CHAQUE client (hôte inclus) : il n'y a pas de "simulation serveur" séparée de la
    /// simulation cliente, conformément au modèle lockstep (cf. NetworkCommandRelay).
    ///
    /// PRÉREQUIS PROJET (à faire une fois dans Unity, pas dans ce script) :
    /// - Installer le package "Netcode for GameObjects" via le Package Manager.
    /// - Une scène avec un NetworkManager (choix du transport : Unity Transport suffit pour un
    ///   premier test LAN), un DataRegistry, un NetworkCommandRelay (NetworkObject) et ce script.
    /// </summary>
    public class NetworkGameBootstrap : MonoBehaviour
    {
        [SerializeField] private SimulationClock clock;
        [SerializeField] private NetworkCommandRelay commandRelay;
        [SerializeField] private DataRegistry dataRegistry;
        [SerializeField] private EraDefinition startingEra;

        private SimWorld _world;
        private SimCommandQueue _commandQueue;

        /// <summary>
        /// Seed de partie : DOIT être générée une seule fois par l'hôte et distribuée à tous les
        /// clients avant le premier tick (ex. via un ClientRpc au moment du lancement de partie),
        /// jamais générée indépendamment par chaque client — sinon le hasard diverge dès le
        /// premier appel à DeterministicRandom. Ce bootstrap utilise une seed fixe pour l'instant :
        /// à remplacer par une vraie distribution de seed avant le premier test à 2 machines.
        /// </summary>
        private const uint PlaceholderSeed = 12345;

        private void Awake()
        {
            _world = new SimWorld();
            var context = new SimulationContext(PlaceholderSeed, _world);
            _commandQueue = new SimCommandQueue(context);

            clock.OnSimulationTick += _commandQueue.ExecuteTick;
            commandRelay.SetLocalCommandQueue(_commandQueue);
        }

        /// <summary>À appeler quand un joueur (local ou distant) rejoint la partie — typiquement
        /// depuis un gestionnaire de connexion NGO (NetworkManager.OnClientConnectedCallback),
        /// pas encore câblé ici pour rester focalisé sur le socle de simulation.</summary>
        public void RegisterPlayer(int playerId)
        {
            _world.RegisterPlayer(playerId, startingEra);
        }

        /// <summary>Exemple d'utilisation côté joueur local : demander la production d'une unité.
        /// L'entité n'apparaît PAS immédiatement (pas de prédiction locale dans ce squelette) —
        /// elle apparaîtra au TargetTick fixé par le serveur, en même temps sur tous les clients.
        /// Ajouter une prédiction locale optimiste sera une amélioration de confort à traiter
        /// plus tard, séparément de la correction du modèle lockstep lui-même.</summary>
        public void RequestProduceUnit(UnitDefinition unit, FixedPoint x, FixedPoint y)
        {
            commandRelay.SubmitProduce(unit.unitId, x, y);
        }

        public void RequestGather(int gathererEntityId, ResourceType resource, int amount)
        {
            commandRelay.SubmitGather(gathererEntityId, resource.resourceId, amount);
        }

        public void RequestAttack(int attackerEntityId, int targetEntityId, UnitDefinition attackerDefinition)
        {
            commandRelay.SubmitAttack(attackerEntityId, targetEntityId, attackerDefinition.unitId);
        }

        public SimWorld World => _world; // exposé en lecture pour EntityViewSync
    }
}
