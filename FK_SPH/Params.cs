using System;
using FK_CLI;

static class Params
{
    public static int FlowedParticleNum = 0;

    // 粒子追加可能な範囲
    public static Rectangle ClickableRect;

    // 流体粒子のマテリアル
    public static fk_Material ParticleMaterial = new fk_Material();

    // 壁粒子のマテリアル
    public static fk_Material WallMaterial = new fk_Material();

    // 初期流体
    public static Rectangle ParticleRect;

    // 空間分割領域
    public static Rectangle CellRect;

    // 壁境界
    public static Rectangle[] Rects;

    // 動く壁のオフセット
    public static fk_Vector MoveWallOffset = new fk_Vector();

    // 動く壁
    public static Rectangle MoveWallRect;

    // 動く床のオフセット
    public static fk_Vector MoveFloorOffset = new fk_Vector();

    // 動く床
    public static Rectangle MoveFloorRect;

    // 回転する壁のオフセット
    public static fk_Vector RotateOffset = new fk_Vector();

    // 回転する壁
    public static Rectangle[] RotateRect_1;

    public static Rectangle[] RotateRect_2;

    public static Rectangle[] RotateRect_3;

    // 表示するサイズ(1以下)
    public static double ShapeRad;

    // 粒子の影響範囲
    public static double SmoothRad;

    // 粒子の重さ
    public static double Mass;

    // 圧力の剛性
    public static double Stiffness;

    // 基準密度
    public static double Density0;

    // 粘性係数
    public static double Viscosity;

    // 重力
    public static fk_Vector Gravity;

    // 時間刻み
    public static double Dt;
}