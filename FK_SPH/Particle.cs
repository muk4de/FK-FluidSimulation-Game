using FK_CLI;
using System;


// パーティクル単体のクラス
// 速度変数はあるが、位置更新は行わない
class Particle
{
    public bool isActive;
    public bool isWall;



    fk_Model model;
    fk_Vector _pos = new fk_Vector(0, 0, 0);
    fk_Vector _vel = new fk_Vector(0, 0, 0);
    fk_Vector _vel2 = new fk_Vector(0, 0, 0);
    public fk_Vector Force = new fk_Vector(0, 0, 0);

    public double Pressure = 0;
    public double Density = 0;

    // コンストラクタ
    public Particle(fk_Vector argPos, bool argAsWall = false)
    {
        isActive = true;
        isWall = argAsWall;
        model = new fk_Model();
        model.ShadowDraw = false;
        model.SmoothMode = false;

        if (isWall)
        {
            model.Material = Params.WallMaterial;
            model.Shape = new fk_Block(Params.ShapeRad * 20.0, Params.ShapeRad * 20.0, Params.ShapeRad * 20.0);
            //model.Shape = new fk_Sphere(1, Params.ShapeRad * 10);
        }
        else
        {
            var mat = new fk_Material();
            var dif = Params.ParticleMaterial.Diffuse;
            var rand1 = new Random().NextDouble();
            var rand2 = new Random().NextDouble();
            var col = new fk_Color(dif.r + (rand1 - rand2) * 0.8, dif.g + rand1 * rand1 * 0.5, 0.0);

            mat.Diffuse = col;
            mat.Ambient = col;
            model.Material = mat;
            model.Shape = new fk_Block(Params.ShapeRad * 30.0, Params.ShapeRad * 30.0, 0);
        }

        _pos = argPos * 10;
        model.GlMoveTo(argPos * 10.0);
    }

    // 位置ベクトルプロパティ
    public fk_Vector Pos
    {
        get
        {
            return _pos / 10.0;
        }
        set
        {
            _pos = value * 10.0;
        }
    }

    // 速度プロパティ
    public fk_Vector Vel
    {
        get { return _vel; }
        set
        {
            _vel = value;
        }
    }

    public fk_Vector Vel2
    {
        get { return _vel2; }
        set
        {
            _vel2 = value;
        }
    }

    public void Move(fk_Vector argPos)
    {
        var pos = _pos + argPos;
        model.GlMoveTo(pos);
        _pos = pos;
    }

    public void Rotate(fk_Vector argCenter, double argRadian, bool argChangeVel = false)
    {
        if (argRadian == 0) return;
        
        model.GlRotateWithVec(argCenter * 10, fk_Axis.Z, argRadian);
        if(argChangeVel)
        {
            Vel2 = (model.Position - _pos) / 10.0 * Params.Dt;
        }
        _pos = model.Position;
    }

    public void Entry(fk_AppWindow argWin)
    {
        argWin.Entry(model);
    }

    public void Remove(fk_AppWindow argWin)
    {
        argWin.Remove(model);
    }

    public void UpdatePos()
    {
        model.GlMoveTo(_pos);
    }

    public void ChangeMaterial(fk_Material argMat)
    {
        model.Material = argMat;
    }
}
