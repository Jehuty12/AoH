namespace AgeOfHistory.Simulation
{
    /// <summary>
    /// Générateur pseudo-aléatoire déterministe, seedé une fois en début de partie (seed
    /// distribuée par l'hôte à tous les clients avant le premier tick). Ne JAMAIS utiliser
    /// UnityEngine.Random dans un chemin de code exécuté par la simulation (ISimCommand.Execute,
    /// logique de combat, IA, marché économique...) : son état interne n'est pas synchronisable
    /// entre clients et provoquera une désynchronisation de partie.
    ///
    /// Implémentation : xorshift32, simple, rapide, et strictement identique sur toutes les
    /// plateformes cibles (contrairement à des implémentations basées sur des types flottants
    /// qui peuvent varier légèrement selon l'architecture CPU).
    /// </summary>
    public class DeterministicRandom
    {
        private uint _state;

        public DeterministicRandom(uint seed)
        {
            _state = seed == 0 ? 1u : seed; // xorshift ne supporte pas un état à 0
        }

        public uint NextUInt()
        {
            _state ^= _state << 13;
            _state ^= _state >> 17;
            _state ^= _state << 5;
            return _state;
        }

        /// <summary>Entier déterministe dans [min, max).</summary>
        public int NextRange(int min, int max)
        {
            return min + (int)(NextUInt() % (uint)(max - min));
        }
    }
}
