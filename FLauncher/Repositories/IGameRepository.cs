﻿using FLauncher.Model;

namespace FLauncher.Repositories
{
    public interface IGameRepository
    {

      Task<IEnumerable<Game>> GetTopGames();
        Task Download_game(Game game, String saveLocation, Gamer gamer);
        Task Play_Game(Game game,Gamer gamer );
        Task Upload_game(GamePublisher publisher,Game game, string selectedFilePath, string message);

        Task<IEnumerable<Achivement>> GetAchivesFromGame(Game game);
        Task<IEnumerable<UnlockAchivement>> GetUnlockAchivementsFromGame(IEnumerable<Achivement> achivements, Gamer gamer);
        Task<IEnumerable<Achivement>> GetAchivementsFromUnlocks(IEnumerable<UnlockAchivement> unlockAchivements);
        Task<Achivement> GetAchivementFromUnlock(UnlockAchivement unlock);
        Task<IEnumerable<Achivement>> GetLockAchivement(IEnumerable<Achivement> achivements, Gamer gamer);
        Task<bool> IsBuyGame(Game game, Gamer gamer);
        Task<bool> IsPublishGame(Game game, GamePublisher publisher
            );
        Task<bool> isDownload(Game game, Gamer gamer);

        Task<IEnumerable<Game>> GetGamesByGamer(Gamer gamer);

        Task Uninstall_Game(Gamer gamer, Game game);
        Task Reinstall(Game game, Gamer gamer);
        Task<IEnumerable<TrackingRecords>> GetTrackingFromGamerGame(Gamer gamer, Game game);
        Task<TrackingPlayers> GetTrackingFromGame(Game game);
        Task<IEnumerable<Game>> GetAllGame();
        Task<IEnumerable<Game>> GetGameByInformation(string inputName, List<string> genres, string pubs);

    }
}
