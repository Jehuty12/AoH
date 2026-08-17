using System.Collections.Generic;
using AgeOfHistory.Data;
using AgeOfHistory.Simulation;

namespace AgeOfHistory.Economy
{
    /// <summary>
    /// Marché partagé et compétitif entre tous les joueurs de la partie (décidé : cf. section 3bis
    /// du cahier des charges — comme dans le monde réel, pas un système isolé par joueur).
    ///
    /// Fait partie de la simulation déterministe : toute variation de cours doit être calculée
    /// à partir de SimulationContext.Random (jamais UnityEngine.Random) et de données déjà connues
    /// de tous les clients (ordres d'achat/vente reçus via ISimCommand), pour rester synchronisée
    /// sans échange d'état supplémentaire.
    ///
    /// Squelette à ce stade — la logique de formation des cours, spéculation, cryptomonnaies et
    /// finance quantique (ères spéculatives) reste à concevoir en détail (voir CLAUDE.md section 5).
    /// </summary>
    public class MarketSimulation
    {
        private readonly Dictionary<string, float> _currentPrices = new Dictionary<string, float>();

        public float GetPrice(ResourceType resource) =>
            _currentPrices.TryGetValue(resource.resourceId, out var price) ? price : 0f;

        /// <summary>Appelé de façon déterministe à chaque tick (ou tous les N ticks) par la simulation.</summary>
        public void UpdateTick(SimulationContext context)
        {
            // TODO : logique de formation des cours (offre/demande agrégée des joueurs,
            // volatilité accrue pour les ressources spéculatives — cf. ResourceType.isSpeculative).
        }
    }

    /// <summary>Commande réseau : un joueur passe un ordre d'achat/vente sur le marché partagé.</summary>
    public class MarketOrderCommand : ISimCommand
    {
        public int TargetTick { get; }
        public int IssuerPlayerId { get; }
        public string ResourceId { get; }
        public int Amount { get; }
        public bool IsBuyOrder { get; }

        public MarketOrderCommand(int targetTick, int issuerPlayerId, string resourceId, int amount, bool isBuyOrder)
        {
            TargetTick = targetTick;
            IssuerPlayerId = issuerPlayerId;
            ResourceId = resourceId;
            Amount = amount;
            IsBuyOrder = isBuyOrder;
        }

        public void Execute(SimulationContext context)
        {
            // TODO : appliquer l'ordre à MarketSimulation partagée.
        }
    }
}
