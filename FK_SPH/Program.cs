using FK_CLI;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;



#region Functions

// ウィンドウの初期設定
static fk_AppWindow WindowSetup()
{
    var window = new fk_AppWindow();
    window.Size = new fk_Dimension(800, 800);
    window.BGColor = new fk_Color(0.25, 0.25, 0.25);
    window.FPS = 60;
    return window;
}


static fk_SpriteModel SpriteSetup(fk_AppWindow argWin, bool argStart, double argPosX, double argPosY, int argDPI, int argPTSize, int argMonospace, int argBold, fk_Color argColor)
{
    var sModel = new fk_SpriteModel();
    sModel.SetPositionLT(argPosX, argPosY);
    sModel.Text.DPI = argDPI;
    sModel.Text.PTSize = argPTSize;
    sModel.Text.BoldStrength = argBold;
    sModel.Text.MonospaceSize = argMonospace;
    sModel.Text.ForeColor = argColor;
    sModel.Text.BackColor = new fk_Color(0, 0, 0, 0);
    sModel.Text.FillColor(new fk_Color(0, 0, 0, 0));
    sModel.Text.ShadowColor = new fk_Color(0, 0, 0, 0);
    if (argStart) argWin.Entry(sModel);
    return sModel;
}

static fk_Model GenerateLineBox(Rectangle argRect, fk_Color argColor, fk_AppWindow argWin, bool argStart)
{
    var model = new fk_Model();
    model.Shape = new fk_Block(argRect.width * 10, argRect.height * 10, 0);
    model.ShadowDraw = false;
    model.DrawMode = fk_Draw.LINE;
    model.LineColor = argColor;
    model.GlMoveTo(argRect.center * 10);
    if (argStart) argWin.Entry(model);
    return model;
}

#endregion


void SetParametor()
{
    Params.ParticleMaterial.Diffuse = new fk_Color(1, 0, 0);
    Params.ParticleMaterial.Ambient = new fk_Color(1, 0, 0);
    Params.ParticleMaterial.Emission = new fk_Color(0.8, 0.5, 0);
    Params.ParticleMaterial.Alpha = 1f;

    Params.WallMaterial.Diffuse = new fk_Color(0.5, 0.5, 0.5);
    Params.WallMaterial.Ambient = new fk_Color(0.5, 0.5, 0.5);

    Params.ClickableRect = new Rectangle(0.1, 1.5, 1.8, 0.5);
    Params.ParticleRect = new Rectangle(0.1, 0.5, 0, 0);
    Params.CellRect = new Rectangle(-0.1, -0.1, 2.2, 2.2);
    Params.Rects = new[] { new Rectangle(0, 0, 0.1, 0.36), new Rectangle(0, 0.5, 0.1, 1.5), new Rectangle(0.6, 0, 1.3, 0.1), new Rectangle(1.9, 0, 0.1, 2), new Rectangle(0.0, 1, 0.75, 0.1, -Math.PI / 8)
                        , new Rectangle(0.3, 0.6, 0.8, 0.1), new Rectangle(0.00, 0.3, 0.3, 0.1, Math.PI / 16), new Rectangle(0.6, 0.1, 0.1, 0.2)};

    Params.RotateRect_1 = new[] { new Rectangle(0.25, 1.6, 0.5, 0.06), new Rectangle(0.25, 1.6, 0.5, 0.06, Math.PI / 2) };
    Params.RotateRect_2 = new[] { new Rectangle(1.25, 0.3, 0.5, 0.06), new Rectangle(1.25, 0.3, 0.5, 0.06, Math.PI / 2) };
    Params.RotateRect_3 = new[] { new Rectangle(1.125, 1, 0.75, 0.06), new Rectangle(1.125, 1, 0.75, 0.06, Math.PI / 2) };

    Params.MoveWallRect = new Rectangle(1, 1.5, 0.06, 0.25);
    Params.MoveFloorRect = new Rectangle(1.5, 1.5, 0.25, 0.06);

    Params.ShapeRad = 0.01;
    Params.SmoothRad = 0.03;
    Params.Density0 = 1000;
    Params.Mass = 4.0 * Params.ShapeRad * Params.ShapeRad * Params.Density0;
    Params.Stiffness = 100;
    Params.Viscosity = 20;
    Params.Gravity = new fk_Vector(0.0, -9.8, 0.0);
    Params.Dt = 0.0020;
}

SetParametor();

var window = WindowSetup();
window.CameraPos = Params.CellRect.center * 10 + new fk_Vector(0, 2.5, 40);

GenerateLineBox(Params.CellRect, new fk_Color(1, 1, 1), window, true);

GenerateLineBox(Params.ClickableRect, new fk_Color(0, 0, 1), window, true);

var pHandler = new ParticleHandler(window);


// スクリーン座標変換用平面
var plane = new fk_Plane();
plane.SetPosNormal(new fk_Vector(), new fk_Vector(0, 0, 1));





//// スタート後のUI
var pumpAmount = 1000;
var goalAmount = 500;
var currentAmount = pumpAmount;
var pumpText = SpriteSetup(window, false, -200, 350, 100, 25, 15, 2, new fk_Color(1, 1, 1));
pumpText.DrawText($"液体残量 {currentAmount}/{pumpAmount}", true);

var pumpGauge = SpriteSetup(window, false, -200, 300, 100, 30, 30, 5, new fk_Color(1, 0.25, 0));
pumpGauge.DrawText($"{new String('■', (int)Math.Ceiling(currentAmount / (pumpAmount / 10.0)))}", true);

