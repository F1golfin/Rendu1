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