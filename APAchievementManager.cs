using ApClient.data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ApClient;


public class APAchievementManager
{
    private PlayerCardProgress _progress = new PlayerCardProgress();


    public void Load(PlayerCardProgress progress)
    {
        _progress = progress ?? new PlayerCardProgress();
        _progress.Cards ??= new Dictionary<int, CardRecord>();
        _progress.CompletedAchievements ??= new HashSet<string>();
    }


    public PlayerCardProgress Save()
    {
        return _progress;
    }


    public CardAddedDTO AddOpenedCard(CardData card)
    {
        if (card == null)
        {
            Plugin.Log("AddOpenedCard called with null card!");
            return null;
        }
        int id = card.GetUniqueHash();
        if (!_progress.Cards.TryGetValue(id, out var record))
            _progress.Cards[id] = record = new CardRecord();

        Plugin.Log("adding new card to achievement manager");
        record.Opened++;
       
        return new CardAddedDTO { cardRecord = record, newAchievements = EvaluateAchievements() };
    }

    public List<string> AddSoldCard(CardData card)
    {
        int id = card.GetUniqueHash();
        if (!_progress.Cards.TryGetValue(id, out var record))
            _progress.Cards[id] = record = new CardRecord();

        record.Sold++;

        return EvaluateAchievements();
    }

    public List<string> AddGradedCard(CardData card)
    {
        int id = card.GetUniqueHash();
        if (!_progress.Cards.TryGetValue(id, out var record))
            _progress.Cards[id] = record = new CardRecord();

        record.Grades.Add(card.cardGrade);

        return EvaluateAchievements();
    }

    public bool HasCardOpened(int id)
    {
        return _progress.Cards.TryGetValue(id, out var record) && record.Opened > 0;
    }

    public IEnumerable<string> GetCompletedAchievements()
        => _progress.CompletedAchievements;

    private List<string> EvaluateAchievements()
    {


        return null;
    }
}