var flowedNumText = SpriteSetup(window, false, 200, 350, 100, 25, 15, 2, new fk_Color(1, 1, 1));
flowedNumText.DrawText($"流れた液体 {Params.FlowedParticleNum}/{goalAmount}", true);

var flowedGauge = SpriteSetup(window, false, 200, 300, 100, 30, 30, 5, new fk_Color(1, 0.25, 0));
flowedGauge.DrawText($"{new String('■', (int)Math.Floor(Params.FlowedParticleNum / (double)goalAmount))}", true);


var clearText = SpriteSetup(window, false, 0, 0, 100, 100, 100, 10, new fk_Color(1, 1, 0));
clearText.DrawText($"クリア！");


//// スタート前のUI
var explainText = SpriteSetup(window, true, 0, 350, 100, 25, 15, 2, new fk_Color(1, 1, 1));
explainText.DrawText($"機械を動かして液体を下の口に流しやすくせよ Enterで準備終了");

var keyExplainText = SpriteSetup(window, true, 0, 275, 100, 25, 15, 5, new fk_Color(1, 1, 1));
keyExplainText.DrawText($"それぞれ数字キーを押しながらマウスを動かし移動\n1 黄の回転ブレード  2 青の左右に動く壁  3 緑の上下に動く床", true);

var fpsText = SpriteSetup(window, true, 350, -350, 100, 25, 15, 2, new fk_Color(1, 1, 1));

//// 実行中だけ使用する変数
bool isStart = false;
bool isEnd = false;
int fps = 60;

fpsText.DrawText($"FPS:{fps}" , true);

window.Open();
while (window.Update() == true)
{
    if(window.GetKeyStatus(fk_Key.UP, fk_Switch.DOWN))
    {
        fps++;
        window.FPS = fps;
        fpsText.DrawText($"FPS:{fps}", true);
    }
    if(window.GetKeyStatus(fk_Key.DOWN, fk_Switch.DOWN))
    {
        fps--;
        window.FPS = fps;
        fpsText.DrawText($"FPS:{fps}", true);
    }
    if (isStart == false)
    {
        DragToMoveWalls();
        if (window.GetKeyStatus(fk_Key.ENTER, fk_Switch.DOWN))
        {
            isStart = true;
            window.Remove(explainText);
            window.Remove(keyExplainText);
            window.Entry(pumpText);
            window.Entry(pumpGauge);
            window.Entry(flowedNumText);
            window.Entry(flowedGauge);
        }
    }
    else
    {
        ClickToAddParticle();

        pHandler.Update();

        flowedNumText.DrawText($"流れた液体 {Params.FlowedParticleNum}/{goalAmount}", true);
        flowedGauge.DrawText($"{new String('■', (int)Math.Floor(Params.FlowedParticleNum / (goalAmount / 10.0)))}", true);
        if (Params.FlowedParticleNum / goalAmount >= 1 && !isEnd)
        {
            isEnd = true;
            window.Entry(clearText);
        }
    }
}

void DragToMoveWalls()
{
    if (window.GetKeyStatus('1', fk_Switch.PRESS))
    {
        var mousePos = window.MousePosition;
        var (status, worldPos) = window.GetProjectPosition(mousePos.x, mousePos.y, plane);

        var diff = (worldPos - Params.RotateRect_1[0].center * 10.0) - Params.RotateOffset;
        Params.RotateOffset = worldPos - Params.RotateRect_1[0].center * 10;
        pHandler.MoveParticles(pHandler.rotateWallParticles_1, diff);
    }
    else if (window.GetKeyStatus('2', fk_Switch.PRESS))
    {
        var mousePos = window.MousePosition;
        var (status, worldPos) = window.GetProjectPosition(mousePos.x, mousePos.y, plane);

        var diff = (worldPos - Params.MoveWallRect.center * 10.0) - Params.MoveWallOffset;
        Params.MoveWallOffset = worldPos - Params.MoveWallRect.center * 10;
        pHandler.MoveParticles(pHandler.moveWallParticles, diff);
    }
    else if (window.GetKeyStatus('3', fk_Switch.PRESS))
    {
        var mousePos = window.MousePosition;
        var (status, worldPos) = window.GetProjectPosition(mousePos.x, mousePos.y, plane);

        var diff = (worldPos - Params.MoveFloorRect.center * 10.0) - Params.MoveFloorOffset;
        Params.MoveFloorOffset = worldPos - Params.MoveFloorRect.center * 10;
        pHandler.MoveParticles(pHandler.moveFloorParticles, diff);
    }
}

void ClickToAddParticle()
{
    if (window.GetMouseStatus(fk_MouseButton.M1))
    {
        if (currentAmount <= 0)
        {
            pumpText.DrawText($"もう残量がありません！", true);
            return;
        }
        var mousePos = window.MousePosition;
        var (status, worldPos) = window.GetProjectPosition(mousePos.x, mousePos.y, plane);

        if (!Rectangle.CheckIfInRect(Params.ClickableRect, worldPos, true)) return;

        pHandler.AddNewParticle(worldPos, window);


        currentAmount--;
        pumpText.DrawText($"液体残量 {currentAmount}/{pumpAmount}", true);
        pumpGauge.DrawText($"{new String('■', (int)Math.Ceiling(currentAmount / (pumpAmount / 10.0)))}", true);
    }
}