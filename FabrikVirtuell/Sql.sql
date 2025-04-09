CREATE DATABASE my_database;
USE my_database;

CREATE TABLE Machines (
    MachineID INT AUTO_INCREMENT PRIMARY KEY,
    CurrentState VARCHAR(50),
    SignalLightState VARCHAR(50)
);

CREATE TABLE Jobs (
    JobID INT AUTO_INCREMENT PRIMARY KEY,
    JobName VARCHAR(100),
    Product VARCHAR(100),
    Quantity INT,
    ProducedQuantity INT,
    CurrentState VARCHAR(50),
    MachineID INT,
    FOREIGN KEY (MachineID) REFERENCES Machines(MachineID)
);

CREATE TABLE JobManagers (
    JobManagerID INT AUTO_INCREMENT PRIMARY KEY,
    CurrentJobID INT,
    CurrentStep INT,
    MachineID INT,
    FOREIGN KEY (CurrentJobID) REFERENCES Jobs(JobID),
    FOREIGN KEY (MachineID) REFERENCES Machines(MachineID)
);
