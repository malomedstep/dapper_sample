CREATE TABLE Authors (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL
);

CREATE TABLE Books (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(255) NOT NULL,
    AuthorId INT NOT NULL FOREIGN KEY REFERENCES Authors(Id)
);

INSERT INTO Authors (Name)
VALUES ('Jeffrey Richter'), ('J.R.R. Tolkien'), ('George R.R. Martin')

INSERT INTO Books (Title, AuthorId) 
VALUES 
('CLR via C#', 1), 
('Distributed Applications', 1), 
('Hobbit', 2), 
('Lord of the Rigns', 2), 
('Game of Thrones', 3)