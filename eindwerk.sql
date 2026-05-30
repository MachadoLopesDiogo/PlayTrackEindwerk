SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
DROP DATABASE IF EXISTS voetbaldb;
CREATE DATABASE voetbaldb;
USE voetbaldb;               


CREATE TABLE Wedstrijd (
  IDWedstrijd INT AUTO_INCREMENT PRIMARY KEY,
  Datum DATE ,
  Score VARCHAR(10) NOT NULL,
  Thuis varchar(50) Not null,
  Uit varchar(50) Not null,
  ThuisNaam varchar(50) not null,
  UitNaam varchar(50) not null
);
INSERT INTO Wedstrijd (Datum, Score, Thuis, Uit, ThuisNaam, UitNaam) VALUES
('2025-10-14', '2-1', '2', '1', 'Anderlecht','Olen United');
CREATE TABLE Seizoen (
  IDSeizoen INT AUTO_INCREMENT PRIMARY KEY,
  NiveauSeizoen INT NOT NULL,
  FKWedstrijd INT NOT NULL,
  FOREIGN KEY (FKWedstrijd) REFERENCES Wedstrijd(IDWedstrijd)
);
INSERT INTO Seizoen (NiveauSeizoen, FKWedstrijd) VALUES
(1,1);
CREATE TABLE Speler (
  IDSpeler INT AUTO_INCREMENT PRIMARY KEY,
  SpelerVoornaam VARCHAR(50) NOT NULL,
  SpelerAchternaam VARCHAR(50) NOT NULL,
  Positie VARCHAR(10) NOT NULL,
  Team varchar(20) NOT NULL
);
INSERT INTO Speler (IDSpeler, SpelerVoornaam, SpelerAchternaam, Positie,Team) VALUES
(10,'Diogo','Machado Lopes','CMD','Olen United');
CREATE TABLE WedstrijdHeeftSpeler (
  FKSpeler INT NOT NULL,
  FKWedstrijd INT NOT NULL,
  AantalGoals INT NOT NULL,
  AantalAssists INT NOT NULL,
  AantalGeleRodeKaarten INT NOT NULL,
  AantalGrofFouten INT NOT NULL,
  AantalBelangrijkeActies INT NOT NULL,
  AantalBelangrijkeTackles INT NOT NULL,
  RatingOp10 INT NOT NULL,
  PRIMARY KEY (FKSpeler, FKWedstrijd),
  FOREIGN KEY (FKSpeler) REFERENCES Speler(IDSpeler),
  FOREIGN KEY (FKWedstrijd) REFERENCES Wedstrijd(IDWedstrijd)
);
INSERT INTO WedstrijdHeeftSpeler VALUES
(10,1,1,0,0,1,2,0,8);
SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;

