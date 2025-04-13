INSERT INTO [FINAIL-PROJECT-ITI].[dbo].[Trips]
    ([TripId], [Name], [Description], [StartDate], [EndDate], [Duration], [Money], [AvailablePeople], [MaxPeople], [IsDeleted], [OutOfDate])
VALUES
(1, 'Adventure in Alps', 'A 7-day hiking trip through the Swiss Alps.', '2025-06-10 08:00:00', '2025-06-17 18:00:00', 7, 1500.00, 10, 15, 0, 0),
(2, 'Safari in Kenya', 'Explore the savannahs and wildlife of Kenya.', '2025-07-01 09:00:00', '2025-07-10 20:00:00', 10, 2400.00, 5, 10, 0, 0),
(3, 'Beach Escape Bali', 'Relaxation and surfing at the beautiful beaches of Bali.', '2025-05-20 10:00:00', '2025-05-27 16:00:00', 8, 1800.00, 8, 12, 0, 0),
(4, 'Tokyo City Tour', 'Experience modern life and traditional culture in Tokyo.', '2025-08-01 08:30:00', '2025-08-07 21:00:00', 7, 2000.00, 12, 15, 0, 0),
(5, 'Northern Lights Norway', 'Chase the Northern Lights in Norway.', '2024-12-01 17:00:00', '2024-12-05 22:00:00', 5, 1300.00, 3, 8, 0, 1),
(6, 'Ancient Egypt Tour', 'Visit the Pyramids and historical sites across Egypt.', '2025-09-10 07:45:00', '2025-09-20 19:00:00', 11, 1600.00, 6, 10, 0, 0),
(7, 'Amazon Rainforest Expedition', 'A wild adventure through the Amazon.', '2025-03-15 06:00:00', '2025-03-25 18:30:00', 11, 2200.00, 4, 6, 0, 1),
(8, 'Rome & Vatican', 'History, art, and cuisine in the heart of Italy.', '2025-04-05 09:00:00', '2025-04-12 20:00:00', 8, 1700.00, 9, 12, 0, 0);
