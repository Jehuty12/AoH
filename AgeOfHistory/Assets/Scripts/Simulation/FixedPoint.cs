namespace AgeOfHistory.Simulation
{
    /// <summary>
    /// Nombre en virgule fixe Q16.16 (16 bits partie entière, 16 bits partie fractionnaire),
    /// stocké dans un long pour éviter tout débordement lors des multiplications. Remplace les
    /// float dans toute la simulation pour garantir un résultat strictement identique sur toutes
    /// les plateformes/CPU — condition nécessaire au lockstep déterministe (cf. CLAUDE.md, point
    /// de vigilance déterminisme flottant, maintenant traité par ce type).
    ///
    /// RÈGLE : ne JAMAIS utiliser float/double dans un calcul qui doit rester synchronisé entre
    /// clients (positions, dégâts, tout ce qui influence l'état du jeu). La conversion vers float
    /// (ToFloat) n'est autorisée que dans la couche de vue (rendu Unity), jamais dans SimWorld,
    /// SimCommandQueue, ou une implémentation d'ISimCommand.
    /// </summary>
    public readonly struct FixedPoint : System.IEquatable<FixedPoint>
    {
        private const int FractionalBits = 16;
        private const long One = 1L << FractionalBits;

        public readonly long RawValue;

        private FixedPoint(long rawValue) => RawValue = rawValue;

        public static readonly FixedPoint Zero = new FixedPoint(0);

        public static FixedPoint FromInt(int value) => new FixedPoint((long)value << FractionalBits);

        /// <summary>Reconstruit un FixedPoint à partir de sa valeur brute (utilisé par la couche
        /// réseau pour désérialiser une position transmise) — ne jamais construire un FixedPoint
        /// à la main autrement qu'à partir d'une valeur brute déjà produite par ce type.</summary>
        public static FixedPoint FromRaw(long rawValue) => new FixedPoint(rawValue);

        /// <summary>
        /// À utiliser uniquement pour des constantes de conception fixées à l'avance (ex. vitesse
        /// d'une unité définie dans les données), jamais pour un calcul produit pendant la
        /// simulation en cours d'exécution — le résultat de cette conversion doit être identique
        /// sur tous les clients car il vient d'une donnée statique, pas d'un calcul runtime.
        /// </summary>
        public static FixedPoint FromDesignValue(float value) => new FixedPoint((long)(value * One));

        /// <summary>Conversion pour le RENDU UNIQUEMENT (couche de vue). Ne jamais réinjecter le
        /// résultat dans un calcul de simulation.</summary>
        public float ToFloat() => RawValue / (float)One;

        public static FixedPoint operator +(FixedPoint a, FixedPoint b) => new FixedPoint(a.RawValue + b.RawValue);
        public static FixedPoint operator -(FixedPoint a, FixedPoint b) => new FixedPoint(a.RawValue - b.RawValue);
        public static FixedPoint operator *(FixedPoint a, FixedPoint b) => new FixedPoint((a.RawValue * b.RawValue) >> FractionalBits);
        public static FixedPoint operator /(FixedPoint a, FixedPoint b) => new FixedPoint((a.RawValue << FractionalBits) / b.RawValue);
        public static bool operator ==(FixedPoint a, FixedPoint b) => a.RawValue == b.RawValue;
        public static bool operator !=(FixedPoint a, FixedPoint b) => a.RawValue != b.RawValue;
        public static bool operator <(FixedPoint a, FixedPoint b) => a.RawValue < b.RawValue;
        public static bool operator >(FixedPoint a, FixedPoint b) => a.RawValue > b.RawValue;
        public static bool operator <=(FixedPoint a, FixedPoint b) => a.RawValue <= b.RawValue;
        public static bool operator >=(FixedPoint a, FixedPoint b) => a.RawValue >= b.RawValue;

        public bool Equals(FixedPoint other) => RawValue == other.RawValue;
        public override bool Equals(object obj) => obj is FixedPoint other && Equals(other);
        public override int GetHashCode() => RawValue.GetHashCode();
        public override string ToString() => ToFloat().ToString("F4");
    }
}
