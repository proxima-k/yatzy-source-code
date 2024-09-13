using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGameUI : MonoBehaviour {
        
    [SerializeField] private Transform _UIContainer;
    [SerializeField] private Transform _finalScoreContainer;
    [SerializeField] private ScoreUI _finalScoreUIPrefab;
    
    [SerializeField] private TextMeshProUGUI _winnerText;
    [SerializeField] private Button _exitButton;
    
    [SerializeField] private ScorecardUIHolder _scorecardUIHolder;
    
    private List<ScoreUI> _finalScoreUIList = new List<ScoreUI>();
    private List<int> _finalScoreList = new List<int>();
    
    private List<Player> _players = new List<Player>();

    private void Awake() {
        _exitButton.onClick.AddListener(ExitButton_OnClick);
        
        _winnerText.text = "";
        _exitButton.gameObject.SetActive(false);
        Hide();
    }

    private void Start() {
        GameHandler.Instance.OnGameEnd += GameHandler_OnGameEnd;
    }
    
    private void ExitButton_OnClick() {
        ConnectionHandler.Instance.Disconnect();
        SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
    }
    
    private void GameHandler_OnGameEnd(object sender, EventArgs e) {
        _players.AddRange(GameHandler.Instance.Players);
        
        Show();
        // todo: save all references so when a player leaves it doesn't affect the UI
        
        foreach (var player in _players) {
            ScoreUI finalScoreDisplay = Instantiate(_finalScoreUIPrefab, _finalScoreContainer);
            _finalScoreUIList.Add(finalScoreDisplay);
            _finalScoreList.Add(0);
            
            finalScoreDisplay.ChangeImageColor(player.PlayerColor);
            finalScoreDisplay.UpdateText("0");
        }
        
        
        gameObject.transform.SetAsLastSibling();
        _scorecardUIHolder.transform.SetAsLastSibling();
        _scorecardUIHolder.HideToggleScorecardButton();
        
        // move scorecardUI to the center
        RectTransform scorecardUIRectTf = _scorecardUIHolder.GetComponent<RectTransform>();
        scorecardUIRectTf.anchorMin = new Vector2(1f, 0.5f);
        scorecardUIRectTf.anchorMax = new Vector2(1f, 0.5f);
        
        // force rebuild layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(scorecardUIRectTf);
        
        _scorecardUIHolder.MoveToPosition(scorecardUIRectTf, new Vector2(scorecardUIRectTf.anchoredPosition.x, 0f));
        
        StartCoroutine(CountUpScoreRoutine());
    }
    
    
    private void Show() {
        _UIContainer.gameObject.SetActive(true);
    }

    private void Hide() {
        _UIContainer.gameObject.SetActive(false);
    }
    
    // animation for counting up score
    private IEnumerator CountUpScoreRoutine() {
        
        yield return new WaitForSeconds(3f);
        
        float highlightDuration = 0.75f;
        
        // move ScorecardUI vertically up to center
        
        // get PlayerScorecard
        
        // get reference to PlayerScoresColumnUI
        List<PlayerScoresColumnUI> playerScoresColumnUIList = _scorecardUIHolder.ScorecardUI.PlayerScoresColumnUIList;
        // get reference to ScoreUI and RuleSectionScoreUI
        
        List<RuleSection> ruleSectionList = GameHandler.Instance.GameSettings.RuleSections;

        int ruleIndex = 0;
        foreach (var ruleSection in ruleSectionList) {
            foreach (var rule in ruleSection.CombinationRules) {
                foreach (var player in _players) {
                    PlayerScorecard playerScorecard = player.PlayerScorecard;
                    RuleScore ruleScore = playerScorecard.RuleScores[ruleIndex];
                    
                    ScoreUI scoreUI = playerScoresColumnUIList[_players.IndexOf(player)].RuleScoreUIList[ruleIndex];
                    
                    scoreUI.UpdateText(ruleScore.Score);
                    scoreUI.FlashScore(ruleScore.Score, player.PlayerColor, highlightDuration);
                    
                    _finalScoreList[_players.IndexOf(player)] += ruleScore.Score;
                    _finalScoreUIList[_players.IndexOf(player)].UpdateText(_finalScoreList[_players.IndexOf(player)]);
                }
                ruleIndex++;
                
                yield return new WaitForSeconds(highlightDuration);
            }
            
            if (!ruleSection.HasBonus)
                continue;
            
            // flash sum
            foreach (var player in _players) {
                PlayerScorecard playerScorecard = player.PlayerScorecard;
                RuleSectionScore ruleSectionScore = playerScorecard.RuleSectionScoreList[ruleSectionList.IndexOf(ruleSection)];
                
                ScoreUI sumScoreUI = playerScoresColumnUIList[_players.IndexOf(player)].RuleSectionScoreUIList[ruleSectionList.IndexOf(ruleSection)].SumScoreUI;
                
                sumScoreUI.UpdateText(ruleSectionScore.SectionScore);
                sumScoreUI.FlashScore(ruleSectionScore.SectionScore, player.PlayerColor, highlightDuration);
                
            }
            yield return new WaitForSeconds(highlightDuration);
            
            // flash bonus
            foreach (var player in _players) {
                PlayerScorecard playerScorecard = player.PlayerScorecard;
                RuleSectionScore ruleSectionScore = playerScorecard.RuleSectionScoreList[ruleSectionList.IndexOf(ruleSection)];
                
                ScoreUI bonusScoreUI = playerScoresColumnUIList[_players.IndexOf(player)].RuleSectionScoreUIList[ruleSectionList.IndexOf(ruleSection)].BonusScoreUI;
                
                bonusScoreUI.UpdateText(ruleSectionScore.SectionBonus);
                bonusScoreUI.FlashScore(ruleSectionScore.SectionBonus, player.PlayerColor, highlightDuration);
                
                _finalScoreList[_players.IndexOf(player)] += ruleSectionScore.SectionBonus;
                _finalScoreUIList[_players.IndexOf(player)].UpdateText(_finalScoreList[_players.IndexOf(player)]);
            }
            yield return new WaitForSeconds(highlightDuration);
            
        }
        
        // flash total score
        foreach (var player in _players) {
            PlayerScorecard playerScorecard = player.PlayerScorecard;
            
            ScoreUI totalScoreUI = playerScoresColumnUIList[_players.IndexOf(player)].TotalScoreUI;
            
            totalScoreUI.UpdateText(playerScorecard.GetTotalScore());
            totalScoreUI.FlashScore(playerScorecard.GetTotalScore(), player.PlayerColor, highlightDuration);
            
            // _finalScoreList[_players.IndexOf(player)] += playerScorecard.GetTotalScore();
            _finalScoreUIList[_players.IndexOf(player)].UpdateText(_finalScoreList[_players.IndexOf(player)]);
        }
        
        List<int> winnerIndexList = new List<int>();
        
        int highestScore = 0;
        for (int i = 0; i < _finalScoreList.Count; i++) {
            if (_finalScoreList[i] > highestScore) {
                highestScore = _finalScoreList[i];
                winnerIndexList.Clear();
                winnerIndexList.Add(i);
            } else if (_finalScoreList[i] == highestScore) {
                winnerIndexList.Add(i);
            }
        }

        yield return new WaitForSeconds(0.5f);

        List<RectTransform> winnerRectTfList = new List<RectTransform>();
        foreach (var winnerIndex in winnerIndexList) {
            winnerRectTfList.Add((RectTransform)_finalScoreUIList[winnerIndex].transform);
        }
        
        foreach (var winnerRectTf in winnerRectTfList) {
            Vector2 targetSize = new Vector2(winnerRectTf.sizeDelta.x + 50f, winnerRectTf.sizeDelta.y + 20f);
            
            // lerp size
            float tValue = 0.075f;
            while (Vector2.SqrMagnitude(winnerRectTf.sizeDelta - targetSize) > 0.01f) {
                winnerRectTf.sizeDelta = Vector2.Lerp(winnerRectTf.sizeDelta, targetSize, tValue);
                yield return null;
            }
            winnerRectTf.sizeDelta = targetSize;
            
        }
        
        
        // show winner and exit button
        string winnerText = "";

        if (winnerIndexList.Count != _players.Count) {
            for (int i = 0; i < winnerIndexList.Count; i++) {
                if (i > 0 && i == winnerIndexList.Count - 1)
                    winnerText += " & ";
                else if (i > 0)
                    winnerText += ", ";
                
                winnerText += _players[winnerIndexList[i]].Name;
            }

            winnerText += " win";
            winnerText += winnerIndexList.Count > 1 ? "" : "s";
            winnerText += "!";
        } else {
            winnerText = "It's a tie!";
            
            if (_players.Count == 1) {
                winnerText = "You expect me to say you won? Keep dreaming ;)";
            }
        }
        
        _winnerText.text = winnerText;
        
        
        _exitButton.gameObject.SetActive(true);
        
        
        
        yield return null;
    }
    
    // fade in UI animation
    // after all animation are done, then disconnect the client and show button to go back to main menu
}
