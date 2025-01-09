using FK_CLI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


// パーティクルの生成、位置更新を行う
class ParticleHandler
{
    Particle[] particles;
    public Particle[] moveWallParticles = new Particle[0];
    public Particle[] moveFloorParticles = new Particle[0];
    public Particle[] rotateWallParticles_1 = new Particle[0];
    Particle[] rotateWallParticles_2 = new Particle[0];
    Particle[] rotateWallParticles_3 = new Particle[0];


    Cell cell;
    Kernel kernel;
    public static Random rand = new Random();

    double h2;
    double time;

    static readonly int tNum = 8;

    // コンストラクタ
    public ParticleHandler(fk_AppWindow argWin)
    {
        h2 = Params.SmoothRad * Params.SmoothRad;
        kernel = new Kernel(Params.SmoothRad);
        // パーティクルを生成し、argNumの形状に並べる
        InitialParticle(argWin);
        SetWindow(argWin);
    }

    void InitialParticle(fk_AppWindow argWin)
    {
        var tmpParticles = new List<Particle>();

        // 壁の配置
        for (int i = 0; i < Params.Rects.Length; i++)
        {
            CreateParticleRect(ref tmpParticles, Params.Rects[i], true);
        }

        // 動く壁の配置
            moveWallParticles = CreateParticleRect(ref tmpParticles, Params.MoveWallRect, true, fk_Material.LightBlue);

        // 動く床の配置
        moveFloorParticles = CreateParticleRect(ref tmpParticles, Params.MoveFloorRect, true, fk_Material.LightGreen);

        // 回転する壁の配置
        for (int i = 0; i < Params.RotateRect_1.Length; i++)
        {
            var tmp = CreateParticleRect(ref tmpParticles, Params.RotateRect_1[i], true, fk_Material.Yellow);

            rotateWallParticles_1 = rotateWallParticles_1.Concat(tmp).ToArray();
        }

        for(int i = 0; i < Params.RotateRect_2.Length; i++)
        {
            var tmp = CreateParticleRect(ref tmpParticles, Params.RotateRect_2[i], true);
            rotateWallParticles_2 = rotateWallParticles_2.Concat(tmp).ToArray();
        }

        for(int i = 0; i < Params.RotateRect_3.Length; i++)
        {
            var tmp = CreateParticleRect(ref tmpParticles, Params.RotateRect_3[i], true);
            rotateWallParticles_3 = rotateWallParticles_3.Concat(tmp).ToArray();
        }

        var wallParticleNum = tmpParticles.Count;

        // 液体粒子初期配置
        CreateParticleRect(ref tmpParticles, Params.ParticleRect, false);

        Console.WriteLine($"particleNum : {tmpParticles.Count - wallParticleNum}");
        Console.WriteLine($"wallNum : {wallParticleNum}");


        particles = tmpParticles.ToArray();
        Console.WriteLine($"allNum:{particles.Length}");


        cell = new Cell(Params.CellRect, Params.SmoothRad, argWin);
    }

    Particle[] CreateParticleRect(ref List<Particle> argContainer, Rectangle argRect, bool argWall = false, fk_Material argMat = null)
    {
        var list = new List<Particle>();
        int nx = (int)(argRect.width / (Params.ShapeRad * 2.0));
        int ny = (int)(argRect.height / (Params.ShapeRad * 2.0));

        for (int i = 0; i < nx; i++)
        {
            for (int j = 0; j < ny; j++)
            {
                var x = argRect.left + (i + 0.5) * Params.ShapeRad * 2.0;
                var y = argRect.bottom + (j + 0.5) * Params.ShapeRad * 2.0;
                var p = new Particle(new fk_Vector(x, y), argWall);
                p.Rotate(argRect.center, argRect.radian);
                if(argMat != null) p.ChangeMaterial(argMat);
                argContainer.Add(p);
                list.Add(p);
            }
        }

        return list.ToArray();
    }


    public void AddNewParticle(fk_Vector argPos, fk_AppWindow argWin)
    {
        var tmpParticles = particles.ToList<Particle>();
        var randVec = new fk_Vector(rand.NextDouble(), rand.NextDouble());
        var p = new Particle(argPos / 10.0 + randVec * 0.1);
        p.Entry(argWin);
        p.Vel2 = Params.Gravity / 2.0;
        tmpParticles.Add(p);


        particles = tmpParticles.ToArray();
    }

    
    public void MoveParticles(Particle[] argPs, fk_Vector argMoveVec)
    {
        for (int i = 0; i < argPs.Length; i++)
        {
            if (!argPs[i].isActive) continue;
            argPs[i].Move(argMoveVec);
        }
    }


    // パーティクルをウィンドウに表示
    void SetWindow(fk_AppWindow argWin)
    {
        foreach (var P in particles)
        {
            P.Entry(argWin);
        }
    }

