namespace AgeOfHistory.Simulation
{
    /// <summary>
    /// Une commande de simulation = la SEULE chose qui transite sur le réseau (modèle lockstep,
    /// cf. CLAUDE.md section 2). Jamais d'état d'unité synchronisé directement.
    ///
    /// RÈGLE ABSOLUE : Execute() doit être 100% déterministe.
    /// - Pas de UnityEngine.Random -> utiliser DeterministicRandom (seedé et synchronisé).
    /// - Pas de dépendance à Time.deltaTime réel -> utiliser le tick fixe de SimulationClock.
    /// - Pas d'itération sur des collections non ordonnées (Dictionary/HashSet) dont l'ordre
    ///   pourrait varier d'un client à l'autre -> trier explicitement ou utiliser des listes ordonnées.
    /// Toute violation de ces règles casse la synchronisation multijoueur silencieusement
    /// (désync détectable seulement après coup) — à vérifier avant tout commit touchant ce fichier
    /// ou une classe qui implémente ISimCommand.
    /// </summary>
    public interface ISimCommand
    {
        /// <summary>Tick de simulation auquel cette commande doit être exécutée (même valeur sur tous les clients).</summary>
        int TargetTick { get; }

        /// <summary>Identifiant du joueur/civilisation à l'origine de la commande.</summary>
        int IssuerPlayerId { get; }

        void Execute(SimulationContext context);
    }
}
