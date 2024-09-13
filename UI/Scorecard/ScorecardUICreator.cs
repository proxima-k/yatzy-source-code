using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// responsible for creating a scorecard UI
// todo: perhaps it's also fine to put this under scorecard UI script since the ScorecardUI script will also require initialization
public class ScorecardUICreator : MonoBehaviour {

    [SerializeField] private Transform _scorecardUIPrefab;
    [SerializeField] private Transform _ruleNameUIPrefab;
    [SerializeField] private Transform _ruleColorUIPrefab;
    [SerializeField] private Transform _ruleScoreUIPrefab;
    [SerializeField] private Transform _ruleNameUIContainerPrefab;
    [SerializeField] private Transform _ruleScoreUIContainerPrefab;


    
    
    public ScorecardUI CreateScorecardUI(List<Player> playerList, List<RuleSection> ruleSectionList) {
        
        Transform root = Instantiate(_scorecardUIPrefab, transform);

        Transform[] containerArray = SetupContainers(root);
        
        Transform ruleNameContainer = containerArray[0];
        Transform playerRuleScoreContainer = containerArray[1];
        
        List<ScoreUI> ruleScoreUIList = new ();
        List<RuleSectionScoreUI> ruleSectionUIList = new ();
        
        foreach (var ruleSection in ruleSectionList) {
            foreach (var rule in ruleSection.CombinationRules) {
                // TextMeshProUGUI ruleScoreText = CreateRow(containerArray, rule.RuleName);
                ScoreUI scoreUI = CreateRuleNameAndScorePair(containerArray, rule.RuleName);
                ruleScoreUIList.Add(scoreUI);
            }

            if (!ruleSection.HasBonus) 
                continue;
            
            ScoreUI sumScoreUI = CreateRuleNameAndScorePair(containerArray, "SUM", true); 
            ScoreUI bonusScoreUI = CreateRuleNameAndScorePair(containerArray, "BONUS", true);
            RuleSectionScoreUI ruleSectionScoreUI = new RuleSectionScoreUI(sumScoreUI, bonusScoreUI);
            ruleSectionUIList.Add(ruleSectionScoreUI);
        }
        
        ScoreUI totalScoreUI = CreateRuleNameAndScorePair(containerArray, "TOTAL", true);
        
        List<PlayerScoresColumnUI> playerScorecardUIList = new List<PlayerScoresColumnUI>();
        
        PlayerScoresColumnUI playerScoresColumnUI = containerArray[1].AddComponent<PlayerScoresColumnUI>();
        Image playerColorImage = containerArray[1].GetChild(0).GetComponent<Image>();
        playerScoresColumnUI.Initialize(playerColorImage, ruleScoreUIList, ruleSectionUIList, totalScoreUI);
        
        playerScorecardUIList.Add(playerScoresColumnUI);
        for (int i = 1; i < playerList.Count; i++) {
            var newPlayerScorecardUI = Instantiate(playerScoresColumnUI, root);
            // newPlayerScorecardUI.AssignPlayer(playerList[i]);
            playerScorecardUIList.Add(newPlayerScorecardUI);
        }
        // i put this at last is because of events, I'm unsure if duplicating GameObjects will keep the events subscribed
        // playerScorecardUI.AssignPlayer(playerList[0]);
        
        // add scorecard UI and connect components to the script
        // ScoreboardUI scoreboardUI = root.AddComponent<ScoreboardUI>();
        
        
        ScorecardUI scorecardUI = root.AddComponent<ScorecardUI>();
        scorecardUI.Initialize(playerScorecardUIList);
        
        ResizeContainer(root, containerArray[0]);
        
        return scorecardUI;
    }

    private Transform[] SetupContainers(Transform root) {
        Transform[] containers = new Transform[2];
        
        Transform ruleNameContainer = Instantiate(_ruleNameUIContainerPrefab, root);
        Transform emptyCell = CreateRuleNameUI(ruleNameContainer, "Players");
        DisableBackgroundImage(emptyCell);

        containers[0] = ruleNameContainer;
        
        
        // create a player container
        Transform playerRuleScoreContainer = Instantiate(_ruleScoreUIContainerPrefab, root);
        // create the player color
        Transform playerColor = Instantiate(_ruleColorUIPrefab, playerRuleScoreContainer);
        // playerColor.GetComponent<Image>().color = players[index].PlayerColor;
        
        containers[1] = playerRuleScoreContainer;
        

        return containers;
    }
    

    private ScoreUI CreateRuleNameAndScorePair(Transform[] containers, string ruleName, bool disableBackground = false) {
        Transform ruleNameContainer = containers[0];
        Transform playerRuleScoreContainer = containers[1];
        
        CreateRuleNameUI(ruleNameContainer, ruleName, disableBackground);
        ScoreUI ruleScoreUI = CreateRuleScoreUI(playerRuleScoreContainer, disableBackground);
        
        return ruleScoreUI;
    }
    
    private ScoreUI CreateRuleScoreUI(Transform parent, bool disableBackground = false) {
        Transform ruleScore = Instantiate(_ruleScoreUIPrefab, parent);
        if (disableBackground)
            DisableBackgroundImage(ruleScore);
        return ruleScore.GetComponentInChildren<ScoreUI>();
    }
    
    private Transform CreateRuleNameUI(Transform parent, string ruleName, bool disableBackground = false) {
        Transform rule = Instantiate(_ruleNameUIPrefab, parent);
        rule.name = ruleName;
        if (disableBackground)
            DisableBackgroundImage(rule);
        rule.GetComponentInChildren<TextMeshProUGUI>().text = ruleName;
        return rule;
    }
    
    private void DisableBackgroundImage(Transform targetTransform) {
        Image backgroundImage = targetTransform.GetComponentInChildren<Image>();
        if (backgroundImage != null) {
            backgroundImage.enabled = false;
        }
    }

    // todo: refactor this code to be reusable
    private void ResizeContainer(Transform rootToRebuild, Transform container) {
        // i need to add extra width to the container
        // didn't know that this function exist, thank you internet
        LayoutRebuilder.ForceRebuildLayoutImmediate(rootToRebuild.GetComponent<RectTransform>());
        
        RectTransform containerRectTransform = container.GetComponent<RectTransform>();
        Vector2 sizeDelta = containerRectTransform.sizeDelta;
        
        container.GetComponent<ContentSizeFitter>().enabled = false;
        containerRectTransform.sizeDelta = new Vector2(sizeDelta.x + 40, sizeDelta.y);
    }
    
    // add scorecardUpdate script
    // foreach player
    // subscribe player.scorecard.onsubmitscore += function
        // has access to rule index and score
        // then the script itself will update the references it has
}
