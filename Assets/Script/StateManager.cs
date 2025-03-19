using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager
{
    private readonly UIView _uiView;
    private readonly ILocalizationManager _localizationManager;

    private TowerState _towerState;
    public StateManager(UIView uiView, ILocalizationManager localizationManager)
    {
        _uiView = uiView;
        _localizationManager = localizationManager;
    }

    public void UpdateState(TowerState towerState)
    {
        if (_towerState == TowerState.Null || towerState == TowerState.Null)
        {
            _towerState = towerState;

            StateInText();
        }
    }

    private void StateInText()
    {
        switch (_towerState)
        {
            case TowerState.InTower:
                _uiView.CommentText.text = _localizationManager.GetLocalizedText("IN_TOWER");
                break;
            case TowerState.InHole:
                _uiView.CommentText.text = _localizationManager.GetLocalizedText("IN_HOLE");
                break;
            case TowerState.TowerHeight:
                _uiView.CommentText.text = _localizationManager.GetLocalizedText("TOWER_HEIGHT");
                break;
            case TowerState.Missed:
                _uiView.CommentText.text = _localizationManager.GetLocalizedText("MISSED");
                break;
            case TowerState.Null:
                _uiView.CommentText.text = _localizationManager.GetLocalizedText("NULL");
                break;
        }
    }
}
