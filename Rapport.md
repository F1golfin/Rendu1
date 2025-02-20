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

- Insertion d'un **client** et d'un **cuisinier** avec des rôles distincts.

```sql
INSERT INTO users (password, role, type, email, nom, prenom, adresse, telephone)
VALUES 
    ('hash_mdp_client', 'Client', 'Particulier', 'client@example.com', 'Dupont', 'Jean', '10 Rue de Paris, 75000 Paris', '0601020304'),
    ('hash_mdp_cuisinier', 'Cuisinier', 'Particulier', 'cuisinier@example.com', 'Martin', 'Paul', '20 Rue de Lyon, 69000 Lyon', '0611223344'),
    ('hash_mdp_double', 'Client,Cuisinier', 'Particulier', 'chefclient@example.com', 'Durand', 'Alice', '5 Rue des Lilas, 75015 Paris', '0622334455');
```

- Création d'une commande entre un client et un cuisinier.

```sql
INSERT INTO commandes (client_id, cuisinier_id, adresse_depart, adresse_arrivee, prix_total, statut)
VALUES 
    (1, 2, '20 Rue de Lyon, 69000 Lyon', '10 Rue de Paris, 75000 Paris', 25.50, 'Commandee');
```

- Insertion d'un plat proposé par un cuisinier et associer un plat à une commande existante (ex. commande avec id = 1)

```sql
INSERT INTO plats (nom_plat, type, nb_personne, nb_plats, date_fabrication, date_peremption, prix_par_personne, style_cuisine, ingredients, cuisinier)
VALUES 
    ('Lasagnes maison', 'Plat Principal', 2, 5, '2025-02-20', '2025-02-25', 12.00, 'Italien', 'Pâtes, viande, tomate, fromage', 2);

UPDATE plats SET commande = 1 WHERE id = 1;
```

- Un client évalue un cuisinier avec une note et un commentaire.

```sql
INSERT INTO evaluations (client_id, cuisinier_id, note, commentaire)
VALUES 
    (1, 2, 5, 'Superbe plat, très savoureux et bien présenté !');
```

- Liste des plats non commandés et encore en stock.

```sql
SELECT * FROM plats WHERE commande IS NULL AND nb_plats > 0;
```

- Liste des commandes passées par un client (id = 1).

```sql
SELECT c.id, c.heure_commande, c.prix_total, c.statut, u.nom AS cuisinier
FROM commandes c
JOIN users u ON c.cuisinier_id = u.id
WHERE c.client_id = 1;
```
---
## 2. Code c#




