/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

CREATE DATABASE IF NOT EXISTS `RecipeDatabase` /*!40100 DEFAULT CHARACTER SET latin1 */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `RecipeDatabase`;

CREATE TABLE IF NOT EXISTS `Equipment` (
  `EquipmentID` int unsigned NOT NULL AUTO_INCREMENT,
  `EquipmentName` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci DEFAULT NULL,
  PRIMARY KEY (`EquipmentID`),
  FULLTEXT KEY `EquipmentName` (`EquipmentName`)
) ENGINE=InnoDB AUTO_INCREMENT=330 DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `EquipmentInRecipe` (
  `EquipmentID` int unsigned NOT NULL,
  `RecipeID` int unsigned NOT NULL,
  PRIMARY KEY (`EquipmentID`,`RecipeID`),
  KEY `FK_RecipeID_on_EquipmentInRecipe` (`RecipeID`),
  CONSTRAINT `FK_EquipmentID_on_EquipmentInRecipe` FOREIGN KEY (`EquipmentID`) REFERENCES `Equipment` (`EquipmentID`) ON DELETE CASCADE,
  CONSTRAINT `FK_RecipeID_on_EquipmentInRecipe` FOREIGN KEY (`RecipeID`) REFERENCES `Recipe` (`RecipeID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `FileManager` (
  `FileID` char(128) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `RecipeID` int unsigned NOT NULL,
  `NumberOfViews` int unsigned NOT NULL,
  `DateUploaded` datetime NOT NULL,
  `LastAccessed` datetime NOT NULL,
  PRIMARY KEY (`FileID`),
  KEY `FK_RecipeID_on_FileManager` (`RecipeID`),
  CONSTRAINT `FK_RecipeID_on_FileManager` FOREIGN KEY (`RecipeID`) REFERENCES `Recipe` (`RecipeID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `Method` (
  `StepNumber` int unsigned NOT NULL AUTO_INCREMENT,
  `RecipeID` int unsigned NOT NULL,
  `MethodText` varchar(65355) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`StepNumber`,`RecipeID`) USING BTREE,
  KEY `RecipeID` (`RecipeID`),
  FULLTEXT KEY `MethodText` (`MethodText`),
  CONSTRAINT `RecipeID` FOREIGN KEY (`RecipeID`) REFERENCES `Recipe` (`RecipeID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=164 DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `PasswordResetToken` (
  `ResetTokenID` varchar(50) NOT NULL,
  `ResetTokenIDViewed` int unsigned NOT NULL DEFAULT '0',
  `OTPUsed` int unsigned NOT NULL DEFAULT '0',
  `OTP` varchar(6) DEFAULT NULL,
  `UserName` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`ResetTokenID`),
  KEY `UserName` (`UserName`),
  CONSTRAINT `UserName` FOREIGN KEY (`UserName`) REFERENCES `Users` (`UserName`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `Recipe` (
  `RecipeID` int unsigned NOT NULL AUTO_INCREMENT,
  `Servings` tinyint unsigned NOT NULL DEFAULT '0',
  `MealType` varchar(20) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL DEFAULT '',
  `RecipeName` tinytext NOT NULL,
  `Kcal` decimal(6,2) DEFAULT NULL,
  `Saturates` decimal(6,2) DEFAULT NULL,
  `Carbohydrates` decimal(6,2) DEFAULT NULL,
  `Sugar` decimal(6,2) DEFAULT NULL,
  `Fibre` decimal(6,2) DEFAULT NULL,
  `Protein` decimal(6,2) DEFAULT NULL,
  `Salt` decimal(6,2) DEFAULT NULL,
  `Fat` decimal(6,2) DEFAULT NULL,
  `CookingTime` smallint DEFAULT '0',
  `PreperationTime` smallint DEFAULT '0',
  `Difficulty` varchar(20) CHARACTER SET latin1 COLLATE latin1_swedish_ci DEFAULT NULL,
  `PageVisits` int unsigned DEFAULT NULL,
  `LastRequested` datetime DEFAULT NULL,
  `Description` varchar(63355) CHARACTER SET latin1 COLLATE latin1_swedish_ci DEFAULT NULL,
  `ManualUpload` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`RecipeID`),
  FULLTEXT KEY `Description` (`Description`),
  FULLTEXT KEY `RecipeName` (`RecipeName`)
) ENGINE=InnoDB AUTO_INCREMENT=92 DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `RecoveryEmailAddress` (
  `EmailAddress` varchar(255) NOT NULL,
  `UserName` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`EmailAddress`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `Review` (
  `ReviewID` int unsigned NOT NULL AUTO_INCREMENT,
  `RecipeID` int unsigned NOT NULL DEFAULT '1',
  `ReviewersName` tinytext CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `ReviewTitle` text CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `ReviewText` text CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `StarCount` int unsigned NOT NULL DEFAULT '1',
  `DateSubmitted` datetime NOT NULL,
  PRIMARY KEY (`ReviewID`) USING BTREE,
  KEY `ReviewFK` (`RecipeID`) USING BTREE,
  CONSTRAINT `ReviewFK` FOREIGN KEY (`RecipeID`) REFERENCES `Recipe` (`RecipeID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=141 DEFAULT CHARSET=latin1;


CREATE TABLE IF NOT EXISTS `SearchQuery` (
  `SearchTerm` varchar(100) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `ResultAsJSON` varchar(5000) DEFAULT NULL,
  PRIMARY KEY (`SearchTerm`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


CREATE TABLE IF NOT EXISTS `StopWords` (
  `Value` varchar(10) NOT NULL,
  PRIMARY KEY (`Value`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*!40000 ALTER TABLE `StopWords` DISABLE KEYS */;
INSERT INTO `StopWords` (`Value`) VALUES
	('a'),
	('about'),
	('add'),
	('an'),
	('are'),
	('as'),
	('at'),
	('be'),
	('by'),
	('com'),
	('de'),
	('en'),
	('for'),
	('form'),
	('from'),
	('how'),
	('i'),
	('in'),
	('is'),
	('it'),
	('la'),
	('of'),
	('on'),
	('onto'),
	('or'),
	('place'),
	('prepare'),
	('put'),
	('that'),
	('the'),
	('this'),
	('to'),
	('und'),
	('was'),
	('what'),
	('when'),
	('where'),
	('who'),
	('will'),
	('with'),
	('www');
/*!40000 ALTER TABLE `StopWords` ENABLE KEYS */;

CREATE TABLE IF NOT EXISTS `UserDefinedIngredients` (
  `IngredientID` int unsigned NOT NULL AUTO_INCREMENT,
  `IngredientName` varchar(255) NOT NULL DEFAULT '',
  `TypeOf` int DEFAULT NULL,
  PRIMARY KEY (`IngredientID`),
  FULLTEXT KEY `IngredientName` (`IngredientName`)
) ENGINE=InnoDB AUTO_INCREMENT=239 DEFAULT CHARSET=latin1;


CREATE TABLE IF NOT EXISTS `UserDefinedIngredientsInRecipe` (
  `IngredientID` int unsigned NOT NULL,
  `RecipeID` int unsigned NOT NULL,
  `Quantity` decimal(20,6) NOT NULL,
  `Unit` varchar(25) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  PRIMARY KEY (`IngredientID`,`RecipeID`) USING BTREE,
  KEY `FK_RecipeID_on_IngredientsInRecipe` (`RecipeID`) USING BTREE,
  CONSTRAINT `FK_UserDefinedIngredientsInRecipe_Recipe` FOREIGN KEY (`RecipeID`) REFERENCES `Recipe` (`RecipeID`) ON DELETE CASCADE,
  CONSTRAINT `FK_UserDefinedIngredientsInRecipe_UserDefinedIngredients` FOREIGN KEY (`IngredientID`) REFERENCES `UserDefinedIngredients` (`IngredientID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;

CREATE TABLE IF NOT EXISTS `Users` (
  `UserName` varchar(200) NOT NULL DEFAULT '',
  `SHA512` varchar(128) NOT NULL DEFAULT '',
  `Role` varchar(128) NOT NULL DEFAULT '',
  PRIMARY KEY (`UserName`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
