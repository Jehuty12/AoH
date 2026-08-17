using AgeOfHistory.Simulation;
using Unity.Netcode;
using UnityEngine;

namespace AgeOfHistory.Networking
{
    /// <summary>
    /// Cœur du lockstep en réseau. Principe (cf. CLAUDE.md section 2, modèle lockstep déterministe) :
    /// 1. Un client envoie son INTENTION (ex. "je veux attaquer") au serveur/hôte via ServerRpc —
    ///    sans TargetTick, il ne connaît pas encore quand elle sera exécutée.
    /// 2. Le serveur est la SEULE autorité qui fixe le TargetTick : tick serveur courant + un délai
    ///    fixe (InputDelayTicks). Ce délai laisse le temps à la commande d'atteindre tous les
    ///    clients avant le tick où elle doit s'exécuter, pour que la simulation locale de CHAQUE
    ///    client (y compris l'émetteur) l'exécute au même tick, sans attendre bloquant sur le réseau
    ///    tant que la latence reste sous le délai.
    /// 3. Le serveur rediffuse la commande, tick inclus, à TOUS les clients via ClientRpc
    ///    (y compris l'émetteur d'origine — il ne doit jamais exécuter sa propre commande en
    ///    "prédiction" locale sans passer par ce même chemin, sinon son état diverge des autres).
    /// 4. Chaque client (dont l'hôte) reçoit la commande, la reconstruit via CommandFactory,
    ///    et l'enfile dans SON PROPRE SimCommandQueue local — c'est TOUJOURS la simulation
    ///    locale de chaque client qui exécute, jamais le serveur qui "pousse" un résultat.
    ///
    /// LIMITE CONNUE (à traiter avant la Phase suivante, pas un détail à ignorer) : si un client
    /// ne reçoit pas une commande à temps (latence > InputDelayTicks) ou rejoint en cours de
    /// partie, ce squelette n'a PAS de mécanisme de resynchronisation (rattrapage, snapshot d'état).
    /// À concevoir explicitement plutôt que de découvrir le problème en test réel.
    /// </summary>
    public class NetworkCommandRelay : NetworkBehaviour
    {
        [SerializeField] private int inputDelayTicks = 3;
        [SerializeField] private SimulationClock clock;
        [SerializeField] private SimCommandQueue localCommandQueue; // assigné par le bootstrap réseau, pas par l'inspecteur

        public void SetLocalCommandQueue(SimCommandQueue queue) => localCommandQueue = queue;

        // --- Émission (n'importe quel client, y compris l'hôte) ---

        public void SubmitGather(int gathererEntityId, string resourceId, int amount)
        {
            var envelope = CommandFactory.ToEnvelopeGather(gathererEntityId, resourceId, amount, (int)NetworkManager.LocalClientId);
            SubmitCommandServerRpc(envelope);
        }

        public void SubmitProduce(string unitDefinitionId, FixedPoint x, FixedPoint y)
        {
            var envelope = CommandFactory.ToEnvelopeProduce(unitDefinitionId, x, y, (int)NetworkManager.LocalClientId);
            SubmitCommandServerRpc(envelope);
        }

        public void SubmitAttack(int attackerEntityId, int targetEntityId, string attackerUnitDefinitionId)
        {
            var envelope = CommandFactory.ToEnvelopeAttack(attackerEntityId, targetEntityId, attackerUnitDefinitionId, (int)NetworkManager.LocalClientId);
            SubmitCommandServerRpc(envelope);
        }

        // --- Côté serveur/hôte : seule autorité pour fixer le TargetTick ---

        [ServerRpc(RequireOwnership = false)]
        private void SubmitCommandServerRpc(NetworkCommandEnvelope envelope)
        {
            envelope.TargetTick = clock.CurrentTick + inputDelayTicks;
            BroadcastCommandClientRpc(envelope);
        }

        // --- Réception par TOUS les clients (y compris l'émetteur et l'hôte) ---

        [ClientRpc]
        private void BroadcastCommandClientRpc(NetworkCommandEnvelope envelope)
        {
            var registry = DataRegistry.Instance;
            if (registry == null)
            {
                Debug.LogError("[NetworkCommandRelay] DataRegistry introuvable — impossible de résoudre la commande reçue.");
                return;
            }

            var command = CommandFactory.FromEnvelope(envelope, registry);
            if (command != null)
                localCommandQueue.Enqueue(command);
        }
    }
}
