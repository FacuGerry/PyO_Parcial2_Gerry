using System;

public class TurnManager
{
    public event Action<int> OnTurnChanged;

    private int _maxIndex;
    private int _index = 0;

    public TurnManager(int maxIndex)
    {
        _maxIndex = maxIndex;
    }

    public void StartTurns() => OnTurnChanged?.Invoke(_index);

    public void ChangeTurn()
    {
        _index++;
        if (_index >= _maxIndex)
            _index = 0;

        OnTurnChanged?.Invoke(_index);
    }
}