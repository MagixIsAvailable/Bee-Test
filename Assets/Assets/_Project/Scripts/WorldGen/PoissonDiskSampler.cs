using System;
using System.Collections.Generic;
using UnityEngine;

/// Simple 2D Poisson-disk sampler for a rectangle area.
/// Supports variable radius through a callback.
public class PoissonDiskSampler
{
    readonly Rect rect;
    readonly float baseRadius;
    readonly float cellSize;
    readonly Vector2Int gridSize;
    readonly System.Random rng;

    readonly List<Vector2> points = new List<Vector2>();
    readonly List<Vector2> active = new List<Vector2>();
    readonly Vector2?[,] grid;

    const int kCandidatesPerPoint = 30;

    public PoissonDiskSampler(Rect rect, float baseRadius, System.Random rng)
    {
        this.rect = rect;
        this.baseRadius = Mathf.Max(0.01f, baseRadius);
        this.cellSize = baseRadius / Mathf.Sqrt(2f);
        this.gridSize = new Vector2Int(
            Mathf.CeilToInt(rect.width / cellSize),
            Mathf.CeilToInt(rect.height / cellSize)
        );
        this.grid = new Vector2?[gridSize.x, gridSize.y];
        this.rng = rng;
    }

    public IEnumerable<Vector2> SamplesWithVariableRadius(Func<Vector2, float> radiusAt, int targetCount)
    {
        points.Clear(); active.Clear();
        Array.Clear(grid, 0, grid.Length);

        // start with a random point
        var first = new Vector2(
            (float)(rng.NextDouble() * rect.width + rect.xMin),
            (float)(rng.NextDouble() * rect.height + rect.yMin)
        );
        AddPoint(first);

        while (active.Count > 0 && points.Count < targetCount)
        {
            int idx = rng.Next(0, active.Count);
            Vector2 center = active[idx];
            float rCenter = Mathf.Max(0.01f, radiusAt(center));

            bool found = false;
            for (int k = 0; k < kCandidatesPerPoint; k++)
            {
                float ang = (float)(rng.NextDouble() * Mathf.PI * 2.0);
                float rad = rCenter * (1f + (float)rng.NextDouble()); // between r and 2r
                Vector2 candidate = center + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * rad;

                if (!rect.Contains(candidate)) continue;

                float rCand = Mathf.Max(0.01f, radiusAt(candidate));
                if (IsFarEnough(candidate, Mathf.Min(rCenter, rCand)))
                {
                    AddPoint(candidate);
                    found = true;
                    break;
                }
            }

            if (!found) active.RemoveAt(idx);
        }

        return points;
    }

    void AddPoint(Vector2 p)
    {
        points.Add(p);
        active.Add(p);
        var gi = GridIndex(p);
        grid[gi.x, gi.y] = p;
    }

    bool IsFarEnough(Vector2 p, float minDist)
    {
        Vector2Int gi = GridIndex(p);
        int rx = 2;
        for (int y = gi.y - rx; y <= gi.y + rx; y++)
        for (int x = gi.x - rx; x <= gi.x + rx; x++)
        {
            if (x < 0 || y < 0 || x >= gridSize.x || y >= gridSize.y) continue;
            var q = grid[x, y];
            if (q.HasValue)
            {
                if ((q.Value - p).sqrMagnitude < minDist * minDist)
                    return false;
            }
        }
        return true;
    }

    Vector2Int GridIndex(Vector2 p)
    {
        int gx = Mathf.FloorToInt((p.x - rect.xMin) / cellSize);
        int gz = Mathf.FloorToInt((p.y - rect.yMin) / cellSize);
        gx = Mathf.Clamp(gx, 0, gridSize.x - 1);
        gz = Mathf.Clamp(gz, 0, gridSize.y - 1);
        return new Vector2Int(gx, gz);
    }
}
