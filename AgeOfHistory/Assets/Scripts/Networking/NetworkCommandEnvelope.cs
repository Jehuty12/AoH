using Unity.Netcode;

namespace AgeOfHistory.Networking
{
    /// <summary>
    /// Représentation réseau d'une ISimCommand : uniquement des types primitifs + des IDs string
    /// (jamais de référence directe à un ScriptableObject, cf. DataRegistry). Le TargetTick est
    /// laissé à 0 par le client émetteur ; c'est le SERVEUR/HÔTE qui le fixe à réception
    /// (tick serveur courant + délai d'entrée fixe), jamais le client — sinon deux clients avec
    /// une horloge locale légèrement différente pourraient viser des ticks différents pour ce
    /// qu'ils croient être "la même" commande.
    /// </summary>
    public struct NetworkCommandEnvelope : INetworkSerializable
    {
        public CommandType Type;
        public int TargetTick;
        public int IssuerPlayerId;

        // Champs génériques réutilisés selon le CommandType (voir CommandFactory pour le mapping).
        public int EntityIdA;   // gatherer / attacker
        public int EntityIdB;   // target (attaque)
        public FixedValue PositionX;
        public FixedValue PositionY;
        public int Amount;
        public FixedString64 ResourceOrUnitId; // ID string de ResourceType ou UnitDefinition selon Type

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Type);
            serializer.SerializeValue(ref TargetTick);
            serializer.SerializeValue(ref IssuerPlayerId);
            serializer.SerializeValue(ref EntityIdA);
            serializer.SerializeValue(ref EntityIdB);
            serializer.SerializeValue(ref PositionX);
            serializer.SerializeValue(ref PositionY);
            serializer.SerializeValue(ref Amount);
            serializer.SerializeValue(ref ResourceOrUnitId);
        }
    }

    public enum CommandType : byte
    {
        GatherResource = 0,
        ProduceUnit = 1,
        Attack = 2
    }

    /// <summary>Wrapper réseau-safe pour FixedPoint (transporte juste le RawValue en long).</summary>
    public struct FixedValue : INetworkSerializable
    {
        public long RawValue;

        public static implicit operator FixedValue(AgeOfHistory.Simulation.FixedPoint fp) =>
            new FixedValue { RawValue = fp.RawValue };

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref RawValue);
        }
    }

    /// <summary>String réseau bornée (évite d'allouer via System.String côté Netcode).</summary>
    public struct FixedString64 : INetworkSerializable
    {
        public Unity.Collections.FixedString64Bytes Value;

        public static implicit operator FixedString64(string s) =>
            new FixedString64 { Value = new Unity.Collections.FixedString64Bytes(s) };

        public static implicit operator string(FixedString64 f) => f.Value.ToString();

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Value);
        }
    }
}