    // メイン処理
    public void Update()
    {
        time += Params.Dt;

        // マルチコア処理
        var tasks = new Task[tNum];
        cell.Clear();
        for (int i = 0; i < tNum; i++)
        {
            tasks[i] = cell.Add_Async(particles, tNum, i);
        }
        Task.WaitAll(tasks);


        for (int i = 0; i < tNum; i++)
        {
            tasks[i] = CalcDensityAndPressure_Async(i);
        }
        Task.WaitAll(tasks);

        for (int i = 0; i < tNum; i++)
        {
            tasks[i] = CalcForce_Async(i);
        }
        Task.WaitAll(tasks);

        for (int i = 0; i < tNum; i++)
        {
            tasks[i] = PositionVelocityUpdate_Async(i);
        }
        Task.WaitAll(tasks);


        MoveWall(ref moveWallParticles, new fk_Vector(-1, 0), 0.5);

        MoveWall(ref moveFloorParticles, new fk_Vector(0, 1), 0.5);

        RotateWall(ref rotateWallParticles_1, Params.RotateRect_1[0].center + Params.RotateOffset / 10.0, Math.PI * Params.Dt);

        RotateWall(ref rotateWallParticles_2, Params.RotateRect_2[0].center, -Math.PI * Params.Dt);

        RotateWall(ref rotateWallParticles_3, Params.RotateRect_3[0].center, Math.PI * Params.Dt);

        UpdateParticlePos();
    }


    async Task CalcDensityAndPressure_Async(int argID)
    {
        await Task.Run(() => CalcDensityAndPressure(argID));
    }

    async Task CalcForce_Async(int argID)
    {
        await Task.Run(() => CalcForce(argID));
    }

    async Task PositionVelocityUpdate_Async(int argID)
    {
        await Task.Run(() => PositionVelocityUpdate(argID));
    }

    // 密度と圧力の計算
    void CalcDensityAndPressure(int argID)
    {
        for (int i = argID; i < particles.Length; i += tNum)
        {
            if (!particles[i].isActive) continue;
            var sum = 0.0;
            foreach (var p in cell.GetNeighborIndex(particles[i]))
            {
                var diff = p.Pos - particles[i].Pos;
                // poly6関数での計算
                sum += kernel.KernelFunc(diff.Dist2());

                //if (i == 0) particles[n].ChangeMaterial(fk_Material.Red);
            }

            particles[i].Density = sum * Params.Mass;
            particles[i].Pressure = Math.Max(Params.Stiffness * (particles[i].Density - Params.Density0), 0.0);
        }
    }


    void CalcForce(int argID)
    {
        for (int i = argID; i < particles.Length; i += tNum)
        {
            if (!particles[i].isActive) continue;
            var forceSum = new fk_Vector();
            var pressureSum = new fk_Vector();
            var viscositySum = new fk_Vector();
            var checkWp = new fk_Vector();
            foreach (var p in cell.GetNeighborIndex(particles[i]))
            {
                if (particles[i] == p) continue;
                var diff = p.Pos - particles[i].Pos;
                var r = diff.Dist();


                // 圧力項
                var wp = kernel.GradientFunc(diff);
                var fp = Params.Mass
                    * (p.Pressure / (particles[i].Density * particles[i].Density)
                    + particles[i].Pressure / (particles[i].Density * particles[i].Density));
                pressureSum += wp * fp;

                // 粘性項
                var H2 = r * r + 0.01 * h2;
                var dv = particles[i].Vel - p.Vel;
                var fv = Params.Mass * 2.0 * Params.Viscosity / (p.Density * particles[i].Density) * (diff * wp) / H2;

                viscositySum += fv * dv;
            }

            // 重力
            forceSum += Params.Gravity + pressureSum + viscositySum;

            particles[i].Force = forceSum;

            //if (i == 0) Console.WriteLine($"press:{pressureSum}, visc:{viscositySum}, force:{particles[i].Force}, wp:{checkWp}");
        }
    }


    void MoveWall(ref Particle[] argPs, fk_Vector argVel, double argLoopTime)
    {
        if (argPs == null) return;
        fk_Vector moveVel;
        if (time % argLoopTime < argLoopTime / 2.0)
        {
            moveVel = argVel;
        }
        else
        {
            moveVel = -argVel;
        }
        for (int i = 0; i < argPs.Length; i++)
        {
            if (!argPs[i].isActive || argPs[i] == null) continue;

            argPs[i].Force = new fk_Vector();

            argPs[i].Vel2 = moveVel;

            argPs[i].Pos = argPs[i].Pos + argPs[i].Vel2 * Params.Dt;

            argPs[i].Vel = argPs[i].Vel2 + 0.5 * argPs[i].Force * Params.Dt;

            argPs[i].UpdatePos();
        }
    }

    void RotateWall(ref Particle[] argPs, fk_Vector argCenter, double argRadian)
    {
        for (int i = 0; i < argPs.Length; i++)
        {
            if (!argPs[i].isActive || argPs[i] == null) continue;
            argPs[i].Rotate(argCenter, argRadian, true);
        }
    }


    void PositionVelocityUpdate(int argID)
    {
        for (int i = argID; i < particles.Length; i += tNum)
        {
            if (!particles[i].isActive || particles[i].isWall) continue;
            particles[i].Vel2 = particles[i].Vel2 + particles[i].Force * Params.Dt;

            particles[i].Pos = particles[i].Pos + particles[i].Vel2 * Params.Dt;

            particles[i].Vel = particles[i].Vel2 + 0.5 * particles[i].Force * Params.Dt;
        }
    }


    void UpdateParticlePos()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            if (!particles[i].isActive || particles[i].isWall) continue;
            particles[i].UpdatePos();
        }
    }
}
