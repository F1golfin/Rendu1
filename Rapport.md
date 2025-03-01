# Rapport de Projet – Rendu 1

Ce rapport est structuré en deux grandes parties:
1. **Schéma Entité/Association et script SQL**
2. **Prompts issus des IA génératives concernant la visualisation du graphe**


---

## 1. Schéma Entité/Association et script SQL

### 1.1 Schéma Entité/Association (E/A)

Le schéma Entité/Association ci-dessous illustre la structure conceptuelle de la base de données de l’application **LivinParis** :

- **users** : Contient les informations sur les utilisateurs (clients, cuisiniers, livreurs, etc.).
- **commandes** : Regroupe les données relatives aux commandes (heure, adresse de départ, prix total).
- **plats** : Liste les plats disponibles, avec leurs caractéristiques (nom, nombre de parts, date de fabrication, etc.).
- **recettes** : Référence les recettes utilisées pour les plats.
- **lignes_commandes** : Détaille les différentes lignes associées à une commande (adresse d’arrivée, statut de la livraison).
- **evaluations** : Permet aux utilisateurs de noter et commenter les services.

![](/Files/SchemaEA.png "Schéma entités associations")

## 1.2 Modèle Logique des Données (MLD)

![](/Files/SchemaMLD.png "Modèle Logique des Données")

---

### 1.3 Script SQL

Le script suivant crée et initialise la base **livin_paris** avec toutes les tables nécessaires.

```mysql
DROP DATABASE IF EXISTS livin_paris;
CREATE DATABASE livin_paris;
USE livin_paris;

CREATE TABLE users
(
    user_id    BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    password   VARCHAR(255)                       NOT NULL,
    role       SET ('Client', 'Cuisinier')        NOT NULL,
    type       ENUM ('Particulier', 'Entreprise') NOT NULL,
    email      VARCHAR(100) UNIQUE                NOT NULL,
    nom        VARCHAR(50)                        NOT NULL, -- Pour les entreprises contient le nom du contact
    prenom     VARCHAR(50)                        NOT NULL, -- Pour les entreprises contient le prenom du contact
    adresse    VARCHAR(255)                       NOT NULL,
    telephone  VARCHAR(15) UNIQUE                 NOT NULL,
    entreprise VARCHAR(50)                                  -- Pour les entreprises contient le nom de l'entreprise, NULL pour les particuliers

);

CREATE TABLE commandes
(
    commande_id    BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    heure_commande DATETIME,
    adresse_depart TEXT          NOT NULL,
    prix_total     DECIMAL(8, 2) NOT NULL, -- Pourrait etre recalculer
    client_id      BIGINT UNSIGNED,
    cuisinier_id   BIGINT UNSIGNED,

    FOREIGN KEY (client_id) REFERENCES users (user_id) ON DELETE SET NULL,
    FOREIGN KEY (cuisinier_id) REFERENCES users (user_id) ON DELETE SET NULL
);

CREATE TABLE recettes
(
    recette_id         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    nom_recette        VARCHAR(100)                                 NOT NULL,
    type               ENUM ('Entrée', 'Plat Principal', 'Dessert') NOT NULL,
    ingredients        TEXT                                         NOT NULL,
    style_cuisine      INT                                          NOT NULL, -- ENUM ?
    regime_alimentaire VARCHAR(50),                                           -- SET ? null si pas de regime
    parent_recette_id  BIGINT UNSIGNED UNIQUE,

    FOREIGN KEY (parent_recette_id) REFERENCES recettes (recette_id) ON DELETE SET NULL
);

CREATE TABLE lignes_commandes
(
    ligne_commande_id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    heure_livraison   DATETIME,
    adresse_arrivee   TEXT                                                            NOT NULL,
    statut            ENUM ('Commandee', 'Preparee', 'En cours', 'Livree', 'Annulee') NOT NULL,
    commande_id       BIGINT UNSIGNED                                                 NOT NULL,

    FOREIGN KEY (commande_id) REFERENCES commandes (commande_id)
);

CREATE TABLE plats
(
    plat_id           BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    nom_plat          VARCHAR(100)    NOT NULL,
    nb_parts          INT             NOT NULL,
    date_fabrication  DATE            NOT NULL,
    date_peremption   DATE            NOT NULL,
    prix_par_personne DECIMAL(6, 2)   NOT NULL,
    photo             LONGBLOB,
    cuisinier_id      BIGINT UNSIGNED NOT NULL,
    recette_id        BIGINT UNSIGNED NOT NULL,
    commande_id       BIGINT UNSIGNED, -- Null si le plat n'a pas été commandé

    FOREIGN KEY (cuisinier_id) REFERENCES users (user_id),
    FOREIGN KEY (recette_id) REFERENCES recettes (recette_id),
    FOREIGN KEY (commande_id) REFERENCES commandes (commande_id)
);

CREATE TABLE evaluation
(
    client_id       BIGINT UNSIGNED,
    cuisinier_id    BIGINT UNSIGNED,
    note            INT CHECK (note BETWEEN 1 AND 5),
    commentaire     TEXT,
    date_evaluation DATETIME,

    PRIMARY KEY (client_id, cuisinier_id),
    FOREIGN KEY (client_id) REFERENCES users (user_id),
    FOREIGN KEY (cuisinier_id) REFERENCES users (user_id)
);
```
### 1.4 Exemples de Requêtes SQL

