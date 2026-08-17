# Cahier des charges — Age of History
*(nom de travail — voir alternatives en section 8)*

## 1. Pitch

Un jeu de stratégie en temps réel (RTS) inspiré d'**Age of Empires** : gestion de ressources, construction, recherche technologique, production et combat d'unités. La différenciation forte : la ligne du temps est **étendue bien au-delà de ce que fait Age of Empires classique**. Le joueur traverse toutes les ères historiques jusqu'à l'époque actuelle (2026), puis continue dans des **ères spéculatives** (proche futur, ère spatiale, post-singularité...), avec une échelle de progression qui peut grimper très haut (âges/niveaux 3000, 3500, 4000+) grâce à du contenu généré/étendu pour les ères lointaines.

## 2. Boucle de jeu (game loop) — base RTS

1. Le joueur démarre avec un point de départ (colons, un bâtiment principal) à la première ère.
2. Récolte de ressources (bois, nourriture, pierre/or, + ressources spécifiques aux ères plus tardives : pétrole, électricité, données...).
3. Construction de bâtiments, production d'unités civiles et militaires.
4. Recherche technologique dans un arbre par ère : une fois les prérequis remplis, le joueur **avance d'ère** (comme le passage Âge sombre → Âge féodal → Âge des châteaux → Âge impérial dans AoE2).
5. Combat contre IA/joueurs adverses, expansion territoriale, objectifs de victoire (domination, score, survie...).
6. Chaque changement d'ère débloque de nouveaux bâtiments, unités, technologies et un nouveau visuel/thème.

## 3. Système de progression par ères

| Ère | Ressources/thème | Bâtiments/unités typiques |
|---|---|---|
| Âge de pierre | Bois, nourriture | Cabanes, chasseurs, silex |
| Âge du bronze/fer | + pierre, or | Forges, archers, chars |
| Antiquité | Empires, routes commerciales | Légions, catapultes, temples |
| Moyen Âge | Féodalité | Châteaux, chevaliers, arbalétriers |
| Renaissance | Poudre à canon | Canons, mousquetaires, galions |
| Ère industrielle | Charbon, usines, **premières bourses/marchés financiers** | Trains, fusils, artillerie, banques |
| Époque moderne (→2026) | Pétrole, électronique, **marchés boursiers, cryptomonnaies** | Chars, avions, drones, cyber-recherche, plateformes de trading |
| Ères spéculatives (post-2026) | Énergie, données, **finance algorithmique/quantique** | Robots, exosquelettes, unités spatiales, IA autonomes, calcul quantique |

À affiner ère par ère (bon sujet pour des prompts dédiés : "détaille l'arbre technologique de l'ère antiquité").

**Méthode de conception retenue** : pour les ères déjà couvertes par les jeux Age of Empires existants (préhistoire → renaissance/ère industrielle grosso modo), on reprend leurs arbres technologiques et unités comme **base de référence**, puis on les enrichit avec de nouvelles branches (liées à notre système économique, aux civilisations/pays spécifiques, etc.). Pour les ères que AoE ne couvre pas (époque moderne, ères spéculatives post-2026), il faudra construire l'arbre technologique entièrement à partir de zéro — ce sera un des axes de prompts dédiés à venir, ère par ère.

