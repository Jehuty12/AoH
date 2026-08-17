# Age of History

RTS multijoueur à progression historique — de la préhistoire à 2026, puis au-delà (ères spéculatives, paliers procéduraux). Mode Histoire scénarisé par pays avec scénarios partagés multi-perspectives, civilisations asymétriques, économie de marché partagée entre joueurs.

Documents de référence :
- `CLAUDE.md` — contexte projet pour Claude (à la racine, lu automatiquement par Claude Code).
- `docs/cahier-des-charges.md` — cahier des charges complet (pitch, boucle de jeu, ères, roadmap détaillée en 8 phases).

## Statut actuel : Phase 0 — fondation technique

- [x] Structure de données data-driven (ères, civilisations, factions, unités, bâtiments, technologies, scénarios) — `Assets/Scripts/Data/`
- [x] Squelette de simulation déterministe (lockstep) — `Assets/Scripts/Simulation/`
- [x] Squelette du système économique — `Assets/Scripts/Economy/`
- [x] Boucle économie → production jouable en local, un seul client (`GameBootstrap` + `EntityViewSync`, sans réseau réel branché)
- [x] Positions en virgule fixe (`FixedPoint`, Q16.16) — déterminisme garanti sur les positions
- [x] Points de vie en virgule fixe, combat de base (`AttackCommand`) — déterminisme garanti sur les dégâts
- [x] Intégration Netcode for GameObjects (`Assets/Scripts/Networking/`) — relais de commandes, registre de contenu par ID, bootstrap réseau
- [ ] **Prérequis avant tout test réseau** : installer le package Netcode for GameObjects dans le projet Unity, configurer une scène avec `NetworkManager` + `DataRegistry` + `NetworkCommandRelay` (NetworkObject) + `NetworkGameBootstrap`
- [ ] Distribution réelle de la seed de partie par l'hôte (le bootstrap réseau utilise une seed en dur pour l'instant — voir commentaire `PlaceholderSeed`)
- [ ] Test de connexion réseau réel (au moins 2 clients en LAN)
- [ ] Mécanisme de resynchronisation (client en retard, connexion en cours de partie) — non traité, limite connue et documentée dans `CLAUDE.md`
- [ ] Portée, temps d'attaque, pathfinding vers la cible (le combat actuel est instantané, sans déplacement)

## Stack

Unity + C#, Netcode for GameObjects, simulation en lockstep déterministe (voir `CLAUDE.md` section 2 pour les règles strictes de déterminisme à respecter dans tout code de simulation).

## Structure du projet

```
Assets/Scripts/
  Data/         Définitions data-driven (ScriptableObjects) : ères, civilisations, factions,
                unités, bâtiments, technologies, scénarios du mode Histoire.
  Simulation/   Cœur déterministe : commandes réseau, horloge à tick fixe, RNG synchronisé,
                virgule fixe (FixedPoint), état de simulation (SimWorld), commandes de gameplay
                (récolte, production, attaque), GameBootstrap (test local sans réseau),
                EntityViewSync (pont vers les GameObjects affichés).
  Networking/   Intégration Netcode for GameObjects : NetworkCommandRelay (relais des commandes,
                seul le serveur fixe le TargetTick), DataRegistry (résolution des définitions par
                ID string sur le réseau), NetworkCommandEnvelope (format de sérialisation),
                CommandFactory (conversion commande <-> enveloppe réseau), NetworkGameBootstrap
                (point d'entrée du jeu réel, remplace GameBootstrap une fois le réseau branché).
  Economy/      Marché partagé entre joueurs (squelette, logique de cours à concevoir).
```

## Prochaines étapes

Voir `CLAUDE.md` et le cahier des charges pour la suite de la Phase 0, puis Phase 1 (2-3 premières ères jouables).