Cette section présente plusieurs requêtes SQL permettant de manipuler la base de données **LivinParis**.

Voici des requêtes pour insérer des données dans la base :

```sql
-- 1. Insérer un utilisateur (Client)
INSERT INTO users (password, role, type, email, nom, prenom, adresse, telephone, entreprise)
VALUES ('mdp123', 'Client', 'Particulier', 'client@example.com', 'Dupont', 'Jean', '12 Rue de Paris, 75001', '0601020304', NULL);

-- 2. Insérer un utilisateur (Cuisinier)
INSERT INTO users (password, role, type, email, nom, prenom, adresse, telephone, entreprise)
VALUES ('mdp456', 'Cuisinier', 'Particulier', 'cuisinier@example.com', 'Martin', 'Sophie', '15 Rue de Lyon, 69002', '0611223344', NULL);

-- 3. Insérer une recette
INSERT INTO recettes (nom_recette, type, ingredients, style_cuisine, regime_alimentaire)
VALUES ('Salade César', 'Entrée', 'Salade, Poulet, Parmesan, Croutons, Sauce César', 1, 'Végétarien');

-- 4. Insérer un plat basé sur une recette existante
INSERT INTO plats (nom_plat, nb_parts, date_fabrication, date_peremption, prix_par_personne, cuisinier_id, recette_id)
VALUES ('Salade César Gourmande', 4, '2025-03-01', '2025-03-05', 9.99, 2, 1);

-- 5. Insérer une commande passée par un client
INSERT INTO commandes (heure_commande, adresse_depart, prix_total, client_id, cuisinier_id)
VALUES ('2025-03-01 12:00:00', '12 Rue de Paris, 75001', 39.96, 1, 2);

-- 6. Ajouter une ligne de commande pour un plat commandé
INSERT INTO lignes_commandes (heure_livraison, adresse_arrivee, statut, commande_id)
VALUES ('2025-03-01 13:00:00', '12 Rue de Paris, 75001', 'Commandee', 1);

-- 7. Insérer une évaluation d'un client sur un cuisinier
INSERT INTO evaluation (client_id, cuisinier_id, note, commentaire, date_evaluation)
VALUES (1, 2, 5, 'Excellent plat et livraison rapide !', '2025-03-01 14:00:00');
```
Voici des requêtes pour consulter des données dans la base :

```mysql
-- 1. Sélectionner tous les utilisateurs
SELECT * FROM users;

-- 1.bis Sélectionner tous les cuisiniers
SELECT * FROM users WHERE role = 'Cuisinier';

-- 2. Lister toutes les commandes avec les informations du client et du cuisinier
SELECT c.commande_id, c.heure_commande, c.adresse_depart, c.prix_total,
       u_client.nom AS client_nom, u_client.prenom AS client_prenom,
       u_cuisinier.nom AS cuisinier_nom, u_cuisinier.prenom AS cuisinier_prenom
FROM commandes c
LEFT JOIN users u_client ON c.client_id = u_client.user_id
LEFT JOIN users u_cuisinier ON c.cuisinier_id = u_cuisinier.user_id;

-- 3. Récupérer la liste des plats disponibles avec leurs recettes
SELECT p.plat_id, p.nom_plat, p.nb_parts, p.date_fabrication, p.date_peremption, p.prix_par_personne,
       r.nom_recette, r.type AS type_recette, r.ingredients
FROM plats p
INNER JOIN recettes r ON p.recette_id = r.recette_id;

-- 4. Afficher la moyenne des notes données aux cuisiniers
SELECT cuisinier_id, AVG(note) AS moyenne_note
FROM evaluation
GROUP BY cuisinier_id;

-- 5. Lister les commandes en cours de livraison
SELECT lc.ligne_commande_id, c.commande_id, lc.heure_livraison, lc.adresse_arrivee, lc.statut
FROM lignes_commandes lc
INNER JOIN commandes c ON lc.commande_id = c.commande_id
WHERE lc.statut = 'En cours';
```
---
## 2. Code c#


