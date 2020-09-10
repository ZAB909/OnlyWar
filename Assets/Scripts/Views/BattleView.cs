﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Iam.Scripts.Views
{
    public class BattleView : MonoBehaviour
    {
        public UnityEvent<int> OnSquadSelected;
        
        [SerializeField]
        private Text BattleLog;
        [SerializeField]
        private Text TempPlayerWoundTrack;
        [SerializeField]
        private GameObject Map;
        [SerializeField]
        private GameObject NextStepButton;
        [SerializeField]
        private GameObject SquadPrefab;
        [SerializeField]
        private ScrollRect ScrollRect;
        [SerializeField]
        private GameSettings GameSettings;

        private bool _scrollToBottom = false;
        private readonly Dictionary<int, GameObject> _squadMap;
        private Text _nextStepButtonText;

        public BattleView()
        {
            _squadMap = new Dictionary<int, GameObject>();
        }

        public void LateUpdate()
        {
            if(_scrollToBottom)
            {
                Canvas.ForceUpdateCanvases();
                ScrollRect.verticalNormalizedPosition = 0;
                _scrollToBottom = false;
                Canvas.ForceUpdateCanvases();
            }
        }

        public void SetMapSize(Vector2 size)
        {
            size.Scale(GameSettings.BattleMapScale);
            Map.GetComponent<RectTransform>().sizeDelta = size;
        }

        public void AddSquad(int id, string name, Vector2 position, Vector2 size, Color color)
        {
            GameObject squad = Instantiate(SquadPrefab,
                                position,
                                Quaternion.Euler(0, 0, -90),
                                Map.transform);
            
            squad.GetComponentInChildren<Text>().text = name;
            squad.GetComponent<Image>().color = color;
            squad.GetComponent<Button>().onClick.AddListener(() => Squad_OnClick(id));
            
            var rt = squad.GetComponent<RectTransform>();
            position.Scale(GameSettings.BattleMapScale);
            rt.anchoredPosition = position;
            //rt.rotation = gameObject.GetComponent<RectTransform>().rotation;


            size.Scale(GameSettings.BattleMapScale);
            rt.sizeDelta = size;
            
            _squadMap[id] = squad;
        }

        public void MoveSquad(int id, Vector2 newPosition, Vector2 newSize)
        {
            var rt = _squadMap[id].GetComponent<RectTransform>();
            newPosition.Scale(GameSettings.BattleMapScale);
            rt.anchoredPosition = newPosition;
            newSize.Scale(GameSettings.BattleMapScale);
            rt.sizeDelta = newSize;
        }

        public void RemoveSquad(int id)
        {
            GameObject.Destroy(_squadMap[id]);
            _squadMap.Remove(id);
        }

        public void Clear()
        {
            BattleLog.text = "";
            TempPlayerWoundTrack.text = "";

            foreach(KeyValuePair<int, GameObject> kvp in _squadMap)
            {
                GameObject.Destroy(kvp.Value);
            }
            _squadMap.Clear();
        }

        public void ClearBattleLog()
        {
            BattleLog.text = "";
        }

        public void LogToBattleLog(string text)
        {
            BattleLog.text += text + "\n";
            _scrollToBottom = true;
        }

        public void UpdateNextStepButton(string text, bool enabled)
        {
            NextStepButton.gameObject.SetActive(enabled);
            if(_nextStepButtonText == null)
            {
                _nextStepButtonText = NextStepButton.GetComponentInChildren<Text>();
            }
            _nextStepButtonText.text = text;
        }

        public void OverwritePlayerWoundTrack(string text)
        {
            TempPlayerWoundTrack.text = text;
        }

        private void Squad_OnClick(int squadId)
        {
            OnSquadSelected.Invoke(squadId);
            
        }
    }
}
