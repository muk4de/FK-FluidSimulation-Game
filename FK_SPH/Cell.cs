using FK_CLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// ó±éqÇãÛä‘Ç≈ï™ÇØÅAîzóÒÇ…ì¸ÇÍÇÈ
class Cell
{
    LinkedList<Particle>[,] bucket;
    int nx;
    int ny;
    Rectangle rect;
    double h;
    double h2;
    fk_AppWindow window;
    public Cell(Rectangle argRect, double argH, fk_AppWindow argWin)
    {
        window = argWin;
        h = argH;
        h2 = argH * argH;
        nx = (int)(argRect.width / h);
        ny = (int)(argRect.height / h);
        rect = argRect;
        bucket = new LinkedList<Particle>[nx, ny];
        Console.WriteLine($"bucketNum:{nx * ny}");
        for (int i = 0; i < nx; i++)
        {
            for (int j = 0; j < ny; j++)
            {
                bucket[i, j] = new LinkedList<Particle>();
            }
        }
    }


    public void Clear()
    {
        foreach (var i in bucket)
        {
            i.Clear();
        }
    }

    public async Task Add_Async(Particle[] argPs, int argTNum, int argID)
    {
        await Task.Run(() => Add(argPs, argTNum, argID));
    }


    public void Add(Particle[] argPs, int argTNum, int argID)
    {
        for (int i = argID; i < argPs.Length; i += argTNum)
        {
            if (!argPs[i].isActive || argPs[i] == null) continue;
            var x = IndexX(argPs[i]);
            var y = IndexY(argPs[i]);
            if (x < 0 || y < 0 || x >= nx || y >= ny)
            {
                DisactiveParticle(argPs[i], x, y);
                continue;
            }
            bucket[IndexX(argPs[i]), IndexY(argPs[i])].AddLast(argPs[i]);
        }
    }


    private void DisactiveParticle(Particle argP, int argX, int argY)
    {
        argP.isActive = false;
        //argP.Remove(window);
        argP.ChangeMaterial(fk_Material.MatBlack);
        if (argX >= 5 && argX <= 25 && argY == -1)
        {
            Params.FlowedParticleNum++;
        }
    }

    private int IndexX(Particle argP)
    {
        return (int)((argP.Pos.x - rect.left) / h);
    }


    private int IndexY(Particle argP)
    {
        return (int)((argP.Pos.y - rect.bottom) / h);
    }


    public List<Particle> GetNeighborIndex(Particle argP)
    {
        List<Particle> value = new List<Particle>();
        var iX = IndexX(argP);
        var iY = IndexY(argP);

        for (int x = iX - 1; x <= iX + 1; x++)
        {
            if (x < 0 || x >= nx) continue;

            for (int y = iY - 1; y <= iY + 1; y++)
            {
                if (y < 0 || y >= ny) continue;


                foreach (var p in bucket[x, y])
                {
                    var diff = p.Pos - argP.Pos;
                    if (diff.Dist2() >= h2) continue;
                    value.Add(p);
                }
            }
        }

        return value;
    }
}