using ApClient.data;
using ApClient.ui;
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


    public CardRecord AddOpenedCard(CardData card)
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

        Plugin.m_SessionHandler.CompleteLocationChecks(APinfoMenu.Instance.UpdateOpenLocationValues(card));
        return record;
    }

    public CardRecord AddSoldCard(CardData card)
    {
        int id = card.GetUniqueHash();
        if (!_progress.Cards.TryGetValue(id, out var record))
            _progress.Cards[id] = record = new CardRecord();

        record.Sold++;
        Plugin.m_SessionHandler.CompleteLocationChecks(APinfoMenu.Instance.UpdateSellLocationValues(card));
        return record;
    }

    public CardRecord AddGradedCard(CardData card)
    {
        int id = card.GetUniqueHash();
        if (!_progress.Cards.TryGetValue(id, out var record))
            _progress.Cards[id] = record = new CardRecord();

        record.Grades.Add(card.cardGrade);

        Plugin.m_SessionHandler.CompleteLocationChecks(APinfoMenu.Instance.UpdateGradeLocationValues(card, card.cardGrade));
        return record;
    }

    public bool HasCardOpened(int id)
    {
        return _progress.Cards.TryGetValue(id, out var record) && record.Opened > 0;
    }

    public IEnumerable<string> GetCompletedAchievements()
        => _progress.CompletedAchievements;
}

//for each card in dictionary, check if they match each achievement. increment count for achievement.

//when a card is added, for each achievement see if they match and increment