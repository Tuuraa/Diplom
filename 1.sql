--
-- File generated with SQLiteStudio v3.4.4 on ¬т дек 17 14:30:37 2024
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: Commands
CREATE TABLE IF NOT EXISTS "Commands" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Commands" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Phrases" TEXT NOT NULL,
    "Action" TEXT NOT NULL,
    "Type" TEXT NOT NULL,
    "ScenarioId" INTEGER NULL,
    CONSTRAINT "FK_Commands_Scenarios_ScenarioId" FOREIGN KEY ("ScenarioId") REFERENCES "Scenarios" ("Id") ON DELETE CASCADE
);

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
