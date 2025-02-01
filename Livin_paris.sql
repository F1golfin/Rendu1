DROP DATABASE IF EXISTS livin_paris;
CREATE DATABASE livin_paris;
USE livin_paris;

CREATE TABLE users
(
    id         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
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

CREATE TABLE livraisons
(
    id           BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    cuisinier_id BIGINT UNSIGNED                            NOT NULL,
    statut       ENUM ('Planifiee', 'En cours', 'Terminee') NOT NULL,
    heure_debut  TIMESTAMP,
    heure_fin    TIMESTAMP,

    FOREIGN KEY (cuisinier_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE commandes
(
    id              BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    client_id       BIGINT UNSIGNED,
    cuisinier_id    BIGINT UNSIGNED,
    livraison_id    BIGINT UNSIGNED DEFAULT NULL,
    heure_commande  TIMESTAMP       DEFAULT CURRENT_TIMESTAMP,
    heure_livraison TIMESTAMP,

    -- Détails de la commande
    adresse_depart  TEXT                                                            NOT NULL, -- Permet de figer une fois la commande réalisée
    adresse_arrivee TEXT                                                            NOT NULL,
    prix_total      DECIMAL(8, 2)                                                   NOT NULL, -- Pourrait etre recalculer
    statut          ENUM ('Commandee', 'Preparee', 'En cours', 'Livree', 'Annulee') NOT NULL,

    FOREIGN KEY (client_id) REFERENCES users (id) ON DELETE SET NULL,
    FOREIGN KEY (cuisinier_id) REFERENCES users (id) ON DELETE SET NULL,
    FOREIGN KEY (livraison_id) REFERENCES livraisons (id)
);

CREATE TABLE plats
(

    id                 BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    nom                VARCHAR(100)                                 NOT NULL,
    type               ENUM ('Entrée', 'Plat Principal', 'Dessert') NOT NULL,
    nb_personne        INT                                          NOT NULL,
    nb_plats           INT                                          NOT NULL, -- nb de plats disponible pour ce cuisinier
    date_fabrication   DATE                                         NOT NULL,
    date_peremption    DATE                                         NOT NULL,
    prix_par_personne  DECIMAL(6, 2)                                NOT NULL,
    style_cuisine      VARCHAR(50)                                  NOT NULL, -- Enum ?
    regime_alimentaire VARCHAR(50),                                           -- Enum ? Set ? null is pas de regime
    ingredients        TEXT                                         NOT NULL,
    photo              TEXT,                                                  -- Url ou BLOB ?
    cuisinier          BIGINT UNSIGNED                              NOT NULL,
    commande           BIGINT UNSIGNED,                                       -- Null si le plat n'a pas été commandé

    FOREIGN KEY (cuisinier) REFERENCES users (id) ON DELETE CASCADE,
    FOREIGN KEY (commande) REFERENCES commandes (id)
);

CREATE TABLE evaluations
(
    id              BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    client_id       BIGINT UNSIGNED,
    cuisinier_id    BIGINT UNSIGNED NOT NULL,
    note            INT CHECK (note BETWEEN 1 AND 5),
    commentaire     TEXT,
    date_evaluation TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (client_id) REFERENCES users (id) ON DELETE SET NULL,
    FOREIGN KEY (cuisinier_id) REFERENCES users (id) ON DELETE CASCADE
);
