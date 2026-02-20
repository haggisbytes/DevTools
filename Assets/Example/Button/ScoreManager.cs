using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private Text _scoreText;
    [SerializeField] private int _scoreToAdd;

    private int _currentScore = 0;

    [Button("Add Score")]
    public void AddScore()
    {
        _currentScore += _scoreToAdd;
        UpdateScoreUI();
    }

    #region Update UI
    private void UpdateScoreUI()
    {
        if (_scoreText != null)
        {
            _scoreText.text = $"Score: {_currentScore}";
        }
    }
    #endregion
}