Pour le mode Histoire, même logique : les scénarios déjà traités par des jeux existants (AoE ou d'autres) peuvent servir de référence de structure (objectifs, rythme, mécaniques de script), tout en étant adaptés à notre propre système multi-perspectives/branchement (section 4bis).

Pour les ères spéculatives (post-2026), l'idée est de s'ancrer sur des **technologies réellement en développement aujourd'hui** plutôt que de la pure science-fiction, par exemple : calcul quantique, exosquelettes, IA autonome, énergie de fusion, conquête spatiale privée — en plus des pistes déjà citées (robots, unités spatiales). Ce sera un axe de prompt dédié pour lister et prioriser ces technologies avant de les transformer en contenu jouable.

## 3bis. Système économique (bourse, spéculation, cryptomonnaies)

À partir de l'ère industrielle, une couche économique vient s'ajouter à la gestion de ressources classique :

- **Ère industrielle → moderne** : apparition d'une bourse/marché où les ressources produites peuvent être achetées/vendues à un cours fluctuant, possibilité de spéculer (acheter bas, revendre haut) pour financer l'armée/l'expansion plutôt que par la seule production directe.
- **Époque moderne (→2026)** : ajout des cryptomonnaies comme ressource/mécanique parallèle — volatilité plus forte, gains/risques plus élevés, éventuellement une mécanique de "minage" liée à la puissance de calcul du joueur.
- **Ères spéculatives** : finance algorithmique/quantique — le marché peut devenir prévisible/exploitable via la recherche technologique (bots de trading, calcul quantique appliqué à la spéculation), ouvrant une voie de puissance alternative au combat pur.
- Objectif design : offrir une **voie économique** parallèle à la voie militaire classique (comme la "victoire économique" dans certains 4X), sans que ce soit obligatoire pour progresser.
- **Décidé : marché partagé et compétitif entre tous les joueurs/IA de la partie**, comme dans le monde réel — un même cours de ressource/crypto fluctue pour tout le monde, la spéculation d'un joueur peut donc influencer indirectement les autres (offre/demande commune). Ça implique une simulation de marché centralisée côté serveur/moteur de partie, pas un système isolé par joueur.

## 4. Système de paliers (décidé)

Les paliers 3000/3500/4000 sont un **compteur d'ère diégétique, en mode infini/procédural** :
- Les ~8 grandes ères historiques (préhistoire → 2026) sont écrites à la main, riches et uniques.
- Au-delà de 2026, le jeu bascule en génération procédurale : chaque nouveau tier de recherche ajoute une unité au compteur d'ère (reskins progressifs, montée de stats, variations de tech), ce qui permet de grimper jusqu'à 3000, 4000+ sans contenu dessiné à la main pour chaque palier.
- Une **couche légère de méta-progression** (déblocages permanents entre parties) est ajoutée par-dessus pour la rejouabilité, mais elle n'est pas le moteur du compteur — c'est un bonus.

## 4bis. Mode Histoire (campagnes scénarisées)

En plus du mode "partie libre / infini" ci-dessus, le jeu propose un **mode Histoire** façon campagnes AoE : des scénarios scriptés basés sur des épisodes historiques réels, jouables indépendamment de la progression infinie.

- Chaque scénario a sa propre carte, ses objectifs scriptés (conquête, défense, survie, escorte...), et peut imposer des contraintes historiques (ex. forces de départ asymétriques, renforts programmés, conditions de victoire/défaite fidèles aux faits).
- Exemples d'épisodes jouables : les campagnes napoléoniennes (conquête de l'Europe), la Première Guerre mondiale (front franco-allemand), la Seconde Guerre mondiale (invasion de la France en 1940), et d'autres à définir au fur et à mesure (Antiquité : conquêtes romaines, Moyen Âge : croisades, etc.).
- **Structure par factions mondiales** : les campagnes sont organisées par grande faction/région du monde (Europe, Asie, Afrique, Amérique). À l'intérieur d'une faction, le joueur choisit un **pays précis** (voir confirmation en 5bis), chaque pays ayant sa propre ligne de campagnes.
- **Scénarios partagés multi-perspectives** : certains épisodes historiques sont vécus par plusieurs pays en même temps (ex. le débarquement de Normandie, jouable côté France, Angleterre ou États-Unis, tous contre l'Allemagne). Ces scénarios partagés ont **une même trame de base avec des variations par pays** (position de départ, forces disponibles, objectifs nuancés selon la perspective), puis **divergent ensuite** vers une suite propre à chaque pays, toujours fidèle à l'histoire réelle.
- Concrètement, la structure du mode Histoire n'est pas une simple liste linéaire d'épisodes par pays, mais un **graphe de scénarios** : certains nœuds sont partagés entre plusieurs pays (avec des variantes), d'autres sont propres à un seul pays, et les branches se rejoignent ou divergent au fil de la chronologie réelle.
- Techniquement, chaque scénario = une carte + un fichier de script d'objectifs/événements, indépendant du système procédural infini. Un scénario partagé = une trame commune + un jeu de paramètres par pays (position, forces, objectifs) plutôt que plusieurs scénarios dupliqués — pour éviter de maintenir 3 copies quasi identiques d'un même événement.
- Ce mode est un gros morceau de contenu (recherche historique, équilibrage par scénario) — à traiter par lots, un scénario/époque à la fois, plutôt que tout d'un coup.

## 5bis. Civilisations asymétriques

Chaque civilisation jouable a ses propres avantages, comme dans AoE (bonus économiques, unités uniques, technologies exclusives).

- Chaque civilisation garde une identité cohérente à travers les ères qu'elle traverse (ex. une civilisation orientée cavalerie reste forte en cavalerie de l'Antiquité à l'ère moderne, avec des unités qui évoluent en conséquence).
- Structure de données : une civilisation = un ensemble de modificateurs (bonus de ressources, coûts réduits, unités/bâtiments uniques, technologies exclusives) appliqué par-dessus la base commune à toutes les ères.
- Ce système s'articule avec le mode Histoire : certains scénarios historiques (ex. campagne napoléonienne) peuvent imposer une civilisation précise avec son roster de l'époque.
- À concevoir par vagues : définir d'abord 2-3 civilisations "pilotes" sur une même ère pour valider le système de bonus, avant de le généraliser à toutes les ères et civilisations.
- **Confirmé** : les 4 factions mondiales (Europe, Asie, Afrique, Amérique) regroupent des **pays jouables individuellement** (ex. faction Europe → France, Allemagne, Angleterre...), chaque pays ayant ses propres bonus/unités uniques — c'est ce niveau "pays" qui constitue la civilisation jouable au sens strict, la faction n'étant qu'un regroupement régional.

## 5. Systèmes transverses

- **Arbre technologique** par ère, avec recherche débloquant bâtiments/unités/bonus.
- **IA adverse** (au minimum une IA de base gérant sa propre économie/armée).
- **Carte et pathfinding** : déplacement de groupes d'unités, navigation, brouillard de guerre.
- **Sauvegarde/partie** : sauvegarde de partie en cours, éventuellement progression méta persistante (selon réponse au point 4).
- **Multijoueur** : à trancher — un RTS multijoueur est un chantier technique nettement plus lourd qu'un solo vs IA.

## 6. Stack technique recommandée

Un RTS est bien plus exigeant qu'un jeu d'arcade simple : pathfinding, sélection de groupes d'unités, IA, gestion de beaucoup d'entités simultanées. Ton profil est .NET + cloud, avec de l'expérience web (Angular, PWA) et une machine correcte (i9-9900K, RTX 3070).

### Option A (recommandée pour un solo dev .NET) — Godot 4.x + C#
- Reste dans ton écosystème .NET (capitalise sur ta stack pro, cohérent avec ta recherche de poste Développeur .NET & Cloud).
- Gratuit, léger, navigation/pathfinding intégrés (NavigationServer2D), bon support de grandes scènes.
- Nécessitera de construire toi-même les briques RTS (sélection multiple, ordres de groupe, IA) — pas de template RTS officiel, mais faisable.
- Export Windows/Web pour partager une démo dans ton portfolio.

### Option B — Unity + C#
- Toujours en C#, mais avec un écosystème d'assets RTS beaucoup plus riche (templates de sélection d'unités, pathfinding avancé, RTS starter kits) qui peut faire gagner beaucoup de temps de dev sur un genre aussi complexe.
- Inconvénient : politique de licence/runtime fee plus incertaine, éditeur plus lourd.
- Pour un RTS en solo, c'est objectivement le choix qui réduit le plus le risque technique (le pathfinding/gestion d'unités RTS est un gros morceau, mieux vaut ne pas tout réinventer).

