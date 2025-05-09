INSERT INTO Race (Id, Name) VALUES
                                (1, 'Elf'),
                                (2, 'Human'),
                                (3, 'Dwarf'),
                                (4, 'Orc'),
                                (5, 'Goblin');


INSERT INTO ExperienceLevel (Id, Name) VALUES
                                           (1, 'Novice'),
                                           (2, 'Experienced'),
                                           (3, 'Veteran'),
                                           (4, 'Master'),
                                           (5, 'Legendary');

INSERT INTO Adventurer (Id, Nickname, RaceId, ExperienceId, PersonId) VALUES
                                                                          (1, 'Stormbreaker', 2, 3, 'CL202304150001XY'),
                                                                          (2, 'Nightblade', 1, 2, 'AX134401012728CE'),
                                                                          (3, 'Ironfist', 3, 4, 'DP088807202222AB');



INSERT INTO Person (Id, FirstName, MiddleName, LastName, HasBounty) VALUES
                                                                        ('AX134401012728CE', 'John', 'T.', 'Silver', 0),
                                                                        ('BZ199912251234ZZ', 'Mira', NULL, 'Nightingale', 1),
                                                                        ('CL202304150001XY', 'Grog', NULL, 'Stonehammer', 0),
                                                                        ('DP088807202222AB', 'Aria', 'L.', 'Stormborn', 0),
                                                                        ('EZ000111280000QW', 'Thorn', NULL, 'Darkblade', 1);


