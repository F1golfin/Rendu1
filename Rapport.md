# Rapport de Projet – Rendu 1

Ce rapport est structuré en deux grandes parties:
1. **Schéma Entité/Association et script SQL**
2. **Prompts issus des IA génératives concernant la visualisation du graphe**


---

## 1. Schéma Entité/Association et script SQL

### 1.1 Schéma Entité/Association (E/A)

#### Présentation
Le schéma E/A ci-dessous illustre la structure conceptuelle de la base de données pour l’application **LivinParis**.
- **users** : Gère les données de tous les utilisateurs (clients, cuisiniers, etc.).
- **commandes** : Contient les informations nécessaires à une commande (client, cuisinier, adresses, etc.).
- **plats** : Liste les plats proposés par les cuisiniers.
- **evaluations** : Stocke les notes et commentaires des clients sur les cuisiniers.

#### Schéma

![](/Files/SchemaEA.png "Schéma entités associations")

### 1.2 Script SQL

Le script suivant crée et initialise la base **livin_paris** avec toutes les tables nécessaires.

```sql
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
    adresse    TEXT                               NOT NULL,
    telephone  VARCHAR(15) UNIQUE                 NOT NULL,
    entreprise VARCHAR(50)                                  -- Pour les entreprises contient le nom de l'entreprise, NULL pour les particuliers
);

CREATE TABLE commandes
(
    commande_id     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    heure_commande  DATETIME,
    heure_livraison DATETIME,
    adresse_depart  TEXT                                                            NOT NULL, -- Permet de figer une fois la commande réalisée
    adresse_arrivee TEXT                                                            NOT NULL,
    prix_total      DECIMAL(8, 2)                                                   NOT NULL, -- Pourrait etre recalculer
    statut          ENUM ('Commandee', 'Preparee', 'En cours', 'Livree', 'Annulee') NOT NULL,
    client_id       BIGINT UNSIGNED,
    cuisinier_id    BIGINT UNSIGNED,

    FOREIGN KEY (client_id) REFERENCES users (user_id) ON DELETE SET NULL,
    FOREIGN KEY (cuisinier_id) REFERENCES users (user_id) ON DELETE SET NULL
);

CREATE TABLE plats
(
    plat_id            BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    nom_plat           VARCHAR(100)                                 NOT NULL,
    type               ENUM ('Entrée', 'Plat Principal', 'Dessert') NOT NULL,
    nb_parts           INT                                          NOT NULL,
    date_fabrication   DATE                                         NOT NULL,
    date_peremption    DATE                                         NOT NULL,
    prix_par_personne  DECIMAL(6, 2)                                NOT NULL,
    style_cuisine      VARCHAR(50)                                  NOT NULL, -- ENUM ?
    regime_alimentaire VARCHAR(50),                                           -- SET ? null si pas de regime
    ingredients        TEXT                                         NOT NULL,
    photo              LONGBLOB,
    cuisinier_id       BIGINT UNSIGNED                              NOT NULL,
    commande_id        BIGINT UNSIGNED,                                       -- Null si le plat n'a pas été commandé

    FOREIGN KEY (cuisinier_id) REFERENCES users (user_id),
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
### 1.3 Exemples de Requêtes SQL

Cette section présente plusieurs requêtes SQL permettant de manipuler la base de données **LivinParis**. 
Ces requêtes couvrent l'insertion, la mise à jour, la suppression et la récupération d'informations utiles.

- Insertion d'un **client**, d'un **cuisinier** et une personne qui a le double rôle.

```sql
INSERT INTO users (password, role, type, email, nom, prenom, adresse, telephone, entreprise)
VALUES
    ('password123', 'Client', 'Particulier', 'client@mail.com', 'Dupont', 'Jean', '10 rue de Paris, 75001 Paris', '0601020304', NULL),
    ('securepass', 'Cuisinier', 'Entreprise', 'chef@mail.com', 'Le Gourmet', 'Michel', '20 avenue de Lyon, 75002 Paris', '0605060708', 'Restaurant Le Gourmet'),
    ('hash_mdp_double', 'Client,Cuisinier', 'Particulier', 'chefclient@example.com', 'Durand', 'Alice', '5 Rue des Lilas, 75015 Paris', '0622334455', NULL);
```

- Création d'une commande entre un client et un cuisinier.

```sql
INSERT INTO commandes (heure_commande, heure_livraison, adresse_depart, adresse_arrivee, prix_total, statut, client_id, cuisinier_id)
VALUES
    (NOW(), DATE_ADD(NOW(), INTERVAL 2 HOUR), '10 rue de Paris, 75001 Paris', '30 boulevard Haussmann, 75009 Paris', 25.50, 'Commandee', 1, 2);
```

- Insertion d'un plat proposé par un cuisinier.

```sql
INSERT INTO plats (nom_plat, type, nb_parts, date_fabrication, date_peremption, prix_par_personne, style_cuisine, regime_alimentaire, ingredients, cuisinier_id)
VALUES
    ('Lasagnes', 'Plat Principal', 4, '2025-02-20', '2025-02-25', 12.50, 'Italienne', 'Végétarien', 'Pâtes, tomate, fromage, béchamel', 2);
```

- Un client évalue un cuisinier avec une note et un commentaire.

```sql
INSERT INTO evaluation (client_id, cuisinier_id, note, commentaire, date_evaluation)
VALUES
    (1, 2, 5, 'Les plats sont excellents et la livraison rapide !', NOW());
```

- Récupérer la liste des clients.

```sql
SELECT user_id, nom, prenom, email, adresse, telephone FROM users WHERE role = 'Client';
```

- Afficher les plats d’un cuisinier.

```sql
SELECT nom_plat, type, nb_parts, prix_par_personne, style_cuisine, regime_alimentaire FROM plats WHERE cuisinier_id = 2;
```

- Afficher les évaluations d’un cuisinier.

```sql
SELECT u.nom, u.prenom, e.note, e.commentaire, e.date_evaluation
FROM evaluation e JOIN users u ON e.client_id = u.user_id
WHERE e.cuisinier_id = 2;
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