### Option C — Web (TypeScript + PixiJS/Phaser + moteur RTS custom)
- Cohérent avec tes projets Angular/PWA existants, déploiement trivial pour montrer une démo.
- Le plus risqué techniquement pour un RTS (pathfinding, beaucoup d'unités, IA) : tu devrais réimplémenter des briques que Godot/Unity offrent déjà partiellement.

**Décision : Unity + C#.** Les deux moteurs restent en C# (donc aucune perte côté valorisation .NET), mais Unity réduit fortement le risque technique d'un RTS solo grâce à son écosystème d'assets (sélection de groupes, pathfinding avancé, starter kits RTS) — un vrai gain de temps sur un projet aussi ambitieux (campagnes historiques + mode infini + civilisations asymétriques).

### Multijoueur — décision structurante

**Décidé : le multijoueur est prévu dès le départ**, pas ajouté après coup. C'est une décision qui change l'architecture dès la Phase 0 :
- Il faut choisir une solution de réseau dès le prototype (candidats côté Unity : Netcode for GameObjects, Mirror, ou une solution tierce type Photon Fusion) — ce choix conditionne comment sont écrits tous les systèmes de gameplay ensuite.
- La simulation doit être **autoritaire côté serveur/hôte** dès le premier prototype (pas seulement pour le marché économique partagé, mais pour la production, le combat, les déplacements), sinon il faudra tout réécrire plus tard pour gérer la synchronisation.
- Le mode solo vs IA reste indispensable (developpement/test plus simple, et mode de jeu à part entière), mais il doit tourner sur la **même architecture réseau** que le multijoueur (un "solo" = une partie hébergée localement avec un seul joueur), pas sur un code séparé.
- Conséquence directe : le prototype de la Phase 0 doit inclure un test de connexion réseau basique en plus de la boucle de jeu, pas seulement une boucle solo — c'est un des premiers risques techniques à valider.

**Décision : Unity Netcode for GameObjects (NGO), avec un modèle de synchronisation par commandes (lockstep déterministe).**
- **Transport** : NGO plutôt que Mirror ou Photon. C'est la solution officielle Unity — gratuite, documentation et tutoriels les plus abondants, ce qui compte beaucoup pour l'évolutivité et l'arrivée future de collaborateurs (ils trouveront plus facilement des ressources et des devs formés dessus que sur Mirror, et tu évites la dépendance à un service payant tiers comme Photon dont les coûts grimpent avec le nombre de joueurs simultanés).
- **Modèle de synchronisation** : pour un RTS avec potentiellement des dizaines/centaines d'unités, synchroniser l'état complet de chaque unité (position, vie, cible...) sature vite la bande passante. L'approche historique du genre (AoE, StarCraft) est le **lockstep déterministe** : chaque client simule la partie en local à partir des mêmes règles, et seuls les **ordres du joueur** (commandes) sont envoyés sur le réseau et exécutés au même tick par tous les clients. Ça réduit énormément le trafic réseau et scale bien avec le nombre d'unités — c'est le point le plus important à valider techniquement en Phase 0, car toute la logique de jeu doit être écrite de façon déterministe (pas de hasard non synchronisé, pas de dépendance au framerate) pour que ça fonctionne.
- Compromis à connaître : le lockstep déterministe est plus exigeant à mettre en place au départ (toute la simulation doit être déterministe), mais c'est un investissement qui paie sur le long terme vu l'ampleur du projet (multijoueur, mode infini, beaucoup d'unités) — assumé comme faisant partie de l'architecture évolutive de la section 6bis.

## 6bis. Évolutivité du jeu et du code

Vu l'ampleur du projet (des dizaines d'ères, civilisations, scénarios de campagne, système économique), l'architecture doit être pensée dès le prototype pour éviter de tout reconstruire à chaque ajout de contenu. Principes à respecter dès la Phase 0 :

- **Contenu data-driven** : ères, unités, bâtiments, civilisations, technologies et scénarios définis comme des données externes (ScriptableObjects Unity ou fichiers JSON/config), jamais codées en dur dans la logique de jeu. Ajouter une ère ou une civilisation doit être une opération de configuration, pas une modification de code.
- **Systèmes découplés** : le système économique (bourse/crypto), le système de civilisations, le système d'ères et le mode Histoire doivent communiquer par interfaces/événements plutôt que par dépendances directes, pour pouvoir faire évoluer chacun sans casser les autres.
- **Séparation contenu historique / contenu procédural** : le pipeline qui gère les ~8 ères écrites à la main et celui qui génère les tiers procéduraux (au-delà de 2026) doivent partager la même interface de données, pour que le reste du jeu (UI, IA, économie) n'ait pas à distinguer les deux.
- **Tests de non-régression légers** dès que plusieurs ères/civilisations coexistent, pour éviter qu'un ajout casse un contenu existant.
- Cette exigence conforte le choix Unity (ScriptableObjects = outil natif pour ce genre d'architecture data-driven) et sera un point de vigilance explicite dès le prototype (Phase 0), pas quelque chose à rattraper plus tard.

## 7. Roadmap de développement (macro)

**Phase 0 — Prototype technique**
- Setup projet, choix et validation de la solution réseau (multijoueur dès le départ), carte simple, un colon, récolte d'une ressource, un bâtiment, une unité militaire.
- Objectif : boucle économie → production → combat basique jouable en réseau (même en LAN/local à ce stade), une seule ère.

**Phase 1 — Système d'ères (2-3 premières ères)**
- Arbre technologique fonctionnel, transition d'ère, nouveaux bâtiments/unités par ère.

**Phase 2 — IA adverse**
- IA de base capable de gérer son économie et d'attaquer.

**Phase 3 — Extension du contenu historique**
- Ajout successif des ères jusqu'à l'époque moderne (2026).

**Phase 4 — Civilisations asymétriques (pilote)**
- 2-3 civilisations pilotes sur une même ère pour valider le système de bonus/unités uniques, avant généralisation.

**Phase 5 — Ères spéculatives et progression infinie**
- Contenu post-2026, mise en place du système de paliers 3000+ (génération procédurale des tiers), intégration des technologies réelles en développement (calcul quantique, exosquelettes...).

**Phase 5bis — Système économique (bourse/crypto)**
- Marché fluctuant à partir de l'ère industrielle, ajout des cryptomonnaies à l'ère moderne, finance algorithmique/quantique en ères spéculatives.

**Phase 6 — Mode Histoire (campagnes scénarisées)**
- Un scénario pilote (ex. campagne napoléonienne ou 1940) pour valider le système de script d'objectifs, puis extension par lots à d'autres épisodes.

**Phase 7 — Polish**
- UI/UX, équilibrage, son, effets visuels.

**Phase 8 — Publication**
- Export, page itch.io/Steam, intégration au portfolio en ligne.

## 8. Points encore à trancher

1. **Nom du projet** : "Age of History" retenu comme nom de travail. Alternatives possibles si tu veux comparer : *Eras* / *Chronos: Rise of Nations* / *Timeline Wars* / *Empires Through Time* / *Continuum* / *Ascension of Ages*. À vérifier aussi côté disponibilité (nom déjà pris sur Steam/itch.io, proximité trop forte avec la marque "Age of Empires" si tu veux éviter toute confusion commerciale).
2. **Périmètre pilote pour le mode Histoire** : proposition concrète — commencer par **1 seul pays** (la France, par proximité et facilité de recherche historique pour toi) avec un périmètre volontairement restreint pour valider le système avant de généraliser :
   - Un scénario **partagé multi-perspectives** (le débarquement de Normandie, jouable côté France — avec les variantes Angleterre/États-Unis prévues dans la donnée mais pas forcément développées tout de suite).
   - Une **divergence** qui suit (ex. la libération du territoire français après le débarquement, propre à la France).
   - Éventuellement un scénario plus ancien isolé (ex. Napoléon) pour valider que le système fonctionne aussi sur un épisode non partagé.
   - Ça fait 2-3 scénarios pilotes, suffisant pour valider tout le pipeline (carte + script + variantes par pays + branchement) sans se noyer dans le contenu avant même d'avoir un système qui marche.
3. **Solution réseau** : tranchée ci-dessus (Netcode for GameObjects + lockstep déterministe).