Dans le cadre de ce projet, j'ai utilisé une IA générative afin d'améliorer la qualité et l'organisation de mon code en C#. Plusieurs aspects ont été abordés via des prompts spécifiques :

### 2.1. Gestion des erreurs
Afin de garantir la robustesse du programme, j'ai également sollicité l'IA pour identifier les erreurs potentielles à anticiper, je lui ai posé la question suivante en lui donnant mon code :

> *"Quels types d'erreurs basiques dois-je gérer dans mon programme"*

L'IA a suggéré d'inclure l'utilisation de regex pour la gestion des erreurs de lecture de fichier. Par exemple le cas ou le fichier n'est pas trouvé ou bien s'il y a des lignes sautées, des espaces en trop etc... 

### 2.2. Amélioration de la lisibilité des commentaires
Pour rendre mon code plus propre, j'ai utilisé l'IA pour reformuler et structurer mes commentaires en respectant les `///`.

> *"Peux-tu reformuler (et corriger les fautes) et structurer mes commentaires pour qu'ils soient en `///` ?"*

L'IA a alors proposé des reformulations claires et concises en ajoutant des balises XML (`<summary>`, `<param>`, `<returns>`, etc.), améliorant ainsi la maintenabilité du code.

### 2.3. Utilisation de SkiaSharp
J'ai d'abord envoyé la structure de mon code afin de généré un exemple de comment pourrait être utilisé SkiaSharp. Voici un exemple du prompt envoyé :

> *"Peux-tu me donner un code d'exemple pour afficher mon graphe en utilisant SkiaSharp, sachant que je ne sais pas bien utiliser cette librairie"*

Il ma donc fourni un code assez simple mais l'affichage ne correspondait pas à mes attentes. Le graphe était en cercle et les sommets se supperposaient. J'ai donc ajouter une partie de hasard au code qu'il m'a fourni afin que les sommets soient répartis sur la fenêtre d'affichage. En revanche, le problème des sommets qui se supperposent n'était pas traité.
J'ai donc fais un prompt afin de savoir comment faire pour empecher ce problème. Il à amélioré mon code afin qu'au moment où les points sont généré au hasard, si les points sont trop proche, le points est regénéré. Voici le résultat final :  

![](/Files/GrapheResultatFinal.png "Résultat")

## 3. Annalyse du graphe 

### 3.1. Est-ce-que le graphe est connexe ?
En observant le résultat ci-dessus, on constate que le graphe n'est pas connexe car certains sommets ne sont reliés à aucun autre sommet. Cela est dû à la façon de lire le fichier "soc-karate.txt".
En effet, pour créer un graphe à partir du fichier, nous avons besoin du nombre de sommets. Pour obtenir ce nombre, on regarde le nombre de lignes dans le fichier. Dans le fichier fourni, on en compte 33, nous avons donc 33 sommets. En revanche, si nous regardons le fichier en détail, on voit que certains sommets ne sont pas précisés. Par exemple, on passe de 13 à 17. D'où le fait que 14, 15 et 16 soient dans le "vide".

### 3.2. Le graphe contient-il des circuits ?
En observant le document fournis, on remarque la présence des arêtes suivantes : (1,2) ; (2,3) ; (3,1). Il s'agit ici d'un cycle d'après la définition du cours car on part du sommet 1 et on revient au sommet 1. Ainsi, on peut en conclure qu'il y a au moins un cycle dans le graphe. 
La méthode utilisée est donc une sorte de relation de Chasles, chercher des arêtes qui reliées ensembles forment un chemin qui part d'un sommet et revient vers ce même sommet. 
Par cette méthode, on trouve aussi le cycle (31,32) ; (32,33) ; (31,33).
