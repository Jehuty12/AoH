# Age of History — Contexte projet pour Claude

Ce fichier est lu automatiquement par Claude Code à chaque session sur ce dépôt. Il doit rester la source de vérité courte du projet — le cahier des charges complet (à placer dans `/docs/cahier-des-charges.md`) contient le détail ; ce fichier contient ce qu'il faut savoir *avant de toucher au code*.

## 1. Pitch en une phrase

RTS multijoueur façon Age of Empires, dont la ligne du temps s'étend de la préhistoire à 2026 puis au-delà (ères spéculatives, paliers procéduraux jusqu'à 3000+), avec un mode Histoire scénarisé (campagnes par pays, scénarios partagés multi-perspectives) et une couche économique (bourse/crypto) partagée entre joueurs.

## 2. Stack imposée

- **Moteur** : Unity, C#.
- **Réseau** : Unity Netcode for GameObjects (NGO). Multijoueur natif dès le prototype — ne jamais coder une fonctionnalité de gameplay comme si le jeu était solo puis "ajouter le réseau après".
- **Modèle de simulation** : **lockstep déterministe**. Seuls les ordres/commandes du joueur transitent sur le réseau ; chaque client rejoue la même simulation localement à partir des mêmes données et des mêmes commandes, au même tick. Conséquence stricte : **toute la logique de gameplay doit être déterministe** — pas de `Random` non seedé/non synchronisé, pas de dépendance au framerate ou à `Time.deltaTime` réel dans la simulation, pas d'ordre de traitement dépendant de l'itération de collections non ordonnées. Si une fonctionnalité ne peut pas être écrite de façon déterministe, s'arrêter et signaler le problème plutôt que de contourner silencieusement.
- **Mode solo** : ce n'est pas un code séparé, c'est une partie hébergée localement avec un seul joueur, sur la même architecture réseau que le multijoueur.

**Point de vigilance traité — déterminisme flottant** : les positions de simulation utilisent désormais `FixedPoint` (virgule fixe Q16.16, voir `Assets/Scripts/Simulation/FixedPoint.cs`), pas `float`. Règle stricte : `float`/`double` interdits dans tout calcul devant rester synchronisé entre clients (positions, dégâts, ressources...) ; la conversion vers `float` n'est permise que dans la couche de vue (rendu), jamais dans `SimWorld`, `SimCommandQueue`, ou une implémentation d'`ISimCommand`. `SimEntity.CurrentHitPoints` est maintenant en `FixedPoint` aussi (combat de base implémenté, voir `AttackCommand`).

**Réseau — implémenté** (`Assets/Scripts/Networking/`) : `NetworkCommandRelay` reçoit l'intention d'un client (ServerRpc), le serveur fixe seul le `TargetTick` (tick courant + délai fixe d'entrée), rediffuse à tous les clients (ClientRpc) qui enfilent la commande dans leur `SimCommandQueue` locale. `DataRegistry` résout les définitions (unités, ressources) par ID string à réception — jamais de référence directe à un ScriptableObject sur le réseau. `NetworkGameBootstrap` remplace `GameBootstrap` pour le jeu réel (celui-ci reste comme test d'isolation sans réseau).

**Limite connue, non traitée** : pas de mécanisme de resynchronisation (client en retard, connexion en cours de partie). À concevoir explicitement avant tout test au-delà d'un LAN à faible latence.

## 3. Principe non négociable : contenu data-driven

Ères, unités, bâtiments, technologies, civilisations/pays, factions, scénarios de campagne : tout doit être défini comme **donnée externe** (ScriptableObjects Unity ou config JSON), jamais codé en dur dans la logique de jeu. Ajouter une ère, un pays ou une unité doit être une opération de configuration/contenu, pas une modification de code. Avant d'ajouter du contenu, vérifier qu'il rentre dans un schéma de données existant ; si le schéma doit évoluer, le faire évoluer proprement plutôt que de créer un cas spécial.

Systèmes à garder découplés (communication par interfaces/événements, pas par dépendances directes) :
- Système d'ères / progression (historique + procédural post-2026, même interface pour les deux)
- Système de civilisations/pays (bonus, unités uniques)
- Système économique (marché partagé, bourse, cryptomonnaies)
- Mode Histoire (graphe de scénarios, pas une liste linéaire — voir section 4)

## 4. Structure du mode Histoire (spécifique, à ne pas simplifier)

Le mode Histoire est un **graphe de scénarios**, pas une liste par pays :
- Certains scénarios sont **partagés entre plusieurs pays** (ex. débarquement de Normandie : France/Angleterre/États-Unis contre l'Allemagne) — une **trame commune + un jeu de paramètres par pays** (position de départ, forces, objectifs), jamais des copies dupliquées du même scénario.
- Les branches **divergent ensuite** vers une suite propre à chaque pays, fidèle à la chronologie réelle.
- Organisation par **factions mondiales** (Europe, Asie, Afrique, Amérique) → **pays jouables** à l'intérieur de chaque faction, chaque pays = une civilisation avec ses bonus propres.
- Pour le contenu déjà couvert par les jeux Age of Empires existants : reprendre leurs arbres technologiques/structures de scénario comme base, puis enrichir. Pour l'époque moderne et les ères spéculatives (post-2026) : concevoir à partir de zéro, en s'appuyant sur des technologies réellement en développement (calcul quantique, exosquelettes, etc.).

## 5. Système économique

Marché **partagé et compétitif entre tous les joueurs** de la partie (pas un système isolé par joueur), à partir de l'ère industrielle : cours fluctuant, spéculation, puis cryptomonnaies à l'époque moderne, puis finance algorithmique/quantique en ères spéculatives. Simulation centralisée côté hôte/serveur, cohérente avec le modèle lockstep (le marché est un système déterministe comme le reste).

## 6. Règles de travail pour Claude sur ce projet

- **Toujours mettre à jour `README.md`** quand une décision structurante change (stack, architecture, structure de données, scope d'une phase) — ne pas attendre qu'on te le demande explicitement.
- Avant d'implémenter une fonctionnalité de gameplay, vérifier qu'elle respecte le déterminisme lockstep (section 2) et le principe data-driven (section 3). Si un choix d'implémentation semble aller à l'encontre de l'un des deux, le signaler avant de coder plutôt que de trancher seul.
- Ne pas dupliquer du contenu (scénarios, arbres technologiques) quand une variante paramétrable suffit (voir section 4).
- Respecter la roadmap en phases définie dans le cahier des charges complet : ne pas anticiper le contenu d'une phase tant que la fondation technique de la phase précédente n'est pas validée.
- En cas d'ambiguïté sur une règle de design (bonus d'un pays, détail d'un scénario, etc.) non tranchée dans le cahier des charges, poser la question plutôt que d'inventer une réponse définitive.

## 7. Documents de référence

- Cahier des charges complet : `/docs/cahier-des-charges.md` (pitch, boucle de jeu, tableau des ères, roadmap détaillée en 8 phases, points encore ouverts).
