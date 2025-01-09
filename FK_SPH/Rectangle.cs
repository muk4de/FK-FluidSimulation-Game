
using FK_CLI;

class Rectangle
{
    public double width;
    public double height;
    public double radian;

    public fk_Vector center;
    public double top;
    public double bottom;
    public double right;
    public double left;

    public Rectangle(double argX, double argY, double argWidth, double argHeight, double argRadian = 0)
    {
        width = argWidth;
        height = argHeight;
        radian = argRadian;

        left = argX;
        bottom = argY;
        right = argX + argWidth;
        top = argY + argHeight;

        center = new fk_Vector(argX + argWidth / 2, argY + argHeight / 2);
    }

    public static bool CheckIfInRect(Rectangle argRect, fk_Vector argPos, bool argOriginalPos = false)
    {
        var pos = argPos;
        if (argOriginalPos) pos /= 10.0;
        if (pos.x < argRect.left) return false;
        if (pos.x > argRect.right) return false;
        if (pos.y < argRect.bottom) return false;
        if (pos.y > argRect.top) return false;
        return true;
    }
}