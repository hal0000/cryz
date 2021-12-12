using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlay : MonoBehaviour
{
    [SerializeField]
    int _tileCount, _combinationVersion;
    [SerializeField]
    Color[] _colors;

    [SerializeField]
    GameObject _TilePrefab;

    Tile[,] _Tiles;

    public core.Coroutine.Job WorkJob = new core.Coroutine.Job();

    int _seed;
    public int Seed
    {
        get => Seed;
        set
        {
            if (value != _seed)
            {
                random = new Random(_seed = value);
            }
        }
    }
    Random random = new Random(123456);
    RectTransform _rectTransform;

    public void Start()
    {
        if (TryGetComponent(out RectTransform rect))
            _rectTransform = rect;
        _Tiles = new Tile[_tileCount, _tileCount];
        for (int row = 0; row < _tileCount; row++)
            for (int col = 0; col < _tileCount; col++)
                _Tiles[col, row] = CreateTile(row, col);
        UpdateCombinations();
    }

    Tile CreateTile(int row, int col, bool animatePos = false, bool isNew = false)
    {
        var no = col == 3 && row == 3 ? 5 : random.Int(0, 5);
        var temp = Instantiate(_TilePrefab, this.transform);
        temp.name = $"Row:{row} Col:{col}";
        if (temp.TryGetComponent(out Tile Tile))
        {
            Tile.No = no;
            Tile.Sprite.color = _colors[no];
            Tile.Col = col;
            Tile.Row = row;
            Tile.RectTransform.sizeDelta = new Vector2(_rectTransform.rect.width / _tileCount, _rectTransform.rect.height / _tileCount);
            Tile.UpdatePosition(animatePos);
            return Tile;
        }
        return null;
    }

    bool IsSameGroup(Tile t1, Tile t2)
    {
        if (t1.No < 6) return t1.No == t2.No;
        return t2.No >= 6;
    }
    List<Tile> FindGroup(Tile Tile)
    {
        var result = new List<Tile>();
        var ptr = 0;
        result.Add(Tile);
        while (ptr < result.Count)
        {
            for (int i = 0; i < 4; i++)
            {
                int x = 0, y = 0;
                switch (i)
                {
                    case 0: x = -1; break;
                    case 1: x = 1; break;
                    case 2: y = -1; break;
                    case 3: y = 1; break;
                }
                var col = result[ptr].Col + x;
                var row = result[ptr].Row + y;
                if (col >= 0 && col < _tileCount && row >= 0 && row < _tileCount)
                {
                    var t = _Tiles[col, row];
                    if (t != null && IsSameGroup(Tile, t) && !result.Contains(t)) result.Add(t);
                }
            }
            ptr++;
        }
        return result;
    }

    public void ClickTile(int col, int row)
    {
        if (WorkJob.Busy) return;
        if (col < 0 || col >= _tileCount || row < 0 || row >= _tileCount) return;
        List<Tile> group = FindGroup(_Tiles[col, row]);
        if (group.Count > 0 && group.Count > 1)
        {
            core.Coroutine.Start(this.gameObject, () => Work(_Tiles[col, row], group));
        }
    }

    IEnumerable Work(Tile first, List<Tile> group)
    {
        int groupCount = group.Count;
        if (first.IsSimple)
        {
            for (int i = 0; i < groupCount; i++)
            {
                var tile = group[i];
                this._Tiles[tile.Col, tile.Row] = null;
                DestroyTile(tile);
            }
        }
        else
        {
            this._Tiles[first.Col, first.Row] = null;
            DestroyTile(first);
        }

        yield return WorkJob.Wait();

        ShiftTiles();
        FillTiles(groupCount);
        yield return WorkJob.Wait();
        UpdateCombinations();
        yield return WorkJob.Wait();
    }

    //Alti bos olan taslari asagi kaydirir
    void ShiftTiles()
    {
        for (int x = 0; x < _tileCount; x++)
        {
            for (int y = 1; y < _tileCount; y++)
            {
                if (_Tiles[x, y] == null)
                {
                    for (int i = y; i > 0; i--)
                        _Tiles[x, i] = _Tiles[x, i - 1];
                    _Tiles[x, 0] = null;
                }
            }
        }

        for (int x = 0; x < _tileCount; x++)
            for (int y = 0; y < _tileCount; y++)
            {
                var Tile = _Tiles[x, y];
                if (Tile != null && Tile.Row != y)
                {
                    Tile.Row = y;
                    Tile.UpdatePosition(true);
                }
            }
    }

    void FillTiles(int groupCount)
    {
        int counter = 0;
        for (int x = 0; x < _tileCount; x++)
        {
            var y = 0;
            while (y < _tileCount && _Tiles[x, y] == null)
                y++;
            for (int i = 0; i < y; i++)
            {
                _Tiles[x, i] = CreateTile(i, x, true, true);
                counter++;
            }
            if (counter == groupCount)
                break;
        }
    }

    bool UpdateCombinations()
    {
        var hasCombination = false;
        _combinationVersion++;
        for (int x = 0; x < _tileCount; x++)
            for (int y = 0; y < _tileCount; y++)
            {
                if (_Tiles[x, y] != null && _Tiles[x, y].CombinationVersion != _combinationVersion)
                {
                    var group = FindGroup(_Tiles[x, y]);
                    if (group.Count > 0)
                    {
                        if (!group[0].IsSimple) hasCombination = true;
                        else
                        {
                            if (group.Count > 1) hasCombination = true;
                        }
                    }
                }
            }
        return hasCombination;
    }

    void DestroyTile(Tile Tile)
    {
        if (Tile != null)
        {
            _Tiles[Tile.Col, Tile.Row] = null;
            core.Coroutine.Start(this.gameObject, () => _DestroyTile(Tile), WorkJob);
        }
    }

    IEnumerable _DestroyTile(Tile Tile)
    {
        if (Tile.IsSimple)
            Destroy(Tile.gameObject);
        else
        {
            switch (Tile.No)
            {
                case 6:
                case 7:
                case 8:
                    yield return null;
                    break;
            }
        }
    }
} 