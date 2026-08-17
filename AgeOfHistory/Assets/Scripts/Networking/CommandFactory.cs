using AgeOfHistory.Simulation;
using UnityEngine;

namespace AgeOfHistory.Networking
{
    /// <summary>
    /// Traduit une ISimCommand locale en enveloppe réseau (envoi) et inversement (réception).
    /// Seul point du code qui a besoin de connaître à la fois les commandes de simulation et
    /// le format réseau — si une nouvelle commande de gameplay est ajoutée, c'est ici (et dans
    /// CommandType) qu'il faut l'enregistrer, nulle part ailleurs.
    /// </summary>
    public static class CommandFactory
    {
        public static NetworkCommandEnvelope ToEnvelopeGather(int gathererEntityId, string resourceId, int amount, int issuerPlayerId)
        {
            return new NetworkCommandEnvelope
            {
                Type = CommandType.GatherResource,
                IssuerPlayerId = issuerPlayerId,
                EntityIdA = gathererEntityId,
                Amount = amount,
                ResourceOrUnitId = resourceId
                // TargetTick laissé à 0 : fixé par le serveur dans NetworkCommandRelay.
            };
        }

        public static NetworkCommandEnvelope ToEnvelopeProduce(string unitDefinitionId, FixedPoint x, FixedPoint y, int issuerPlayerId)
        {
            return new NetworkCommandEnvelope
            {
                Type = CommandType.ProduceUnit,
                IssuerPlayerId = issuerPlayerId,
                PositionX = x,
                PositionY = y,
                ResourceOrUnitId = unitDefinitionId
            };
        }

        public static NetworkCommandEnvelope ToEnvelopeAttack(int attackerEntityId, int targetEntityId, string attackerUnitDefinitionId, int issuerPlayerId)
        {
            return new NetworkCommandEnvelope
            {
                Type = CommandType.Attack,
                IssuerPlayerId = issuerPlayerId,
                EntityIdA = attackerEntityId,
                EntityIdB = targetEntityId,
                ResourceOrUnitId = attackerUnitDefinitionId
            };
        }

        /// <summary>Reconstruit la commande à exécuter localement. Retourne null si une définition
        /// référencée est introuvable dans le registre (contenu désynchronisé entre clients —
        /// ne devrait jamais arriver si tous les clients ont le même build, mais on ne fait jamais
        /// confiance aveuglément à des données reçues par le réseau).</summary>
        public static ISimCommand FromEnvelope(NetworkCommandEnvelope envelope, DataRegistry registry)
        {
            switch (envelope.Type)
            {
                case CommandType.GatherResource:
                    var resource = registry.GetResource(envelope.ResourceOrUnitId);
                    if (resource == null)
                    {
                        Debug.LogError($"[CommandFactory] Ressource inconnue reçue du réseau : {(string)envelope.ResourceOrUnitId}");
                        return null;
                    }
                    return new GatherResourceCommand(envelope.TargetTick, envelope.IssuerPlayerId, envelope.EntityIdA, resource, envelope.Amount);

                case CommandType.ProduceUnit:
                    var unitToProduce = registry.GetUnit(envelope.ResourceOrUnitId);
                    if (unitToProduce == null)
                    {
                        Debug.LogError($"[CommandFactory] Unité inconnue reçue du réseau : {(string)envelope.ResourceOrUnitId}");
                        return null;
                    }
                    return new ProduceUnitCommand(envelope.TargetTick, envelope.IssuerPlayerId, unitToProduce,
                        FixedPoint.FromRaw(envelope.PositionX.RawValue), FixedPoint.FromRaw(envelope.PositionY.RawValue));

                case CommandType.Attack:
                    var attackerDef = registry.GetUnit(envelope.ResourceOrUnitId);
                    if (attackerDef == null)
                    {
                        Debug.LogError($"[CommandFactory] Définition d'attaquant inconnue reçue du réseau : {(string)envelope.ResourceOrUnitId}");
                        return null;
                    }
                    return new AttackCommand(envelope.TargetTick, envelope.IssuerPlayerId, envelope.EntityIdA, envelope.EntityIdB, attackerDef);

                default:
                    Debug.LogError($"[CommandFactory] CommandType non géré : {envelope.Type}");
                    return null;
            }
        }
    }
}
