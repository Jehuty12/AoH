using System.Collections.Generic;
using System.Linq;

namespace AgeOfHistory.Simulation
{
    /// <summary>
    /// Contexte partagé passé à chaque commande lors de son exécution (accès à l'état de partie,
    /// au générateur déterministe, etc.). À étendre au fil du développement (registre des entités,
    /// état du marché économique, etc.) sans jamais y injecter de référence à Unity.Random ou
    /// à un état non-déterministe.
    /// </summary>
    public class SimulationContext
    {
        public DeterministicRandom Random { get; }
        public SimWorld World { get; }
        public int CurrentTick { get; internal set; }

        public SimulationContext(uint randomSeed, SimWorld world)
        {
            Random = new DeterministicRandom(randomSeed);
            World = world;
        }
    }

    /// <summary>
    /// Reçoit les commandes de tous les clients (via NGO) et les exécute dans un ordre
    /// STRICTEMENT identique sur toutes les machines : d'abord par tick cible, puis par
    /// IssuerPlayerId à tick égal (jamais par ordre d'arrivée réseau, qui varie d'un client
    /// à l'autre).
    /// </summary>
    public class SimCommandQueue
    {
        private readonly List<ISimCommand> _pending = new List<ISimCommand>();
        private readonly SimulationContext _context;

        public SimCommandQueue(SimulationContext context)
        {
            _context = context;
        }

        public void Enqueue(ISimCommand command) => _pending.Add(command);

        /// <summary>À appeler à chaque tick de SimulationClock : exécute toutes les commandes ciblant ce tick.</summary>
        public void ExecuteTick(int tick)
        {
            _context.CurrentTick = tick;

            var due = _pending
                .Where(c => c.TargetTick == tick)
                .OrderBy(c => c.IssuerPlayerId) // ordre déterministe, jamais par arrivée réseau
                .ToList();

            foreach (var command in due)
            {
                command.Execute(_context);
            }

            _pending.RemoveAll(c => c.TargetTick == tick);
        }
    }
}
